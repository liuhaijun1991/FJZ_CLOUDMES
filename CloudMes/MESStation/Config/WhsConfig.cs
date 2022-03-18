using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESPubLab;
using MESStation.Interface.SAPRFC;
using MES_DCN.Broadcom;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using System.IO;
using System.Reflection;
using static MESDataObject.Constants.PublicConstants;
using Newtonsoft.Json;
using MESPubLab.MESStation.Label;

namespace MESStation.Config
{
    public class WhsConfig : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
        protected APIInfo FGetToList = new APIInfo()
        {
            FunctionName = "GetToListData",
            Description = "Get ToList",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetToDetailDataByToNo = new APIInfo()
        {
            FunctionName = "GetToDetailDataByToNo",
            Description = "GetToDetailDataByToNo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ToNo", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FShipOutDoCqa = new APIInfo()
        {
            FunctionName = "ShipOutDoCqa",
            Description = "ShipOutDoCqa",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DnNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DnLine", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetCqaType = new APIInfo()
        {
            FunctionName = "GetCqaType",
            Description = "GetCqaType",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DnNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DnLine", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetGtDataByDnAndDnLine = new APIInfo()
        {
            FunctionName = "GetGtDataByDnAndDnLine",
            Description = "GetGtDataByDnAndDnLine",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Dn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DnLine", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FGetDnDetailDataByDnNo = new APIInfo()
        {
            FunctionName = "GetDnDetailDataByDnNo",
            Description = "GetDnDetailDataByDnNo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DnNo", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FShipOutGtAll = new APIInfo()
        {
            FunctionName = "ShipOutGtAll",
            Description = "ShipOutGtAll",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DnNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DnLine", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Bu", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FHWTGetToDetailDataByToNo = new APIInfo()
        {
            FunctionName = "HWTGetToDetailDataByToNo",
            Description = "HWT SHIPPING STATION GET TO DETAIL BY TO NO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TO_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetShippingListByType = new APIInfo()
        {
            FunctionName = "GetShippingListByType",
            Description = "Get Shipping List By Type",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Type", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Value", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DNLine", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetDNLineList = new APIInfo()
        {
            FunctionName = "GetDNLineList",
            Description = "Get DN Line List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FCancelShipping = new APIInfo()
        {
            FunctionName = "CancelShipping",
            Description = "Cancel Shipping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Type", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Value", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DNLine", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Remark", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FGetDnCustomer = new APIInfo()
        {
            FunctionName = "GetDnCustomer",
            Description = "GetDnCustomer",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Dn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAsnDataByDnInfo = new APIInfo()
        {
            FunctionName = "GetAsnDataByDnInfo",
            Description = "GetAsnDataByDnInfo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Dn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllInWhsPallet = new APIInfo()
        {
            FunctionName = "GetAllInWhsPallet",
            Description = "GetAllInWhsPallet",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PALLET_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DATE_FROM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DATE_TO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_SIZE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_NUM", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetPalletDetail = new APIInfo()
        {
            FunctionName = "GetPalletDetail",
            Description = "GetPalletDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PALLET_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FTEST = new APIInfo()
        {
            FunctionName = "TEST",
            Description = "TEST",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WEIGHT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CANCEL", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FGetJuniperASNShippingLabel = new APIInfo()
        {
            FunctionName = "GetJuniperASNShippingLabel",
            Description = "Get Juniper ASN Shipping Label",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ASN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetJuniperASNPackList = new APIInfo()
        {
            FunctionName = "GetJuniperASNPackList",
            Description = "Get Juniper ASN Pack List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ASN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetJuniperPrintShippingLabel = new APIInfo()
        {
            FunctionName = "GetJuniperPrintShippingLabel",
            Description = "Get Juniper Print Shipping Label",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ASN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetPackFifoNoControl = new APIInfo()
        {
            FunctionName = "GetPackFifoNoControl",
            Description = "GetPackFifoNoControl",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddPackFifoNoControl = new APIInfo()
        {
            FunctionName = "AddPackFifoNoControl",
            Description = "AddPackFifoNoControl",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        public WhsConfig()
        {
            this.Apis.Add(FGetToList.FunctionName, FGetToList);
            this.Apis.Add(FGetToDetailDataByToNo.FunctionName, FGetToDetailDataByToNo);
            this.Apis.Add(FShipOutGtAll.FunctionName, FShipOutGtAll);
            this.Apis.Add(FShipOutDoCqa.FunctionName, FShipOutDoCqa);
            this.Apis.Add(FHWTGetToDetailDataByToNo.FunctionName, FHWTGetToDetailDataByToNo);
            this.Apis.Add(FGetShippingListByType.FunctionName, FGetShippingListByType);
            this.Apis.Add(FGetDNLineList.FunctionName, FGetDNLineList);
            this.Apis.Add(FCancelShipping.FunctionName, FCancelShipping);
            this.Apis.Add(FGetDnCustomer.FunctionName, FGetDnCustomer);
            this.Apis.Add(FGetAsnDataByDnInfo.FunctionName, FGetAsnDataByDnInfo);
            this.Apis.Add(FGetAllInWhsPallet.FunctionName, FGetAllInWhsPallet);
            this.Apis.Add(FGetPalletDetail.FunctionName, FGetPalletDetail);
            this.Apis.Add(FTEST.FunctionName, FTEST);
            this.Apis.Add(FGetJuniperASNPackList.FunctionName, FGetJuniperASNPackList);
            this.Apis.Add(FGetJuniperASNShippingLabel.FunctionName, FGetJuniperASNShippingLabel);
            this.Apis.Add(FGetJuniperPrintShippingLabel.FunctionName, FGetJuniperPrintShippingLabel);
            this.Apis.Add(FGetPackFifoNoControl.FunctionName, FGetPackFifoNoControl);
            this.Apis.Add(FAddPackFifoNoControl.FunctionName, FAddPackFifoNoControl);
        }

        public void GetToListData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM
                    .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                        rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                    .OrderBy((rth) => rth.TO_CREATETIME, OrderByType.Desc)
                    .GroupBy(rth => new { rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME })
                    .Select(rth => new { rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME }).ToList();
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

        public void GetToDetailDataByToNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string ToNo = Data["ToNo"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_TO_DETAIL>().Where(x => x.TO_NO == ToNo)
                    .OrderBy(x => x.TO_ITEM_NO, OrderByType.Asc).ToList();
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

        public void GetWaitShipOutDnDetailDataByDnNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string DnNo = Data["DnNo"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == DnNo && x.DN_FLAG == "0")
                    .OrderBy(x => x.DN_LINE, OrderByType.Asc).ToList();
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

        public void GetWaitShipOutToDetailDataByToNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string ToNo = Data["ToNo"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_TO_DETAIL, R_DN_STATUS>((rtd, rds) => rtd.DN_NO == rds.DN_NO && rtd.TO_NO == ToNo && rds.DN_FLAG == "0").Select((rtd, rds) => rtd)
                    .OrderBy(rtd => rtd.TO_ITEM_NO, OrderByType.Asc).ToList().Distinct().ToList();
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

        public void GetDnDetailDataByDnNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string DnNo = Data["DnNo"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == DnNo)
                    .OrderBy(x => x.DN_LINE, OrderByType.Asc).ToList();
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

        public void GetGtDataByDnAndDnLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data,
            MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string dn = Data["Dn"].ToString().Trim(),
                   dnLine = Data["DnLine"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_DN_STATUS, C_SHIPPING_ROUTE_DETAIL>((rds, csrd) => rds.GTTYPE == csrd.ROUTENAME && rds.DN_NO == dn && rds.DN_LINE == dnLine)
                    .OrderBy((rds, csrd) => csrd.SEQ, OrderByType.Asc)
                    .Select((rds, csrd) => new { csrd.ID, csrd.ROUTENAME, csrd.SEQ, csrd.ACTIONNAME, csrd.ACTIONTYPE, csrd.FROM_PLANT, csrd.TO_PLANT, csrd.FROM_STOCK, csrd.TO_STOCK, csrd.RFC_NAME, GTEVENT = rds.GTEVENT })
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
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetAllWaitGtData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_DN_STATUS>()
                    .OrderBy(rds => rds.GT_FLAG, OrderByType.Asc).OrderBy(rds => rds.DN_FLAG, OrderByType.Asc).OrderBy(rds => rds.EDITTIME, OrderByType.Asc)
                    .ToList().Distinct().ToList();
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

        public void GetGtDataByStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string Status = Data["Status"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_DN_STATUS>()
                    .OrderBy(rds => rds.GT_FLAG, OrderByType.Asc).OrderBy(rds => rds.DN_FLAG, OrderByType.Asc).OrderBy(rds => rds.EDITTIME, OrderByType.Asc)
                    .ToList().Distinct().ToList();
                if (int.Parse(Status) > -1)
                    res = res.FindAll(t => t.DN_FLAG == Status);
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


        void doGT(OleExec oleDB, C_SHIPPING_ROUTE_DETAIL csrdDetail, R_DN_STATUS rDnStatus, string bu)
        {
            string ErrorMessage = string.Empty;
            string To = string.Empty;

            R_TO_DETAIL toDetail = oleDB.ORM.Queryable<R_TO_DETAIL>().Where(t => t.DN_NO == rDnStatus.DN_NO).ToList().FirstOrDefault();
            if (toDetail != null)
            {
                To = toDetail.TO_NO;
            }
            if (csrdDetail.RFC_NAME == "ZRFC_SFC_NSG_0011")
            {
                ZRFC_SFC_NSG_0011 ZRFC_SFC_NSG_0011 = new ZRFC_SFC_NSG_0011(bu);
                ZRFC_SFC_NSG_0011.SetValue("I_BKTXT", rDnStatus.DN_NO);
                ZRFC_SFC_NSG_0011.SetValue("I_MATNR", rDnStatus.SKUNO);
                ZRFC_SFC_NSG_0011.SetValue("I_ERFMG", rDnStatus.QTY.ToString());
                ZRFC_SFC_NSG_0011.SetValue("I_FROM", csrdDetail.FROM_STOCK);
                ZRFC_SFC_NSG_0011.SetValue("I_TO", csrdDetail.TO_STOCK);
                ZRFC_SFC_NSG_0011.SetValue("PLANT", csrdDetail.TO_PLANT);
                ZRFC_SFC_NSG_0011.SetValue("I_BUDAT", "");
                ZRFC_SFC_NSG_0011.CallRFC();
                if (ZRFC_SFC_NSG_0011.GetValue("O_FLAG").ToString().Equals("1"))
                {
                    WriteLog.WriteIntoMESLog("SFCDB", bu, "DOGT", WriteLog.GetCurrentMethodFullName(), "",
                        $@"{csrdDetail.ACTIONNAME}:{ZRFC_SFC_NSG_0011.GetValue("O_MESSAGE")}", "", rDnStatus.DN_NO, "F",
                        rDnStatus.GTEVENT, "", "N");
                    //throw new Exception($@"{rDnStatus.DN_NO}:{csrdDetail.ACTIONNAME}=>SAP执行返回异常,请联系SAPIT,异常信息:{ZRFC_SFC_NSG_0011.GetValue("O_MESSAGE")}");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114703", new string[] { rDnStatus.DN_NO, csrdDetail.ACTIONNAME, ZRFC_SFC_NSG_0011.GetValue("O_MESSAGE") }));
                }
                else
                    WriteLog.WriteIntoMESLog("SFCDB", bu, "DOGT", WriteLog.GetCurrentMethodFullName(), "",
                        $@"{csrdDetail.ACTIONNAME}:{ZRFC_SFC_NSG_0011.GetValue("O_MESSAGE")}", "", rDnStatus.DN_NO, "P",
                        rDnStatus.GTEVENT, "", "N");
            }
            else if (csrdDetail.RFC_NAME == "ZRFC_NSG_SD_0003")
            {
                ZRFC_NSG_SD_0003 ZRFC_NSG_SD_0003 = new ZRFC_NSG_SD_0003(bu);
                ZRFC_NSG_SD_0003.SetValue("P_VBELN", rDnStatus.DN_NO);
                ZRFC_NSG_SD_0003.SetValue("P_WADAT", System.DateTime.Now.ToString("yyyy-MM-dd"));
                ZRFC_NSG_SD_0003.CallRFC();
                if (ZRFC_NSG_SD_0003.GetValue("O_FLAG").ToString().Equals("1"))
                {
                    WriteLog.WriteIntoMESLog("SFCDB", bu, "DOGT", WriteLog.GetCurrentMethodFullName(), "",
                        $@"{csrdDetail.ACTIONNAME}:{ZRFC_NSG_SD_0003.GetValue("O_MESSAGE")}", "", rDnStatus.DN_NO, "F",
                        rDnStatus.GTEVENT, "", "N");
                    //throw new Exception($@"{rDnStatus.DN_NO}:{csrdDetail.ACTIONNAME}=>SAP执行返回异常,请联系SAPIT,异常信息:{ZRFC_NSG_SD_0003.GetValue("O_MESSAGE")}");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114703", new string[] { rDnStatus.DN_NO, csrdDetail.ACTIONNAME, ZRFC_NSG_SD_0003.GetValue("O_MESSAGE") }));
                }
                else
                    WriteLog.WriteIntoMESLog("SFCDB", bu, "DOGT", WriteLog.GetCurrentMethodFullName(), "",
                        $@"{csrdDetail.ACTIONNAME}:{ZRFC_NSG_SD_0003.GetValue("O_MESSAGE")}", "", rDnStatus.DN_NO, "P",
                        rDnStatus.GTEVENT, "", "N");
            }
            else if (csrdDetail.RFC_NAME == "ZRFC_SFC_NSG_0024B")
            {
                ZRFC_SFC_NSG_0024B ZRFC_SFC_NSG_0024B = new ZRFC_SFC_NSG_0024B(bu);
                ZRFC_SFC_NSG_0024B.SetValue("I_VBELN", rDnStatus.DN_NO);
                ZRFC_SFC_NSG_0024B.SetValue("I_WERKS", csrdDetail.TO_PLANT);
                ZRFC_SFC_NSG_0024B.SetValue("I_LOGINNAME", "SFCSYSTEM");
                ZRFC_SFC_NSG_0024B.SetValue("I_WADAT", System.DateTime.Now.ToString("yyyy-MM-dd"));
                ZRFC_SFC_NSG_0024B.CallRFC();
                if (ZRFC_SFC_NSG_0024B.GetValue("O_FLAG").ToString().Equals("1"))
                {
                    WriteLog.WriteIntoMESLog("SFCDB", bu, "DOGT", WriteLog.GetCurrentMethodFullName(), "",
                        $@"{csrdDetail.ACTIONNAME}:{ZRFC_SFC_NSG_0024B.GetValue("O_MESSAGE")}", "", rDnStatus.DN_NO, "F",
                        rDnStatus.GTEVENT, "", "N");
                    //throw new Exception($@"{rDnStatus.DN_NO}:{csrdDetail.ACTIONNAME}=>SAP执行返回异常,请联系SAPIT,异常信息:{ZRFC_SFC_NSG_0024B.GetValue("O_MESSAGE")}");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114703", new string[] { rDnStatus.DN_NO, csrdDetail.ACTIONNAME, ZRFC_SFC_NSG_0024B.GetValue("O_MESSAGE") }));
                }
                else
                    WriteLog.WriteIntoMESLog("SFCDB", bu, "DOGT", WriteLog.GetCurrentMethodFullName(), "",
                        $@"{csrdDetail.ACTIONNAME}:{ZRFC_SFC_NSG_0024B.GetValue("O_MESSAGE")}", "", rDnStatus.DN_NO, "P",
                        rDnStatus.GTEVENT, "", "N");
            }
            else if (csrdDetail.RFC_NAME == "ZRFC_SFC_NSG_0004")
            {
                ZRFC_SFC_NSG_0004 ZRFC_SFC_NSG_0004 = new ZRFC_SFC_NSG_0004(bu);
                ZRFC_SFC_NSG_0004.SetValue("I_TKNUM", To);
                ZRFC_SFC_NSG_0004.SetValue("I_VBELN", rDnStatus.DN_NO);
                ZRFC_SFC_NSG_0004.SetValue("I_POSNR", rDnStatus.DN_LINE);
                ZRFC_SFC_NSG_0004.SetValue("I_FROM", csrdDetail.FROM_STOCK);
                ZRFC_SFC_NSG_0004.SetValue("I_TO", csrdDetail.TO_STOCK);
                ZRFC_SFC_NSG_0004.SetValue("PLANT", csrdDetail.TO_PLANT);
                ZRFC_SFC_NSG_0004.CallRFC();
                if (ZRFC_SFC_NSG_0004.GetValue("O_FLAG") == "1")
                {
                    ErrorMessage = ZRFC_SFC_NSG_0004.GetValue("O_MESSAGE").ToString();
                    WriteLog.WriteIntoMESLog(oleDB, BU, "ShippingGTPGI", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName.ToString(), "ShippingGT", csrdDetail.RFC_NAME + ErrorMessage, "", "ShippingGT");
                    throw new Exception(ErrorMessage);
                }
            }
            else if (csrdDetail.RFC_NAME == "ZRFC_SFC_NSG_0006")
            {

                ZRFC_SFC_NSG_0006 ZRFC_SFC_NSG_0006 = new ZRFC_SFC_NSG_0006(bu);
                ZRFC_SFC_NSG_0006.SetValue("I_TKNUM", To);
                ZRFC_SFC_NSG_0006.SetValue("P_WADAT", DateTime.Now.ToString("yyyy-MM-dd"));
                ZRFC_SFC_NSG_0006.CallRFC();

                if (ZRFC_SFC_NSG_0006.GetValue("O_FLAG") == "1")
                {
                    ErrorMessage = ZRFC_SFC_NSG_0006.GetValue("O_MESSAGE").ToString();
                    WriteLog.WriteIntoMESLog(oleDB, BU, "ShippingGTPGI", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName.ToString(), "ShippingPGI", csrdDetail.RFC_NAME + ErrorMessage, "", "ShippingPGI");
                    throw new Exception(ErrorMessage);
                }

            }
        }

        public void ShipOutDoCqa(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data,
            MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string dn = Data["Dn"].ToString().Trim(),
                   dnLine = Data["DnLine"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Ado.UseTran(() =>
                {
                    var resRds = oleDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dn && x.DN_LINE == dnLine && x.DN_FLAG == ENUM_R_DN_STATUS.DN_WAIT_CQA.Ext<EnumValueAttribute>().Description).ToList();
                    if (resRds.Count == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180804171505";
                        return;
                    }
                    //JUNIPER R_DN_STATUS中的skuno是groupid與
                    var resRdsc = oleDB.ORM.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER>((rds, cs, cse, cc) => rds.SKUNO == cs.SKUNO && cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                        .Where((rds, cs, cse, cc) => rds.DN_NO == dn && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc) => new { cc.CUSTOMER_NAME, rds.ID }).ToList().FirstOrDefault();

                    #region 客戶分支
                    if (!BU.Equals("VNJUNIPER") && resRdsc.CUSTOMER_NAME.ToUpper().Trim().Equals(Customer.NETGEAR.ExtValue()))
                    {
                        resRds.FirstOrDefault().DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_SEND_ASN.ExtValue();
                        var ctlobj = oleDB.ORM.Queryable<R_NETGEAR_PTM_CTL>().Where(t => t.SHIPORDERID == resRdsc.ID)
                            .ToList().FirstOrDefault();
                        ctlobj.CQA = ENUM_R_NETGEAR_PTM_CTL.CQA_END.ExtValue();
                        oleDB.ORM.Updateable(ctlobj).ExecuteCommand();
                        #region 生成和發送暫時分開寫;
                        NetgearPtmObj netgearPtmObj = new NetgearPtmObj(this.BU, this.DBPools["SFCDB"]);
                        var Gfuncres = netgearPtmObj.GanarationFileByCtl(ctlobj, ConfigurationManager.AppSettings["tempfile"], oleDB.ORM);
                        if (!Gfuncres.IsSuccess)
                            throw Gfuncres.ErrorException;
                        var Sfuncres = netgearPtmObj.SendPtmData(ctlobj, ConfigurationManager.AppSettings["tempfile"], oleDB.ORM);
                        if (!Sfuncres.IsSuccess)
                            throw Sfuncres.ErrorException;
                        //resRds.FirstOrDefault().DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_GT.ExtValue();
                        #endregion
                    }
                    //else
                    //    resRds.FirstOrDefault().DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_GT.ExtValue();
                    #endregion
                    resRds.FirstOrDefault().DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_GT.ExtValue();
                    resRds.FirstOrDefault().EDITTIME = System.DateTime.Now;
                    oleDB.ORM.Updateable(resRds.FirstOrDefault()).WhereColumns(x => new { x.DN_NO, x.DN_LINE }).ExecuteCommand();
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
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

        /// <summary>
        /// dnline做GT時鎖定和解鎖;lockflag=true 執行Lock;lockflag=false 執行unLock;
        /// </summary>
        /// <param name="dn"></param>
        /// <param name="dnLine"></param>
        /// <param name="lockflag"></param>
        void LockDnGt(string dn, string dnLine, bool lockflag)
        {
            OleExec oleDB = null;
            string gtFlag = lockflag ? "0" : "2";
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var resRds = oleDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dn && x.DN_LINE == dnLine && x.DN_FLAG == "2").ToList();
                if (resRds.Count == 0 || resRds.FirstOrDefault().GT_FLAG == "1")
                    return;
                if (resRds.FirstOrDefault().GT_FLAG != gtFlag)
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113407", new string[] { lockflag.ToString(), dn, dnLine }));
                //throw new Exception($@"LockDnGt:{lockflag.ToString()}=>Dn:{dn},DnLine:{dnLine} Status is Err!");
                resRds.FirstOrDefault().GT_FLAG = lockflag ? "2" : "0";
                oleDB.ORM.Updateable(resRds.FirstOrDefault()).WhereColumns(x => new { x.DN_NO, x.DN_LINE }).ExecuteCommand();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void ShipOutGtAll(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data,
            MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string dn = Data["Dn"].ToString().Trim(),
                   dnLine = Data["DnLine"].ToString().Trim(),
                   bu = Data["Bu"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var resCsrds = oleDB.ORM.Queryable<R_DN_STATUS, C_SHIPPING_ROUTE_DETAIL>((rds, csrd) => rds.GTTYPE == csrd.ROUTENAME && rds.DN_NO == dn && rds.DN_LINE == dnLine)
                    .OrderBy((rds, csrd) => csrd.SEQ, OrderByType.Asc)
                    .Select((rds, csrd) => csrd)
                    .ToList();
                var resRds = oleDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dn && x.DN_LINE == dnLine && x.DN_FLAG == "2")
                    .ToList();
                if (resRds.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20180804171505";
                    return;
                }
                else if (resRds.FirstOrDefault().GT_FLAG == "2")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20180825153500";
                    return;
                }
                else if (resRds.FirstOrDefault().GT_FLAG != "0")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20180804171505";
                    return;
                }

                LockDnGt(dn, dnLine, true);
                for (int i = 0; i < resCsrds.Count; i++)
                {
                    if (resCsrds[i].SEQ == resRds.FirstOrDefault().GTEVENT)
                    {
                        doGT(oleDB, resCsrds[i], resRds.FirstOrDefault(), bu);
                        resRds.FirstOrDefault().GTEVENT = i == resCsrds.Count - 1 ? "END" : resCsrds[i + 1].SEQ;
                        resRds.FirstOrDefault().DN_FLAG = i == resCsrds.Count - 1 ? "3" : "2";
                        resRds.FirstOrDefault().GT_FLAG = i == resCsrds.Count - 1 ? "1" : "2";
                        resRds.FirstOrDefault().EDITTIME = System.DateTime.Now;
                        resRds.FirstOrDefault().GTDATE = System.DateTime.Now;
                        oleDB.ORM.Updateable(resRds.FirstOrDefault()).WhereColumns(x => new { x.DN_NO, x.DN_LINE }).ExecuteCommand();
                    }
                }
                var res = oleDB.ORM.Queryable<R_DN_STATUS, C_SHIPPING_ROUTE_DETAIL>((rds, csrd) => rds.GTTYPE == csrd.ROUTENAME && rds.DN_NO == dn && rds.DN_LINE == dnLine)
                    .OrderBy((rds, csrd) => csrd.SEQ, OrderByType.Asc)
                    .Select((rds, csrd) => new { csrd.ID, csrd.ROUTENAME, csrd.SEQ, csrd.ACTIONNAME, csrd.ACTIONTYPE, csrd.FROM_PLANT, csrd.TO_PLANT, csrd.FROM_STOCK, csrd.TO_STOCK, csrd.RFC_NAME, GTEVENT = rds.GTEVENT })
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
                LockDnGt(dn, dnLine, false);
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        /// <summary>
        /// HWT SHIPPING STATION GET TO DETAIL BY TO NO
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void HWTGetToDetailDataByToNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string to_no = Data["TO_NO"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var data = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_TO_DETAIL_HWT>().Where(r => r.TO_NO == to_no).OrderBy(r => r.DN_NO, OrderByType.Asc)
                    .Select(r => new { r.TO_ITEM_NO, r.DN_NO, r.DN_STARTTIME, r.DN_ENDTIME, r.DN_CUSTOMER, r.PLANT }).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = data;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void HWTGetDNDetailDataByDnNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string dn_no = Data["DN_NO"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var data = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_DN_DETAIL>().Where(r => r.DN_NO == dn_no).OrderBy(r => r.DN_ITEM_NO, OrderByType.Asc)
                    .Select(r => new { r.DN_ITEM_NO, r.P_NO, r.P_NO_QTY, r.REAL_QTY, r.PRICE, r.SO_NO, r.SO_ITEM_NO, r.PO_NO, r.NET_WEIGHT, r.GROSS_WEIGHT, r.PLANT }).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = data;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetShippingListByType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string type = Data["Type"] == null ? "" : Data["Type"].ToString().Trim();
                string value = Data["Value"] == null ? "" : Data["Value"].ToString().Trim();
                string dn_line = Data["DNLine"] == null ? "" : Data["DNLine"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(sfcdb, DBTYPE);
                if (type == "DN")
                {
                    StationReturn.Data = TRSD.GetShipDetailByDN(sfcdb, value, dn_line);
                }
                else if (type == "SN")
                {
                    StationReturn.Data = sfcdb.ORM.Queryable<R_SHIP_DETAIL>().Where(r => r.SN == value).ToList();
                }
                else if (type == "PALLET")
                {
                    StationReturn.Data = sfcdb.ORM.Queryable<R_SHIP_DETAIL, R_PACKING, R_SN_PACKING, R_PACKING>((rsd, rp, rsp, rpg) => rsd.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && rp.PARENT_PACK_ID == rpg.ID)
                        .Where((rsd, rp, rsp, rpg) => rpg.PACK_NO == value).Select((rsd, rp, rsp, rpg) => rsd).ToList();
                }
                else
                {
                    StationReturn.Data = "";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception exception)
            {
                //throw exception;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = "";
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetDNLineList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string dn = Data["DN"] == null ? "" : Data["DN"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(sfcdb, DBTYPE);
                StationReturn.Data = sfcdb.ORM.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == dn).Select(r => r.DN_LINE).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = "";
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void CancelShipping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string type = Data["Type"] == null ? "" : Data["Type"].ToString().Trim();
                string inputValue = Data["Value"] == null ? "" : Data["Value"].ToString().Trim();
                string dn_line = Data["DNLine"] == null ? "" : Data["DNLine"].ToString().Trim();
                string remark = Data["Remark"] == null ? "" : Data["Remark"].ToString().Trim();

                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_SN TRS = new T_R_SN(SFCDB, DBTYPE);
                T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(SFCDB, DBTYPE);
                T_R_DN_STATUS TRDS = new T_R_DN_STATUS(SFCDB, DBTYPE);
                T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(SFCDB, DBTYPE);
                T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(SFCDB, DBTYPE);
                T_R_PACKING TRP = new T_R_PACKING(SFCDB, DBTYPE);
                T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBTYPE);


                int result = 0;
                string dn_no = "";
                string current_station = "";
                List<C_ROUTE_DETAIL> listRoute = new List<C_ROUTE_DETAIL>();
                List<R_SN> listSN = new List<R_SN>();
                R_SN objSN;
                R_SHIP_DETAIL objShipDetail;
                R_SN_STATION_DETAIL objStationDetail;
                R_DN_STATUS objDNStatus;
                DateTime sysDateTime = TRS.GetDBDateTime(SFCDB);
                switch (type)
                {
                    case "SN":
                        #region Cancel Ship Out By SN
                        objSN = TRS.LoadData(inputValue, SFCDB);
                        objShipDetail = TRSD.GetShipDetailBySN(SFCDB, objSN.SN);
                        if (objSN == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { inputValue }));
                        }
                        if (objSN.NEXT_STATION != "SHIPFINISH" && objSN.CURRENT_STATION != "SHIPOUT")
                        {
                            //throw new MESReturnMessage(objSN.SN + " Hasn't Been Shipped Yet!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115950", new string[] { objSN.SN }));
                        }
                        if (objShipDetail == null)
                        {
                            //throw new MESReturnMessage(objSN.SN + " No Shipping Record!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141412", new string[] { objSN.SN }));
                        }
                        dn_no = objShipDetail.DN_NO;
                        dn_line = objShipDetail.DN_LINE;
                        objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, dn_no, dn_line);
                        if (objDNStatus.DN_FLAG == "3")
                        {
                            //做完GT不給退SHIPPING
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141720", new string[] { dn_no, dn_line }));
                            //throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                        }

                        listRoute = TCRD.GetLastStations(objSN.ROUTE_ID, "SHIPOUT", SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
                        objSN.CURRENT_STATION = listRoute.FirstOrDefault().STATION_NAME;
                        objSN.NEXT_STATION = "SHIPOUT";
                        objSN.SHIPPED_FLAG = "0";
                        objSN.SHIPDATE = null;
                        objSN.EDIT_EMP = LoginUser.EMP_NO;
                        objSN.EDIT_TIME = sysDateTime;
                        result = TRS.Update(objSN, SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                        }

                        result = TRSD.CancelShip(SFCDB, objShipDetail);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL" }));
                        }

                        objStationDetail = TRSSD.GetDetailBySnAndStation(objSN.SN, "SHIPOUT", SFCDB);
                        objStationDetail.SN = "RS_" + objStationDetail.SN;
                        result = TRSSD.Update(objStationDetail, SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL" }));
                        }

                        objDNStatus.DN_FLAG = "0";
                        objDNStatus.EDITTIME = sysDateTime;
                        result = TRDS.Update(SFCDB, objDNStatus);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                        }
                        #endregion
                        break;
                    case "PALLET":
                        CancelShipOutByPallet(SFCDB, inputValue);
                        break;
                        #region Cancel Ship Out By Pallet
                        R_PACKING objPack = TRP.GetPackingByPackNo(inputValue, SFCDB);
                        if (objPack == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { inputValue }));
                        }
                        listSN = TRP.GetSnListByPalletID(objPack.ID, SFCDB);
                        if (listSN.Count == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { inputValue }));
                            //throw new MESReturnMessage($@"The Pallet {inputValue} Is Empty!");
                        }
                        var list = listSN.Select(r => r.SN).ToList();
                        var shipedDnList = SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(r => list.Contains(r.SN)).Select(r => new { r.DN_NO, r.DN_LINE }).Distinct().ToList();
                        foreach (var item in shipedDnList)
                        {
                            objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, item.DN_NO, item.DN_LINE);
                            if (objDNStatus.DN_FLAG == "3")
                            {
                                //做完GT不給退SHIPPING
                                //throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141720", new string[] { dn_no, dn_line }));
                            }
                        }

                        listRoute = TCRD.GetLastStations(listSN.FirstOrDefault().ROUTE_ID, "SHIPOUT", SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
                        current_station = listRoute.FirstOrDefault().STATION_NAME;
                        foreach (R_SN sn in listSN)
                        {
                            objShipDetail = TRSD.GetShipDetailBySN(SFCDB, sn.SN);
                            if (objShipDetail == null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141412", new string[] { sn.SN }));
                                //throw new MESReturnMessage(sn.SN + " No Shipping Record!");
                            }
                            if (sn.NEXT_STATION != "SHIPFINISH" && sn.CURRENT_STATION != "SHIPOUT")
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115950", new string[] { sn.SN }));
                                //throw new MESReturnMessage(sn.SN + " Hasn't Been Shipped Yet!");
                            }
                            sn.CURRENT_STATION = current_station;
                            sn.NEXT_STATION = "SHIPOUT";
                            sn.SHIPPED_FLAG = "0";
                            sn.SHIPDATE = null;
                            sn.EDIT_EMP = LoginUser.EMP_NO;
                            sn.EDIT_TIME = sysDateTime;
                            result = TRS.Update(sn, SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN " + sn.SN }));
                            }

                            R_SN_STATION_DETAIL objStationDetail_P = TRSSD.GetDetailBySnAndStation(sn.SN, "SHIPOUT", SFCDB);
                            objStationDetail_P.SN = "RS_" + objStationDetail_P.SN;
                            result = TRSSD.Update(objStationDetail_P, SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + sn.SN }));
                            }

                            //dn_no = objShipDetail.DN_NO;
                            //dn_line = objShipDetail.DN_LINE;
                            //objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, dn_no, dn_line);
                            //if (objDNStatus.DN_FLAG == "3")
                            //{
                            //    //做完GT不給退SHIPPING
                            //    //throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141720", new string[] { dn_no, dn_line }));
                            //}

                            result = TRSD.CancelShip(SFCDB, objShipDetail);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + sn.SN }));
                            }
                        }

                        //objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, dn_no, dn_line);
                        //objDNStatus.DN_FLAG = "0";
                        //objDNStatus.EDITTIME = sysDateTime;
                        //result = TRDS.Update(SFCDB, objDNStatus);
                        //if (result == 0)
                        //{
                        //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                        //}
                        foreach (var item in shipedDnList)
                        {
                            objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, item.DN_NO, item.DN_LINE);
                            objDNStatus.DN_FLAG = "0";
                            objDNStatus.EDITTIME = sysDateTime;
                            result = TRDS.Update(SFCDB, objDNStatus);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                            }
                        }
                        #endregion
                        break;
                    case "DN":
                        #region Cancel Ship Out By DN 
                        dn_no = inputValue;
                        objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, dn_no, dn_line);
                        if (objDNStatus.DN_FLAG == "3")
                        {
                            //做完GT不給退SHIPPING
                            //throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141720", new string[] { dn_no, dn_line }));
                        }
                        List<R_SHIP_DETAIL> listShip = TRSD.GetShipDetailByDN(SFCDB, dn_no, dn_line);
                        if (listShip.Count == 0)
                        {
                            //throw new MESReturnMessage(dn_no + " No Shipping Record!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141412", new string[] { dn_no }));
                        }
                        List<string> snList = listShip.Select(r => r.SN).Distinct().ToList();
                        var palletList = SFCDB.ORM
                            .Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((sn, rsp, carton, pallet) => sn.ID == rsp.SN_ID && rsp.PACK_ID == carton.ID && carton.PARENT_PACK_ID == pallet.ID)
                            .Where((sn, rsp, carton, pallet) => snList.Contains(sn.SN)).Select((sn, rsp, carton, pallet) => pallet.PACK_NO).Distinct().ToList();
                        foreach (var palletNo in palletList)
                        {
                            CancelShipOutByPallet(SFCDB, palletNo);
                        }
                        break;

                        foreach (R_SHIP_DETAIL sd in listShip)
                        {
                            objSN = TRS.LoadData(sd.SN, SFCDB);
                            if (objSN.NEXT_STATION != "SHIPFINISH" && objSN.CURRENT_STATION != "SHIPOUT")
                            {
                                //throw new MESReturnMessage(objSN.SN + " Hasn't Been Shipped Yet!");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115950", new string[] { objSN.SN }));
                            }

                            listRoute = TCRD.GetLastStations(objSN.ROUTE_ID, "SHIPOUT", SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
                            current_station = listRoute.FirstOrDefault().STATION_NAME;

                            objSN.CURRENT_STATION = current_station;
                            objSN.NEXT_STATION = "SHIPOUT";
                            objSN.SHIPPED_FLAG = "0";
                            objSN.SHIPDATE = null;
                            objSN.EDIT_EMP = LoginUser.EMP_NO;
                            objSN.EDIT_TIME = sysDateTime;
                            result = TRS.Update(objSN, SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN " + objSN.SN }));
                            }

                            objStationDetail = TRSSD.GetDetailBySnAndStation(objSN.SN, "SHIPOUT", SFCDB);
                            objStationDetail.SN = "RS_" + objStationDetail.SN;
                            result = TRSSD.Update(objStationDetail, SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + objSN.SN }));
                            }

                            result = TRSD.CancelShip(SFCDB, sd);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + objSN.SN }));
                            }
                        }
                        objDNStatus.DN_FLAG = "0";
                        objDNStatus.EDITTIME = sysDateTime;
                        result = TRDS.Update(SFCDB, objDNStatus);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                        }
                        #endregion
                        break;
                    default:
                        throw new MESReturnMessage(type + ",Input Type Error!");
                        //break;
                }
                WriteLog.WriteIntoMESLog(SFCDB, BU, "RETURN_SHIPPING", "MESStation.Config.WhsConfig", "CancelShipping",
                            remark, "", LoginUser.EMP_NO, inputValue, "");

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Message = "OK";

                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = "";
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetRMAMoveLocation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_CONTROL TCC = new T_C_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                C_CONTROL Control = TCC.GetControlByName("", sfcdb);
                if (Control != null)
                {
                    StationReturn.Data = Control.CONTROL_VALUE.Split(',').ToList();
                }
                else
                {
                    StationReturn.Data = new List<string>() { "F701", "F503", "WIP", "F101", "SHIPPING" };
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetDnCustomer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string dn = Data["Dn"].ToString().Trim(),
                dnLine = Data["DnLine"].ToString().Trim();
            string resRds = "";
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                if (this.BU.Equals("VNJUNIPER"))
                {
                    var wPrefix = oleDB.ORM.Queryable<R_SHIP_DETAIL, R_SN>((r, x) => r.SN == x.SN).Where((r, x) => r.DN_NO == dn && x.VALID_FLAG == "1").Select((r, x) => x.WORKORDERNO).ToList().FirstOrDefault();
                    if (wPrefix.StartsWith("007"))
                    {
                        resRds = oleDB.ORM.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER, R_PRE_WO_HEAD>((rds, cs, cse, cc, rpw) => rds.SKUNO == rpw.GROUPID && rpw.PID == cs.SKUNO && cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                        .Where((rds, cs, cse, cc, rpw) => rds.DN_NO == dn && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc, rpw) => cc.CUSTOMER_NAME).ToList().FirstOrDefault();
                    }
                    else
                    {

                        resRds = oleDB.ORM.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER>((rds, cs, cse, cc) => rds.SKUNO == cs.SKUNO && cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                        .Where((rds, cs, cse, cc) => rds.DN_NO == dn && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc) => cc.CUSTOMER_NAME).ToList().FirstOrDefault();
                    }


                }
                else
                {
                    resRds = oleDB.ORM.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER>((rds, cs, cse, cc) => rds.SKUNO == cs.SKUNO && cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                    .Where((rds, cs, cse, cc) => rds.DN_NO == dn && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc) => cc.CUSTOMER_NAME).ToList().FirstOrDefault();
                }
                //var resRds = oleDB.ORM.Queryable<R_DN_STATUS,C_SKU,C_SERIES,C_CUSTOMER>((rds,cs,cse,cc)=>rds.SKUNO==cs.SKUNO && cs.C_SERIES_ID==cse.ID && cse.CUSTOMER_ID==cc.ID)
                //    .Where((rds, cs, cse, cc) => rds.DN_NO == dn && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc) =>cc.CUSTOMER_NAME).ToList().FirstOrDefault();

                if (string.IsNullOrEmpty(resRds))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MSGCODE20200703165013";
                    return;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = resRds;

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

        public void GetAsnDataByDnInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string dn = Data["Dn"].ToString().Trim(),
                dnLine = Data["DnLine"].ToString().Trim();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var resRds = oleDB.ORM.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER>((rds, cs, cse, cc) => rds.SKUNO == cs.SKUNO && cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                    .Where((rds, cs, cse, cc) => rds.DN_NO == dn && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc) => new { cc.CUSTOMER_NAME, rds.ID }).ToList().FirstOrDefault();
                if (resRds.CUSTOMER_NAME.ToUpper().Trim().Equals(Customer.NETGEAR.Ext<EnumValueAttribute>().Description))
                {
                    var res = oleDB.ORM.Queryable<R_NETGEAR_PTM_DATA>().Where(t => t.SHIPORDERID == resRds.ID)
                        .ToDataTable();
                    if (res == null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20200703165013";
                        return;
                    }
                    res.Columns.Remove("ID");
                    res.Columns.Remove("SHIPORDERID");
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000026";
                    StationReturn.Data = res;
                }
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

        public void GetAllInWhsPallet(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string pallet_no = Data["PALLET_NO"] == null ? "" : Data["PALLET_NO"].ToString().Trim();
                string skuno = Data["SKUNO"] == null ? "" : Data["SKUNO"].ToString().Trim();
                string status = Data["STATUS"] == null ? "" : Data["STATUS"].ToString().Trim();
                string date_from = Data["DATE_FROM"] == null ? "" : Data["DATE_FROM"].ToString().Trim();
                string date_to = Data["DATE_TO"] == null ? "" : Data["DATE_TO"].ToString().Trim();
                string page_size = Data["PAGE_SIZE"] == null ? "" : Data["PAGE_SIZE"].ToString().Trim();
                string page_num = Data["PAGE_NUM"] == null ? "" : Data["PAGE_NUM"].ToString().Trim();
                string skunoSql = "", dateSql = "", totalSql = "", pageSql = "", shipSql = "";
                status = status == "" ? "ALL" : status;
                page_size = page_size == "" ? "50" : page_size;
                page_num = page_num == "" ? "1" : page_num;

                if (!string.IsNullOrEmpty(skuno))
                {
                    skunoSql = $@" and rpp.skuno = '{skuno}'";
                }
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    dateSql = $@" and rsd.edit_time between to_date('{date_from} 00:00:00','yyyy/mm/dd HH24:mi:ss') and to_date('{date_to} 00:00:00','yyyy/mm/dd HH24:mi:ss')";
                }
                switch (status.ToUpper())
                {
                    case "ALL":
                        shipSql = " and sn.next_station in ('SHIPOUT','SHIPFINISH')";
                        break;
                    case "WAIT TO SHIP":
                        shipSql = " and sn.next_station='SHIPOUT' and not exists(select * from r_ship_detail rs where rs.sn=sn.sn) ";
                        break;
                    case "SHIPFINISH":
                        shipSql = " and sn.next_station='SHIPFINISH' and exists(select * from r_ship_detail rs where rs.sn=sn.sn)";
                        break;
                    default:
                        shipSql = "and sn.next_station in ('SHIPOUT','SHIPFINISH')";
                        break;
                }

                if (!string.IsNullOrEmpty(pallet_no))
                {
                    totalSql = $@"select * from (
                                    select rpp.pack_no,rpp.skuno,count(sn.sn) as qty, '' as status,max(rsd.edit_time) as cbs_date from
                                     r_sn_station_detail rsd,r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp
                                     where sn.id=rsp.sn_id and rpc.id=rsp.pack_id  and rpp.id=rpc.parent_pack_id and rpp.pack_type='PALLET'  
                                      and sn.valid_flag='1' and sn.id=rsd.r_sn_id and rsd.station_name='CBS' 
                                    and rpp.pack_no='{pallet_no}' {shipSql}
                                    group by rpp.pack_no,rpp.skuno)  ";
                }
                else
                {
                    totalSql = $@"select * from (
                                    select rpp.pack_no,rpp.skuno,count(sn.sn) as qty, '' as status,max(rsd.edit_time) as cbs_date from
                                     r_sn_station_detail rsd,r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp
                                     where sn.id=rsp.sn_id and rpc.id=rsp.pack_id  and rpp.id=rpc.parent_pack_id and rpp.pack_type='PALLET'  
                                     and sn.valid_flag='1' and sn.id=rsd.r_sn_id and rsd.station_name='CBS' 
                                     {skunoSql} {dateSql} {shipSql}
                                    group by rpp.pack_no,rpp.skuno)  ";
                }

                DataTable dt = SFCDB.ExecuteDataTable($@"select count(*) as total from ({totalSql}) temp", CommandType.Text, null);
                string total = dt.Rows[0]["TOTAL"].ToString() == "" ? "0" : dt.Rows[0]["TOTAL"].ToString();
                if (total == "0")
                {
                    throw new Exception("No Data!");
                }

                pageSql = $@"select * from (select temp.*,rownum as rn from ({totalSql} order by cbs_date desc ) temp ) where rn> ({page_num} - 1) * {page_size} and rn<= {page_num} * {page_size}";


                dt = SFCDB.ExecuteDataTable(pageSql, CommandType.Text, null);
                List<object> listObj = new List<object>();
                object obj = null;
                string no = "";
                string pack_no = "";
                string out_skuno = "";
                string qty = "";
                string out_status = "";
                string cbs_time = "";

                foreach (DataRow row in dt.Rows)
                {
                    no = row["RN"].ToString();
                    pack_no = row["PACK_NO"].ToString();
                    out_skuno = row["SKUNO"].ToString();
                    qty = row["QTY"].ToString();
                    out_status = GetPalletStatus(SFCDB, pack_no);
                    cbs_time = row["CBS_DATE"].ToString();
                    obj = new
                    {
                        NO = no,
                        PACK_NO = pack_no,
                        SKUNO = out_skuno,
                        QTY = qty,
                        STATUS = out_status,
                        CBS_DATE = cbs_time
                    };
                    listObj.Add(obj);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { Total = total, Rows = listObj };
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

        public void GetPalletDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string pallet_no = Data["PALLET_NO"] == null ? "" : Data["PALLET_NO"].ToString().Trim();
                string sql = "";
                if (string.IsNullOrEmpty(pallet_no))
                {
                    //throw new Exception("Please Input Pallet No!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "Pallet No" }));
                }
                sql = $@"select rpp.pack_no,rpp.skuno,sn.workorderno,sn.sn,sn.next_station,rsd.edit_time as csb_date,rs.dn_no,rs.dn_line,rs.shipdate
                         from r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp,r_sn_station_detail rsd,r_ship_detail rs
                        where sn.id=rsp.sn_id and rpc.id=rsp.pack_id and rpp.id=rpc.parent_pack_id  and rpp.pack_type='PALLET'
                         and sn.valid_flag='1' and sn.sn=rsd.sn and sn.id=rsd.r_sn_id and rsd.station_name='CBS' 
                         and sn.sn=rs.sn(+)  and rpp.pack_no='{pallet_no}' ";

                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = dt;
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
        private string GetPalletStatus(OleExec SFCDB, string pallet_no)
        {
            string sql = $@"select count(distinct sn.next_station) as row_station, next_station 
                            from r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp
                            where sn.id=rsp.sn_id and rpc.id=rsp.pack_id 
                            and rpp.id=rpc.parent_pack_id and rpp.pack_type='PALLET'
                            and sn.valid_flag='1' and rpp.pack_no='{pallet_no}' group by sn.next_station";
            DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            string next_station = "";
            string status = "";
            if (dt.Rows.Count != 1)
            {
                status = "More Next Station";
            }
            else
            {
                next_station = dt.Rows[0]["NEXT_STATION"].ToString();
                status = next_station;
                sql = $@"select * from r_ship_detail rs where rs.sn in (
                            select sn.sn  from r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp
                            where sn.id=rsp.sn_id and rpc.id=rsp.pack_id 
                            and rpp.id=rpc.parent_pack_id and rpp.pack_type='PALLET'
                            and sn.valid_flag='1' and rpp.pack_no='{pallet_no}')";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                switch (next_station)
                {
                    case "SHIPOUT":
                        if (dt.Rows.Count > 0)
                        {
                            status = "Exist Ship Detail,Next Station Is Shipout!";
                        }
                        break;
                    case "SHIPFINISH":
                        if (dt.Rows.Count == 0)
                        {
                            status = "Not Exist Ship Detail,Next Station Is Shipfinish!";
                        }
                        break;
                    default:
                        status = $@"Next Station [{next_station}] Error!";
                        break;
                }
            }
            return status;
        }

        private string GetPalletCBSDate(OleExec SFCDB, string pallet_no)
        {
            string sql = $@"select rpp.pack_no,rpp.skuno,to_char(rsd.edit_time,'yyyy/mm/dd') as cbs_date from
                             r_sn_station_detail rsd,r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp
                              where sn.id=rsp.sn_id and rpc.id=rsp.pack_id 
                            and rpp.id=rpc.parent_pack_id and rpp.pack_type='PALLET'  
                            and sn.next_station in ('SHIPOUT','SHIPFINISH')
                            and sn.valid_flag='1' and sn.id=rsd.r_sn_id and rsd.station_name='CBS' 
                            and rpp.pack_no='{pallet_no}'
                            group by rpp.pack_no,rpp.skuno,to_char(rsd.edit_time,'yyyy/mm/dd')";
            DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                return dt.Rows[0]["CBS_DATE"].ToString();
            }
        }

        private string GetPalletShipedDate(OleExec SFCDB, string pallet_no)
        {
            string sql = $@"select rpp.pack_no,rpp.skuno,to_char(rs.shipdate,'yyyy/mm/dd') as shipdate from
                             r_ship_detail rs,r_sn sn,r_sn_packing rsp,r_packing rpc,r_packing rpp
                              where sn.id=rsp.sn_id and rpc.id=rsp.pack_id 
                            and rpp.id=rpc.parent_pack_id  and rpp.pack_type='PALLET'  
                            and sn.next_station ='SHIPFINISH' and sn.valid_flag='1' and sn.sn=rs.sn
                            and rpp.pack_no='{pallet_no}'
                            group by rpp.pack_no,rpp.skuno,to_char(rs.shipdate,'yyyy/mm/dd')";
            DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                return dt.Rows[0]["SHIPDATE"].ToString();
            }
        }

        public void GetJuniperPoStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<O_PO_STATUS_MAP_J>().Where(t => t.NAME == "9" || t.NAME == "10" || t.NAME == "11" || t.NAME == "12" || t.NAME == "28" || t.NAME == "29" || t.NAME == "30" || t.NAME == "31").ToList();
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

        public void GetJuniperPoByStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string Status = Data["Status"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                #region version 1.0
                //var res = oleDB.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J, R_ORDER_WO>((a, b, c, d) => a.ID == b.POID && b.STATUSID == c.NAME && a.PREWO == d.WO)
                //    .Where((a, b, c, d) => c.NAME == Status && d.VALID == "1" && b.VALIDFLAG == "1")
                //    .Select((a, b, c, d) => new { a.PONO, a.POLINE, a.PID, a.PREWO, a.QTY, PO_FLAG = c.NAME, c.DESCRIPTION, a.POTYPE, a.PREASN, a.FINALASN, a.CREATETIME, a.EDITTIME }).OrderBy(a => a.PONO).OrderBy(a => a.POLINE).ToList();
                #endregion

                #region version 1.1
                //var res = oleDB.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J, R_ORDER_WO, O_I137_ITEM, R_WO_GROUPID>((a, b, c, d, e, f) => new object[] { JoinType.Inner,a.ID == b.POID,
                //    JoinType.Inner,b.STATUSID == c.NAME,JoinType.Inner,a.PREWO==d.WO,JoinType.Inner,a.ITEMID==e.ID, JoinType.Left,a.PREWO==f.WO})
                //    .Where((a, b, c, d, e, f) => c.NAME == Status && d.VALID == "1" && b.VALIDFLAG == "1")
                //    .Select((a, b, c, d, e, f) => new { a.PONO, a.POLINE, a.PID, a.PREWO, f.GROUPID, a.QTY, PO_FLAG = c.NAME, c.DESCRIPTION, e.PODELIVERYDATE, a.POTYPE, a.PREASN, a.FINALASN, a.CREATETIME, a.EDITTIME })
                //    .OrderBy(a => a.PONO).OrderBy(a => a.POLINE).ToList();
                #endregion

                #region version 1.2
                //var strSql = $@"
                //            select bb.*,    (case when bb.EarlyShipDate-sysdate>0 then 'N'
                //                                                    ELSE 'Y'
                //                                                    END ) AS  EarlyShipStatus   from (
                //            select aa.UPOID,
                //                                                   aa.PONO,
                //                       aa.POLINE,
                //                       aa.PID,
                //                       aa.PREWO,
                //                       aa.GROUPID,
                //                       aa.QTY,
                //                       aa.PO_FLAG,
                //                       aa.DESCRIPTION,
                //                       aa.PODELIVERYDATE,
                //                       aa.POTYPE,
                //                       aa.PREASN,
                //                       aa.FINALASN,
                //                       aa.EDITTIME,
                //                       (case
                //                         when aa.i282time is null then
                //                          'pending'
                //                         when aa.errorcode is not null then
                //                          'received with error'
                //                         when aa.deliverynumber is not null then
                //                          'received with DN'
                //                         else
                //                          'NA'
                //                       end) I282Status,
                //                       aa.salesordernumber,
                //                       aa.salesorderlineitem,
                //                       aa.custreqshipdate as CRSD,
                //                       case when TO_CHAR(aa.preasndate,'D')=1 then aa.preasndate-2
                //                         when TO_CHAR(aa.preasndate,'D')=7 then aa.preasndate-1
                //                           else  aa.preasndate end as EarlyShipDate                                    
                //                  from (select a.UPOID,
                //                               a.PONO,
                //                               a.POLINE,
                //                               a.PID,
                //                               a.PREWO,
                //                               f.GROUPID,
                //                               a.QTY,
                //                               c.NAME as PO_FLAG,
                //                               c.DESCRIPTION,
                //                               e.PODELIVERYDATE,
                //                               a.POTYPE,
                //                               a.PREASN,
                //                               a.FINALASN,
                //                               a.EDITTIME,
                //                               g.deliverynumber,
                //                               g.errorcode,
                //                               g.errordescription,
                //                               g.createtime as i282time,
                //                               h.salesordernumber,
                //                               e.salesorderlineitem,
                //                               e.custreqshipdate,               
                //                               (case 
                //                                 when e.podeliverydate=e.custreqshipdate then e.custreqshipdate                                           
                //                                 when e.podeliverydate<e.custreqshipdate then e.podeliverydate                                       
                //                                 when e.podeliverydate-e.custreqshipdate<=4 then e.custreqshipdate  
                //                                 else e.podeliverydate-4
                //                                 end) preasndate ,
                //                               row_number() over(partition by a.pono, a.poline, g.ASNNUMBER order by g.DELIVERYNUMBER asc, g.CREATETIME desc) r
                //                          from O_ORDER_MAIN a
                //                         inner join O_PO_STATUS b
                //                            on a.id = b.poid
                //                           and b.validflag = '1'
                //                         inner join O_PO_STATUS_MAP_J c
                //                            on c.name = b.statusid
                //                         inner join R_ORDER_WO d
                //                            on a.prewo = d.wo
                //                           and d.valid = '1'
                //                         inner join O_I137_ITEM e
                //                            on a.itemid = e.id
                //                          left join R_WO_GROUPID f
                //                            on a.prewo = f.wo
                //                          left join r_i282 g
                //                            on g.asnnumber = a.preasn
                //                          left join o_i137_head h
                //                          on e.tranid=h.tranid    
                //                         where c.name = '{Status}') aa
                //                  where aa.r = 1) bb   
                //                 order by bb.pono, bb.poline";
                #endregion

                #region version 1.3,add ORDERTYPE COL BY JD 2021-8-21 10:28,add completedelivery by ljd 2021-9-7 17:40
                var strSql = $@"
                            select bb.*,    (case when bb.EarlyShipDate-sysdate>0 then 'N'
                                                                    ELSE 'Y'
                                                                    END ) AS  EarlyShipStatus   from (
                            select aa.UPOID,
                                       aa.PONO,
                                       aa.POLINE,
                                       aa.PID,
                                       aa.PREWO,
                                       aa.GROUPID,
                                       aa.QTY,
                                       aa.PO_FLAG,
                                       aa.DESCRIPTION,
                                       aa.PODELIVERYDATE,
                                       aa.POTYPE,
                                       aa.PREASN,
                                       aa.FINALASN,
                                       aa.EDITTIME,
                                       aa.ORDERTYPE,
                                       (case
                                         when aa.i282time is null then
                                          'pending'
                                         when aa.errorcode is not null then
                                          'received with error'
                                         when aa.deliverynumber is not null then
                                          'received with DN'
                                         else
                                          'NA'
                                       end) I282Status,
                                       aa.salesordernumber,
                                       aa.salesorderlineitem,
                                       aa.custreqshipdate as CRSD,    
                                       aa.CSD,
                                       aa.preasndate as EarlyShipDate,
                                       aa.completedelivery
                                  from (select a.UPOID,
                                               a.PONO,
                                               a.POLINE,
                                               a.PID,
                                               a.PREWO,
                                               f.GROUPID,
                                               a.QTY,
                                               c.NAME as PO_FLAG,
                                               c.DESCRIPTION,
                                               e.PODELIVERYDATE,
                                               a.POTYPE,
                                               a.PREASN,
                                               a.FINALASN,
                                               a.EDITTIME,
                                               a.ORDERTYPE,
                                               g.deliverynumber,
                                               g.errorcode,
                                               g.errordescription,
                                               g.createtime as i282time,
                                               h.salesordernumber,
                                               h.completedelivery,
                                               e.salesorderlineitem,
                                               e.custreqshipdate, j.csd,       
                                               (case 
                                                 when e.podeliverydate=e.custreqshipdate then e.custreqshipdate                                           
                                                 when e.podeliverydate<e.custreqshipdate then e.podeliverydate                                       
                                                 when e.podeliverydate-e.custreqshipdate<=4 then e.custreqshipdate  
                                                 else e.podeliverydate-4
                                                 end) preasndate ,
                                               row_number() over(partition by a.pono, a.poline, g.ASNNUMBER order by g.DELIVERYNUMBER asc, g.CREATETIME desc) r
                                          from O_ORDER_MAIN a
                                         inner join O_PO_STATUS b
                                            on a.id = b.poid
                                           and b.validflag = '1'
                                         inner join O_PO_STATUS_MAP_J c
                                            on c.name = b.statusid
                                         inner join R_ORDER_WO d
                                            on a.prewo = d.wo
                                           and d.valid = '1'
                                         inner join O_I137_ITEM e
                                            on a.itemid = e.id
                                          left join R_WO_GROUPID f
                                            on a.prewo = f.wo
                                          left join r_i282 g
                                            on g.asnnumber = a.preasn
                                          left join o_i137_head h
                                          on e.tranid=h.tranid    
                                          left join r_jnp_csd_t j
                                          on a.pono=j.pono  and a.poline=j.poline and j.validflag='0'     
                                         where c.name = '{Status}') aa
                                  where aa.r = 1) bb   
                                 order by bb.pono, bb.poline";
                #endregion

                var res = oleDB.ORM.Ado.GetDataTable(strSql);

                if (Status == "12")
                {
                    for (int i = 0; i < res.Rows.Count; i++)
                    {
                        DataRow dr = res.Rows[i];
                        if (dr["I282Status"].ToString() == "pending")
                        {
                            var preasn = dr["PREASN"].ToString();
                            var ack = oleDB.ORM.Queryable<O_B2B_ACK>().Where(t => t.F_MSG_TYPE == "I139" && t.F_DOC_NO == preasn).First();
                            if (ack == null)
                            {
                                dr["I282Status"] = "NO I139 ACK";
                            }
                            else
                            {
                                if (ack.EXCEPTIONTYPE == "E")
                                {
                                    dr["I282Status"] = "I139 ACK Err:" + ack.F_MSG;
                                }
                            }

                        }
                    }
                }

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

        public void GetJuniperAsnDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string ASNNUMBER = Data["ASNNUMBER"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_I139>().Where(t => t.ASNNUMBER == ASNNUMBER && t.DELIVERYCODE == "01")
                    .Select(t => new
                    {
                        t.PONUMBER,
                        t.ITEM,
                        t.F_PLANT,
                        t.RECIPIENTID,
                        t.DELIVERYCODE,
                        t.ASNNUMBER,
                        t.ASNCREATIONTIME,
                        t.VENDORID,
                        t.SHIPTOID,
                        t.GROSSWEIGHT,
                        t.GROSSCODE,
                        t.VOLUMEWEIGHT,
                        t.VOLUMECODE,
                        t.ARRIVALDATE,
                        t.ISSUEDATE,
                        t.FREIGHTINVOICEID,
                        t.PN,
                        t.SPECIALREQUEST,
                        t.COO,
                        t.SERIALID,
                        t.SHIPPEDQUANTITY,
                        t.QUANTITY,
                        t.TRANID,
                        t.CREATETIME
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

        public void JuniperSendPreAsn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.PoolBorrowTimeOut = 3600;
                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString().Trim();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                string item = Data["ITEM"] == null ? "" : Data["ITEM"].ToString().Trim();
                string weight = Data["WEIGHT"] == null ? "" : Data["WEIGHT"].ToString().Trim();
                MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                var res = juniperAsn.BuildPreAsn(po, item, this.BU, weight, this);

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

        public void JuniperSendPreAsnCombination(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.PoolItem.BorrowTimeOut = 3600;
                string[] UpoidList = (string[])JsonConvert.Deserialize(Data["UPOIDLIST"].ToString(), typeof(string[]));
                string weight = Data["WEIGHT"] == null ? "" : Data["WEIGHT"].ToString().Trim();
                MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                var res = juniperAsn.BuildPreAsnByCombina(UpoidList, this.BU, weight, this);

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

        public void JuniperSendFinalAsn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.BeginTrain();

                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString().Trim();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                string item = Data["ITEM"] == null ? "" : Data["ITEM"].ToString().Trim();
                string weight = Data["WEIGHT"] == null ? "" : Data["WEIGHT"].ToString().Trim();
                MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                var res = juniperAsn.BuildFinalAsn(po, item, this.BU);

                SFCDB.CommitTrain();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = "";
            }
            catch (Exception exception)
            {
                SFCDB.RollbackTrain();

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

        public void JuniperCancelPreAsn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString().Trim();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                string item = Data["ITEM"] == null ? "" : Data["ITEM"].ToString().Trim();
                string weight = Data["WEIGHT"] == null ? "" : Data["WEIGHT"].ToString().Trim();
                MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                var res = juniperAsn.CancelPreAsn(po, item, this.BU, this.LoginUser.EMP_NO);

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

        public void TEST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {


                SFCDB = this.DBPools["SFCDB"].Borrow();

                //var sn = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN("JUNIPER_PCBA_SN_TEST", SFCDB);

                #region XXXX
                if (false)
                {
                    List<MESDataObject.Module.Juniper.JuniperAutoKpConfig> data = new List<MESDataObject.Module.Juniper.JuniperAutoKpConfig>();
                    MESDataObject.Module.Juniper.JuniperAutoKpConfig d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "MX240BASE3-DC",
                        CUST_PN = "",
                        PN_7XX = "740-063045",
                        SN_RULE = "MX240BASEX|'JN'@@@@@@@'AFC'",
                        QTY = 1,
                        TYPE = "PNO",
                        REV = "02",
                        CLEI_CODE = ""
                    };
                    data.Add(d);
                    d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "FFANTRAYMX240HC",
                        CUST_PN = "",
                        PN_7XX = "711-059360",
                        SN_RULE = "FANTRAY-MXXXXX|****####",
                        QTY = 1,
                        TYPE = "I137",
                        REV = "01",
                        CLEI_CODE = "COUCAKMBAB"
                    };
                    data.Add(d);
                    d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "PWRMX4802400DCB",
                        CUST_PN = "",
                        PN_7XX = "740-063045",
                        SN_RULE = "PWR-MX480-XXXX|'QCS'@@@@@@@@",
                        QTY = 1,
                        TYPE = "I137",
                        REV = "03",
                        CLEI_CODE = "COUPAF6EAC"
                    };
                    data.Add(d);
                    d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "RE-S-X6-64G-UB",
                        CUST_PN = "",
                        PN_7XX = "750-054758",
                        SN_RULE = "RE-S-X6|****####",
                        QTY = 1,
                        TYPE = "I137",
                        REV = "10",
                        CLEI_CODE = "COUCAU7BAB"
                    };
                    data.Add(d);
                    d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "SCBE2-MX-BB",
                        CUST_PN = "",
                        PN_7XX = "750-055976",
                        SN_RULE = "SCBE2-MX-XX|****####",
                        QTY = 1,
                        TYPE = "I137",
                        REV = "23",
                        CLEI_CODE = "COUCATYBAC"
                    };
                    data.Add(d);

                    d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "CHAS-BP3-MX240S",
                        CUST_PN = "",
                        PN_7XX = "750-047865",
                        SN_RULE = "CHAS-BPX-MXXX|****####",
                        QTY = 1,
                        TYPE = "SAP_HB",
                        REV = "01",
                        CLEI_CODE = "COMLN10BRA"
                    };
                    data.Add(d);
                    d = new MESDataObject.Module.Juniper.JuniperAutoKpConfig()
                    {
                        PN = "CRAFT-MX240-S",
                        CUST_PN = "",
                        PN_7XX = "760-021392",
                        SN_RULE = "CRAFT-MX|****####",
                        QTY = 1,
                        TYPE = "SAP_HB",
                        REV = "05",
                        CLEI_CODE = "COUCAEXBAB"
                    };
                    data.Add(d);
                    MESPubLab.Json.JsonSave.SaveToDB(data, "007A0000002L", "JuniperAutoKPConfig", "", SFCDB, BU, true);
                }
                #endregion

                string type = Data["TYPE"] == null ? "" : Data["TYPE"].ToString().Trim();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                string item = Data["ITEM"] == null ? "" : Data["ITEM"].ToString().Trim();
                string weight = Data["WEIGHT"] == null ? "" : Data["WEIGHT"].ToString().Trim();
                string cancel = Data["CANCEL"] == null ? "" : Data["CANCEL"].ToString().Trim();

                if (cancel == "CANCEL")
                {
                    MESJuniper.SendData.JuniperASNObj ff = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                    var res = ff.CancelPreAsn(po, item, this.BU, this.LoginUser.EMP_NO);
                }
                else if (type == "PREASN")
                {
                    MESJuniper.SendData.JuniperASNObj ff = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                    var res = ff.BuildPreAsn(po, item, this.BU, weight, this);
                }
                else if (type == "FINALASN")
                {
                    try
                    {
                        SFCDB.BeginTrain();
                        MESJuniper.SendData.JuniperASNObj ff = new MESJuniper.SendData.JuniperASNObj(SFCDB.ORM);
                        var res = ff.BuildFinalAsn(po, item, this.BU);
                        SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        SFCDB.RollbackTrain();
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    throw new Exception("type error");
                }

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

        public void GetJuniperPrintShippingLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string input_asn = Data["ASN"] == null ? "" : Data["ASN"].ToString().Trim();
                if (string.IsNullOrEmpty(input_asn))
                {
                    //throw new Exception("Please Input PO And PO Item!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "PO And PO Item" }));
                }
                if (input_asn.Equals("0"))
                {
                    //throw new Exception($@"[{input_asn}] PREASN Not Create Or Cancel!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143825", new string[] { input_asn }));
                }
                if (!input_asn.StartsWith("PRESHIP_"))
                {
                    //throw new Exception($@"[{input_asn}] PREASN ERROR!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151527", new string[] { input_asn }));
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list_o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO, I137_I>((o, r, i) => o.ID == r.ORIGINALID && i.ID == o.ITEMID && o.PONO == i.PONUMBER && o.POLINE == i.ITEM)
                    .Where((o, r, i) => o.PREASN == input_asn && r.VALID == "1" && i.ACTIONCODE != "02")
                    .OrderBy((o, r, i) => i.SALESORDERLINEITEM, OrderByType.Asc).Select((o, r, i) => o)
                    .ToList();
                list_o_main = list_o_main.Distinct().ToList();
                if (list_o_main.Count == 0)
                {
                    //throw new Exception($@"PREASN[{input_asn}] No Data To Print!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151913", new string[] { input_asn }));
                }
                bool bPrinted = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(r => r.PROGRAM_NAME == "PrintShippingLabel" && r.FUNCTION_NAME == "PrintShippingLabel" && r.DATA1 == input_asn).Any();

                bool bCheck = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDoubleCheck" && r.CATEGORY == "ShippingLabelCheck"
                                && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && SqlFunc.ToUpper(r.VALUE) == "YES").Any();
                if (!bPrinted && bCheck)
                {
                    //double check pallte/carton
                    JuniperASNDoubleCheck(SFCDB, input_asn, $@"PRE_ASN:{input_asn},ShippingLabelDoubleCheck");
                }

                #region 獲取模板
                R_F_CONTROL template_file = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl"
                  && r.CATEGORY == "ShippingLabelTemplate" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM")
                    .OrderBy(r => r.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (template_file == null)
                {
                    //throw new Exception("Juniper Shipping Label Template No Setting![R_F_CONTROL]");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152054"));
                }
                if (string.IsNullOrEmpty(template_file.VALUE))
                {
                    //throw new Exception("Juniper Shipping Label Template File Name Is Null![R_F_CONTROL]");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152446"));
                }
                if (string.IsNullOrEmpty(template_file.EXTVAL))
                {
                    //throw new Exception("The LABELTYPE Of Juniper Shipping Label Is Null![R_F_CONTROL]");                    
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "ShippingLabelTemplate's LabelType" }));
                }
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_Label tempLabel = SFCDB.ORM.Queryable<R_Label>().Where(r => r.LABELNAME == template_file.VALUE).ToList().FirstOrDefault();
                if (tempLabel == null)
                {
                    //throw new Exception($@"{template_file.VALUE} Packlist Label Not Exists.R_LABEL");      
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "Packlist Label: " + template_file.VALUE }));

                }
                T_R_Label TRL = new T_R_Label(SFCDB, DBTYPE);
                T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DBTYPE);

                Row_R_Label RL = TRL.GetLabelConfigByLabelName(tempLabel.LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(template_file.EXTVAL, SFCDB);
                if (RL == null)
                {
                    //throw new System.Exception($@"Can't Get Label File By LabelName:{tempLabel.LABELNAME}");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153446", new string[] { tempLabel.LABELNAME }));
                }
                if (RC == null)
                {
                    //throw new System.Exception($@"{template_file.EXTVAL},Label Type No Setting!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "LabelType: " + template_file.EXTVAL }));
                }
                MESPubLab.MESStation.Label.LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (MESPubLab.MESStation.Label.LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = MESPubLab.Json.JsonSave.GetFromDB<MESPubLab.MESStation.Label.ConfigableLabelBase>(RC.CLASS, SFCDB);
                    Lab = API_CLASS;
                }
                var label_input_asn = Lab.Inputs.Find(l => l.StationSessionType == "ASN" && l.StationSessionKey == "1");
                if (label_input_asn == null)
                {
                    //throw new System.Exception($@"{template_file.EXTVAL},Label Type No Setting [SessionType=ASN,SessionKey=1] Input!");
                }
                //label_input_asn.Value = input_asn;

                var label_input_po = Lab.Inputs.Find(l => l.StationSessionType == "PO" && l.StationSessionKey == "1");
                if (label_input_po == null)
                {
                    //throw new System.Exception($@"{template_file.EXTVAL},Label Type No Setting [SessionType=PO,SessionKey=1] Input!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { template_file.EXTVAL }));
                }
                label_input_po.Value = list_o_main.FirstOrDefault().PONO;
                var label_input_line = Lab.Inputs.Find(l => l.StationSessionType == "POLINE" && l.StationSessionKey == "1");
                if (label_input_line == null)
                {
                    //throw new System.Exception($@"{template_file.EXTVAL},Label Type No Setting [SessionType=POLINE,SessionKey=1] Input!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { template_file.EXTVAL }));

                }
                label_input_line.Value = list_o_main.FirstOrDefault().POLINE;

                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = 1;
                Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                Lab.MakeLabel(SFCDB);
                var noprint = Lab.Outputs.Find(t => t.Name == "NotPrint" && t.Value.ToString() == "TRUE");
                if (noprint != null)
                {
                    return;
                }
                List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);
                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                }
                Dictionary<string, List<MESPubLab.MESStation.Label.LabelBase>> LabelPrints = new Dictionary<string, List<MESPubLab.MESStation.Label.LabelBase>>();
                LabelPrints.Add(RL.R_FILE_NAME, pages);

                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, this.DBTYPE);
                R_MES_LOG log_object = new R_MES_LOG();
                log_object.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                log_object.PROGRAM_NAME = "PrintShippingLabel";
                log_object.CLASS_NAME = "MESStation.Config.WhsConfig";
                log_object.FUNCTION_NAME = "PrintShippingLabel";
                log_object.DATA1 = input_asn;
                log_object.EDIT_EMP = LoginUser.EMP_NO;
                log_object.EDIT_TIME = SFCDB.ORM.GetDate();

                SFCDB.ORM.Insertable<R_MES_LOG>(log_object).ExecuteCommand();

                StationReturn.Data = new { LabelPrints = LabelPrints };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";

                #endregion

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

        /// <summary>
        /// 下載整個ANS/DN的PACKING LIST
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetJuniperASNPackList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                //string input_po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                //string input_item = Data["ITEM"] == null ? "" : Data["ITEM"].ToString().Trim();
                //string input_wo = Data["WO"] == null ? "" : Data["WO"].ToString().Trim();
                string input_asn = Data["ASN"] == null ? "" : Data["ASN"].ToString().Trim();
                if (string.IsNullOrEmpty(input_asn))
                {
                    //throw new Exception("Please Input PO And PO Item!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "PO And PO Item" }));
                }
                if (input_asn.Equals("0"))
                {
                    //throw new Exception($@"[{input_asn}] PREASN Not Create Or Cancel!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143825", new string[] { input_asn }));
                }
                if (!input_asn.StartsWith("PRESHIP_"))
                {
                    //throw new Exception($@"[{input_asn}] PREASN ERROR!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151527", new string[] { input_asn }));
                }
                var list_o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO, I137_I>((o, r, i) => o.ID == r.ORIGINALID && i.ID == o.ITEMID && o.PONO == i.PONUMBER && o.POLINE == i.ITEM && o.PREWO == r.WO)
                    .Where((o, r, i) => o.PREASN == input_asn && r.VALID == "1" && i.ACTIONCODE != "02")
                    .OrderBy((o, r, i) => i.SALESORDERLINEITEM, OrderByType.Asc).Select((o, r, i) => o).ToList().Distinct().ToList();
                if (list_o_main.Count == 0)
                {
                    //throw new Exception($@"PREASN[{input_asn}] No Data To Print!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151913", new string[] { input_asn }));
                }

                string tran_id = SFCDB.ORM.Queryable<I137_I>().Where(r => r.ID == list_o_main.FirstOrDefault().ITEMID).ToList().FirstOrDefault().TRANID;
                I137_H firstHead = SFCDB.ORM.Queryable<I137_H>().Where(i => i.TRANID == tran_id && i.PONUMBER == list_o_main.FirstOrDefault().PONO).ToList().FirstOrDefault();
                if (firstHead == null)
                {
                    //throw new Exception($@"{input_asn},NOT EXISTS IN O_I137_HEAD!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163133", new string[] { input_asn }));
                }

                var list_i282 = SFCDB.ORM.Queryable<R_I282>().Where(i => i.ASNNUMBER == input_asn && SqlFunc.IsNullOrEmpty(i.ERRORCODE) == true).ToList();
                if (list_i282.Count == 0)
                {
                    //throw new Exception($@"{input_asn},No I282 Data!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152058", new string[] { input_asn }));
                }
                R_I282 first_i282 = list_i282.OrderByDescending(r => r.CREATETIME).ToList().FirstOrDefault();
                if (string.IsNullOrEmpty(first_i282.DELIVERYNUMBER))
                {
                    //throw new Exception($@"[{input_asn}] DN Number IS NULL");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163416", new string[] { input_asn }));
                }
                string dn = first_i282.DELIVERYNUMBER;

                bool bPrinted = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(r => r.PROGRAM_NAME == "PrintPackList" && r.FUNCTION_NAME == "PrintPackList" && r.DATA1 == input_asn && r.DATA2 == dn).Any();

                bool bCheck = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDoubleCheck" && r.CATEGORY == "PackListCheck"
                               && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && SqlFunc.ToUpper(r.VALUE) == "YES").Any();
                if (!bPrinted && bCheck)
                {
                    //double check pallte/carton
                    JuniperASNDoubleCheck(SFCDB, input_asn, $@"PRE_ASN:{input_asn},PrintPackListDoubleCheck");
                }

                var list_i139 = SFCDB.ORM.Queryable<R_I139>().Where(i => i.ASNNUMBER == input_asn && i.DELIVERYCODE == "01").ToList();
                R_I139 firstI139 = list_i139.OrderByDescending(r => r.CREATETIME).ToList().FirstOrDefault();

                PackingLabelValue label_value = new PackingLabelValue();
                label_value.DeliveryNumber = dn.TrimStart('0');
                #region Sold By的取值邏輯               
                switch (firstHead.SOLDBYORG)
                {
                    case "1000":
                    case "1010":
                        label_value.SoldBy = "Juniper Networks Inc.\n1133 Innovation Way\nSunnyvale, CA 94089\nUSA";
                        break;
                    case "3230":
                        //按PPT 來做先 2021.01.11
                        label_value.SoldBy = "Juniper Networks International B.V.\nBoeing Avenue 240\n1119 PZ Schiphol-Rijk\nAmsterdam\nNetherlands";
                        //string ShipToCountry = firstHead.SHIPTOCOUNTRYCODE;
                        //if (ShipToCountry != "AU" && ShipToCountry != "IN" && ShipToCountry != "UK/GB")
                        //{
                        //    label_value.SoldBy = "Juniper Networks International B.V.\nBoeing Avenue 240\n1119 PZ Schiphol-Rijk\nAmsterdam\nNetherlands";
                        //}
                        //else if (ShipToCountry == "UK" || ShipToCountry == "GB")
                        //{
                        //    label_value.SoldBy = "Juniper Networks (UK) Limited\nBuilding 1, Aviator Park, Station Rd\nAddlestone KT15 2PG\nUK";
                        //}
                        //else if (ShipToCountry == "AU")
                        //{
                        //    label_value.SoldBy = "Juniper Networks Australia Pty Limited\nLevel 26/55 Collins Street\nMelbourne VIC 3000\nAustralia";
                        //}
                        //else if (ShipToCountry == "IN")
                        //{
                        //    label_value.SoldBy = "Juniper Networks Solution India Private Limited\nSurvey No.111/1 to 115/4, Wing A &B\nAmane Belandur Khane Village\n2nd Floor, Elnath-Exora Business Park\nMarathahalli, Sarjapur Outer Ring Road, Bangalore -560 -103";
                        //}
                        //else
                        //{
                        //    throw new Exception($@"3230 {ShipToCountry} Not Setting!");
                        //}
                        break;
                    case "3011":
                        label_value.SoldBy = "Juniper Networks (UK) Limited\nBuilding 1, Aviator Park, Station Rd\nAddlestone KT15 2PG\nUK";
                        break;
                    case "2810":
                        label_value.SoldBy = "Juniper Networks Australia Pty Limited\nLevel 26/55 Collins Street\nMelbourne VIC 3000\nAustralia";
                        break;
                    case "2380":
                        label_value.SoldBy = "Juniper Networks Solution India Private Limited\nSurvey No.111/1 to 115/4, Wing A &B\nAmane Belandur Khane Village\n2nd Floor, Elnath-Exora Business Park\nMarathahalli, Sarjapur Outer Ring Road, Bangalore -560 -103";
                        break;
                    default:
                        throw new Exception("PO BILL TO COMPANY ERROR!");
                }
                #endregion

                #region Bill To取值邏輯                
                string bill_to_line1 = firstHead.BILLTOID;
                string bill_to_line2 = firstHead.BILLTOCOMPANY;
                string bill_to_line3 = firstHead.BILLTOHOUSEID;
                string bill_to_line4 = firstHead.BILLTOSTREETNAME == "NA" ? "" : firstHead.BILLTOSTREETNAME;
                string bill_to_city = firstHead.BILLTOCITYNAME;
                string bill_to_region_code = firstHead.BILLTOREGIONCODE == "NA" ? "" : firstHead.BILLTOREGIONCODE;//區域代碼
                string bill_to_postal_code = firstHead.BILLTOSTREETPOSTALCODE;//郵政編碼
                string bill_to_line5 = "";

                if (string.IsNullOrEmpty(bill_to_region_code))
                {
                    bill_to_line5 = $@"{bill_to_city} {bill_to_postal_code}";
                }
                else
                {
                    bill_to_line5 = $@"{bill_to_city} {bill_to_region_code} {bill_to_postal_code}";
                }
                string bill_to_line6 = firstHead.BILLTOCOUNTRYCODE;
                if (!string.IsNullOrEmpty(bill_to_line4))
                {
                    label_value.BillTo = $@"{bill_to_line1}\n{bill_to_line2}\n{bill_to_line3}\n{bill_to_line4}\n{bill_to_line5}\n{bill_to_line6}";
                }
                else
                {
                    label_value.BillTo = $@"{bill_to_line1}\n{bill_to_line2}\n{bill_to_line3}\n{bill_to_line5}\n{bill_to_line6}";
                }
                #endregion

                #region Ship To取值邏輯  shipping label

                string ship_to_line1 = firstHead.SHIPTOID;
                string ship_to_line2 = firstHead.SHIPTOCOMPANY;
                string ship_to_line3 = firstHead.SHIPTOHOUSEID;
                string ship_to_line4 = firstHead.SHIPTOSTREETNAME;
                string ship_to_city = firstHead.SHIPTOCITYNAME;
                string ship_to_region_code = firstHead.SHIPTOREGIONCODE == "NA" ? "" : firstHead.SHIPTOREGIONCODE;//區域代碼
                string ship_to_postal_code = firstHead.SHIPTOSTREETPOSTALCODE;//郵政編碼
                string ship_to_line5 = "";
                if (string.IsNullOrEmpty(ship_to_region_code))
                {
                    ship_to_line5 = $@"{ship_to_city} {ship_to_postal_code}";
                }
                else
                {
                    ship_to_line5 = $@"{ship_to_city} {ship_to_region_code} {ship_to_postal_code}";
                }
                string ship_to_line6 = firstHead.SHIPTOCOUNTRYCODE;
                if (!string.IsNullOrEmpty(ship_to_line4))
                {
                    label_value.ShipTo = $@"{ship_to_line1}\n{ship_to_line2}\n{ship_to_line3}\n{ship_to_line4}\n{ship_to_line5}\n{ship_to_line6}";
                }
                else
                {
                    label_value.ShipTo = $@"{ship_to_line1}\n{ship_to_line2}\n{ship_to_line3}\n{ship_to_line5}\n{ship_to_line6}";
                }
                #endregion

                label_value.ShippingNotes = (firstHead.SHIPPINGNOTE == "NA" || string.IsNullOrEmpty(firstHead.SHIPPINGNOTE)) ? "" : firstHead.SHIPPINGNOTE;//500字 後期手動調整，只打在第一頁
                label_value.ForwardingAgent = (firstHead.SOFRTCARRIER == "NA" || string.IsNullOrEmpty(firstHead.SOFRTCARRIER)) ? "" : firstHead.SOFRTCARRIER;
                label_value.ShipVia = "";//永遠為空值               
                label_value.IncoTermPlace = $@"{firstHead.INCO1} / {firstHead.INCO2}";
                label_value.LVAS = (firstHead.SHIPPMETHOD == "NA" || string.IsNullOrEmpty(firstHead.SHIPPMETHOD)) ? "" : firstHead.SHIPPMETHOD;

                if (string.IsNullOrEmpty(firstI139.GROSSWEIGHT))
                {
                    //throw new Exception($@"{input_asn} GROSSWEIGHT Is Null Or Empty!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163701", new string[] { input_asn }));
                }
                //1kg=2.205lb
                double total_kg = Convert.ToDouble(firstI139.GROSSWEIGHT);
                decimal total_lb = Math.Round((decimal)(total_kg * 2.205), 2, MidpointRounding.AwayFromZero);
                label_value.TotalWeight = $@"{total_kg}/{total_lb}";
                double net_kg = Convert.ToDouble(firstI139.NETWEIGHT);
                decimal net_lb = Math.Round((decimal)(net_kg * 2.205), 2, MidpointRounding.AwayFromZero);
                label_value.TotalNetWeight = $@"{net_kg}/{net_lb}";

                var asn_item = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO, O_I137_ITEM>((o, r, i) => o.ID == r.ORIGINALID && o.ITEMID == i.ID && o.PONO == i.PONUMBER && o.POLINE == i.ITEM)
                     .Where((o, r, i) => o.PREASN == input_asn && r.VALID == "1" && i.ACTIONCODE != "02")
                    .OrderBy((o, r, i) => o.POLINE, OrderByType.Asc)
                    .Select((o, r, i) => i).ToList();

                //label_value.TotalPieces = asn_item.Select(l => l.SALESORDERLINEITEM).Distinct().ToList().Count().ToString();//妈蛋，改
                //label_value.TotalPieces = list_i139.Select(r => new { r.PONUMBER, r.ITEM, r.QUANTITY }).Distinct().Sum(r => int.Parse(r.QUANTITY)).ToString();//妈蛋，又改
                var listPack = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_SN, R_SN_PACKING, R_PACKING>
                    ((OM, rs, rsp, rp) => OM.PREWO == rs.WORKORDERNO && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                    .Where((OM, rs, rsp, rp) => OM.PREASN == input_asn && rs.VALID_FLAG == "1")
                    .Select((OM, rs, rsp, rp) => rp.ID);

                label_value.TotalPieces = listPack.Distinct().Count().ToString();//改到这里，妈蛋
                label_value.TotalCartons = listPack.Distinct().Count().ToString();
                label_value.OrderNumber = string.IsNullOrEmpty(firstHead.SALESORDERNUMBER) ? "" : firstHead.SALESORDERNUMBER.TrimStart('0');
                if (firstHead.SODATE == null)
                {
                    //throw new Exception($@"{input_asn} SODATE O_I137_HEAD Error!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163910", new string[] { input_asn }));
                }
                label_value.OrderDate = ((DateTime)firstHead.SODATE).ToString("dd-MMM-yyyy", new System.Globalization.CultureInfo("en-us"));
                label_value.CustomerPO = (firstHead.CUSTOMERPONUMBER == "NA" || string.IsNullOrEmpty(firstHead.CUSTOMERPONUMBER)) ? "" : firstHead.CUSTOMERPONUMBER;
                label_value.SalesPerson = (firstHead.SALESPERSON == "NA" || string.IsNullOrEmpty(firstHead.SALESPERSON)) ? "" : firstHead.SALESPERSON;
                label_value.ContactPerson = (firstHead.SHIPTODEVIATINGFULLNAME == "NA" || string.IsNullOrEmpty(firstHead.SHIPTODEVIATINGFULLNAME)) ? "" : firstHead.SHIPTODEVIATINGFULLNAME;


                #region 每條詳細數據 普通BTS,BTS BNDL,CTO 都不一樣
                List<PackingLabelItem> listItem = new List<PackingLabelItem>();
                string qtln = "";
                string ln = "";// BTS SALESORDERLINEITEM
                string pn = "";//bts pn
                string cp = "";
                string pdn = "";//根據 i.PN 來查   ///COT CUSTOMER_PART_NUMBER DESCRIPTION
                string uom = "";//bts 固定EA,
                string order_qty = "";
                string ship_qty = "";
                string cpr = "";
                string clei = "";
                Label.Public.JuniperGroup juniperLabelGroup = new Label.Public.JuniperGroup();
                foreach (var om in list_o_main)
                {
                    bool bcancel = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == om.ITEMID && i.PONUMBER == om.PONO && i.ITEM == om.POLINE && i.ACTIONCODE == "02").Any();
                    if (bcancel)
                    {
                        //CANCEL 掉的 PONO ITEM 不送
                        continue;
                    }
                    // Parent CustomerPartNumber=O_I137_ITEM.CUSTPRODID
                    var listOII = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == om.ITEMID && i.PONUMBER == om.PONO && i.ITEM == om.POLINE).ToList();
                    // Child CustomerPartNumber=O_I137_DETAIL.COMCUSTPRODID
                    var listOID = SFCDB.ORM.Queryable<I137_D>().Where(i => i.TRANID == listOII.FirstOrDefault().TRANID && i.PONUMBER == om.PONO && i.ITEM == om.POLINE).ToList();

                    var oi = listOII.Find(t => t.ID == om.ITEMID);
                    var cl2 = oi.CARTONLABEL2;
                    if (cl2 == null)
                    {
                        cl2 = "";
                    }
                    if (om.POTYPE == "BTS")
                    {
                        #region BTS                        
                        bool isBundle = juniperLabelGroup.IsBundle(SFCDB, om.PREWO, om.ITEMID);

                        if (isBundle && cl2.ToUpper() != "BULK")
                        {
                            #region BNDL
                            var bndl_first = listOII.Find(r => string.IsNullOrEmpty(r.MATERIALID) == false);
                            if (bndl_first == null)
                            {
                                //throw new Exception($@"{om.PONO},{om.POLINE} IS BNDL,BUT MATERIALID IS NULL!");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164059", new string[] { om.PONO, om.POLINE }));
                            }

                            var first_attr = GetAglieAttrByCust(SFCDB, bndl_first.MATERIALID, om.PLANT);
                            PackingLabelItem bndl = new PackingLabelItem();
                            bndl.QtLn = string.IsNullOrEmpty(bndl_first.SOID) ? "" : bndl_first.SOID.TrimStart('0');
                            bndl.Ln = string.IsNullOrEmpty(bndl_first.SOID) ? "" : bndl_first.SOID.TrimStart('0');
                            bndl.ProductNumber = bndl_first.MATERIALID;
                            bndl.CustomerPartNumber = "";// bndl_first.PN,
                            bndl.ProductDescription = first_attr == null ? "" : first_attr.DESCRIPTION;
                            bndl.UoM = "";
                            bndl.SerialNumber = "";
                            bndl.OrderQty = ChangeToF3(bndl_first.SOQTY);// ChangeToF3(bndl_first.QUANTITY);//2021.05.04 Tat-Ho 要求改爲
                            bndl.ShipQty = ChangeToF3(bndl_first.SOQTY);// ChangeToF3(bndl_first.QUANTITY);//2021.05.04 Tat-Ho 要求改爲
                            bndl.CLEI = "";
                            bndl.CPR = "";
                            listItem.Add(bndl);
                            if (bndl.ProductDescription.Length == 0)
                            {
                                bndl.ProductDescription = "(Bundle Parent)";
                            }
                            else if (bndl.ProductDescription.Length < 25)
                            {
                                bndl.ProductDescription += "\r\n(Bundle Parent)";
                            }
                            else
                            {
                                listItem.Add(new PackingLabelItem
                                {
                                    BNDLParent = true,
                                    QtLn = "",
                                    Ln = "",
                                    ProductNumber = "",
                                    CustomerPartNumber = "",// bndl_first.PN,
                                    ProductDescription = "(Bundle Parent)",
                                    UoM = "",
                                    SerialNumber = "",
                                    OrderQty = "",
                                    ShipQty = "",
                                    CLEI = "",
                                    CPR = ""
                                });
                            }
                            var bndl_child = listOII.OrderBy(i => i.SALESORDERLINEITEM);
                            foreach (var bc in bndl_child)
                            {
                                var bndl_sn = list_i139.Where(i => i.PONUMBER == om.PONO && i.ITEM == om.POLINE).ToList();
                                foreach (var i139 in bndl_sn)
                                {
                                    PackingLabelItem pli = new PackingLabelItem();
                                    pli.QtLn = string.IsNullOrEmpty(bndl_first.SOID) ? "" : bndl_first.SOID.TrimStart('0');
                                    pli.Ln = string.IsNullOrEmpty(bc.SALESORDERLINEITEM) ? "" : bc.SALESORDERLINEITEM.TrimStart('0');
                                    pli.ProductNumber = i139.PN;
                                    //var i_item = listOII.Where(i => i.PN == i139.PN).ToList().FirstOrDefault();
                                    //pli.CustomerPartNumber = i_item == null ? "" : i_item.CUSTPRODID; 
                                    var i_item = listOID.Where(i => i.COMPONENTID == i139.PN && i.COMSALESORDERLINEITEM == bc.SALESORDERLINEITEM).ToList().FirstOrDefault();
                                    pli.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                    O_AGILE_ATTR aglie_bndl = GetAglieAttrByCust(SFCDB, i139.PN, om.PLANT);
                                    pli.ProductDescription = aglie_bndl == null ? "" : aglie_bndl.DESCRIPTION;
                                    pli.UoM = bc.UNITCODE;
                                    pli.OrderQty = ChangeToF3(i139.QUANTITY);
                                    pli.ShipQty = ChangeToF3(pli.OrderQty);
                                    pli.CPR = aglie_bndl == null ? "" : aglie_bndl.CPR_CODE;
                                    pli.CLEI = SFCDB.ORM.Queryable<R_SN_KP>()
                                            .Where(r => r.VALID_FLAG == 1 && r.EXKEY2 == "CLEI" && r.SN == i139.SERIALID)
                                            .Select(r => r.EXVALUE2).ToList().FirstOrDefault();
                                    pli.SerialNumber = i139.SERIALID;
                                    listItem.Add(pli);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region NOT BNDL
                            bool hideBom = SFCDB.ORM.Queryable<R_SAP_HB>().Where(r => r.WO == om.PREWO && SqlFunc.StartsWith(r.CUSTPARENTPN, "HB")).Any();
                            var bts_sn = list_i139.Where(i => i.PONUMBER == om.PONO && i.ITEM == om.POLINE).Select(r => r.SERIALID).Distinct().ToList();
                            var bts_first = listOII.FirstOrDefault();
                            qtln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');  // BTS SALESORDERLINEITEM
                            ln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');// BTS SALESORDERLINEITEM
                            pn = bts_first.PN;//bts pn
                            cp = bts_first.CUSTPRODID;
                            O_AGILE_ATTR aglie_bts = GetAglieAttrByCust(SFCDB, bts_first.PN, om.PLANT); //根據 i.PN 來查  ///COT CUSTOMER_PART_NUMBER DESCRIPTION  
                            pdn = aglie_bts == null ? "" : aglie_bts.DESCRIPTION;
                            uom = bts_first.UNITCODE; //bts 固定EA,                           
                            order_qty = om.QTY;  //BTS po line 總數                            
                            ship_qty = order_qty; //BTS po line 總數                            
                            cpr = aglie_bts == null ? "" : aglie_bts.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                            var b_sn = bts_sn[0];
                            clei = SFCDB.ORM.Queryable<R_SN_KP>()
                                       .Where(r => r.SN == b_sn && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALID_FLAG == 1 && r.EXKEY2 == "CLEI")
                                       .Select(r => r.EXVALUE2).ToList().FirstOrDefault();
                            //2021.03.02 BTS NOT BNDL do not to print hide bom info
                            hideBom = false;
                            if (hideBom)
                            {
                                foreach (var s in bts_sn)
                                {
                                    var i054Tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == s).OrderBy(r => r.CREATETIME, OrderByType.Desc)
                                        .Select(r => r.TRANID).ToList().FirstOrDefault();
                                    if (string.IsNullOrEmpty(i054Tranid))
                                    {
                                        //throw new Exception($@"{s} Not In I054");
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164515", new string[] { s }));
                                    }
                                    var i054_parent = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Parent").ToList();
                                    if (i054_parent.Count > 1)
                                    {
                                        //throw new Exception($@"SN:{s},TRANID:{i054Tranid} More Parent In I054");
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170115", new string[] { s, i054Tranid }));
                                    }
                                    if (i054_parent.Count == 0)
                                    {
                                        //throw new Exception($@"WO:{om.PREWO} Have Hide Bom,But SN:{s},TRANID:{i054Tranid} No Parent In I054");
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170558", new string[] { om.PREWO, s, i054Tranid }));
                                    }
                                    listItem.Add(new PackingLabelItem
                                    {
                                        QtLn = qtln,
                                        Ln = ln,
                                        ProductNumber = i054_parent.FirstOrDefault().PARENTMODEL,
                                        CustomerPartNumber = cp,
                                        ProductDescription = aglie_bts == null ? "" : aglie_bts.DESCRIPTION,
                                        UoM = uom,
                                        SerialNumber = i054_parent.FirstOrDefault().PARENTSN,
                                        OrderQty = ChangeToF3(om.QTY),
                                        ShipQty = ChangeToF3(om.QTY),
                                        CLEI = "",// clei,
                                        CPR = ""// cpr
                                    });
                                    var i054_child = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child")
                                        .OrderBy(r => r.CHILDMATERIAL, OrderByType.Asc).OrderBy(r => r.SN, OrderByType.Asc).ToList();
                                    foreach (var ic in i054_child)
                                    {
                                        var i054_aglie = GetAglieAttrByCust(SFCDB, ic.CHILDMATERIAL, om.PLANT);
                                        var qty = SFCDB.ORM.Queryable<R_I054>()
                                            .Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child" && r.CHILDMATERIAL == ic.CHILDMATERIAL)
                                            .ToList().Count().ToString();

                                        PackingLabelItem plItem = new PackingLabelItem();
                                        plItem.QtLn = qtln;
                                        plItem.Ln = "";
                                        plItem.ProductNumber = ic.CHILDMATERIAL;
                                        //var i_item = listOID.Where(i => i.COMPONENTID == ic.CHILDMATERIAL && string.IsNullOrEmpty(i.COMSALESORDERLINEITEM)).ToList().FirstOrDefault();
                                        //plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                        plItem.CustomerPartNumber = "";
                                        plItem.ProductDescription = i054_aglie == null ? "" : i054_aglie.DESCRIPTION;
                                        plItem.UoM = uom;
                                        plItem.OrderQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                        plItem.ShipQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                        plItem.CPR = i054_aglie == null ? "" : i054_aglie.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                        plItem.CLEI = ic.CLEICODE;
                                        plItem.SerialNumber = ic.SN;
                                        listItem.Add(plItem);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var s in bts_sn)
                                {
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = qtln;
                                    plItem.Ln = ln;
                                    plItem.ProductNumber = pn;
                                    plItem.CustomerPartNumber = cp;
                                    plItem.ProductDescription = pdn;
                                    plItem.UoM = uom;
                                    plItem.OrderQty = ChangeToF3(order_qty); ;//BTS po line 總數
                                    plItem.ShipQty = ChangeToF3(order_qty);//BTS po line 總數
                                    plItem.CPR = cpr; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = clei;//BTS 一樣只顯示一行
                                    plItem.SerialNumber = s;//BTS 139 SERIALID
                                    listItem.Add(plItem);
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        bool printBTS = false;
                        var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                            .Where((c, s) => c.SKUNO == om.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                            .ToList().FirstOrDefault();
                        bool sku_control = cseries == null ? false : true;
                        bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                              && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == om.PREWO).Any();
                        printBTS = sku_control ? sku_control : wo_control;
                        if (printBTS)
                        {
                            var bts_sn = list_i139.Where(i => i.PONUMBER == om.PONO && i.ITEM == om.POLINE).Select(r => r.SERIALID).Distinct().ToList();
                            var bts_first = listOII.FirstOrDefault();
                            qtln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');  // BTS SALESORDERLINEITEM
                            ln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');// BTS SALESORDERLINEITEM
                            pn = bts_first.PN;//bts pn
                            cp = bts_first.CUSTPRODID;
                            O_AGILE_ATTR aglie_bts = GetAglieAttrByCust(SFCDB, bts_first.PN, om.PLANT); //根據 i.PN 來查  ///COT CUSTOMER_PART_NUMBER DESCRIPTION  
                            pdn = aglie_bts == null ? "" : aglie_bts.DESCRIPTION;
                            uom = bts_first.UNITCODE; //bts 固定EA,                           
                            order_qty = om.QTY;  //BTS po line 總數                            
                            ship_qty = order_qty; //BTS po line 總數                            
                            cpr = aglie_bts == null ? "" : aglie_bts.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                            var b_sn = bts_sn[0];
                            clei = SFCDB.ORM.Queryable<R_SN_KP>()
                                       .Where(r => r.SN == b_sn && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALID_FLAG == 1 && r.EXKEY2 == "CLEI")
                                       .Select(r => r.EXVALUE2).ToList().FirstOrDefault();
                            foreach (var s in bts_sn)
                            {
                                var i054Tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == s).OrderBy(r => r.CREATETIME, OrderByType.Desc)
                                    .Select(r => r.TRANID).ToList().FirstOrDefault();
                                if (string.IsNullOrEmpty(i054Tranid))
                                {
                                    //throw new Exception($@"{s} Not In I054");
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164515", new string[] { s }));
                                }
                                var i054_parent = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Parent").ToList();
                                if (i054_parent.Count > 1)
                                {
                                    //throw new Exception($@"SN:{s},TRANID:{i054Tranid} More Parent In I054");
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170115", new string[] { s, i054Tranid }));
                                }
                                if (i054_parent.Count == 0)
                                {
                                    //throw new Exception($@"WO:{om.PREWO} Have Hide Bom,But SN:{s},TRANID:{i054Tranid} No Parent In I054");
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170558", new string[] { om.PREWO, s, i054Tranid }));
                                }
                                listItem.Add(new PackingLabelItem
                                {
                                    QtLn = qtln,
                                    Ln = ln,
                                    ProductNumber = i054_parent.FirstOrDefault().PARENTMODEL,
                                    CustomerPartNumber = cp,
                                    ProductDescription = aglie_bts == null ? "" : aglie_bts.DESCRIPTION,
                                    UoM = uom,
                                    SerialNumber = i054_parent.FirstOrDefault().PARENTSN,
                                    OrderQty = ChangeToF3(om.QTY),
                                    ShipQty = ChangeToF3(om.QTY),
                                    CLEI = "",// clei,
                                    CPR = ""// cpr
                                });
                                var child_hide = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child"
                                    && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == true)
                                    .OrderBy(r => r.CHILDMATERIAL, OrderByType.Asc).OrderBy(r => r.SN, OrderByType.Asc).ToList();
                                foreach (var ch in child_hide)
                                {
                                    var i054_aglie = GetAglieAttrByCust(SFCDB, ch.CHILDMATERIAL, om.PLANT);
                                    var qty = SFCDB.ORM.Queryable<R_I054>()
                                        .Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child" && r.CHILDMATERIAL == ch.CHILDMATERIAL
                                        && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == true
                                        ).ToList().Count().ToString();

                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = qtln;
                                    plItem.Ln = string.IsNullOrEmpty(ch.SALESORDERLINENUMBER) ? "" : ch.SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.ProductNumber = ch.CHILDMATERIAL;
                                    var i_item = listOID.Where(i => i.COMPONENTID == ch.CHILDMATERIAL && i.COMSALESORDERLINEITEM == ch.SALESORDERLINENUMBER).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                    //plItem.CustomerPartNumber = "";
                                    plItem.ProductDescription = i054_aglie == null ? "" : i054_aglie.DESCRIPTION;
                                    plItem.UoM = uom;
                                    plItem.OrderQty = string.IsNullOrEmpty(ch.SN) ? ChangeToF3(ch.QTY) : ChangeToF3(qty);
                                    plItem.ShipQty = string.IsNullOrEmpty(ch.SN) ? ChangeToF3(ch.QTY) : ChangeToF3(qty);
                                    plItem.CPR = i054_aglie == null ? "" : i054_aglie.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = ch.CLEICODE;
                                    plItem.SerialNumber = ch.SN;
                                    listItem.Add(plItem);
                                }

                                var i054_child = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child"
                                    && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false)
                                    .OrderBy(r => r.CHILDMATERIAL, OrderByType.Asc).OrderBy(r => r.SN, OrderByType.Asc).ToList();
                                foreach (var ic in i054_child)
                                {
                                    var i054_aglie = GetAglieAttrByCust(SFCDB, ic.CHILDMATERIAL, om.PLANT);
                                    var qty = SFCDB.ORM.Queryable<R_I054>()
                                        .Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child" && r.CHILDMATERIAL == ic.CHILDMATERIAL && r.SALESORDERLINENUMBER == ic.SALESORDERLINENUMBER)
                                        .ToList().Count().ToString();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = qtln;
                                    plItem.Ln = string.IsNullOrEmpty(ic.SALESORDERLINENUMBER) ? "" : ic.SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.ProductNumber = ic.CHILDMATERIAL;
                                    var i_item = listOID.Where(i => i.COMPONENTID == ic.CHILDMATERIAL && i.COMSALESORDERLINEITEM == ic.SALESORDERLINENUMBER).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                    //plItem.CustomerPartNumber = "";
                                    plItem.ProductDescription = i054_aglie == null ? "" : i054_aglie.DESCRIPTION;
                                    plItem.UoM = uom;
                                    plItem.OrderQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                    plItem.ShipQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                    plItem.CPR = i054_aglie == null ? "" : i054_aglie.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = ic.CLEICODE;
                                    plItem.SerialNumber = ic.SN;
                                    listItem.Add(plItem);
                                }
                            }
                        }
                        else
                        {
                            #region CTO
                            //CTO的數據從R_I244來
                            var so = firstHead.SALESORDERNUMBER.TrimStart('0');
                            var cto_sn = list_i139.Where(i => i.PONUMBER == om.PONO && i.ITEM == om.POLINE).Select(r => r.SERIALID).Distinct().ToList();
                            foreach (var csn in cto_sn)
                            {
                                var i244 = SFCDB.ORM.Queryable<R_I244>().Where(r => r.SALESORDERNUMBER == so && r.PARENTSN == csn).ToList();
                                var parent = i244.FindAll(r => r.PNTYPE == "Parent");
                                string uom_cto = "";
                                foreach (var p in parent)
                                {
                                    O_AGILE_ATTR aglie_p = GetAglieAttrByCust(SFCDB, p.PARENTMODEL, om.PLANT);
                                    var iil = listOII.Where(r => r.PN == p.PARENTMODEL).ToList();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = p.SALESORDERLINENUMBER;
                                    plItem.Ln = p.SALESORDERLINENUMBER;
                                    plItem.ProductNumber = p.PARENTMODEL;
                                    plItem.CustomerPartNumber = iil.Count == 0 ? "" : iil.FirstOrDefault().CUSTPRODID;//???
                                    plItem.ProductDescription = aglie_p == null ? "" : aglie_p.DESCRIPTION;
                                    plItem.UoM = iil.Count == 0 ? "" : iil.FirstOrDefault().UNITCODE;
                                    plItem.OrderQty = ChangeToF3(om.QTY);
                                    plItem.ShipQty = ChangeToF3(om.QTY);
                                    plItem.CPR = ""; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = "";
                                    plItem.SerialNumber = p.PARENTSN;
                                    listItem.Add(plItem);
                                    uom_cto = plItem.UoM;
                                }
                                var hidebom = i244.FindAll(r => r.PNTYPE == "Child" && string.IsNullOrEmpty(r.SALESORDERLINENUMBER));
                                foreach (var h in hidebom)
                                {
                                    O_AGILE_ATTR aglie_h = GetAglieAttrByCust(SFCDB, h.CHILDMATERIAL, om.PLANT);

                                    string h_qty = i244.Where(r => r.CHILDMATERIAL == h.CHILDMATERIAL && r.SALESORDERLINENUMBER == h.SALESORDERLINENUMBER).ToList().Count().ToString();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = string.IsNullOrEmpty(parent.FirstOrDefault().SALESORDERLINENUMBER) ? "" : parent.FirstOrDefault().SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.Ln = "";
                                    plItem.ProductNumber = h.CHILDMATERIAL;

                                    //var iil = listOII.Where(r => r.PN == h.CHILDMATERIAL).ToList();
                                    //plItem.CustomerPartNumber = iil.FirstOrDefault() == null ? "" : iil.FirstOrDefault().CUSTPRODID;

                                    var i_item = listOID.Where(i => i.COMPONENTID == h.CHILDMATERIAL && string.IsNullOrEmpty(i.SALESORDERLINEITEM)).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;

                                    plItem.ProductDescription = aglie_h == null ? "" : aglie_h.DESCRIPTION;
                                    plItem.UoM = uom_cto;
                                    plItem.OrderQty = ChangeToF3(h_qty);
                                    plItem.ShipQty = ChangeToF3(h_qty);
                                    plItem.CPR = aglie_h == null ? "" : aglie_h.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = h.CLEICODE;
                                    plItem.SerialNumber = h.SN;
                                    listItem.Add(plItem);
                                }
                                var child = i244.FindAll(r => r.PNTYPE == "Child" && !string.IsNullOrEmpty(r.SALESORDERLINENUMBER)).OrderBy(r => r.SALESORDERLINENUMBER);
                                foreach (var c in child)
                                {
                                    O_AGILE_ATTR aglie_c = GetAglieAttrByCust(SFCDB, c.CHILDMATERIAL, om.PLANT);
                                    string c_qty = (i244.Where(r => r.CHILDMATERIAL == c.CHILDMATERIAL && r.SALESORDERLINENUMBER == c.SALESORDERLINENUMBER).ToList().Count()).ToString();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = string.IsNullOrEmpty(parent.FirstOrDefault().SALESORDERLINENUMBER) ? "" : parent.FirstOrDefault().SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.Ln = string.IsNullOrEmpty(c.SALESORDERLINENUMBER) ? "" : c.SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.ProductNumber = c.CHILDMATERIAL;

                                    //var iil = listOII.Where(r => r.PN == c.CHILDMATERIAL).ToList();
                                    //plItem.CustomerPartNumber = iil.FirstOrDefault() == null ? "" : iil.FirstOrDefault().CUSTPRODID;

                                    var i_item = listOID.Where(i => i.COMPONENTID == c.CHILDMATERIAL && i.COMSALESORDERLINEITEM == c.SALESORDERLINENUMBER).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;

                                    plItem.ProductDescription = aglie_c == null ? "" : aglie_c.DESCRIPTION;
                                    plItem.UoM = uom_cto;
                                    plItem.OrderQty = ChangeToF3(c_qty);
                                    plItem.ShipQty = plItem.OrderQty;
                                    plItem.CPR = aglie_c == null ? "" : aglie_c.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = c.CLEICODE;
                                    plItem.SerialNumber = c.SN;
                                    listItem.Add(plItem);
                                }
                            }
                            #endregion
                        }
                    }
                }
                label_value.ItemList = listItem;
                #endregion

                #region 獲取模板
                R_F_CONTROL template_file = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl"
                  && r.CATEGORY == "PackingListTemplate" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM")
                    .OrderBy(r => r.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (template_file == null)
                {
                    //throw new Exception("Juniper Packing List Template No Setting![R_F_CONTROL]");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "PackingListTemplate" }));

                }
                if (string.IsNullOrEmpty(template_file.VALUE))
                {
                    //throw new Exception("Juniper Packing List Template File Name Is Null![R_F_CONTROL]");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "PackingListTemplate's Name" }));
                }
                //if (string.IsNullOrEmpty(template_file.EXTVAL))
                //{
                //    throw new Exception("Juniper Packing List Template First Number Is Null![R_F_CONTROL]");
                //}
                //R_F_CONTROL_EX template_ex = SFCDB.ORM.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == template_file.ID && r.NAME == "LastNumber")
                //    .OrderBy(r => r.SEQ_NO, OrderByType.Desc).ToList().FirstOrDefault();
                //if (template_ex == null)
                //{
                //    throw new Exception("Juniper Packing List Template  Last Number No Setting![R_F_CONTROL_EX]");
                //}
                //if (string.IsNullOrEmpty(template_ex.VALUE))
                //{
                //    throw new Exception("Juniper Packing List Template  Last Number Is Null![R_F_CONTROL_EX]");
                //}
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_Label tempLabel = SFCDB.ORM.Queryable<R_Label>().Where(r => r.LABELNAME == template_file.VALUE).ToList().FirstOrDefault();
                //R_Label tempLabel = SFCDB.ORM.Queryable<R_Label>().Where(r => r.LABELNAME == "JuniperPackingListRevised.xlsx").ToList().FirstOrDefault();
                if (tempLabel == null)
                {
                    //throw new Exception($@"{template_file.VALUE} Packlist Label Not Exists.R_LABEL");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "PackingListTemplate's Name" }));
                }
                R_FILE file = TRF.GetFileByFileName(tempLabel.R_FILE_NAME, "LABEL", SFCDB);
                if (file == null)
                {
                    //throw new Exception($@"{tempLabel.R_FILE_NAME} Not Exists.R_FILE");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "Label:" + tempLabel.R_FILE_NAME }));
                }
                Row_R_FILE rowFile = (Row_R_FILE)TRF.GetObjByID(file.ID, SFCDB);
                byte[] template = (byte[])rowFile["BLOB_FILE"];

                //int first_num = 0;
                //int last_num = 0;
                //try
                //{
                //    first_num = Convert.ToInt32(template_file.EXTVAL);
                //    last_num = Convert.ToInt32(template_ex.VALUE);
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception($@"Packing List Template File First/Last Page Number Error!{ex.Message}");
                //}
                string excel_name = $@"(PREASN){input_asn}_(DN){dn}_{SFCDB.ORM.GetDate().ToString("yyyyMMddHHmmss")}.xlsx";
                //用於在服務器保存生成的PacklingList Excel
                string path = System.IO.Directory.GetCurrentDirectory() + "\\PackingList\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                #endregion
                //byte[] output_excel = SetPacklingValue_02(path, excel_name, label_value, template, first_num, last_num);
                byte[] output_excel = SetPacklingValue_03(path, excel_name, label_value, template);
                string content = Convert.ToBase64String(output_excel);

                DateTime sysdate = SFCDB.ORM.GetDate();

                if (!bPrinted)
                {
                    foreach (var m in list_o_main)
                    {
                        SFCDB.ORM.Updateable<O_PO_STATUS>().SetColumns(r => r.VALIDFLAG == "0").Where(r => r.POID == m.ID && r.VALIDFLAG == "1").ExecuteCommand();
                        O_PO_STATUS _28Status = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(SFCDB.ORM, BU),
                            POID = m.ID,
                            STATUSID = "28",
                            VALIDFLAG = "1",
                            CREATETIME = sysdate,
                            EDITTIME = sysdate
                        };
                        SFCDB.ORM.Insertable<O_PO_STATUS>(_28Status).ExecuteCommand();
                    }
                }
                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, this.DBTYPE);
                R_MES_LOG log_object = new R_MES_LOG();
                log_object.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                log_object.PROGRAM_NAME = "PrintPackList";
                log_object.CLASS_NAME = "MESStation.Config.WhsConfig";
                log_object.FUNCTION_NAME = "PrintPackList";
                log_object.DATA1 = input_asn;
                log_object.DATA2 = dn;
                log_object.EDIT_EMP = LoginUser.EMP_NO;
                log_object.EDIT_TIME = sysdate;
                SFCDB.ORM.Insertable<R_MES_LOG>(log_object).ExecuteCommand();

                StationReturn.Data = new { FileName = excel_name, Content = content };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
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


        public void GetJuniperDOAPrintShippingLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string input_asn = Data["ASN"] == null ? "" : Data["ASN"].ToString().Trim();
                if (string.IsNullOrEmpty(input_asn))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "PO And PO Item" }));
                }
                if (input_asn.Equals("0"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143825", new string[] { input_asn }));
                }
                if (!input_asn.StartsWith("PRESHIP_"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151527", new string[] { input_asn }));
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list_o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO, I137_I>((o, r, i) => o.ID == r.ORIGINALID && i.ID == o.ITEMID && o.PONO == i.PONUMBER && o.POLINE == i.ITEM)
                    .Where((o, r, i) => o.PREASN == input_asn && r.VALID == "1" && i.ACTIONCODE != "02")
                    .OrderBy((o, r, i) => i.SALESORDERLINEITEM, OrderByType.Asc).Select((o, r, i) => o)
                    .ToList();
                list_o_main = list_o_main.Distinct().ToList();
                if (list_o_main.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151913", new string[] { input_asn }));
                }
                bool bPrinted = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(r => r.PROGRAM_NAME == "PrintShippingLabel" && r.FUNCTION_NAME == "PrintShippingLabel" && r.DATA1 == input_asn).Any();

                bool bCheck = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDoubleCheck" && r.CATEGORY == "ShippingLabelCheck"
                                && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && SqlFunc.ToUpper(r.VALUE) == "YES").Any();
                if (!bPrinted && bCheck)
                {
                    JuniperASNDoubleCheck(SFCDB, input_asn, $@"PRE_ASN:{input_asn},ShippingLabelDoubleCheck");
                }

                #region 獲取模板
                R_F_CONTROL template_file = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl"
                  && r.CATEGORY == "DOAShippingLabelTemplate" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM")
                    .OrderBy(r => r.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (template_file == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152054"));
                }
                if (string.IsNullOrEmpty(template_file.VALUE))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152446"));
                }
                if (string.IsNullOrEmpty(template_file.EXTVAL))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "DOAShippingLabelTemplate's LabelType" }));
                }
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_Label tempLabel = SFCDB.ORM.Queryable<R_Label>().Where(r => r.LABELNAME == template_file.VALUE).ToList().FirstOrDefault();
                if (tempLabel == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "Packlist Label: " + template_file.VALUE }));

                }
                T_R_Label TRL = new T_R_Label(SFCDB, DBTYPE);
                T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DBTYPE);

                Row_R_Label RL = TRL.GetLabelConfigByLabelName(tempLabel.LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(template_file.EXTVAL, SFCDB);
                if (RL == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153446", new string[] { tempLabel.LABELNAME }));
                }
                if (RC == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "LabelType: " + template_file.EXTVAL }));
                }
                MESPubLab.MESStation.Label.LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (MESPubLab.MESStation.Label.LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = MESPubLab.Json.JsonSave.GetFromDB<MESPubLab.MESStation.Label.ConfigableLabelBase>(RC.CLASS, SFCDB);
                    Lab = API_CLASS;
                }
                var label_input_asn = Lab.Inputs.Find(l => l.StationSessionType == "ASN" && l.StationSessionKey == "1");
                if (label_input_asn == null)
                {
                    //throw new System.Exception($@"{template_file.EXTVAL},Label Type No Setting [SessionType=ASN,SessionKey=1] Input!");
                }
                //label_input_asn.Value = input_asn;

                var label_input_po = Lab.Inputs.Find(l => l.StationSessionType == "PO" && l.StationSessionKey == "1");
                if (label_input_po == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { template_file.EXTVAL }));
                }
                label_input_po.Value = list_o_main.FirstOrDefault().PONO;
                var label_input_line = Lab.Inputs.Find(l => l.StationSessionType == "POLINE" && l.StationSessionKey == "1");
                if (label_input_line == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { template_file.EXTVAL }));

                }
                label_input_line.Value = list_o_main.FirstOrDefault().POLINE;

                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = 1;
                Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                Lab.MakeLabel(SFCDB);
                var noprint = Lab.Outputs.Find(t => t.Name == "NotPrint" && t.Value.ToString() == "TRUE");
                if (noprint != null)
                {
                    return;
                }
                List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);
                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                }
                Dictionary<string, List<MESPubLab.MESStation.Label.LabelBase>> LabelPrints = new Dictionary<string, List<MESPubLab.MESStation.Label.LabelBase>>();
                LabelPrints.Add(RL.R_FILE_NAME, pages);

                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, this.DBTYPE);
                R_MES_LOG log_object = new R_MES_LOG();
                log_object.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                log_object.PROGRAM_NAME = "PrintShippingLabel";
                log_object.CLASS_NAME = "MESStation.Config.WhsConfig";
                log_object.FUNCTION_NAME = "PrintShippingLabel";
                log_object.DATA1 = input_asn;
                log_object.EDIT_EMP = LoginUser.EMP_NO;
                log_object.EDIT_TIME = SFCDB.ORM.GetDate();

                SFCDB.ORM.Insertable<R_MES_LOG>(log_object).ExecuteCommand();

                StationReturn.Data = new { LabelPrints = LabelPrints };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";

                #endregion

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
        public void GetJuniperDOAPackList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string input_asn = Data["ASN"] == null ? "" : Data["ASN"].ToString().Trim();
                if (string.IsNullOrEmpty(input_asn))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "PO And PO Item" }));
                }
                if (input_asn.Equals("0"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143825", new string[] { input_asn }));
                }
                if (!input_asn.StartsWith("PRESHIP_"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151527", new string[] { input_asn }));
                }
                var list_o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO, I137_I>((o, r, i) => o.ID == r.ORIGINALID && i.ID == o.ITEMID && o.PONO == i.PONUMBER && o.POLINE == i.ITEM && o.PREWO == r.WO)
                    .Where((o, r, i) => o.PREASN == input_asn && r.VALID == "1" && i.ACTIONCODE != "02")
                    .OrderBy((o, r, i) => i.SALESORDERLINEITEM, OrderByType.Asc).Select((o, r, i) => o).ToList().Distinct().ToList();
                if (list_o_main.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151913", new string[] { input_asn }));
                }

                string tran_id = SFCDB.ORM.Queryable<I137_I>().Where(r => r.ID == list_o_main.FirstOrDefault().ITEMID).ToList().FirstOrDefault().TRANID;
                I137_H firstHead = SFCDB.ORM.Queryable<I137_H>().Where(i => i.TRANID == tran_id && i.PONUMBER == list_o_main.FirstOrDefault().PONO).ToList().FirstOrDefault();
                if (firstHead == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163133", new string[] { input_asn }));
                }

                var list_smAck = SFCDB.ORM.Queryable<R_JNP_DOA_SHIPMENTS_ACK>().Where(i => i.ASNNUMBER == input_asn && SqlFunc.IsNullOrEmpty(i.MESSAGE_CODE) == true).ToList();
                if (list_smAck.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152058", new string[] { input_asn }));
                }
                var first_smAck = list_smAck.OrderByDescending(r => r.CREATETIME).ToList().FirstOrDefault();
                if (string.IsNullOrEmpty(first_smAck.DELIVERYNUMBER))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163416", new string[] { input_asn }));
                }
                string dn = first_smAck.DELIVERYNUMBER;

                bool bPrinted = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(r => r.PROGRAM_NAME == "PrintPackList" && r.FUNCTION_NAME == "PrintPackList" && r.DATA1 == input_asn && r.DATA2 == dn).Any();

                bool bCheck = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperDoubleCheck" && r.CATEGORY == "PackListCheck"
                               && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && SqlFunc.ToUpper(r.VALUE) == "YES").Any();
                if (!bPrinted && bCheck)
                {
                    JuniperASNDoubleCheck(SFCDB, input_asn, $@"PRE_ASN:{input_asn},PrintPackListDoubleCheck");
                }

                var list_shipment = SFCDB.ORM.Queryable<R_JNP_DOA_SHIPMENTS>().Where(i => i.ASNNUMBER == input_asn).ToList();
                var firstShipment = list_shipment.OrderByDescending(r => r.CREATETIME).ToList().FirstOrDefault();

                #region calc weight
                double poWeight = 0.0; //取重量
                double poNetWeight = 0.0; //取重量NetWeight
                foreach (var v in list_o_main)
                {
                    double poLineWeight = 0.0;
                    double poLineNetWeight = 0.0;
                    var sql = $@"SELECT a.sn, b.pack_id, c.snid, c.weight
                            FROM r_sn a
                                LEFT JOIN r_sn_packing   b ON a.id = b.sn_id
                                LEFT JOIN r_weight       c ON b.pack_id = c.snid
                            WHERE a.valid_flag = '1' and a.workorderno = '{v.PREWO}'
                                AND c.snid IS NULL";
                    var dt = SFCDB.ORM.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} SN {dt.Rows[0]["SN"].ToString()} is missing the Weight or/and Weight UOM");
                    }
                    var tt = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_WEIGHT>((d, e, f) => d.ID == e.SN_ID && e.PACK_ID == f.SNID)
                        .Where((d, e, f) => d.VALID_FLAG == "1" && d.WORKORDERNO == v.PREWO)
                        .Select((d, e, f) => new { f.SNID, f.WEIGHT }).Distinct().ToList();
                    if (tt.Count > 0)
                    {
                        poLineWeight = Math.Round(tt.Sum(t => SqlFunc.ToDouble(t.WEIGHT)), 2);//取2位小數就好 2021-12-22
                    }
                    if (poLineWeight <= 0)
                    {
                        throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the Weight or/and Weight UOM");
                    }
                    poWeight = poWeight + poLineWeight;

                    var cSkuType = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((CSK, CSE) => CSK.C_SERIES_ID == CSE.ID)
                        .Where((CSK, CSE) => CSK.SKUNO == v.PID && CSK.SKU_TYPE == "CTO" && CSE.SERIES_NAME == "Juniper-Configurable System")
                        .Select((CSK, CSE) => CSK)
                        .ToList().FirstOrDefault();
                    if (cSkuType == null)
                    {
                        //var cSkuDetail = _db.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT" && t.SKUNO == v.PID).ToList().FirstOrDefault();
                        var cSkuDetail = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && (t.CATEGORY_NAME == "NET_WEIGHT" || t.CATEGORY_NAME == "NET_WEIGHT_CTO") && t.SKUNO == v.PID).ToList().FirstOrDefault();
                        if (cSkuDetail == null)
                        {
                            throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} , C_SKU_DETAIL not setting NetWeight");
                        }
                        else
                        {
                            poLineNetWeight = Double.Parse(cSkuDetail.VALUE) * Double.Parse(v.QTY);
                        }
                        if (poLineNetWeight <= 0)
                        {
                            throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the NetWeight");
                        }
                        if (poLineNetWeight > poLineWeight)
                        {
                            throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} net weight > gross weight");
                        }
                        poNetWeight = poNetWeight + poLineNetWeight;
                    }
                    else
                    {
                        var cSkuDetail = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT_CTO" && t.SKUNO == v.PID).ToList().FirstOrDefault();
                        if (cSkuDetail == null)
                        {
                            throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} SKU IS CTO, C_SKU_DETAIL NOT SETTING NET_WEIGHT_CTO");
                        }
                        else
                        {
                            poLineNetWeight = poLineWeight - Double.Parse(cSkuDetail.VALUE) * Double.Parse(v.QTY);
                        }
                        if (poLineNetWeight <= 0)
                        {
                            throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} is missing the NetWeight");
                        }
                        if (poLineNetWeight > poLineWeight)
                        {
                            throw new Exception($@"PO-LINE {v.PONO}-{v.POLINE} net weight > gross weight");
                        }
                        poNetWeight = poNetWeight + poLineNetWeight;
                    }
                }

                //poWeight = poWeight + Double.Parse(weight);//重量需加上傳進來的棧板重量
                if (poWeight <= 0)
                {
                    throw new Exception($@"missing the Weight or/and Weight UOM");
                }
                if (poNetWeight <= 0)
                {
                    throw new Exception($@"missing the NetWeight or/and NetWeight UOM");
                }
                #endregion

                PackingLabelValue label_value = new PackingLabelValue();
                label_value.DeliveryNumber = dn.TrimStart('0');

                #region Sold By的取值邏輯               
                label_value.SoldBy = "Cloud Network Technology Singapore Pte Ltd\n54 Genting Lane 03 05\nRuby Land Complex\nSunnyvale, CA 94089\nSingapore 349562";
                #endregion

                #region Bill To取值邏輯  
                label_value.BillTo = "Juniper Networks, Inc.\nAttn Accounts Payable\nPO Box 410\nWestford MA 01886\nUS";
                #endregion

                #region Ship To取值邏輯  shipping label

                string ship_to_line1 = firstHead.SHIPTOID;
                string ship_to_line2 = firstHead.SHIPTOCOMPANY;
                string ship_to_line3 = firstHead.SHIPTOHOUSEID;
                string ship_to_line4 = firstHead.SHIPTOSTREETNAME;
                string ship_to_city = firstHead.SHIPTOCITYNAME;
                string ship_to_region_code = firstHead.SHIPTOREGIONCODE == "NA" ? "" : firstHead.SHIPTOREGIONCODE;//區域代碼
                string ship_to_postal_code = firstHead.SHIPTOSTREETPOSTALCODE;//郵政編碼
                string ship_to_line5 = "";
                if (string.IsNullOrEmpty(ship_to_region_code))
                {
                    ship_to_line5 = $@"{ship_to_city} {ship_to_postal_code}";
                }
                else
                {
                    ship_to_line5 = $@"{ship_to_city} {ship_to_region_code} {ship_to_postal_code}";
                }
                string ship_to_line6 = firstHead.SHIPTOCOUNTRYCODE;
                if (!string.IsNullOrEmpty(ship_to_line4))
                {
                    label_value.ShipTo = $@"{ship_to_line1}\n{ship_to_line2}\n{ship_to_line3}\n{ship_to_line4}\n{ship_to_line5}\n{ship_to_line6}";
                }
                else
                {
                    label_value.ShipTo = $@"{ship_to_line1}\n{ship_to_line2}\n{ship_to_line3}\n{ship_to_line5}\n{ship_to_line6}";
                }
                #endregion

                label_value.ShippingNotes = "FGI DOA";
                label_value.ForwardingAgent = "Crossdock - El Paso";
                label_value.ShipVia = "";
                label_value.IncoTermPlace = "DDP / DEST";
                label_value.LVAS = "";


                //1kg=2.205lb
                double total_kg = Convert.ToDouble(poWeight);
                decimal total_lb = Math.Round((decimal)(total_kg * 2.205), 2, MidpointRounding.AwayFromZero);
                label_value.TotalWeight = $@"{total_kg}/{total_lb}";
                double net_kg = Convert.ToDouble(poNetWeight);
                decimal net_lb = Math.Round((decimal)(net_kg * 2.205), 2, MidpointRounding.AwayFromZero);
                label_value.TotalNetWeight = $@"{net_kg}/{net_lb}";

                var asn_item = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO, O_I137_ITEM>((o, r, i) => o.ID == r.ORIGINALID && o.ITEMID == i.ID && o.PONO == i.PONUMBER && o.POLINE == i.ITEM)
                     .Where((o, r, i) => o.PREASN == input_asn && r.VALID == "1" && i.ACTIONCODE != "02")
                    .OrderBy((o, r, i) => o.POLINE, OrderByType.Asc)
                    .Select((o, r, i) => i).ToList();

                //label_value.TotalPieces = asn_item.Select(l => l.SALESORDERLINEITEM).Distinct().ToList().Count().ToString();
                //label_value.TotalPieces = list_shipment.Select(t => new { t.PO_NUMBER, t.PO_LINE_NO, t.SHIPPED_QTY }).Distinct().Sum(t => t.SHIPPED_QTY).ToString();
                var listPack = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_SN, R_SN_PACKING, R_PACKING>
                    ((OM, rs, rsp, rp) => OM.PREWO == rs.WORKORDERNO && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                    .Where((OM, rs, rsp, rp) => OM.PREASN == input_asn && rs.VALID_FLAG == "1")
                    .Select((OM, rs, rsp, rp) => rp.ID);

                label_value.TotalPieces = listPack.Distinct().Count().ToString();
                label_value.TotalCartons = listPack.Distinct().Count().ToString();
                label_value.OrderNumber = string.IsNullOrEmpty(firstHead.SALESORDERNUMBER) ? "" : firstHead.SALESORDERNUMBER.TrimStart('0');
                if (firstHead.CREATETIME == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163910", new string[] { input_asn }));
                }
                label_value.OrderDate = ((DateTime)firstHead.CREATETIME).ToString("dd-MMM-yyyy", new System.Globalization.CultureInfo("en-us"));
                label_value.CustomerPO = (firstHead.CUSTOMERPONUMBER == "NA" || string.IsNullOrEmpty(firstHead.CUSTOMERPONUMBER)) ? "" : firstHead.CUSTOMERPONUMBER;
                label_value.SalesPerson = (firstHead.SALESPERSON == "NA" || string.IsNullOrEmpty(firstHead.SALESPERSON)) ? "" : firstHead.SALESPERSON;
                label_value.ContactPerson = (firstHead.SHIPTODEVIATINGFULLNAME == "NA" || string.IsNullOrEmpty(firstHead.SHIPTODEVIATINGFULLNAME)) ? "" : firstHead.SHIPTODEVIATINGFULLNAME;


                #region 每條詳細數據 普通BTS,BTS BNDL,CTO 都不一樣
                List<PackingLabelItem> listItem = new List<PackingLabelItem>();
                string qtln = "";
                string ln = "";// BTS SALESORDERLINEITEM
                string pn = "";//bts pn
                string cp = "";
                string pdn = "";//根據 i.PN 來查   ///COT CUSTOMER_PART_NUMBER DESCRIPTION
                string uom = "";//bts 固定EA,
                string order_qty = "";
                string ship_qty = "";
                string cpr = "";
                string clei = "";
                Label.Public.JuniperGroup juniperLabelGroup = new Label.Public.JuniperGroup();
                foreach (var om in list_o_main)
                {
                    bool bcancel = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == om.ITEMID && i.PONUMBER == om.PONO && i.ITEM == om.POLINE && i.ACTIONCODE == "02").Any();
                    if (bcancel)
                    {
                        //CANCEL 掉的 PONO ITEM 不送
                        continue;
                    }
                    // Parent CustomerPartNumber=O_I137_ITEM.CUSTPRODID
                    var listOII = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == om.ITEMID && i.PONUMBER == om.PONO && i.ITEM == om.POLINE).ToList();
                    // Child CustomerPartNumber=O_I137_DETAIL.COMCUSTPRODID
                    var listOID = SFCDB.ORM.Queryable<I137_D>().Where(i => i.TRANID == listOII.FirstOrDefault().TRANID && i.PONUMBER == om.PONO && i.ITEM == om.POLINE).ToList();

                    var oi = listOII.Find(t => t.ID == om.ITEMID);
                    var cl2 = oi.CARTONLABEL2;
                    if (cl2 == null)
                    {
                        cl2 = "";
                    }
                    if (om.POTYPE == "BTS")
                    {
                        #region BTS                        
                        bool isBundle = juniperLabelGroup.IsBundle(SFCDB, om.PREWO, om.ITEMID);

                        if (isBundle && cl2.ToUpper() != "BULK")
                        {
                            #region BNDL
                            var bndl_first = listOII.Find(r => string.IsNullOrEmpty(r.MATERIALID) == false);
                            if (bndl_first == null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164059", new string[] { om.PONO, om.POLINE }));
                            }

                            var first_attr = GetAglieAttrByCust(SFCDB, bndl_first.MATERIALID, om.PLANT);
                            PackingLabelItem bndl = new PackingLabelItem();
                            bndl.QtLn = string.IsNullOrEmpty(bndl_first.SOID) ? "" : bndl_first.SOID.TrimStart('0');
                            bndl.Ln = string.IsNullOrEmpty(bndl_first.SOID) ? "" : bndl_first.SOID.TrimStart('0');
                            bndl.ProductNumber = bndl_first.MATERIALID;
                            bndl.CustomerPartNumber = "";// bndl_first.PN,
                            bndl.ProductDescription = first_attr == null ? "" : first_attr.DESCRIPTION;
                            bndl.UoM = "";
                            bndl.SerialNumber = "";
                            bndl.OrderQty = ChangeToF3(bndl_first.SOQTY);// ChangeToF3(bndl_first.QUANTITY);//2021.05.04 Tat-Ho 要求改爲
                            bndl.ShipQty = ChangeToF3(bndl_first.SOQTY);// ChangeToF3(bndl_first.QUANTITY);//2021.05.04 Tat-Ho 要求改爲
                            bndl.CLEI = "";
                            bndl.CPR = "";
                            listItem.Add(bndl);
                            if (bndl.ProductDescription.Length == 0)
                            {
                                bndl.ProductDescription = "(Bundle Parent)";
                            }
                            else if (bndl.ProductDescription.Length < 25)
                            {
                                bndl.ProductDescription += "\r\n(Bundle Parent)";
                            }
                            else
                            {
                                listItem.Add(new PackingLabelItem
                                {
                                    BNDLParent = true,
                                    QtLn = "",
                                    Ln = "",
                                    ProductNumber = "",
                                    CustomerPartNumber = "",// bndl_first.PN,
                                    ProductDescription = "(Bundle Parent)",
                                    UoM = "",
                                    SerialNumber = "",
                                    OrderQty = "",
                                    ShipQty = "",
                                    CLEI = "",
                                    CPR = ""
                                });
                            }
                            var bndl_child = listOII.OrderBy(i => i.SALESORDERLINEITEM);
                            foreach (var bc in bndl_child)
                            {
                                var bndl_sn = list_shipment.Where(i => i.PO_NUMBER == om.PONO && i.PO_LINE_NO == om.POLINE).ToList();
                                foreach (var shipment in bndl_sn)
                                {
                                    PackingLabelItem pli = new PackingLabelItem();
                                    pli.QtLn = string.IsNullOrEmpty(bndl_first.SOID) ? "" : bndl_first.SOID.TrimStart('0');
                                    pli.Ln = string.IsNullOrEmpty(bc.SALESORDERLINEITEM) ? "" : bc.SALESORDERLINEITEM.TrimStart('0');
                                    pli.ProductNumber = shipment.PART_NUMBER;
                                    var i_item = listOID.Where(i => i.COMPONENTID == shipment.PART_NUMBER && i.COMSALESORDERLINEITEM == bc.SALESORDERLINEITEM).ToList().FirstOrDefault();
                                    pli.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                    O_AGILE_ATTR aglie_bndl = GetAglieAttrByCust(SFCDB, shipment.PART_NUMBER, om.PLANT);
                                    pli.ProductDescription = aglie_bndl == null ? "" : aglie_bndl.DESCRIPTION;
                                    pli.UoM = bc.UNITCODE;
                                    pli.OrderQty = ChangeToF3(shipment.SHIPPED_QTY.ToString());
                                    pli.ShipQty = ChangeToF3(pli.OrderQty);
                                    pli.CPR = aglie_bndl == null ? "" : aglie_bndl.CPR_CODE;
                                    pli.CLEI = SFCDB.ORM.Queryable<R_SN_KP>()
                                            .Where(r => r.VALID_FLAG == 1 && r.EXKEY2 == "CLEI" && r.SN == shipment.SERIAL_NUMBER)
                                            .Select(r => r.EXVALUE2).ToList().FirstOrDefault();
                                    pli.SerialNumber = shipment.SERIAL_NUMBER;
                                    listItem.Add(pli);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region NOT BNDL
                            bool hideBom = SFCDB.ORM.Queryable<R_SAP_HB>().Where(r => r.WO == om.PREWO && SqlFunc.StartsWith(r.CUSTPARENTPN, "HB")).Any();
                            var bts_sn = list_shipment.Where(i => i.PO_NUMBER == om.PONO && i.PO_LINE_NO == om.POLINE).Select(r => r.SERIAL_NUMBER).Distinct().ToList();
                            var bts_first = listOII.FirstOrDefault();
                            qtln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');  // BTS SALESORDERLINEITEM
                            ln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');// BTS SALESORDERLINEITEM
                            pn = bts_first.PN;//bts pn
                            cp = bts_first.CUSTPRODID;
                            O_AGILE_ATTR aglie_bts = GetAglieAttrByCust(SFCDB, bts_first.PN, om.PLANT); //根據 i.PN 來查  ///COT CUSTOMER_PART_NUMBER DESCRIPTION  
                            pdn = aglie_bts == null ? "" : aglie_bts.DESCRIPTION;
                            uom = bts_first.UNITCODE; //bts 固定EA,                           
                            order_qty = om.QTY;  //BTS po line 總數                            
                            ship_qty = order_qty; //BTS po line 總數                            
                            cpr = aglie_bts == null ? "" : aglie_bts.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                            var b_sn = bts_sn[0];
                            clei = SFCDB.ORM.Queryable<R_SN_KP>()
                                       .Where(r => r.SN == b_sn && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALID_FLAG == 1 && r.EXKEY2 == "CLEI")
                                       .Select(r => r.EXVALUE2).ToList().FirstOrDefault();

                            //2021.03.02 BTS NOT BNDL do not to print hide bom info
                            hideBom = false;
                            if (hideBom)
                            {
                                foreach (var s in bts_sn)
                                {
                                    var i054Tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == s).OrderBy(r => r.CREATETIME, OrderByType.Desc)
                                        .Select(r => r.TRANID).ToList().FirstOrDefault();
                                    if (string.IsNullOrEmpty(i054Tranid))
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164515", new string[] { s }));
                                    }
                                    var i054_parent = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Parent").ToList();
                                    if (i054_parent.Count > 1)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170115", new string[] { s, i054Tranid }));
                                    }
                                    if (i054_parent.Count == 0)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170558", new string[] { om.PREWO, s, i054Tranid }));
                                    }
                                    listItem.Add(new PackingLabelItem
                                    {
                                        QtLn = qtln,
                                        Ln = ln,
                                        ProductNumber = i054_parent.FirstOrDefault().PARENTMODEL,
                                        CustomerPartNumber = cp,
                                        ProductDescription = aglie_bts == null ? "" : aglie_bts.DESCRIPTION,
                                        UoM = uom,
                                        SerialNumber = i054_parent.FirstOrDefault().PARENTSN,
                                        OrderQty = ChangeToF3(om.QTY),
                                        ShipQty = ChangeToF3(om.QTY),
                                        CLEI = "",// clei,
                                        CPR = ""// cpr
                                    });
                                    var i054_child = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child")
                                        .OrderBy(r => r.CHILDMATERIAL, OrderByType.Asc).OrderBy(r => r.SN, OrderByType.Asc).ToList();
                                    foreach (var ic in i054_child)
                                    {
                                        var i054_aglie = GetAglieAttrByCust(SFCDB, ic.CHILDMATERIAL, om.PLANT);
                                        var qty = SFCDB.ORM.Queryable<R_I054>()
                                            .Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child" && r.CHILDMATERIAL == ic.CHILDMATERIAL)
                                            .ToList().Count().ToString();

                                        PackingLabelItem plItem = new PackingLabelItem();
                                        plItem.QtLn = qtln;
                                        plItem.Ln = "";
                                        plItem.ProductNumber = ic.CHILDMATERIAL;
                                        plItem.CustomerPartNumber = "";
                                        plItem.ProductDescription = i054_aglie == null ? "" : i054_aglie.DESCRIPTION;
                                        plItem.UoM = uom;
                                        plItem.OrderQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                        plItem.ShipQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                        plItem.CPR = i054_aglie == null ? "" : i054_aglie.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                        plItem.CLEI = ic.CLEICODE;
                                        plItem.SerialNumber = ic.SN;
                                        listItem.Add(plItem);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var s in bts_sn)
                                {
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = qtln;
                                    plItem.Ln = ln;
                                    plItem.ProductNumber = pn;
                                    plItem.CustomerPartNumber = cp;
                                    plItem.ProductDescription = pdn;
                                    plItem.UoM = uom;
                                    plItem.OrderQty = ChangeToF3(order_qty); ;//BTS po line 總數
                                    plItem.ShipQty = ChangeToF3(order_qty);//BTS po line 總數
                                    plItem.CPR = cpr; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = clei;//BTS 一樣只顯示一行
                                    plItem.SerialNumber = s;//BTS 139 SERIALID
                                    listItem.Add(plItem);
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        bool printBTS = false;
                        var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                            .Where((c, s) => c.SKUNO == om.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                            .ToList().FirstOrDefault();
                        bool sku_control = cseries == null ? false : true;
                        bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                              && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == om.PREWO).Any();
                        printBTS = sku_control ? sku_control : wo_control;
                        if (printBTS)
                        {
                            var bts_sn = list_shipment.Where(i => i.PO_NUMBER == om.PONO && i.PO_LINE_NO == om.POLINE).Select(r => r.SERIAL_NUMBER).Distinct().ToList();
                            var bts_first = listOII.FirstOrDefault();
                            qtln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');  // BTS SALESORDERLINEITEM
                            ln = string.IsNullOrEmpty(bts_first.SALESORDERLINEITEM) ? "" : bts_first.SALESORDERLINEITEM.TrimStart('0');// BTS SALESORDERLINEITEM
                            pn = bts_first.PN;//bts pn
                            cp = bts_first.CUSTPRODID;
                            O_AGILE_ATTR aglie_bts = GetAglieAttrByCust(SFCDB, bts_first.PN, om.PLANT); //根據 i.PN 來查  ///COT CUSTOMER_PART_NUMBER DESCRIPTION  
                            pdn = aglie_bts == null ? "" : aglie_bts.DESCRIPTION;
                            uom = bts_first.UNITCODE; //bts 固定EA,                           
                            order_qty = om.QTY;  //BTS po line 總數                            
                            ship_qty = order_qty; //BTS po line 總數                            
                            cpr = aglie_bts == null ? "" : aglie_bts.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                            var b_sn = bts_sn[0];
                            clei = SFCDB.ORM.Queryable<R_SN_KP>()
                                       .Where(r => r.SN == b_sn && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALID_FLAG == 1 && r.EXKEY2 == "CLEI")
                                       .Select(r => r.EXVALUE2).ToList().FirstOrDefault();
                            foreach (var s in bts_sn)
                            {
                                var i054Tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == s).OrderBy(r => r.CREATETIME, OrderByType.Desc)
                                    .Select(r => r.TRANID).ToList().FirstOrDefault();
                                if (string.IsNullOrEmpty(i054Tranid))
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164515", new string[] { s }));
                                }
                                var i054_parent = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Parent").ToList();
                                if (i054_parent.Count > 1)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170115", new string[] { s, i054Tranid }));
                                }
                                if (i054_parent.Count == 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170558", new string[] { om.PREWO, s, i054Tranid }));
                                }
                                listItem.Add(new PackingLabelItem
                                {
                                    QtLn = qtln,
                                    Ln = ln,
                                    ProductNumber = i054_parent.FirstOrDefault().PARENTMODEL,
                                    CustomerPartNumber = cp,
                                    ProductDescription = aglie_bts == null ? "" : aglie_bts.DESCRIPTION,
                                    UoM = uom,
                                    SerialNumber = i054_parent.FirstOrDefault().PARENTSN,
                                    OrderQty = ChangeToF3(om.QTY),
                                    ShipQty = ChangeToF3(om.QTY),
                                    CLEI = "",// clei,
                                    CPR = ""// cpr
                                });
                                var child_hide = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child"
                                    && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == true)
                                    .OrderBy(r => r.CHILDMATERIAL, OrderByType.Asc).OrderBy(r => r.SN, OrderByType.Asc).ToList();
                                foreach (var ch in child_hide)
                                {
                                    var i054_aglie = GetAglieAttrByCust(SFCDB, ch.CHILDMATERIAL, om.PLANT);
                                    var qty = SFCDB.ORM.Queryable<R_I054>()
                                        .Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child" && r.CHILDMATERIAL == ch.CHILDMATERIAL
                                        && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == true
                                        ).ToList().Count().ToString();

                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = qtln;
                                    plItem.Ln = string.IsNullOrEmpty(ch.SALESORDERLINENUMBER) ? "" : ch.SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.ProductNumber = ch.CHILDMATERIAL;
                                    var i_item = listOID.Where(i => i.COMPONENTID == ch.CHILDMATERIAL && i.COMSALESORDERLINEITEM == ch.SALESORDERLINENUMBER).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                    plItem.ProductDescription = i054_aglie == null ? "" : i054_aglie.DESCRIPTION;
                                    plItem.UoM = uom;
                                    plItem.OrderQty = string.IsNullOrEmpty(ch.SN) ? ChangeToF3(ch.QTY) : ChangeToF3(qty);
                                    plItem.ShipQty = string.IsNullOrEmpty(ch.SN) ? ChangeToF3(ch.QTY) : ChangeToF3(qty);
                                    plItem.CPR = i054_aglie == null ? "" : i054_aglie.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = ch.CLEICODE;
                                    plItem.SerialNumber = ch.SN;
                                    listItem.Add(plItem);
                                }

                                var i054_child = SFCDB.ORM.Queryable<R_I054>().Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child"
                                    && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false)
                                    .OrderBy(r => r.CHILDMATERIAL, OrderByType.Asc).OrderBy(r => r.SN, OrderByType.Asc).ToList();
                                foreach (var ic in i054_child)
                                {
                                    var i054_aglie = GetAglieAttrByCust(SFCDB, ic.CHILDMATERIAL, om.PLANT);
                                    var qty = SFCDB.ORM.Queryable<R_I054>()
                                        .Where(r => r.TRANID == i054Tranid && r.PARENTSN == s && r.PNTYPE == "Child" && r.CHILDMATERIAL == ic.CHILDMATERIAL && r.SALESORDERLINENUMBER == ic.SALESORDERLINENUMBER)
                                        .ToList().Count().ToString();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = qtln;
                                    plItem.Ln = string.IsNullOrEmpty(ic.SALESORDERLINENUMBER) ? "" : ic.SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.ProductNumber = ic.CHILDMATERIAL;
                                    var i_item = listOID.Where(i => i.COMPONENTID == ic.CHILDMATERIAL && i.COMSALESORDERLINEITEM == ic.SALESORDERLINENUMBER).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;
                                    //plItem.CustomerPartNumber = "";
                                    plItem.ProductDescription = i054_aglie == null ? "" : i054_aglie.DESCRIPTION;
                                    plItem.UoM = uom;
                                    plItem.OrderQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                    plItem.ShipQty = string.IsNullOrEmpty(ic.SN) ? ChangeToF3(ic.QTY) : ChangeToF3(qty);
                                    plItem.CPR = i054_aglie == null ? "" : i054_aglie.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = ic.CLEICODE;
                                    plItem.SerialNumber = ic.SN;
                                    listItem.Add(plItem);
                                }
                            }
                        }
                        else
                        {
                            #region CTO
                            //CTO的數據從R_I244來
                            var so = firstHead.SALESORDERNUMBER.TrimStart('0');
                            var cto_sn = list_shipment.Where(i => i.PO_NUMBER == om.PONO && i.PO_LINE_NO == om.POLINE).Select(r => r.SERIAL_NUMBER).Distinct().ToList();
                            foreach (var csn in cto_sn)
                            {
                                var i244 = SFCDB.ORM.Queryable<R_I244>().Where(r => r.SALESORDERNUMBER == so && r.PARENTSN == csn).ToList();
                                var parent = i244.FindAll(r => r.PNTYPE == "Parent");
                                string uom_cto = "";
                                foreach (var p in parent)
                                {
                                    O_AGILE_ATTR aglie_p = GetAglieAttrByCust(SFCDB, p.PARENTMODEL, om.PLANT);
                                    var iil = listOII.Where(r => r.PN == p.PARENTMODEL).ToList();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = p.SALESORDERLINENUMBER;
                                    plItem.Ln = p.SALESORDERLINENUMBER;
                                    plItem.ProductNumber = p.PARENTMODEL;
                                    plItem.CustomerPartNumber = iil.Count == 0 ? "" : iil.FirstOrDefault().CUSTPRODID;//???
                                    plItem.ProductDescription = aglie_p == null ? "" : aglie_p.DESCRIPTION;
                                    plItem.UoM = iil.Count == 0 ? "" : iil.FirstOrDefault().UNITCODE;
                                    plItem.OrderQty = ChangeToF3(om.QTY);
                                    plItem.ShipQty = ChangeToF3(om.QTY);
                                    plItem.CPR = ""; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = "";
                                    plItem.SerialNumber = p.PARENTSN;
                                    listItem.Add(plItem);
                                    uom_cto = plItem.UoM;
                                }
                                var hidebom = i244.FindAll(r => r.PNTYPE == "Child" && string.IsNullOrEmpty(r.SALESORDERLINENUMBER));
                                foreach (var h in hidebom)
                                {
                                    O_AGILE_ATTR aglie_h = GetAglieAttrByCust(SFCDB, h.CHILDMATERIAL, om.PLANT);

                                    string h_qty = i244.Where(r => r.CHILDMATERIAL == h.CHILDMATERIAL && r.SALESORDERLINENUMBER == h.SALESORDERLINENUMBER).ToList().Count().ToString();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = string.IsNullOrEmpty(parent.FirstOrDefault().SALESORDERLINENUMBER) ? "" : parent.FirstOrDefault().SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.Ln = "";
                                    plItem.ProductNumber = h.CHILDMATERIAL;

                                    var i_item = listOID.Where(i => i.COMPONENTID == h.CHILDMATERIAL && string.IsNullOrEmpty(i.SALESORDERLINEITEM)).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;

                                    plItem.ProductDescription = aglie_h == null ? "" : aglie_h.DESCRIPTION;
                                    plItem.UoM = uom_cto;
                                    plItem.OrderQty = ChangeToF3(h_qty);
                                    plItem.ShipQty = ChangeToF3(h_qty);
                                    plItem.CPR = aglie_h == null ? "" : aglie_h.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = h.CLEICODE;
                                    plItem.SerialNumber = h.SN;
                                    listItem.Add(plItem);
                                }
                                var child = i244.FindAll(r => r.PNTYPE == "Child" && !string.IsNullOrEmpty(r.SALESORDERLINENUMBER)).OrderBy(r => r.SALESORDERLINENUMBER);
                                foreach (var c in child)
                                {
                                    O_AGILE_ATTR aglie_c = GetAglieAttrByCust(SFCDB, c.CHILDMATERIAL, om.PLANT);
                                    string c_qty = (i244.Where(r => r.CHILDMATERIAL == c.CHILDMATERIAL && r.SALESORDERLINENUMBER == c.SALESORDERLINENUMBER).ToList().Count()).ToString();
                                    PackingLabelItem plItem = new PackingLabelItem();
                                    plItem.QtLn = string.IsNullOrEmpty(parent.FirstOrDefault().SALESORDERLINENUMBER) ? "" : parent.FirstOrDefault().SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.Ln = string.IsNullOrEmpty(c.SALESORDERLINENUMBER) ? "" : c.SALESORDERLINENUMBER.TrimStart('0');
                                    plItem.ProductNumber = c.CHILDMATERIAL;

                                    var i_item = listOID.Where(i => i.COMPONENTID == c.CHILDMATERIAL && i.COMSALESORDERLINEITEM == c.SALESORDERLINENUMBER).ToList().FirstOrDefault();
                                    plItem.CustomerPartNumber = i_item == null ? "" : i_item.COMCUSTPRODID;

                                    plItem.ProductDescription = aglie_c == null ? "" : aglie_c.DESCRIPTION;
                                    plItem.UoM = uom_cto;
                                    plItem.OrderQty = ChangeToF3(c_qty);
                                    plItem.ShipQty = plItem.OrderQty;
                                    plItem.CPR = aglie_c == null ? "" : aglie_c.CPR_CODE; //O_AGILE_ATTR 根據PN來取
                                    plItem.CLEI = c.CLEICODE;
                                    plItem.SerialNumber = c.SN;
                                    listItem.Add(plItem);
                                }
                            }
                            #endregion
                        }
                    }
                }
                label_value.ItemList = listItem;
                #endregion

                #region 獲取模板
                R_F_CONTROL template_file = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl"
                  && r.CATEGORY == "DOAPackingListTemplate" && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM")
                    .OrderBy(r => r.EDITTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (template_file == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "DOAPackingListTemplate" }));

                }
                if (string.IsNullOrEmpty(template_file.VALUE))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "DOAPackingListTemplate's Name" }));
                }
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_Label tempLabel = SFCDB.ORM.Queryable<R_Label>().Where(r => r.LABELNAME == template_file.VALUE).ToList().FirstOrDefault();
                if (tempLabel == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171816", new string[] { "DOAPackingListTemplate's Name" }));
                }
                R_FILE file = TRF.GetFileByFileName(tempLabel.R_FILE_NAME, "LABEL", SFCDB);
                if (file == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "Label:" + tempLabel.R_FILE_NAME }));
                }
                Row_R_FILE rowFile = (Row_R_FILE)TRF.GetObjByID(file.ID, SFCDB);
                byte[] template = (byte[])rowFile["BLOB_FILE"];

                string excel_name = $@"(PREASN){input_asn}_(DN){dn}_{SFCDB.ORM.GetDate().ToString("yyyyMMddHHmmss")}.xlsx";
                //用於在服務器保存生成的PacklingList Excel
                string path = System.IO.Directory.GetCurrentDirectory() + "\\PackingList\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                #endregion
                byte[] output_excel = SetPacklingValue_DOA(path, excel_name, label_value, template);
                string content = Convert.ToBase64String(output_excel);

                DateTime sysdate = SFCDB.ORM.GetDate();

                if (!bPrinted)
                {
                    foreach (var m in list_o_main)
                    {
                        SFCDB.ORM.Updateable<O_PO_STATUS>().SetColumns(r => r.VALIDFLAG == "0").Where(r => r.POID == m.ID && r.VALIDFLAG == "1").ExecuteCommand();
                        O_PO_STATUS _28Status = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(SFCDB.ORM, BU),
                            POID = m.ID,
                            STATUSID = "28",
                            VALIDFLAG = "1",
                            CREATETIME = sysdate,
                            EDITTIME = sysdate
                        };
                        SFCDB.ORM.Insertable<O_PO_STATUS>(_28Status).ExecuteCommand();
                    }
                }
                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, this.DBTYPE);
                R_MES_LOG log_object = new R_MES_LOG();
                log_object.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                log_object.PROGRAM_NAME = "PrintPackList";
                log_object.CLASS_NAME = "MESStation.Config.WhsConfig";
                log_object.FUNCTION_NAME = "PrintPackList";
                log_object.DATA1 = input_asn;
                log_object.DATA2 = dn;
                log_object.EDIT_EMP = LoginUser.EMP_NO;
                log_object.EDIT_TIME = sysdate;
                SFCDB.ORM.Insertable<R_MES_LOG>(log_object).ExecuteCommand();

                StationReturn.Data = new { FileName = excel_name, Content = content };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
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


        public string ChangeToF3(string number)
        {
            string n = string.Empty;
            if (!string.IsNullOrEmpty(number) &&
                (System.Text.RegularExpressions.Regex.IsMatch(number, @"^[1-9]\d*|0$") ||
                System.Text.RegularExpressions.Regex.IsMatch(number, @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$")))
            {
                n = Convert.ToDecimal(number).ToString("F3");
            }
            else
            {
                n = "0.000";
            }
            return n;
        }

        public O_AGILE_ATTR GetAglieAttrByCust(OleExec SFCDB, string custpartno, string plant)
        {
            O_AGILE_ATTR last_release = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(r => r.CUSTPARTNO == custpartno && r.PLANT == plant)
                                   .OrderBy(r => r.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
            return last_release;
            //O_AGILE_ATTR aglie = last_release;
            //if (last_release != null)
            //{
            //    DateTime? last_dt = last_release.RELEASE_DATE;
            //    aglie = SFCDB.ORM.Queryable<O_AGILE_ATTR>()
            //    .Where(r => r.CUSTPARTNO == custpartno && r.PLANT == plant && r.RELEASE_DATE == last_dt)
            //    .OrderBy(r => r.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
            //}
            //return aglie;
        }

        /// <summary>
        /// 第一次模板，第一頁和第二頁模板可以放數量一致
        /// </summary>
        /// <returns></returns>
        public byte[] SetPacklingValue_01(string save_path, string save_name, PackingLabelValue label_value, byte[] template, int first_num, int last_num)
        {
            int all_page_lenght = label_value.ItemList.Count;
            int single_page_lenght = first_num;
            int total_page = 0;
            if (all_page_lenght % single_page_lenght == 0)
            {
                total_page = all_page_lenght / single_page_lenght;
            }
            else
            {
                total_page = (all_page_lenght / single_page_lenght) + 1;
            }
            using (Spire.Xls.Workbook ouputfile = new Spire.Xls.Workbook())
            {

                ouputfile.Worksheets.Clear();

                for (int i = 1; i <= total_page; i++)
                {
                    Spire.Xls.Worksheet headSheet = null;
                    Spire.Xls.Worksheet bottomSheet = null;
                    Spire.Xls.Worksheet tempSheet = null;
                    using (Spire.Xls.Workbook tempfile = new Spire.Xls.Workbook())
                    {
                        System.IO.Stream stream = new System.IO.MemoryStream(template);
                        tempfile.LoadFromStream(stream);
                        if (tempfile.Worksheets.Count() == 0)
                        {
                            throw new Exception("No Sheet!");
                        }
                        headSheet = tempfile.Worksheets[0];
                        bottomSheet = tempfile.Worksheets[1];
                    }
                    if (i == 1)
                    {
                        if (total_page == 1)
                        {
                            tempSheet = bottomSheet;
                        }
                        else
                        {
                            //其它頁用tempHead模板
                            //tempHead.AllocatedRange.Copy(page_sheet.Range);
                            tempSheet = headSheet;
                        }
                    }
                    else if (i == total_page)
                    {
                        //最後一一頁用tempBottom模板
                        //tempHead.AllocatedRange.Copy(page_sheet.Range[page_sheet.LastRow + 3, 1]);
                        tempSheet = bottomSheet;
                    }
                    else
                    {
                        //其它頁用tempHead模板
                        //tempHead.AllocatedRange.Copy(page_sheet.Range[page_sheet.LastRow + 3, 1]);                            
                        tempSheet = headSheet;
                    }
                    tempSheet.Name = "Page" + i.ToString();
                    #region 公共部分賦值
                    string[] sold_by_a = label_value.SoldBy.Split(new string[] { "\n" }, StringSplitOptions.None);
                    for (var k = 0; k < sold_by_a.Length; k++)
                    {
                        tempSheet.Range["A" + (6 + k + 1).ToString()].Text = sold_by_a[k] == null ? "" : sold_by_a[k];
                    }
                    string[] bill_to_a = label_value.BillTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < bill_to_a.Length; k++)
                    {
                        tempSheet.Range["F" + (6 + k + 1).ToString()].Text = bill_to_a[k] == null ? "" : bill_to_a[k];
                    }
                    string[] ship_to_a = label_value.ShipTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < ship_to_a.Length; k++)
                    {
                        tempSheet.Range["J" + (6 + k + 1).ToString()].Text = ship_to_a[k] == null ? "" : ship_to_a[k];
                    }
                    //分頁數值
                    tempSheet.Range["N3"].Text = i.ToString();
                    tempSheet.Range["P3"].Text = total_page.ToString();
                    //DN
                    tempSheet.Range["O1"].Text = label_value.DeliveryNumber == null ? "" : label_value.DeliveryNumber.TrimStart('0');
                    if (i == 1)
                    {
                        tempSheet.Range["D14"].Text = label_value.ShippingNotes == "NA" ? "" : label_value.ShippingNotes;
                    }
                    tempSheet.Range["D16"].Text = label_value.ForwardingAgent == null || label_value.ForwardingAgent == "NA" ? "" : label_value.ForwardingAgent;
                    tempSheet.Range["C17"].Text = label_value.ShipVia == null || label_value.ShipVia == "NA" ? "" : label_value.ShipVia;
                    tempSheet.Range["D18"].Text = label_value.IncoTermPlace == null || label_value.IncoTermPlace == "NA" ? "" : label_value.IncoTermPlace;
                    tempSheet.Range["C19"].Text = label_value.LVAS == null || label_value.LVAS == "NA" ? "" : label_value.LVAS;

                    tempSheet.Range["H16"].Text = label_value.TotalWeight;
                    tempSheet.Range["H17"].Text = label_value.TotalNetWeight;
                    tempSheet.Range["H18"].Text = label_value.TotalPieces;
                    tempSheet.Range["H19"].Text = label_value.TotalCartons;

                    tempSheet.Range["M16"].Text = label_value.OrderNumber == null || label_value.OrderNumber == "NA" ? "" : label_value.OrderNumber;
                    tempSheet.Range["L17"].Text = label_value.OrderDate == null || label_value.OrderDate == "NA" ? "" : label_value.OrderDate;
                    tempSheet.Range["M18"].Text = label_value.CustomerPO == null || label_value.CustomerPO == "NA" ? "" : label_value.CustomerPO;
                    tempSheet.Range["M19"].Text = label_value.SalesPerson == null || label_value.SalesPerson == "NA" ? "" : label_value.SalesPerson;
                    tempSheet.Range["M20"].Text = label_value.ContactPerson == null || label_value.ContactPerson == "NA" ? "" : label_value.ContactPerson;
                    #endregion

                    #region 分頁內容 
                    PackingLabelItem pTemp = null;
                    for (int j = 0; j < single_page_lenght; j++)
                    {
                        if ((i - 1) * single_page_lenght + j < label_value.ItemList.Count)
                        {
                            if (j == 0)
                            {
                                pTemp = label_value.ItemList[(i - 1) * single_page_lenght + j];
                                tempSheet.Range["A" + (23 + j).ToString()].Text = pTemp.QtLn == null ? "" : pTemp.QtLn;
                                tempSheet.Range["C" + (23 + j).ToString()].Text = pTemp.Ln == null ? "" : pTemp.Ln;
                                tempSheet.Range["D" + (23 + j).ToString()].Text = pTemp.ProductNumber == null ? "" : pTemp.ProductNumber;
                                tempSheet.Range["D" + (23 + j + 1).ToString()].Text = pTemp.CustomerPartNumber == null ? "" : pTemp.CustomerPartNumber;
                                tempSheet.Range["E" + (23 + j).ToString()].Text = pTemp.ProductDescription == null ? "" : pTemp.ProductDescription;
                                tempSheet.Range["G" + (23 + j).ToString()].Text = pTemp.UoM == null ? "" : pTemp.UoM;
                                tempSheet.Range["I" + (23 + j).ToString()].Text = pTemp.OrderQty == null ? "" : pTemp.OrderQty;
                                tempSheet.Range["J" + (23 + j).ToString()].Text = pTemp.ShipQty == null ? "" : pTemp.ShipQty;
                                tempSheet.Range["L" + (23 + j).ToString()].Text = pTemp.CLEI == null ? "" : pTemp.CLEI;
                                tempSheet.Range["O" + (23 + j).ToString()].Text = pTemp.CPR == null ? "" : pTemp.CPR;
                            }
                            var im = label_value.ItemList[(i - 1) * single_page_lenght + j];
                            if (pTemp.Ln != im.Ln || pTemp.ProductNumber != im.ProductNumber)
                            {
                                pTemp = im;
                                tempSheet.Range["A" + (23 + j).ToString()].Text = pTemp.QtLn == null ? "" : pTemp.QtLn;
                                tempSheet.Range["C" + (23 + j).ToString()].Text = pTemp.Ln == null ? "" : pTemp.Ln;
                                tempSheet.Range["D" + (23 + j).ToString()].Text = pTemp.ProductNumber == null ? "" : pTemp.ProductNumber;
                                tempSheet.Range["D" + (23 + j + 1).ToString()].Text = pTemp.CustomerPartNumber == null ? "" : pTemp.CustomerPartNumber;
                                tempSheet.Range["E" + (23 + j).ToString()].Text = pTemp.ProductDescription == null ? "" : pTemp.ProductDescription;
                                tempSheet.Range["G" + (23 + j).ToString()].Text = pTemp.UoM == null ? "" : pTemp.UoM;
                                tempSheet.Range["I" + (23 + j).ToString()].Text = pTemp.OrderQty == null ? "" : pTemp.OrderQty;
                                tempSheet.Range["J" + (23 + j).ToString()].Text = pTemp.ShipQty == null ? "" : pTemp.ShipQty;
                                tempSheet.Range["L" + (23 + j).ToString()].Text = pTemp.CLEI == null ? "" : pTemp.CLEI;
                                tempSheet.Range["O" + (23 + j).ToString()].Text = pTemp.CPR == null ? "" : pTemp.CPR;
                            }
                            tempSheet.Range["H" + (23 + j).ToString()].Text = im.SerialNumber == null ? "" : im.SerialNumber;
                        }
                    }

                    #endregion

                    ouputfile.Worksheets.AddCopy(tempSheet);
                }
                //先保存
                ouputfile.SaveToFile($@"{save_path}{save_name}", Spire.Xls.ExcelVersion.Version2013);
            }
            byte[] output_byte;
            using (Spire.Xls.Workbook downfile = new Spire.Xls.Workbook())
            {
                downfile.LoadFromFile($@"{save_path}{save_name}");
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                downfile.SaveToStream(ms);
                output_byte = ms.ToArray();
            }
            return output_byte;
        }
        /// <summary>
        /// 第二次模板，第一頁和第二頁模板可以放數量不一致
        /// </summary>
        /// <returns></returns>
        public byte[] SetPacklingValue_02(string save_path, string save_name, PackingLabelValue label_value, byte[] template, int first_num, int last_num)
        {
            int all_page_lenght = label_value.ItemList.Count;
            int single_page_lenght = first_num;
            int total_page = 0;
            if (all_page_lenght % single_page_lenght == 0)
            {
                total_page = all_page_lenght / single_page_lenght;
                //因為是以第一個模板的可以放的行數為分頁行數，若所有數據剛好能放滿第一個模板，則需要在加一個第二個模板，即總數要加1頁
                total_page = total_page + 1;
            }
            else
            {
                int last_data = all_page_lenght % single_page_lenght;
                if (last_data < last_num)
                {
                    total_page = (all_page_lenght / single_page_lenght) + 1;
                }
                else
                {
                    //因為是以第一個模板的可以放的行數為分頁行數，若分頁後還剩下的數量大於第二模板的可以存放的數量，則需要再加一個第1個模板，再加第2個模板，即總數要加2頁
                    total_page = (all_page_lenght / single_page_lenght) + 2;
                }
            }
            Spire.Xls.Worksheet headSheet = null;
            Spire.Xls.Worksheet bottomSheet = null;
            using (Spire.Xls.Workbook tempfile = new Spire.Xls.Workbook())
            {
                System.IO.Stream stream = new System.IO.MemoryStream(template);
                tempfile.LoadFromStream(stream);
                if (tempfile.Worksheets.Count() == 0)
                {
                    throw new Exception("No Sheet!");
                }
                headSheet = tempfile.Worksheets[0];
                bottomSheet = tempfile.Worksheets[1];
            }
            using (Spire.Xls.Workbook ouputfile = new Spire.Xls.Workbook())
            {

                ouputfile.Worksheets.Clear();
                for (int i = 1; i <= total_page; i++)
                {
                    Spire.Xls.Worksheet tempSheet = null;
                    if (i == 1)
                    {
                        if (total_page == 1)
                        {
                            tempSheet = bottomSheet;
                        }
                        else
                        {
                            //其它頁用tempHead模板
                            //tempHead.AllocatedRange.Copy(page_sheet.Range);
                            tempSheet = headSheet;
                        }
                    }
                    else if (i == total_page)
                    {
                        //最後一一頁用tempBottom模板
                        //tempHead.AllocatedRange.Copy(page_sheet.Range[page_sheet.LastRow + 3, 1]);
                        tempSheet = bottomSheet;
                    }
                    else
                    {
                        //其它頁用tempHead模板
                        //tempHead.AllocatedRange.Copy(page_sheet.Range[page_sheet.LastRow + 3, 1]);                            
                        tempSheet = headSheet;
                    }
                    tempSheet.Name = "Page" + i.ToString();
                    #region 公共部分賦值
                    string[] sold_by_a = label_value.SoldBy.Split(new string[] { "\n" }, StringSplitOptions.None);
                    for (var k = 0; k < sold_by_a.Length; k++)
                    {
                        tempSheet.Range["A" + (7 + k).ToString()].Text = sold_by_a[k] == null ? "" : sold_by_a[k];
                    }
                    string[] bill_to_a = label_value.BillTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < bill_to_a.Length; k++)
                    {
                        tempSheet.Range["U" + (7 + k).ToString()].Text = bill_to_a[k] == null ? "" : bill_to_a[k];
                    }
                    string[] ship_to_a = label_value.ShipTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < ship_to_a.Length; k++)
                    {
                        tempSheet.Range["AM" + (7 + k).ToString()].Text = ship_to_a[k] == null ? "" : ship_to_a[k];
                    }
                    //分頁數值
                    tempSheet.Range["BA3"].Text = i.ToString();
                    tempSheet.Range["BC3"].Text = total_page.ToString();
                    //DN
                    tempSheet.Range["BC1"].Text = label_value.DeliveryNumber == null ? "" : label_value.DeliveryNumber.TrimStart('0');
                    if (i == 1)
                    {
                        tempSheet.Range["H14"].Text = label_value.ShippingNotes == "NA" ? "" : label_value.ShippingNotes;
                    }
                    tempSheet.Range["I17"].Text = label_value.ForwardingAgent == null || label_value.ForwardingAgent == "NA" ? "" : label_value.ForwardingAgent;
                    //tempSheet.Range["C17"].Text = label_value.ShipVia == null || label_value.ShipVia == "NA" ? "" : label_value.ShipVia; //模板固定空值
                    tempSheet.Range["H19"].Text = label_value.IncoTermPlace == null || label_value.IncoTermPlace == "NA" ? "" : label_value.IncoTermPlace;
                    tempSheet.Range["E20"].Text = label_value.LVAS == null || label_value.LVAS == "NA" ? "" : label_value.LVAS;

                    tempSheet.Range["AG17"].Text = label_value.TotalWeight;
                    tempSheet.Range["AH18"].Text = label_value.TotalNetWeight;
                    tempSheet.Range["AH19"].Text = label_value.TotalPieces;
                    tempSheet.Range["AH20"].Text = label_value.TotalCartons;

                    tempSheet.Range["AT17"].Text = label_value.OrderNumber == null || label_value.OrderNumber == "NA" ? "" : label_value.OrderNumber;
                    tempSheet.Range["AS18"].Text = label_value.OrderDate == null || label_value.OrderDate == "NA" ? "" : label_value.OrderDate;
                    tempSheet.Range["AT19"].Text = label_value.CustomerPO == null || label_value.CustomerPO == "NA" ? "" : label_value.CustomerPO;
                    tempSheet.Range["AT20"].Text = label_value.SalesPerson == null || label_value.SalesPerson == "NA" ? "" : label_value.SalesPerson;
                    tempSheet.Range["AT21"].Text = label_value.ContactPerson == null || label_value.ContactPerson == "NA" ? "" : label_value.ContactPerson;
                    #endregion

                    #region 分頁內容 
                    List<PackingLabelItem> list_temp = new List<PackingLabelItem>(); //當前分頁內容
                    int start_num = (i - 1) * single_page_lenght;
                    int end_num = i * single_page_lenght;
                    for (int page_n = start_num; page_n < end_num; page_n++)
                    {
                        if (page_n < label_value.ItemList.Count)
                        {
                            list_temp.Add(label_value.ItemList[page_n]);
                        }
                    }

                    PackingLabelItem pTemp = null;
                    int j = 0;
                    foreach (var lt in list_temp)
                    {
                        if (pTemp == null)
                        {
                            pTemp = lt;
                            tempSheet.Range["A" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                            tempSheet.Range["E" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                            tempSheet.Range["I" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                            tempSheet.Range["I" + (27 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                            tempSheet.Range["T" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : (lt.BNDLParent ? lt.ProductDescription + "\n(Bundle Parent)" : lt.ProductDescription);
                            tempSheet.Range["AF" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                            tempSheet.Range["AQ" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                            tempSheet.Range["AU" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                            tempSheet.Range["AX" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                            tempSheet.Range["BE" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                        }
                        if (pTemp.Ln != lt.Ln || pTemp.ProductNumber != lt.ProductNumber)
                        {
                            pTemp = lt;
                            tempSheet.Range["A" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                            tempSheet.Range["E" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                            tempSheet.Range["I" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                            tempSheet.Range["I" + (27 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                            tempSheet.Range["T" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : (lt.BNDLParent ? lt.ProductDescription + "\n(Bundle Parent)" : lt.ProductDescription);
                            tempSheet.Range["AF" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                            tempSheet.Range["AQ" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                            tempSheet.Range["AU" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                            tempSheet.Range["AX" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                            tempSheet.Range["BE" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                        }
                        tempSheet.Range["AI" + (26 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.SerialNumber) ? "" : lt.SerialNumber;
                        j++;
                    }
                    #endregion

                    ouputfile.Worksheets.AddCopy(tempSheet);
                }
                //先保存
                ouputfile.SaveToFile($@"{save_path}{save_name}", Spire.Xls.ExcelVersion.Version2013);
            }
            byte[] output_byte;
            using (Spire.Xls.Workbook downfile = new Spire.Xls.Workbook())
            {
                downfile.LoadFromFile($@"{save_path}{save_name}");
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                downfile.SaveToStream(ms);
                output_byte = ms.ToArray();
            }
            return output_byte;
        }

        public byte[] SetPacklingValue_03(string save_path, string save_name, PackingLabelValue label_value, byte[] template)
        {
            byte[] output_byte;
            int first_page_rows = 0;
            int next_page_rows = 10;
            int last_page_rows = 4;
            int total_page = 0;
            Spire.Xls.Worksheet first_page = null;
            Spire.Xls.Worksheet next_page = null;
            Spire.Xls.Worksheet last_page = null;
            Spire.Xls.Worksheet temp_page = null;
            #region Get first_page label_value.ShippingNotes.Length 
            using (Spire.Xls.Workbook tempfile = new Spire.Xls.Workbook())
            {
                System.IO.Stream stream = new System.IO.MemoryStream(template);
                tempfile.LoadFromStream(stream);
                if (tempfile.Worksheets.Count() == 0)
                {
                    throw new Exception("No Sheet!");
                }
                int note_lenght = label_value.ShippingNotes.Length;
                if (note_lenght <= 330)
                {
                    //Shipping Notes =< 330 Characters
                    first_page = tempfile.Worksheets[0];
                    first_page_rows = 10;
                }
                else if (330 < note_lenght && note_lenght <= 660)
                {
                    //330 < Shipping Notes <= 660
                    first_page = tempfile.Worksheets[1];
                    first_page_rows = 9;
                }
                else if (660 < note_lenght && note_lenght <= 990)
                {
                    //660 < Shipping Notes <= 990
                    first_page = tempfile.Worksheets[2];
                    first_page_rows = 8;
                }
                else if (990 < note_lenght && note_lenght <= 1320)
                {
                    //990 < Shipping Notes <= 1320
                    first_page = tempfile.Worksheets[3];
                    first_page_rows = 6;
                }
                else if (1320 < note_lenght && note_lenght <= 1650)
                {
                    //1320 < Shipping Notes <= 1650
                    first_page = tempfile.Worksheets[4];
                    first_page_rows = 5;
                }
                else if (1650 < note_lenght && note_lenght <= 1980)
                {
                    //1650 < Shipping Notes <= 1980
                    first_page = tempfile.Worksheets[5];
                    first_page_rows = 4;
                }
                else if (1980 < note_lenght && note_lenght <= 2310)
                {
                    //1980 < Shipping Notes <= 2310
                    first_page = tempfile.Worksheets[6];
                    first_page_rows = 3;
                }
                else if (2310 < note_lenght && note_lenght <= 2640)
                {
                    //2310 < Shipping Notes <= 2640
                    first_page = tempfile.Worksheets[7];
                    first_page_rows = 2;
                }
                else if (2640 < note_lenght && note_lenght <= 2970)
                {
                    //2640 < Shipping Notes <= 2970
                    first_page = tempfile.Worksheets[8];
                    first_page_rows = 1;
                }
                else
                {
                    //2970 < Shipping Notes
                    first_page = tempfile.Worksheets[9];
                    first_page_rows = 0;
                }

                next_page = tempfile.Worksheets[10];
                last_page = tempfile.Worksheets[11];
            }
            #endregion

            if (label_value.ItemList.Count > first_page_rows)
            {
                total_page = 1;
                int remaining_rows = label_value.ItemList.Count - first_page_rows;
                total_page = total_page + remaining_rows / next_page_rows;
                if (remaining_rows % next_page_rows == 0)
                {
                    total_page += 1;
                }
                else
                {
                    int lasted_rows = remaining_rows % next_page_rows;
                    if (lasted_rows <= last_page_rows)
                    {
                        total_page += 1;
                    }
                    else
                    {
                        total_page += 2;
                    }
                }
            }
            else
            {
                if (label_value.ItemList.Count >= last_page_rows || first_page_rows != 10)
                {
                    total_page = 2;
                }
                else
                {
                    total_page = 1;
                }
            }
            int total_count = label_value.ItemList.Count;
            int first_rows = 0;
            using (Spire.Xls.Workbook ouputfile = new Spire.Xls.Workbook())
            {
                ouputfile.Worksheets.Clear();
                int printAreaIndex = 0;
                for (int i = 1; i <= total_page; i++)
                {
                    int start_num = 0;
                    int end_num = 0;
                    temp_page = null;
                    if (i == 1)
                    {

                        if (total_page == 1)
                        {
                            temp_page = last_page;
                            start_num = 0;
                            end_num = last_page_rows;
                            first_rows = last_page_rows;
                        }
                        else
                        {
                            temp_page = first_page;
                            start_num = 0;
                            end_num = first_page_rows;
                            first_rows = first_page_rows;
                        }
                        //ShippingNotes only first page
                        temp_page.Range["H14"].Text = label_value.ShippingNotes;
                    }
                    else if (i == total_page)
                    {
                        temp_page = last_page;
                        start_num = first_rows + (i - 2) * next_page_rows;
                        end_num = first_rows + (i - 2) * next_page_rows + last_page_rows;
                    }
                    else
                    {
                        temp_page = next_page;
                        start_num = first_rows + (i - 2) * next_page_rows;
                        end_num = first_rows + (i - 1) * next_page_rows;
                        for (var r = 0; r <= next_page_rows; r++)
                        {
                            temp_page.Range["A" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["E" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["I" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["I" + (27 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["T" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AF" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AQ" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AU" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AX" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["BE" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AI" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                        }

                    }
                    temp_page.Name = "Page" + i.ToString();
                    #region public value
                    //DN
                    temp_page.Range["BC1"].Text = label_value.DeliveryNumber;
                    //page /total page
                    temp_page.Range["BA3"].Text = i.ToString();
                    temp_page.Range["BC3"].Text = total_page.ToString();

                    //sold_by
                    string[] sold_by_a = label_value.SoldBy.Split(new string[] { "\n" }, StringSplitOptions.None);
                    for (var k = 0; k < sold_by_a.Length; k++)
                    {
                        temp_page.Range["A" + (7 + k).ToString()].Text = sold_by_a[k] == null ? "" : sold_by_a[k];
                    }
                    //bill_to
                    string[] bill_to_a = label_value.BillTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < bill_to_a.Length; k++)
                    {
                        temp_page.Range["U" + (7 + k).ToString()].Text = bill_to_a[k] == null ? "" : bill_to_a[k];
                    }
                    //ship_to
                    string[] ship_to_a = label_value.ShipTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < ship_to_a.Length; k++)
                    {
                        temp_page.Range["AM" + (7 + k).ToString()].Text = ship_to_a[k] == null ? "" : ship_to_a[k];
                    }
                    List<PackingLabelItem> list_temp = new List<PackingLabelItem>();
                    for (int page_n = start_num; page_n < end_num; page_n++)
                    {
                        if (page_n < label_value.ItemList.Count)
                        {
                            list_temp.Add(label_value.ItemList[page_n]);
                        }
                    }
                    PackingLabelItem pTemp = null;
                    int range_start_num = 0;
                    int j = 0;
                    if (i == 1)
                    {
                        switch (first_page_rows)
                        {
                            case 10:
                                #region 10
                                //ForwardingAgent
                                temp_page.Range["I17"].Text = label_value.ForwardingAgent;
                                //ShipVia
                                //tempSheet.Range["C17"].Text = label_value.ShipVia; 
                                //IncoTermPlace
                                temp_page.Range["H19"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E20"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG17"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH18"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH19"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH20"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT17"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS18"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT19"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT20"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT21"].Text = label_value.ContactPerson;
                                range_start_num = 26;
                                break;
                            #endregion
                            case 9:
                                #region 9
                                //ForwardingAgent
                                temp_page.Range["I19"].Text = label_value.ForwardingAgent;
                                //ShipVia
                                //tempSheet.Range["C17"].Text = label_value.ShipVia; 
                                //IncoTermPlace
                                temp_page.Range["H21"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E22"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG19"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH20"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH21"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH22"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT19"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS20"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT21"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT22"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT23"].Text = label_value.ContactPerson;
                                range_start_num = 28;
                                break;
                            #endregion
                            case 8:
                                #region 8
                                //ForwardingAgent
                                temp_page.Range["I21"].Text = label_value.ForwardingAgent;
                                //ShipVia
                                //tempSheet.Range["C17"].Text = label_value.ShipVia; 
                                //IncoTermPlace
                                temp_page.Range["H23"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E24"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG21"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH22"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH23"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH24"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT21"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS22"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT23"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT24"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT25"].Text = label_value.ContactPerson;
                                range_start_num = 30;
                                break;
                            #endregion
                            case 6:
                                #region 6
                                //ForwardingAgent
                                temp_page.Range["I25"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H27"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E28"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG25"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH26"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH27"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH28"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT25"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS26"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT27"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT28"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT29"].Text = label_value.ContactPerson;
                                range_start_num = 34;
                                break;
                            #endregion
                            case 5:
                                #region 5
                                //ForwardingAgent
                                temp_page.Range["I27"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H29"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E30"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG27"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH28"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH29"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH30"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT27"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS28"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT29"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT30"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT31"].Text = label_value.ContactPerson;
                                range_start_num = 36;
                                break;
                            #endregion
                            case 4:
                                #region 4
                                //ForwardingAgent
                                temp_page.Range["I29"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H31"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E32"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG29"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH30"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH31"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH32"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT29"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS30"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT31"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT32"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT33"].Text = label_value.ContactPerson;
                                range_start_num = 38;
                                break;
                            #endregion
                            case 3:
                                #region 3
                                //ForwardingAgent
                                temp_page.Range["I31"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H33"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E34"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG31"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH32"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH33"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH34"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT31"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS32"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT33"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT34"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT35"].Text = label_value.ContactPerson;
                                range_start_num = 40;
                                break;
                            #endregion
                            case 2:
                                #region 2
                                //ForwardingAgent
                                temp_page.Range["I33"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H35"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E36"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG33"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH34"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH35"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH36"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT33"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS34"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT35"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT36"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT37"].Text = label_value.ContactPerson;
                                range_start_num = 42;
                                break;
                            #endregion
                            case 1:
                                #region 1
                                //ForwardingAgent
                                temp_page.Range["I35"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H37"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E38"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG35"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH36"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH37"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH38"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT35"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS36"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT37"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT38"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT39"].Text = label_value.ContactPerson;
                                range_start_num = 44;
                                break;
                            #endregion
                            case 0:
                                #region 0
                                //ForwardingAgent
                                temp_page.Range["I37"].Text = label_value.ForwardingAgent;
                                //IncoTermPlace
                                temp_page.Range["H39"].Text = label_value.IncoTermPlace;
                                //LVAS
                                temp_page.Range["E40"].Text = label_value.LVAS;
                                //TotalWeight
                                temp_page.Range["AG37"].Text = label_value.TotalWeight;
                                //TotalNetWeight
                                temp_page.Range["AH38"].Text = label_value.TotalNetWeight;
                                //TotalPieces
                                temp_page.Range["AH39"].Text = label_value.TotalPieces;
                                //TotalCartons
                                temp_page.Range["AH40"].Text = label_value.TotalCartons;
                                //OrderNumber
                                temp_page.Range["AT37"].Text = label_value.OrderNumber;
                                //OrderDate
                                temp_page.Range["AS38"].Text = label_value.OrderDate;
                                //CustomerPO
                                temp_page.Range["AT39"].Text = label_value.CustomerPO;
                                //SalesPerson
                                temp_page.Range["AT40"].Text = label_value.SalesPerson;
                                //ContactPerson
                                temp_page.Range["AT41"].Text = label_value.ContactPerson;
                                break;
                                #endregion
                        }
                        #region row value
                        if (first_page_rows != 0)
                        {
                            pTemp = null;
                            j = 0;
                            foreach (var lt in list_temp)
                            {

                                if (pTemp == null)
                                {
                                    pTemp = lt;
                                    temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                    temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                    temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                    temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                    temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                    temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                    temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                    temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                    temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                    temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                                }
                                if (pTemp.Ln != lt.Ln || pTemp.ProductNumber != lt.ProductNumber)
                                {
                                    pTemp = lt;
                                    temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                    temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                    temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                    temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                    temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                    temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                    temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                    temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                    temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                    temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                                }
                                temp_page.Range["AI" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.SerialNumber) ? "" : lt.SerialNumber;
                                j++;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        //ForwardingAgent
                        temp_page.Range["I17"].Text = label_value.ForwardingAgent;
                        //ShipVia
                        //tempSheet.Range["C17"].Text = label_value.ShipVia; 
                        //IncoTermPlace
                        temp_page.Range["H19"].Text = label_value.IncoTermPlace;
                        //LVAS
                        temp_page.Range["E20"].Text = label_value.LVAS;
                        //TotalWeight
                        temp_page.Range["AG17"].Text = label_value.TotalWeight;
                        //TotalNetWeight
                        temp_page.Range["AH18"].Text = label_value.TotalNetWeight;
                        //TotalPieces
                        temp_page.Range["AH19"].Text = label_value.TotalPieces;
                        //TotalCartons
                        temp_page.Range["AH20"].Text = label_value.TotalCartons;
                        //OrderNumber
                        temp_page.Range["AT17"].Text = label_value.OrderNumber;
                        //OrderDate
                        temp_page.Range["AS18"].Text = label_value.OrderDate;
                        //CustomerPO
                        temp_page.Range["AT19"].Text = label_value.CustomerPO;
                        //SalesPerson
                        temp_page.Range["AT20"].Text = label_value.SalesPerson;
                        //ContactPerson
                        temp_page.Range["AT21"].Text = label_value.ContactPerson;
                        range_start_num = 26;

                        #region row value
                        pTemp = null;
                        j = 0;
                        foreach (var lt in list_temp)
                        {
                            if (pTemp == null)
                            {
                                pTemp = lt;
                                temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                            }
                            if (pTemp.Ln != lt.Ln || pTemp.ProductNumber != lt.ProductNumber)
                            {
                                pTemp = lt;
                                temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                            }
                            temp_page.Range["AI" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.SerialNumber) ? "" : lt.SerialNumber;
                            j++;
                        }
                        #endregion
                    }
                    #endregion

                    #region one page one sheet
                    ouputfile.Worksheets.AddCopy(temp_page);
                    #endregion

                    #region Put All page into one sheet
                    //if (i == 1)
                    //{                        
                    //    ouputfile.Worksheets.AddCopy(temp_page);
                    //    printAreaIndex = Convert.ToInt32(ouputfile.Worksheets[0].PageSetup.PrintArea.Split('$')[4].ToString());
                    //}
                    //else
                    //{
                    //    printAreaIndex += Convert.ToInt32(temp_page.PageSetup.PrintArea.Split('$')[4].ToString());

                    //    temp_page.CopyRow(temp_page.PrintRange, ouputfile.Worksheets[0], 45 * (i-1) + 1, Spire.Xls.CopyRangeOptions.All);
                    //}                   
                    //ouputfile.Worksheets[0].PageSetup.PrintArea = "A1:BH" + printAreaIndex.ToString();
                    //ouputfile.Worksheets[0].Name = "Packlist";
                    #endregion
                }
                ouputfile.SaveToFile($@"{save_path}{save_name}", Spire.Xls.ExcelVersion.Version2013);
            }
            using (Spire.Xls.Workbook downfile = new Spire.Xls.Workbook())
            {
                downfile.LoadFromFile($@"{save_path}{save_name}");
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                downfile.SaveToStream(ms);
                output_byte = ms.ToArray();
            }
            return output_byte;
        }

        public byte[] SetPacklingValue_DOA(string save_path, string save_name, PackingLabelValue label_value, byte[] template)
        {
            byte[] output_byte;
            int first_page_rows = 0;
            int next_page_rows = 10;
            int total_page = 0;
            Spire.Xls.Worksheet first_page = null;
            Spire.Xls.Worksheet next_page = null;
            Spire.Xls.Worksheet temp_page = null;
            #region Get first_page label_value.ShippingNotes.Length 
            using (Spire.Xls.Workbook tempfile = new Spire.Xls.Workbook())
            {
                System.IO.Stream stream = new System.IO.MemoryStream(template);
                tempfile.LoadFromStream(stream);
                if (tempfile.Worksheets.Count() == 0)
                {
                    throw new Exception("No Sheet!");
                }
                int note_lenght = label_value.ShippingNotes.Length;

                first_page = tempfile.Worksheets[0];
                first_page_rows = 10;
                next_page = tempfile.Worksheets[1];
            }
            #endregion

            if (label_value.ItemList.Count > first_page_rows)
            {
                total_page = 1;
                int remaining_rows = label_value.ItemList.Count - first_page_rows;
                total_page = total_page + remaining_rows / next_page_rows;
                if (remaining_rows % next_page_rows != 0)
                {
                    total_page += 1;
                }
            }
            else
            {
                if (first_page_rows != 10)
                {
                    total_page = 2;
                }
                else
                {
                    total_page = 1;
                }
            }
            int total_count = label_value.ItemList.Count;
            int first_rows = 0;
            using (Spire.Xls.Workbook ouputfile = new Spire.Xls.Workbook())
            {
                ouputfile.Worksheets.Clear();
                for (int i = 1; i <= total_page; i++)
                {
                    int start_num = 0;
                    int end_num = 0;
                    temp_page = null;
                    if (i == 1)
                    {
                        temp_page = first_page;
                        start_num = 0;
                        end_num = first_page_rows;
                        first_rows = first_page_rows;

                        //ShippingNotes only first page
                        temp_page.Range["H14"].Text = label_value.ShippingNotes;
                    }
                    else
                    {
                        temp_page = next_page;
                        start_num = first_rows + (i - 2) * next_page_rows;
                        end_num = first_rows + (i - 1) * next_page_rows;
                        for (var r = 0; r <= next_page_rows; r++)
                        {
                            temp_page.Range["A" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["E" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["I" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["I" + (27 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["T" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AF" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AQ" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AU" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AX" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["BE" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                            temp_page.Range["AI" + (26 + r * 2).ToString()].Clear(Spire.Xls.ExcelClearOptions.ClearContent);
                        }

                    }
                    temp_page.Name = "Page" + i.ToString();
                    #region public value
                    //DN
                    temp_page.Range["BC1"].Text = label_value.DeliveryNumber;
                    //page /total page
                    temp_page.Range["BA3"].Text = i.ToString();
                    temp_page.Range["BC3"].Text = total_page.ToString();

                    //sold_by
                    string[] sold_by_a = label_value.SoldBy.Split(new string[] { "\n" }, StringSplitOptions.None);
                    for (var k = 0; k < sold_by_a.Length; k++)
                    {
                        temp_page.Range["A" + (7 + k).ToString()].Text = sold_by_a[k] == null ? "" : sold_by_a[k];
                    }
                    //bill_to
                    string[] bill_to_a = label_value.BillTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < bill_to_a.Length; k++)
                    {
                        temp_page.Range["U" + (7 + k).ToString()].Text = bill_to_a[k] == null ? "" : bill_to_a[k];
                    }
                    //ship_to
                    string[] ship_to_a = label_value.ShipTo.Split(new string[] { "\\n" }, StringSplitOptions.None);
                    for (var k = 0; k < ship_to_a.Length; k++)
                    {
                        temp_page.Range["AM" + (7 + k).ToString()].Text = ship_to_a[k] == null ? "" : ship_to_a[k];
                    }
                    List<PackingLabelItem> list_temp = new List<PackingLabelItem>();
                    for (int page_n = start_num; page_n < end_num; page_n++)
                    {
                        if (page_n < label_value.ItemList.Count)
                        {
                            list_temp.Add(label_value.ItemList[page_n]);
                        }
                    }
                    PackingLabelItem pTemp = null;
                    int range_start_num = 0;
                    int j = 0;
                    if (i == 1)
                    {
                        #region 10
                        //ForwardingAgent
                        temp_page.Range["I17"].Text = label_value.ForwardingAgent;
                        //ShipVia
                        //tempSheet.Range["C17"].Text = label_value.ShipVia; 
                        //IncoTermPlace
                        temp_page.Range["H19"].Text = label_value.IncoTermPlace;
                        //LVAS
                        temp_page.Range["E20"].Text = label_value.LVAS;
                        //TotalWeight
                        temp_page.Range["AG17"].Text = label_value.TotalWeight;
                        //TotalNetWeight
                        temp_page.Range["AH18"].Text = label_value.TotalNetWeight;
                        //TotalPieces
                        temp_page.Range["AH19"].Text = label_value.TotalPieces;
                        //TotalCartons
                        temp_page.Range["AH20"].Text = label_value.TotalCartons;
                        //OrderNumber
                        temp_page.Range["AT17"].Text = label_value.OrderNumber;
                        //OrderDate
                        temp_page.Range["AS18"].Text = label_value.OrderDate;
                        //CustomerPO
                        temp_page.Range["AT19"].Text = label_value.CustomerPO;
                        //SalesPerson
                        temp_page.Range["AT20"].Text = label_value.SalesPerson;
                        //ContactPerson
                        temp_page.Range["AT21"].Text = label_value.ContactPerson;
                        range_start_num = 26;
                        #endregion

                        #region row value
                        if (first_page_rows != 0)
                        {
                            pTemp = null;
                            j = 0;
                            foreach (var lt in list_temp)
                            {

                                if (pTemp == null)
                                {
                                    pTemp = lt;
                                    temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                    temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                    temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                    temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                    temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                    temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                    temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                    temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                    temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                    temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                                }
                                if (pTemp.Ln != lt.Ln || pTemp.ProductNumber != lt.ProductNumber)
                                {
                                    pTemp = lt;
                                    temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                    temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                    temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                    temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                    temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                    temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                    temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                    temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                    temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                    temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                                }
                                temp_page.Range["AI" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.SerialNumber) ? "" : lt.SerialNumber;
                                j++;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        //ForwardingAgent
                        temp_page.Range["I17"].Text = label_value.ForwardingAgent;
                        //ShipVia
                        //tempSheet.Range["C17"].Text = label_value.ShipVia; 
                        //IncoTermPlace
                        temp_page.Range["H19"].Text = label_value.IncoTermPlace;
                        //LVAS
                        temp_page.Range["E20"].Text = label_value.LVAS;
                        //TotalWeight
                        temp_page.Range["AG17"].Text = label_value.TotalWeight;
                        //TotalNetWeight
                        temp_page.Range["AH18"].Text = label_value.TotalNetWeight;
                        //TotalPieces
                        temp_page.Range["AH19"].Text = label_value.TotalPieces;
                        //TotalCartons
                        temp_page.Range["AH20"].Text = label_value.TotalCartons;
                        //OrderNumber
                        temp_page.Range["AT17"].Text = label_value.OrderNumber;
                        //OrderDate
                        temp_page.Range["AS18"].Text = label_value.OrderDate;
                        //CustomerPO
                        temp_page.Range["AT19"].Text = label_value.CustomerPO;
                        //SalesPerson
                        temp_page.Range["AT20"].Text = label_value.SalesPerson;
                        //ContactPerson
                        temp_page.Range["AT21"].Text = label_value.ContactPerson;
                        range_start_num = 26;

                        #region row value
                        pTemp = null;
                        j = 0;
                        foreach (var lt in list_temp)
                        {
                            if (pTemp == null)
                            {
                                pTemp = lt;
                                temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                            }
                            if (pTemp.Ln != lt.Ln || pTemp.ProductNumber != lt.ProductNumber)
                            {
                                pTemp = lt;
                                temp_page.Range["A" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.QtLn) ? "" : lt.QtLn;
                                temp_page.Range["E" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.Ln) ? "" : lt.Ln;
                                temp_page.Range["I" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductNumber) ? "" : lt.ProductNumber;
                                temp_page.Range["I" + (range_start_num + 1 + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CustomerPartNumber) ? "" : lt.CustomerPartNumber;
                                temp_page.Range["T" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ProductDescription) ? "" : lt.ProductDescription;
                                temp_page.Range["AF" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.UoM) ? "" : lt.UoM;
                                temp_page.Range["AQ" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.OrderQty) ? "" : lt.OrderQty;
                                temp_page.Range["AU" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.ShipQty) ? "" : lt.ShipQty;
                                temp_page.Range["AX" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CLEI) ? "" : lt.CLEI;
                                temp_page.Range["BE" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.CPR) ? "" : lt.CPR;
                            }
                            temp_page.Range["AI" + (range_start_num + j * 2).ToString()].Text = string.IsNullOrEmpty(lt.SerialNumber) ? "" : lt.SerialNumber;
                            j++;
                        }
                        #endregion
                    }
                    #endregion

                    #region one page one sheet
                    ouputfile.Worksheets.AddCopy(temp_page);
                    #endregion
                }
                ouputfile.SaveToFile($@"{save_path}{save_name}", Spire.Xls.ExcelVersion.Version2013);
            }
            using (Spire.Xls.Workbook downfile = new Spire.Xls.Workbook())
            {
                downfile.LoadFromFile($@"{save_path}{save_name}");
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                downfile.SaveToStream(ms);
                output_byte = ms.ToArray();
            }
            return output_byte;
        }


        public void JuniperASNDoubleCheck(OleExec SFCDB, string asn, string title)
        {
            T_R_PACKING t_r_packing = new T_R_PACKING(SFCDB, DBTYPE);
            string[] wo_arry = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(o => o.PREASN == asn).Select(o => o.PREWO).ToArray();
            //var pack_list = t_r_packing.GetCartonListByWO(SFCDB, wo_arry);
            var pack_list = t_r_packing.GetPalletListByWO(SFCDB, wo_arry);
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, DBTYPE);
            R_MES_LOG check_log = null;
            int total_times = pack_list.Count;
            UIInputData O = new UIInputData() { };
            O.Timeout = 3000000;
            O.IconType = IconType.Warning;
            O.Type = UIInputType.String;
            O.Tittle = title;
            O.ErrMessage = "No input";
            O.UIArea = new string[] { "40%", "80%" };
            O.OutInputs.Clear();
            O.Message = "PALLET NO";
            O.Name = "PALLET_NO";
            O.CBMessage = "";

            var scan_list = new DisplayOutPut()
            {
                DisplayType = "TextArea",
                Name = "ScanList",
                Value = ""
            };

            var scan_total = new DisplayOutPut()
            {
                DisplayType = "Text",
                Name = "Total",
                Value = $@"0/{total_times}"
            };
            O.OutInputs.Add(scan_total);
            O.OutInputs.Add(scan_list);
            StringBuilder s = new StringBuilder();
            for (var i = 0; i < total_times; i++)
            {

                while (true)
                {
                    var input_sn = O.GetUiInput(this, UIInput.Normal);
                    if (input_sn == null)
                    {
                        O.CBMessage = $@"Please Scan CARTON NO.";
                    }
                    else
                    {
                        string check_value = input_sn.ToString().Trim();
                        if (string.IsNullOrEmpty(check_value))
                        {
                            O.CBMessage = $@"Please Scan CARTON NO.";
                        }
                        else if (check_value.Equals("No input"))
                        {
                            throw new Exception("User Cancel");
                        }
                        else
                        {
                            var k = pack_list.Find(r => r.PACK_NO == check_value);
                            if (k == null)
                            {
                                O.CBMessage = $@"{check_value} not exists in shipping list";
                            }
                            else
                            {
                                s.Append(k.PACK_NO + ",");
                                scan_list.Value = s.ToString();
                                scan_total.Value = $@"{(i + 1).ToString()}/{total_times.ToString()}";
                                pack_list.Remove(k);
                                check_log = new R_MES_LOG();
                                check_log.ID = t_r_mes_log.GetNewID(BU, SFCDB);
                                check_log.PROGRAM_NAME = "ASNDoubleCheck";
                                check_log.CLASS_NAME = "MESStation.Config.WhsConfig";
                                check_log.FUNCTION_NAME = "JuniperASNDoubleCheck";
                                check_log.LOG_MESSAGE = title;
                                check_log.DATA1 = k.PACK_NO;
                                check_log.DATA2 = k.ID;
                                check_log.DATA3 = asn;
                                check_log.EDIT_TIME = SFCDB.ORM.GetDate();
                                check_log.EDIT_EMP = LoginUser.EMP_NO;
                                SFCDB.ORM.Insertable<R_MES_LOG>(check_log).ExecuteCommand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void GetJuniperASNShippingLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string po = Data["PO"] == null ? "" : Data["PO"].ToString().Trim();
                string item = Data["ITEM"] == null ? "" : Data["ITEM"].ToString().Trim();
                string dn = Data["DN"] == null ? "" : Data["DN"].ToString().Trim();
                string asn = Data["ASN"] == null ? "" : Data["ASN"].ToString().Trim();
                //if (string.IsNullOrEmpty(po) && string.IsNullOrEmpty(item))
                //{
                //    throw new Exception("Please Input PO And PO Item!");
                //}
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (SFCDB != null)
                    this.DBPools["SFCDB"].Return(SFCDB);
            }
        }


        public void GetPackFifoNoControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Queryable<R_PACKING_FIFO>().ToList();
                if (Data["STATUS"] != null)
                    res = res.FindAll(t => t.STATUS == Data["STATUS"].ToString());
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void AddPackFifoNoControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var packno = Data["PACKNO"].ToString();
                var reason = Data["REASON"].ToString();
                if (!SFCDB.ORM.Queryable<R_PACKING>().Any(t => t.PACK_NO == packno))
                    throw new Exception($@"PackNo: {packno} is not exists!");
                if (SFCDB.ORM.Queryable<R_PACKING_FIFO>().Any(t => t.PACKNO == packno && t.STATUS == MesBool.Yes.ExtValue()))
                    throw new Exception($@"PackNo: {packno} Already control,pls check!");
                SFCDB.ORM.Insertable(new R_PACKING_FIFO()
                {
                    ID = MesDbBase.GetNewID<R_PACKING_FIFO>(SFCDB.ORM, this.BU),
                    PACKNO = packno,
                    STATUS = MesBool.Yes.ExtValue(),
                    REASON = reason,
                    CREATETIME = DateTime.Now,
                    CREATEBY = this.LoginUser.EMP_NO
                }).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }


        public void DelPackFifoNoControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var ID = Data["ID"].ToString();
                SFCDB.ORM.Updateable<R_PACKING_FIFO>().SetColumns(t => new R_PACKING_FIFO() { STATUS = MesBool.No.ExtValue() }).Where(t => t.ID == ID).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void CancelShipOutByPallet(OleExec SFCDB, string palletNo)
        {
            #region Cancel Ship Out By Pallet
            T_R_PACKING TRP = new T_R_PACKING(SFCDB, DBTYPE);
            T_R_DN_STATUS TRDS = new T_R_DN_STATUS(SFCDB, DBTYPE);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(SFCDB, DBTYPE);
            R_PACKING objPack = TRP.GetPackingByPackNo(palletNo, SFCDB);
            T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(SFCDB, DBTYPE);
            T_R_SN TRS = new T_R_SN(SFCDB, DBTYPE);
            T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(SFCDB, DBTYPE);
            R_DN_STATUS objDNStatus = null;
            if (objPack == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { palletNo }));
            }
            var listSN = TRP.GetSnListByPalletID(objPack.ID, SFCDB);
            if (listSN.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { palletNo }));
            }
            var list = listSN.Select(r => r.SN).ToList();
            var shipedDnList = SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(r => list.Contains(r.SN)).Select(r => new { r.DN_NO, r.DN_LINE }).Distinct().ToList();
            foreach (var item in shipedDnList)
            {
                objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, item.DN_NO, item.DN_LINE);
                if (objDNStatus.DN_FLAG == "3")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141720", new string[] { item.DN_NO, item.DN_LINE }));
                }
            }
            DateTime sysDateTime = SFCDB.ORM.GetDate();
            var listRoute = TCRD.GetLastStations(listSN.FirstOrDefault().ROUTE_ID, "SHIPOUT", SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
            var current_station = listRoute.FirstOrDefault().STATION_NAME;
            var result = 0;
            foreach (R_SN sn in listSN)
            {
                var objShipDetail = TRSD.GetShipDetailBySN(SFCDB, sn.SN);
                if (objShipDetail == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141412", new string[] { sn.SN }));
                }
                if (sn.NEXT_STATION != "SHIPFINISH" && sn.CURRENT_STATION != "SHIPOUT")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115950", new string[] { sn.SN }));
                }
                sn.CURRENT_STATION = current_station;
                sn.NEXT_STATION = "SHIPOUT";
                sn.SHIPPED_FLAG = "0";
                sn.SHIPDATE = null;
                sn.EDIT_EMP = LoginUser.EMP_NO;
                sn.EDIT_TIME = sysDateTime;
                result = TRS.Update(sn, SFCDB);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN " + sn.SN }));
                }

                R_SN_STATION_DETAIL objStationDetail_P = TRSSD.GetDetailBySnAndStation(sn.SN, "SHIPOUT", SFCDB);
                objStationDetail_P.SN = "RS_" + objStationDetail_P.SN;
                result = TRSSD.Update(objStationDetail_P, SFCDB);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + sn.SN }));
                }

                result = TRSD.CancelShip(SFCDB, objShipDetail);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + sn.SN }));
                }
            }
            foreach (var item in shipedDnList)
            {
                objDNStatus = TRDS.GetStatusByNOAndLine(SFCDB, item.DN_NO, item.DN_LINE);
                objDNStatus.DN_FLAG = "0";
                objDNStatus.EDITTIME = sysDateTime;
                result = TRDS.Update(SFCDB, objDNStatus);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                }
            }
            #endregion            
        }
    }
}
