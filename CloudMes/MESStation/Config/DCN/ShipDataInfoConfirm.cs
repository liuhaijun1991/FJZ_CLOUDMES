using MESDataObject;
using MESDataObject.Module;
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
using MESDataObject.Module.ARUBA;
using MESDataObject.Module.DCN;
using static MESDataObject.Common.EnumExtensions;


namespace MESStation.Config.DCN
{
    public class ShipDataInfoConfirm : MesAPIBase
    {

        protected APIInfo FDnConfirm = new APIInfo()
        {
            FunctionName = "DnConfirm",
            Description = "DnConfirm",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FGetDnList = new APIInfo()
        {
            FunctionName = "GetDnList",
            Description = "GetDnList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {}
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetHpeList = new APIInfo()
        {
            FunctionName = "GetHpeList",
            Description = "GetHpeList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {}
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FEditShipData = new APIInfo()
        {
            FunctionName = "EditShipData",
            Description = "EditShipData",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {}
            },
            Permissions = new List<MESPermission>() { }
        };

        public ShipDataInfoConfirm()
        {
            this.Apis.Add(FDnConfirm.FunctionName, FDnConfirm);
            this.Apis.Add(FGetDnList.FunctionName, FGetDnList);
            this.Apis.Add(FGetHpeList.FunctionName, FGetHpeList);
            this.Apis.Add(FEditShipData.FunctionName, FEditShipData);
        }


        public void DnConfirm(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data,MESStationReturn StationReturn)
        {
            OleExec sfcdb = new OleExec();
            DateTime F_TO_DATE = DateTime.Now,
                     F_TO_SHIPDATE = DateTime.Now,
                     F_PO_DATE = DateTime.Now,
                     DbTime = DateTime.Now;
            int ret = 0;
            bool checkExist = false;
            string ID = Data["ID"].ToString().Trim(),
                   str_F_TO_DATE = Data["F_TO_DATE"].ToString().Trim(),
                   str_F_TO_SHIPDATE = Data["F_TO_SHIPDATE"].ToString().Trim(),
                   F_TO_NO = Data["F_TO_NO"].ToString().Trim(),
                   F_TO_DN = Data["F_TO_DN"].ToString().Trim(),
                   F_TO_DN_LINE = Data["F_TO_DN_LINE"].ToString().Trim(),
                   F_TO_TRAILERNO = Data["F_TO_TRAILERNO"].ToString().Trim(),
                   F_CARRIER_TYPE = Data["F_CARRIER_TYPE"].ToString().Trim(),
                   F_CARRIER_CODE = Data["F_CARRIER_CODE"].ToString().Trim(),
                   F_CARRIER_TRAN_TYPE = Data["F_CARRIER_TRAN_TYPE"].ToString().Trim(),
                   F_CARRIER_REF_NO = Data["F_CARRIER_REF_NO"].ToString().Trim(),
                   F_CARRIER_TRAILER_NO = Data["F_CARRIER_TRAILER_NO"].ToString().Trim(),
                   F_ST_NAME = Data["F_ST_NAME"].ToString().Trim(),
                   F_ST_CONTACT = Data["F_ST_CONTACT"].ToString().Trim(),
                   F_ST_CONTACT_MAIL = Data["F_ST_CONTACT_MAIL"].ToString().Trim(),
                   F_ST_CUSTOMERCODE = Data["F_ST_CUSTOMERCODE"].ToString().Trim(),
                   F_ST_ADDRESS = Data["F_ST_ADDRESS"].ToString().Trim(),
                   F_ST_CITY = Data["F_ST_CITY"].ToString().Trim(),
                   F_ST_POSTCODE = Data["F_ST_POSTCODE"].ToString().Trim(),

                   F_ST_STATE_CODE = Data["F_ST_STATE_CODE"].ToString().Trim(),
                   F_ST_COUNTRY_CODE = Data["F_ST_COUNTRY_CODE"].ToString().Trim(),
                   F_PO_NO = Data["F_PO_NO"].ToString().Trim(),
                   F_PO_LINE_NO = Data["F_PO_LINE_NO"].ToString().Trim(),
                   F_PO_LINE_QTY = Data["F_PO_LINE_QTY"].ToString().Trim(),
                   str_F_PO_DATE = Data["F_PO_DATE"].ToString().Trim(),
                   F_INCOTERM = Data["F_INCOTERM"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                F_TO_DATE = Convert.ToDateTime(str_F_TO_DATE);
                F_TO_SHIPDATE = Convert.ToDateTime(str_F_TO_SHIPDATE);
                F_PO_DATE = Convert.ToDateTime(str_F_PO_DATE);
                checkExist = sfcdb.ORM.Queryable<HPE_SHIP_DATA>().Where(h => h.ID == ID).Any();
                if (checkExist)
                {
                    ret = sfcdb.ORM.Updateable<HPE_SHIP_DATA>().SetColumns(r=>new HPE_SHIP_DATA
                    {
                        F_TO_NO = F_TO_NO,
                        F_TO_DATE = F_TO_DATE,
                        F_TO_SHIPDATE = F_TO_SHIPDATE,

                        F_TO_TRAILERNO = F_TO_TRAILERNO,
                        F_CARRIER_TYPE = F_CARRIER_TYPE,
                        F_CARRIER_CODE = F_CARRIER_CODE,
                        F_CARRIER_TRAN_TYPE = F_CARRIER_TRAN_TYPE,
                        F_CARRIER_REF_NO = F_CARRIER_REF_NO,
                        F_CARRIER_TRAILER_NO = F_CARRIER_TRAILER_NO,
                        F_ST_NAME = F_ST_NAME,
                        F_ST_CONTACT = F_ST_CONTACT,
                        F_ST_CONTACT_MAIL = F_ST_CONTACT_MAIL,
                        F_ST_CUSTOMERCODE = F_ST_CUSTOMERCODE,
                        F_ST_ADDRESS = F_ST_ADDRESS,
                        F_ST_CITY = F_ST_CITY,
                        F_ST_POSTCODE = F_ST_POSTCODE,
                        F_ST_STATE_CODE = F_ST_STATE_CODE,
                        F_ST_COUNTRY_CODE = F_ST_COUNTRY_CODE,
                        F_PO_NO = F_PO_NO,
                        F_PO_LINE_NO = F_PO_LINE_NO,
                        F_PO_LINE_QTY = F_PO_LINE_QTY,
                        F_PO_DATE = F_PO_DATE,
                        F_INCOTERM= F_INCOTERM,
                        EDITTIME = GetDBDateTime(),
                        EDIT_EMP = LoginUser.EMP_NO,
                    }).Where(r=>r.ID==ID).ExecuteCommand();

                    if (ret == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        return;
                    }
                }
                else
                {
                    ret = sfcdb.ORM.Insertable<HPE_SHIP_DATA>(new HPE_SHIP_DATA
                    {
                        ID = ID,
                        F_TO_NO = F_TO_NO,
                        F_TO_DATE = F_TO_DATE,
                        F_TO_SHIPDATE = F_TO_SHIPDATE,
                        F_TO_DN = F_TO_DN,
                        F_TO_DN_LINE = F_TO_DN_LINE,

                        F_TO_TRAILERNO = F_TO_TRAILERNO,
                        F_CARRIER_TYPE = F_CARRIER_TYPE,
                        F_CARRIER_CODE = F_CARRIER_CODE,
                        F_CARRIER_TRAN_TYPE = F_CARRIER_TRAN_TYPE,
                        F_CARRIER_REF_NO = F_CARRIER_REF_NO,
                        F_CARRIER_TRAILER_NO = F_CARRIER_TRAILER_NO,
                        F_ST_NAME = F_ST_NAME,
                        F_ST_CONTACT = F_ST_CONTACT,
                        F_ST_CONTACT_MAIL = F_ST_CONTACT_MAIL,
                        F_ST_CUSTOMERCODE = F_ST_CUSTOMERCODE,
                        F_ST_ADDRESS = F_ST_ADDRESS,
                        F_ST_CITY = F_ST_CITY,
                        F_ST_POSTCODE = F_ST_POSTCODE,
                        F_ST_STATE_CODE = F_ST_STATE_CODE,
                        F_ST_COUNTRY_CODE = F_ST_COUNTRY_CODE,
                        F_PO_NO = F_PO_NO,
                        F_PO_LINE_NO = F_PO_LINE_NO,
                        F_PO_LINE_QTY = F_PO_LINE_QTY,
                        F_PO_DATE = F_PO_DATE,
                        F_INCOTERM = F_INCOTERM,
                        CREATETIME = GetDBDateTime(),
                        EDITTIME = GetDBDateTime(),
                        CREATE_EMP = LoginUser.EMP_NO,
                        EDIT_EMP = LoginUser.EMP_NO,
                    }).ExecuteCommand();

                    if (ret == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        return;
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void EditShipData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
 
            string ID = Data["ID"].ToString().Trim();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                HPE_SHIP_DATA hsd = sfcdb.ORM.Queryable<HPE_SHIP_DATA>().Where(h => h.ID == ID).First();
                if (hsd==null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    return;
                }

                StationReturn.Data = hsd;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                throw exception;
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
                var res = sfcdb.ORM.Queryable<R_DN_STATUS, HPE_SHIP_DATA>((r, h) => new object[] { JoinType.Left, r.DN_NO == h.F_TO_DN && r.DN_LINE==h.F_TO_DN_LINE }).Select((r, h) => new { r.ID, r.DN_NO, r.DN_LINE, r.SKUNO, r.QTY, r.DN_FLAG, r.CREATETIME, HID = h.ID }).ToList();

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

        public void GetHpeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = sfcdb.ORM.Queryable<HPE_SHIP_DATA>().ToList();

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

      
    }
}
