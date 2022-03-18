using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;
using static MESDataObject.Constants.PublicConstants;
using System.Data;
using MESDataObject.Module.Juniper;
using MESPubLab.SAP_RFC;
using MESPubLab.MesBase;
using MESJuniper.Base;
using MESJuniper.OrderManagement;
using System.Threading;
using MESStation.Management;

namespace MESStation.Interface.Juniper
{
    public class JuniperInterface : MesAPIBase
    {
        protected APIInfo _ConvertJuniperWo = new APIInfo()
        {
            FunctionName = "ConvertJuniperWo",
            Description = "ConvertJuniperWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "2011-02-01" },

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _JuniperWoAutoKPUpdate = new APIInfo()
        {
            FunctionName = "JuniperWoAutoKPUpdate",
            Description = "JuniperWoAutoKPUpdate",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "2011-02-01" },

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAutoCalculateWOPackage = new APIInfo()
        {
            FunctionName = "JuniperAutoCalculateWOPackage",
            Description = "JuniperAutoCalculateWOPackage",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetWoList = new APIInfo()
        {
            FunctionName = "GetWoList",
            Description = "GetWoList",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FBatchConvertWo = new APIInfo()
        {
            FunctionName = "BatchConvertWo",
            Description = "BatchConvertWo",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FRegeneratedJuniperWo = new APIInfo()
        {
            FunctionName = "RegeneratedJuniperWo",
            Description = "RegeneratedJuniperWo",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        

        public JuniperInterface()
        {
            Apis.Add(_ConvertJuniperWo.FunctionName, _ConvertJuniperWo);
            Apis.Add(_JuniperWoAutoKPUpdate.FunctionName, _JuniperWoAutoKPUpdate);
            Apis.Add(FAutoCalculateWOPackage.FunctionName, FAutoCalculateWOPackage);
            Apis.Add(FGetWoList.FunctionName, FGetWoList);
            Apis.Add(FBatchConvertWo.FunctionName, FBatchConvertWo);
            Apis.Add(FRegeneratedJuniperWo.FunctionName, FRegeneratedJuniperWo);
        }
        public void ConvertJuniperWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            var WO = Data["WO"].ToString();
            try
            {
                var converres = ConvertWoByWo(SFCDB, WO, this.BU);
                if (converres.issuccess)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = converres.msg;
                }
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                #region unlock
                SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(t => new R_MES_LOG() { DATA2 = MesBool.No.ExtValue() })
                    .Where(t => t.FUNCTION_NAME == "ConverWoLock" && t.DATA1 == WO && t.DATA2 == MesBool.Yes.ExtValue()).ExecuteCommand();
                #endregion
                _DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void BatchConvertWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            try
            {
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                var BatchRes = new List<object>();
                var convertWoList = GetConverWoList(SFCDB);
                var issupervisor = new Func<bool>(() =>
                {
                    var rolepri = SFCDB.ORM.Queryable<C_USER, c_user_role, C_ROLE_PRIVILEGE, C_PRIVILEGE>((u, r, p, cp) => u.ID == r.USER_ID && r.ROLE_ID == p.ROLE_ID && p.PRIVILEGE_ID == cp.ID)
                    .Where((u, r, p, cp) => cp.PRIVILEGE_NAME.ToUpper().Contains("CONVERT WO FOR SUPERVISOR") && u.EMP_NO == this.LoginUser.EMP_NO).Select((u, r, p, cp) => p).Any();
                    var userpri = SFCDB.ORM.Queryable<C_USER, C_USER_PRIVILEGE, C_PRIVILEGE>((u, r, cp) => u.ID == r.USER_ID && r.PRIVILEGE_ID == cp.ID)
                    .Where((u, r, cp) => cp.PRIVILEGE_NAME.ToUpper().Contains("CONVERT WO FOR SUPERVISOR") && u.EMP_NO == this.LoginUser.EMP_NO).Select((u, r, cp) => cp).Any();
                    if (rolepri || userpri)
                        return true;
                    return false;
                })();
                foreach (var item in array)
                {
                    var Wo = item["WO"].ToString();
                    var tartgetWo = convertWoList.Select($@"PREWO='{Wo}'");
                    if (issupervisor || (tartgetWo.Count() > 0 && tartgetWo[0]["CONVERSTATUS"].ToString() == "Y"))
                    {
                        var converres = ConvertWoByWo(SFCDB, Wo, this.BU);
                        BatchRes.Add(Tuple.Create(Wo, converres.issuccess ? StationReturnStatusValue.Pass : StationReturnStatusValue.Fail, converres.issuccess ? "Conversion Successful!" : converres.msg));
                    }
                    else
                        BatchRes.Add(Tuple.Create(Wo, StationReturnStatusValue.Fail, $@"Wo:{Wo} .It's not time for deliverydate yet!"));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK,Operation completed!";
                StationReturn.Data = BatchRes;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Message = ee.Message;
                StationReturn.Data = "OK";
            }
            finally
            {
                //#region unlock
                //SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(t => new R_MES_LOG() { DATA2 = MesBool.No.ExtValue() })
                //    .Where(t => t.FUNCTION_NAME == "ConverWoLock" && t.DATA1 == WO && t.DATA2 == MesBool.Yes.ExtValue()).ExecuteCommand();
                //#endregion
                _DBPools["SFCDB"].Return(SFCDB);
            }

        }
        public void RegeneratedJuniperWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            var WO = Data["WO"].ToString();
            try
            {
                var item = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == WO).ToList().FirstOrDefault();
                if (item == null)
                    throw new Exception($@"Wo:{WO} has been reset,pls check!");
                var res = JuniperBase.RegeneratedWoByOne(this.BU, item, SFCDB.ORM);
                if (res.issuccess)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = res.msg;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = res.msg;
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
            }
            finally
            {
                _DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void JuniperWoAutoKPUpdate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            try
            {
                var strWO = Data["WO"].ToString();
                var db = SFCDB.ORM;
                //StationReturn.Data = new { Old = MakeAutoKPConfig(strWO, SFCDB, BU), New = MakeAutoKPConfigNew(strWO, SFCDB, BU) };
                StationReturn.Data = new { New = MakeAutoKPConfigNew(strWO, SFCDB, BU) };
            }
            catch (Exception ee)
            {
                StationReturn.Data = ee;
            }
            finally
            {
                _DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void JuniperAutoCalculateWOPackage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = _DBPools["SFCDB"].Borrow();
                var WO = Data["WO"].ToString();
                var db = SFCDB.ORM;
                var OOM = db.Queryable<O_ORDER_MAIN>().Where(r => r.PREWO == WO).ToList().FirstOrDefault();
                if (OOM == null)
                {
                    throw new Exception("Can't find prewo on O_ORDER_MAIN");
                }
                AutoCalculateWOPackage(SFCDB, OOM, BU);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Message = "OK";
                StationReturn.Data = "OK";
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Message = ex.Message;
                StationReturn.Data = ex.Message;
            }
            finally
            {

                if (SFCDB != null)
                    _DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetWoList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                DataTable dt = new DataTable();
                var res = oleDB.ORM.Queryable<R_WO_BASE>().Where(t => t.EDIT_TIME != null).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
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
        static List<JuniperAutoKpConfig> MakeAutoKPConfig(string strWO, OleExec SFCDB, string BU)
        {
            var db = SFCDB.ORM;
            List<JuniperAutoKpConfig> data = new List<JuniperAutoKpConfig>();
            var order_main = db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strWO).First();
            if (order_main == null)
            {
                throw new Exception($@"o_order_main.prewo {strWO} == null");
            }

            DataTable kpl7xx = null;
            var kpl7xx_pn = "";
            var kpl7xx_rev = "";

            var strsql = $@"select * from(
                                select a.*,row_number() over(partition by item_number order by date_created desc) numbs from o_agile_attr a 
                                    where item_number = '{order_main.PID}' and plant = '{order_main.PLANT}'
                                ) 
                                where numbs=1";
            var kpl = SFCDB.RunSelect(strsql).Tables[0];
            if (kpl.Rows.Count == 0)
            {
                throw new Exception($@"o_agile_attr.item_number {order_main.PID} Can't find, Please check!");
            }

            //非Optics系列不卡r_modelsubpn_map.subpartno與SAPBom是否一致，若SAPBom無資料則取Map資料 Asked By PE 譚義康 2021-12-04
            strsql = $@"select s.series_name from c_sku c, c_series s where c.c_series_id=s.id and c.skuno='{order_main.PID}'";
            var seriesDT = SFCDB.RunSelect(strsql).Tables[0];
            if (seriesDT.Rows.Count == 0) throw new Exception($@"order_main.PID {order_main.PID} Can't find SerieName, Please check!");
            var seriesName = seriesDT.Rows[0]["series_name"].ToString();            

            for (int i = 0; i < kpl.Rows.Count; i++)
            {
                strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
                kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

                kpl7xx_pn = "";
                kpl7xx_rev = "";
                if (kpl7xx.Rows.Count > 0)
                {
                    kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
                    kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();

                    if (kpl7xx_pn == "NA")
                    {
                        kpl7xx_pn = "";
                        kpl7xx_rev = "";
                    }
                    else
                    {
                        if (!SFCDB.ORM.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == strWO && t.PN == kpl7xx_pn).Any() && seriesName == "Juniper-Optics") //儘Optics系列卡關
                        {
                            throw new Exception($@"'{kpl7xx_pn}' not in SAP_BOM");
                        }

                        strsql = $@"select * from r_sap_podetail where wo = '{order_main.PREWO}' and pn = '{kpl7xx_pn}' order by createtime desc";
                        kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                        if (kpl7xx.Rows.Count == 0)
                        {
                            strsql = $@"select * from r_sap_hb where wo = '{order_main.PREWO}' and custpn = '{kpl7xx_pn}' order by createtime desc";
                            kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                            if (kpl7xx.Rows.Count == 0 && seriesName == "Juniper-Optics")//儘Optics系列卡關
                            {
                                throw new Exception($@"PN r_sap_podetail.pn & r_sap_hb.custpn {kpl7xx_pn} Can't find PNREV");
                            }
                        }
                        kpl7xx_rev = kpl7xx.Rows.Count == 0 ? kpl7xx_rev : kpl7xx.Rows[0]["PNREV"].ToString();//非Optics系列取Map表，Optics系列取SAPBom表
                        if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
                    }
                }
                else
                {
                    throw new Exception($@"PN r_modelsubpn_map.partno {kpl.Rows[i]["ITEM_NUMBER"].ToString()} Can't find 7XX Partno");
                }

                JuniperAutoKpConfig d = new JuniperAutoKpConfig()
                {
                    PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
                    PN_SERIALIZATION = "YES",
                    CUST_PN = kpl.Rows[i]["CUSTOMER_PART_NUMBER"].ToString(),
                    PN_7XX = kpl7xx_pn,
                    QTY = 1,
                    TYPE = "PNO",
                    REV = kpl7xx_rev, //kpl.Rows[i]["REV"].ToString(),
                    CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
                };
                if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
                {
                    d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();
                }
                else
                {
                    d.SN_RULE = "";
                }

                if (d.CUST_PN.Contains("CHAS-") || d.CUST_PN.Contains("-CHAS"))
                {
                    d.CHAS_SN = d.PN;
                }
                else
                {
                    d.CHAS_SN = "";
                }

                data.Add(d);

                //if (d.PN.StartsWith("CHAS-"))
                //{
                //    d = new JuniperAutoKpConfig()
                //    {
                //        PN = "PNO-SYS2",
                //        PN_7XX = "",
                //        QTY = 1,
                //        TYPE = "PNO-SYS2",
                //        SN_RULE = "PNO-SYS2|@@@@@@@@",//測試用
                //        REV = "",
                //        CLEI_CODE = ""
                //    };
                //    data.Add(d);
                //}

            }


            //strsql = $@"select t.ITEM_NUMBER , t.SERIAL_NUMBER_MASK, t.CLEI_CODE, s.componentqty, t.SERIALIZATION from (
            //                select a.*,row_number() over(partition by item_number order by date_created desc) numbs from o_agile_attr a 
            //                where customer_part_number in 
            //                ( 
            //                select componentid from o_i137_detail where (o_i137_detail.tranid,o_i137_detail.item) in
            //                (select tranid, item from o_i137_item where id ='{order_main.ITEMID}')
            //                ) and PLANT = '{order_main.PLANT}' and item_number <> 'JUNOS-64' 
            //                )t , (select componentid,componentqty from o_i137_detail where (o_i137_detail.tranid,o_i137_detail.item) in
            //                (select tranid, item from o_i137_item where id ='{order_main.ITEMID}')) s
            //                where t.numbs=1 and t.customer_part_number = s.componentid  ";
            strsql = $@"select componentid, componentqty
                            from o_i137_detail
                            where (o_i137_detail.tranid, o_i137_detail.item) in
                                (select tranid, item
                                    from o_i137_item
                                    where id = '{order_main.ITEMID}')
                            and componentid  NOT LIKE  'JUNOS-64%'";
            var componentkpl = SFCDB.RunSelect(strsql).Tables[0];
            for (int k = 0; k < componentkpl.Rows.Count; k++)
            {
                strsql = $@"select t.ITEM_NUMBER,
                               t.SERIAL_NUMBER_MASK,
                               t.CLEI_CODE,
                               --s.componentqty,
                               t.SERIALIZATION,
                               t.CUSTOMER_PART_NUMBER
                          from (select a.*,
                                       row_number() over(partition by item_number order by date_created desc) numbs
                                  from o_agile_attr a
                                 where a.PLANT = '{order_main.PLANT}'
                                   and a.customer_part_number = '{componentkpl.Rows[k]["componentid"].ToString()}') t
                         where t.numbs = 1";
                kpl = SFCDB.RunSelect(strsql).Tables[0];
                if (kpl.Rows.Count == 0)
                {
                    throw new Exception($@"o_agile_attr.customer_part_number = {componentkpl.Rows[k]["componentid"].ToString()} Can't find, Please check!");
                }

                for (int i = 0; i < kpl.Rows.Count; i++)
                {
                    strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
                    kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

                    kpl7xx_pn = "";
                    kpl7xx_rev = "";
                    if (kpl7xx.Rows.Count > 0)
                    {
                        kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
                        kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();

                        if (kpl7xx_pn == "NA")
                        {
                            kpl7xx_pn = "";
                            kpl7xx_rev = "";
                        }
                        else
                        {
                            strsql = $@"select * from r_sap_podetail where wo = '{order_main.PREWO}' and pn = '{kpl7xx_pn}' order by createtime desc";
                            kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                            if (kpl7xx.Rows.Count == 0)
                            {
                                strsql = $@"select * from r_sap_hb where wo = '{order_main.PREWO}' and custpn = '{kpl7xx_pn}' order by createtime desc";
                                kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                                if (kpl7xx.Rows.Count == 0 && seriesName == "Juniper-Optics")//儘Optics系列卡關
                                {
                                    throw new Exception($@"PN r_sap_podetail.pn & r_sap_hb.custpn {kpl7xx_pn} Can't find PNREV");
                                }
                            }
                            kpl7xx_rev = kpl7xx.Rows.Count == 0 ? kpl7xx_rev : kpl7xx.Rows[0]["PNREV"].ToString();//非Optics系列取Map表，Optics系列取SAPBom表
                            if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
                        }
                    }
                    else
                    {
                        throw new Exception($@"PN r_modelsubpn_map.partno {kpl.Rows[i]["ITEM_NUMBER"].ToString()} Can't find 7XX Partno");
                    }

                    JuniperAutoKpConfig d = new JuniperAutoKpConfig()
                    {
                        PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
                        PN_SERIALIZATION = kpl.Rows[i]["SERIALIZATION"].ToString().ToUpper(),
                        CUST_PN = kpl.Rows[i]["CUSTOMER_PART_NUMBER"].ToString(),
                        PN_7XX = kpl7xx_pn,
                        QTY = float.Parse(componentkpl.Rows[k]["componentqty"].ToString()) / float.Parse(order_main.QTY),//float.Parse(kpl.Rows[i]["componentqty"].ToString()) / float.Parse(order_main.QTY),
                        TYPE = "I137",
                        REV = kpl7xx_rev,//kpl.Rows[i]["REV"].ToString().Length > 2 ? kpl.Rows[i]["REV"].ToString().Substring(0, 2) : kpl.Rows[i]["REV"].ToString(),
                        CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
                    };
                    if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
                    {
                        d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();
                    }
                    else
                    {
                        d.SN_RULE = "";
                    }

                    if (d.CUST_PN.Contains("CHAS-") || d.CUST_PN.Contains("-CHAS"))
                    {
                        d.CHAS_SN = d.PN;
                    }
                    else
                    {
                        d.CHAS_SN = "";
                    }

                    data.Add(d);
                }
            }

            //            strsql = $@"select * from (
            //select a.*,row_number() over(partition by item_number order by release_date desc) numbs from o_agile_attr a where item_number in ( 
            //select PN from r_sap_hb where wo='{order_main.PREWO}' ) and serialization ='Yes' and PLANT = '{order_main.PLANT}') 
            //where numbs=1";
            //從r_sap_hb表取QTY

            //strsql = $@"select t.ITEM_NUMBER , t.SERIAL_NUMBER_MASK, t.CLEI_CODE, s.USAGE, t.SERIALIZATION from (
            //                select a.*,row_number() over(partition by item_number order by date_created desc) numbs from o_agile_attr a where item_number in (
            //                select PN from r_sap_hb where wo = '{order_main.PREWO}' and substr(custparentpn,1,3) = 'HB_') and PLANT = '{order_main.PLANT}') t  , r_sap_hb s
            //                where t.numbs = 1 and t.item_number = s.pn and s.wo = '{order_main.PREWO}' ";
            //kpl = SFCDB.RunSelect(strsql).Tables[0];

            strsql = $@"select PN, USAGE
                          from r_sap_hb
                         where wo = '{order_main.PREWO}'
                           and substr(custparentpn, 1, 3) = 'HB_'";
            componentkpl = SFCDB.RunSelect(strsql).Tables[0];
            for (int k = 0; k < componentkpl.Rows.Count; k++)
            {
                strsql = $@"select t.ITEM_NUMBER,
                                   t.SERIAL_NUMBER_MASK,
                                   t.CLEI_CODE,
                                   --s.USAGE,
                                   t.SERIALIZATION,
                                   t.CUSTOMER_PART_NUMBER
                              from (select a.*,
                                           row_number() over(partition by item_number order by date_created desc) numbs
                                      from o_agile_attr a
                                     where a.PLANT = '{order_main.PLANT}'
                                       and a.item_number = '{componentkpl.Rows[k]["PN"].ToString()}') t
                             where t.numbs = 1";
                kpl = SFCDB.RunSelect(strsql).Tables[0];
                if (kpl.Rows.Count == 0)
                {
                    throw new Exception($@"o_agile_attr.item_number = {componentkpl.Rows[k]["PN"].ToString()} Can't find, Please check!");
                }
                for (int i = 0; i < kpl.Rows.Count; i++)
                {
                    strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
                    kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

                    kpl7xx_pn = "";
                    kpl7xx_rev = "";
                    if (kpl7xx.Rows.Count > 0)
                    {
                        kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
                        kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();

                        if (kpl7xx_pn == "NA")
                        {
                            kpl7xx_pn = "";
                            kpl7xx_rev = "";
                        }
                        else
                        {
                            strsql = $@"select * from r_sap_podetail where wo = '{order_main.PREWO}' and pn = '{kpl7xx_pn}' order by createtime desc";
                            kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                            if (kpl7xx.Rows.Count == 0)
                            {
                                strsql = $@"select * from r_sap_hb where wo = '{order_main.PREWO}' and custpn = '{kpl7xx_pn}' order by createtime desc";
                                kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                                if (kpl7xx.Rows.Count == 0 && seriesName == "Juniper-Optics")//儘Optics系列卡關
                                {
                                    throw new Exception($@"PN r_sap_podetail.pn & r_sap_hb.custpn {kpl7xx_pn} Can't find PNREV");
                                }
                            }
                            kpl7xx_rev = kpl7xx.Rows.Count == 0 ? kpl7xx_rev : kpl7xx.Rows[0]["PNREV"].ToString();//非Optics系列取Map表，Optics系列取SAPBom表
                            if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
                        }
                    }
                    else
                    {
                        throw new Exception($@"PN r_modelsubpn_map.partno {kpl.Rows[i]["ITEM_NUMBER"].ToString()} Can't find 7XX Partno");
                    }

                    JuniperAutoKpConfig d = new JuniperAutoKpConfig()
                    {
                        PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
                        PN_SERIALIZATION = kpl.Rows[i]["SERIALIZATION"].ToString().ToUpper(),
                        CUST_PN = kpl.Rows[i]["CUSTOMER_PART_NUMBER"].ToString(),
                        PN_7XX = kpl7xx_pn,
                        QTY = float.Parse(componentkpl.Rows[k]["USAGE"].ToString()),//float.Parse(kpl.Rows[i]["USAGE"].ToString()),
                        TYPE = "SAP_HB",
                        REV = kpl7xx_rev, //kpl.Rows[i]["REV"].ToString().Length > 2 ? kpl.Rows[i]["REV"].ToString().Substring(0, 2) : kpl.Rows[i]["REV"].ToString(),
                        CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
                    };
                    if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
                    {
                        d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();
                    }
                    else
                    {
                        d.SN_RULE = "";
                    }

                    if (d.CUST_PN.Contains("CHAS-") || d.CUST_PN.Contains("-CHAS"))
                    {
                        d.CHAS_SN = d.PN;
                    }
                    else
                    {
                        d.CHAS_SN = "";
                    }

                    data.Add(d);
                }
            }

            #region BNDL
            //strsql = $@"select * from(
            //                select a.*,row_number() over(partition by item_number order by release_date desc) numbs  from o_agile_attr a where item_number in 
            //                ( 
            //                    select partno from  o_order_option where mainid='{order_main.ID}' and optiontype='BNDL'
            //                ) and PLANT = '{order_main.PLANT}'
            //                ) -- order by item_number , date_created;
            //                where numbs=1";
            //kpl = SFCDB.RunSelect(strsql).Tables[0];
            //for (int i = 0; i < kpl.Rows.Count; i++)
            //{
            //    strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
            //    kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

            //    kpl7xx_pn = "";
            //    kpl7xx_rev = "";
            //    if (kpl7xx.Rows.Count > 0)
            //    {
            //        kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
            //        kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();
            //        if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
            //        if (kpl7xx_pn == "9177-00010")
            //        {
            //            kpl7xx_pn = "";
            //            kpl7xx_rev = "";
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("Can't find 7XX Partno");
            //    }

            //    JuniperAutoKpConfig d = new JuniperAutoKpConfig()
            //    {
            //        //PN = rb == true ? rbpn : kpl.Rows[i]["ITEM_NUMBER"].ToString(), //kpl.Rows[i]["ITEM_NUMBER"].ToString(),
            //        PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
            //        PN_7XX = kpl7xx_pn,
            //        QTY = 1,
            //        TYPE = "BNDL",
            //        REV = kpl7xx_rev, //kpl.Rows[i]["REV"].ToString().Length > 2 ? kpl.Rows[i]["REV"].ToString().Substring(0, 2) : kpl.Rows[i]["REV"].ToString(),
            //        CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
            //    };
            //    if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
            //    {
            //        d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();

            //    }
            //    else
            //    {
            //        d.SN_RULE = "";
            //    }
            //    else
            //    {
            //        d.CHAS_SN = "";
            //    }

            //    data.Add(d);
            //}
            #endregion

            List<JuniperAutoKpConfig> list = new List<JuniperAutoKpConfig>();
            var listPNO = data.Where(r => r.TYPE == "PNO").OrderBy(r => r.PN);
            var listHB = data.Where(r => r.TYPE == "SAP_HB").OrderBy(r => r.PN);
            var listI137 = data.Where(r => r.TYPE == "I137").OrderBy(r => r.PN);
            list.AddRange(listPNO);
            list.AddRange(listHB);
            list.AddRange(listI137);

            JsonSave.SaveToDB(list, strWO, "JuniperAutoKPConfig", "", SFCDB, BU, true);
            return data;
        }

        static List<JuniperAutoKpConfig> MakeAutoKPConfigNew(string strWO, OleExec SFCDB, string BU)
        {
            var db = SFCDB.ORM;
            List<JuniperAutoKpConfig> data = new List<JuniperAutoKpConfig>();
            var order_main = db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strWO).First();
            if (order_main == null)
            {
                throw new Exception($@"o_order_main.prewo {strWO} == null");
            }

            DataTable kpl7xx = null;
            var kpl7xx_pn = "";
            var kpl7xx_rev = "";

            var strsql = $@"select * from(
                                select a.*,row_number() over(partition by item_number order by date_created desc) numbs from o_agile_attr a 
                                    where item_number = '{order_main.PID}' and plant = '{order_main.PLANT}'
                                ) 
                                where numbs=1";
            var kpl = SFCDB.RunSelect(strsql).Tables[0];
            if (kpl.Rows.Count == 0)
            {
                throw new Exception($@"o_agile_attr.item_number {order_main.PID} Can't find, Please check!");
            }

            //非Optics系列不卡r_modelsubpn_map.subpartno與SAPBom是否一致，若SAPBom無資料則取Map資料 Asked By PE 譚義康 2021-12-04
            strsql = $@"select s.series_name from c_sku c, c_series s where c.c_series_id=s.id and c.skuno='{order_main.PID}'";
            var seriesDT = SFCDB.RunSelect(strsql).Tables[0];
            if (seriesDT.Rows.Count == 0) throw new Exception($@"order_main.PID {order_main.PID} Can't find SerieName, Please check!");
            var seriesName = seriesDT.Rows[0]["series_name"].ToString();

            for (int i = 0; i < kpl.Rows.Count; i++)
            {
                strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
                kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

                kpl7xx_pn = "";
                kpl7xx_rev = "";
                if (kpl7xx.Rows.Count > 0)
                {
                    kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
                    kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();

                    if (kpl7xx_pn == "NA")
                    {
                        kpl7xx_pn = "";
                        kpl7xx_rev = "";
                    }
                    else
                    {
                        if (!SFCDB.ORM.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == strWO && t.PN == kpl7xx_pn).Any() && seriesName == "Juniper-Optics") //儘Optics系列卡關
                        {
                            throw new Exception($@"'{kpl7xx_pn}' not in SAP_BOM");
                        }

                        strsql = $@"select * from r_sap_podetail where wo = '{order_main.PREWO}' and pn = '{kpl7xx_pn}' order by createtime desc";
                        kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                        if (kpl7xx.Rows.Count == 0)
                        {
                            strsql = $@"select * from r_sap_hb where wo = '{order_main.PREWO}' and custpn = '{kpl7xx_pn}' order by createtime desc";
                            kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                            if (kpl7xx.Rows.Count == 0 && seriesName == "Juniper-Optics")//儘Optics系列卡關
                            {
                                throw new Exception($@"PN r_sap_podetail.pn & r_sap_hb.custpn {kpl7xx_pn} Can't find PNREV");
                            }
                        }
                        kpl7xx_rev = kpl7xx.Rows.Count == 0 ? kpl7xx_rev : kpl7xx.Rows[0]["PNREV"].ToString();//非Optics系列取Map表，Optics系列取SAPBom表
                        if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
                    }
                }
                else
                {
                    throw new Exception($@"PN r_modelsubpn_map.partno {kpl.Rows[i]["ITEM_NUMBER"].ToString()} Can't find 7XX Partno");
                }

                JuniperAutoKpConfig d = new JuniperAutoKpConfig()
                {
                    PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
                    PN_SERIALIZATION = "YES",
                    CUST_PN = kpl.Rows[i]["CUSTOMER_PART_NUMBER"].ToString(),
                    PN_7XX = kpl7xx_pn,
                    QTY = 1,
                    TYPE = "PNO",
                    REV = kpl7xx_rev, //kpl.Rows[i]["REV"].ToString(),
                    CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
                };
                if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
                {
                    d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();
                }
                else
                {
                    d.SN_RULE = "";
                }

                if (d.CUST_PN.Contains("CHAS-") || d.CUST_PN.Contains("-CHAS"))
                {
                    d.CHAS_SN = d.PN;
                }
                else
                {
                    d.CHAS_SN = "";
                }

                data.Add(d);

                //if (d.PN.StartsWith("CHAS-"))
                //{
                //    d = new JuniperAutoKpConfig()
                //    {
                //        PN = "PNO-SYS2",
                //        PN_7XX = "",
                //        QTY = 1,
                //        TYPE = "PNO-SYS2",
                //        SN_RULE = "PNO-SYS2|@@@@@@@@",//測試用
                //        REV = "",
                //        CLEI_CODE = ""
                //    };
                //    data.Add(d);
                //}

            }


            //strsql = $@"select t.ITEM_NUMBER , t.SERIAL_NUMBER_MASK, t.CLEI_CODE, s.componentqty, t.SERIALIZATION from (
            //                select a.*,row_number() over(partition by item_number order by date_created desc) numbs from o_agile_attr a 
            //                where customer_part_number in 
            //                ( 
            //                select componentid from o_i137_detail where (o_i137_detail.tranid,o_i137_detail.item) in
            //                (select tranid, item from o_i137_item where id ='{order_main.ITEMID}')
            //                ) and PLANT = '{order_main.PLANT}' and item_number <> 'JUNOS-64' 
            //                )t , (select componentid,componentqty from o_i137_detail where (o_i137_detail.tranid,o_i137_detail.item) in
            //                (select tranid, item from o_i137_item where id ='{order_main.ITEMID}')) s
            //                where t.numbs=1 and t.customer_part_number = s.componentid  ";
            strsql = $@"select componentid, componentqty
                            from o_i137_detail
                            where (o_i137_detail.tranid, o_i137_detail.item) in
                                (select tranid, item
                                    from o_i137_item
                                    where id = '{order_main.ITEMID}')
                            and componentid  NOT LIKE  'JUNOS-64%'";
            var componentkpl = SFCDB.RunSelect(strsql).Tables[0];
            for (int k = 0; k < componentkpl.Rows.Count; k++)
            {
                strsql = $@"select t.ITEM_NUMBER,
                               t.SERIAL_NUMBER_MASK,
                               t.CLEI_CODE,
                               --s.componentqty,
                               t.SERIALIZATION,
                               t.CUSTOMER_PART_NUMBER
                          from (select a.*,
                                       row_number() over(partition by item_number order by date_created desc) numbs
                                  from o_agile_attr a
                                 where a.PLANT = '{order_main.PLANT}'
                                   and a.customer_part_number = '{componentkpl.Rows[k]["componentid"].ToString()}') t
                         where t.numbs = 1";
                kpl = SFCDB.RunSelect(strsql).Tables[0];
                if (kpl.Rows.Count == 0)
                {
                    throw new Exception($@"o_agile_attr.customer_part_number = {componentkpl.Rows[k]["componentid"].ToString()} Can't find, Please check!");
                }

                for (int i = 0; i < kpl.Rows.Count; i++)
                {
                    strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
                    kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

                    kpl7xx_pn = "";
                    kpl7xx_rev = "";
                    if (kpl7xx.Rows.Count > 0)
                    {
                        kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
                        kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();

                        if (kpl7xx_pn == "NA")
                        {
                            kpl7xx_pn = "";
                            kpl7xx_rev = "";
                        }
                        else
                        {
                            strsql = $@"select * from r_sap_podetail where wo = '{order_main.PREWO}' and pn = '{kpl7xx_pn}' order by createtime desc";
                            kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                            if (kpl7xx.Rows.Count == 0)
                            {
                                strsql = $@"select * from r_sap_hb where wo = '{order_main.PREWO}' and custpn = '{kpl7xx_pn}' order by createtime desc";
                                kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                                if (kpl7xx.Rows.Count == 0 && seriesName == "Juniper-Optics")//儘Optics系列卡關
                                {
                                    throw new Exception($@"PN r_sap_podetail.pn & r_sap_hb.custpn {kpl7xx_pn} Can't find PNREV");
                                }
                            }
                            kpl7xx_rev = kpl7xx.Rows.Count == 0 ? kpl7xx_rev : kpl7xx.Rows[0]["PNREV"].ToString();//非Optics系列取Map表，Optics系列取SAPBom表
                            if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
                        }
                    }
                    else
                    {
                        throw new Exception($@"PN r_modelsubpn_map.partno {kpl.Rows[i]["ITEM_NUMBER"].ToString()} Can't find 7XX Partno");
                    }

                    JuniperAutoKpConfig d = new JuniperAutoKpConfig()
                    {
                        PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
                        PN_SERIALIZATION = kpl.Rows[i]["SERIALIZATION"].ToString().ToUpper(),
                        CUST_PN = kpl.Rows[i]["CUSTOMER_PART_NUMBER"].ToString(),
                        PN_7XX = kpl7xx_pn,
                        QTY = float.Parse(componentkpl.Rows[k]["componentqty"].ToString()) / float.Parse(order_main.QTY),//float.Parse(kpl.Rows[i]["componentqty"].ToString()) / float.Parse(order_main.QTY),
                        TYPE = "I137",
                        REV = kpl7xx_rev,//kpl.Rows[i]["REV"].ToString().Length > 2 ? kpl.Rows[i]["REV"].ToString().Substring(0, 2) : kpl.Rows[i]["REV"].ToString(),
                        CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
                    };
                    if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
                    {
                        d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();
                    }
                    else
                    {
                        d.SN_RULE = "";
                    }

                    if (d.CUST_PN.Contains("CHAS-") || d.CUST_PN.Contains("-CHAS"))
                    {
                        d.CHAS_SN = d.PN;
                    }
                    else
                    {
                        d.CHAS_SN = "";
                    }

                    data.Add(d);
                }
            }

            //            strsql = $@"select * from (
            //select a.*,row_number() over(partition by item_number order by release_date desc) numbs from o_agile_attr a where item_number in ( 
            //select PN from r_sap_hb where wo='{order_main.PREWO}' ) and serialization ='Yes' and PLANT = '{order_main.PLANT}') 
            //where numbs=1";
            //從r_sap_hb表取QTY

            //strsql = $@"select t.ITEM_NUMBER , t.SERIAL_NUMBER_MASK, t.CLEI_CODE, s.USAGE, t.SERIALIZATION from (
            //                select a.*,row_number() over(partition by item_number order by date_created desc) numbs from o_agile_attr a where item_number in (
            //                select PN from r_sap_hb where wo = '{order_main.PREWO}' and substr(custparentpn,1,3) = 'HB_') and PLANT = '{order_main.PLANT}') t  , r_sap_hb s
            //                where t.numbs = 1 and t.item_number = s.pn and s.wo = '{order_main.PREWO}' ";
            //kpl = SFCDB.RunSelect(strsql).Tables[0];

            strsql = $@"select PN, USAGE
                          from r_sap_hb
                         where wo = '{order_main.PREWO}'
                           and substr(custparentpn, 1, 3) = 'HB_'";
            componentkpl = SFCDB.RunSelect(strsql).Tables[0];
            for (int k = 0; k < componentkpl.Rows.Count; k++)
            {
                strsql = $@"select t.ITEM_NUMBER,
                                   t.SERIAL_NUMBER_MASK,
                                   t.CLEI_CODE,
                                   --s.USAGE,
                                   t.SERIALIZATION,
                                   t.CUSTOMER_PART_NUMBER
                              from (select a.*,
                                           row_number() over(partition by item_number order by date_created desc) numbs
                                      from o_agile_attr a
                                     where a.PLANT = '{order_main.PLANT}'
                                       and a.item_number = '{componentkpl.Rows[k]["PN"].ToString()}') t
                             where t.numbs = 1";
                kpl = SFCDB.RunSelect(strsql).Tables[0];
                if (kpl.Rows.Count == 0)
                {
                    throw new Exception($@"o_agile_attr.item_number = {componentkpl.Rows[k]["PN"].ToString()} Can't find, Please check!");
                }
                for (int i = 0; i < kpl.Rows.Count; i++)
                {
                    strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
                    kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

                    kpl7xx_pn = "";
                    kpl7xx_rev = "";
                    if (kpl7xx.Rows.Count > 0)
                    {
                        kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
                        kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();

                        if (kpl7xx_pn == "NA")
                        {
                            kpl7xx_pn = "";
                            kpl7xx_rev = "";
                        }
                        else
                        {
                            strsql = $@"select * from r_sap_podetail where wo = '{order_main.PREWO}' and pn = '{kpl7xx_pn}' order by createtime desc";
                            kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                            if (kpl7xx.Rows.Count == 0)
                            {
                                strsql = $@"select * from r_sap_hb where wo = '{order_main.PREWO}' and custpn = '{kpl7xx_pn}' order by createtime desc";
                                kpl7xx = SFCDB.RunSelect(strsql).Tables[0];
                                if (kpl7xx.Rows.Count == 0 && seriesName == "Juniper-Optics")//儘Optics系列卡關
                                {
                                    throw new Exception($@"PN r_sap_podetail.pn & r_sap_hb.custpn {kpl7xx_pn} Can't find PNREV");
                                }
                            }
                            kpl7xx_rev = kpl7xx.Rows.Count == 0 ? kpl7xx_rev : kpl7xx.Rows[0]["PNREV"].ToString();//非Optics系列取Map表，Optics系列取SAPBom表
                            if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
                        }
                    }
                    else
                    {
                        throw new Exception($@"PN r_modelsubpn_map.partno {kpl.Rows[i]["ITEM_NUMBER"].ToString()} Can't find 7XX Partno");
                    }

                    JuniperAutoKpConfig d = new JuniperAutoKpConfig()
                    {
                        PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
                        PN_SERIALIZATION = kpl.Rows[i]["SERIALIZATION"].ToString().ToUpper(),
                        CUST_PN = kpl.Rows[i]["CUSTOMER_PART_NUMBER"].ToString(),
                        PN_7XX = kpl7xx_pn,
                        QTY = float.Parse(componentkpl.Rows[k]["USAGE"].ToString()),//float.Parse(kpl.Rows[i]["USAGE"].ToString()),
                        TYPE = "SAP_HB",
                        REV = kpl7xx_rev, //kpl.Rows[i]["REV"].ToString().Length > 2 ? kpl.Rows[i]["REV"].ToString().Substring(0, 2) : kpl.Rows[i]["REV"].ToString(),
                        CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
                    };
                    if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
                    {
                        d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();
                    }
                    else
                    {
                        d.SN_RULE = "";
                    }

                    if (d.CUST_PN.Contains("CHAS-") || d.CUST_PN.Contains("-CHAS"))
                    {
                        d.CHAS_SN = d.PN;
                    }
                    else
                    {
                        d.CHAS_SN = "";
                    }

                    data.Add(d);
                }
            }

            #region BNDL
            //strsql = $@"select * from(
            //                select a.*,row_number() over(partition by item_number order by release_date desc) numbs  from o_agile_attr a where item_number in 
            //                ( 
            //                    select partno from  o_order_option where mainid='{order_main.ID}' and optiontype='BNDL'
            //                ) and PLANT = '{order_main.PLANT}'
            //                ) -- order by item_number , date_created;
            //                where numbs=1";
            //kpl = SFCDB.RunSelect(strsql).Tables[0];
            //for (int i = 0; i < kpl.Rows.Count; i++)
            //{
            //    strsql = $@"select subpartno as ITEM_NUMBER,subpnrev as REV,createtime from r_modelsubpn_map where partno = '{kpl.Rows[i]["ITEM_NUMBER"].ToString()}' order by createtime desc";
            //    kpl7xx = SFCDB.RunSelect(strsql).Tables[0];

            //    kpl7xx_pn = "";
            //    kpl7xx_rev = "";
            //    if (kpl7xx.Rows.Count > 0)
            //    {
            //        kpl7xx_pn = kpl7xx.Rows[0]["ITEM_NUMBER"].ToString();
            //        kpl7xx_rev = kpl7xx.Rows[0]["REV"].ToString();
            //        if (kpl7xx_rev.Length > 2) kpl7xx_rev = kpl7xx_rev.Substring(0, 2);
            //        if (kpl7xx_pn == "9177-00010")
            //        {
            //            kpl7xx_pn = "";
            //            kpl7xx_rev = "";
            //        }
            //    }
            //    else
            //    {
            //        throw new Exception("Can't find 7XX Partno");
            //    }

            //    JuniperAutoKpConfig d = new JuniperAutoKpConfig()
            //    {
            //        //PN = rb == true ? rbpn : kpl.Rows[i]["ITEM_NUMBER"].ToString(), //kpl.Rows[i]["ITEM_NUMBER"].ToString(),
            //        PN = kpl.Rows[i]["ITEM_NUMBER"].ToString(),
            //        PN_7XX = kpl7xx_pn,
            //        QTY = 1,
            //        TYPE = "BNDL",
            //        REV = kpl7xx_rev, //kpl.Rows[i]["REV"].ToString().Length > 2 ? kpl.Rows[i]["REV"].ToString().Substring(0, 2) : kpl.Rows[i]["REV"].ToString(),
            //        CLEI_CODE = kpl.Rows[i]["CLEI_CODE"].ToString()
            //    };
            //    if (kpl.Rows[i]["SERIAL_NUMBER_MASK"] != DBNull.Value && kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString() != null)
            //    {
            //        d.SN_RULE = kpl.Rows[i]["SERIAL_NUMBER_MASK"].ToString();

            //    }
            //    else
            //    {
            //        d.SN_RULE = "";
            //    }
            //    else
            //    {
            //        d.CHAS_SN = "";
            //    }

            //    data.Add(d);
            //}
            #endregion

            List<JuniperAutoKpConfig> list = new List<JuniperAutoKpConfig>();
            var listPNO = data.Where(r => r.TYPE == "PNO").OrderBy(r => r.PN);
            var listHB = data.Where(r => r.TYPE == "SAP_HB").OrderBy(r => r.PN);
            var listI137 = data.Where(r => r.TYPE == "I137").OrderBy(r => r.PN);
            list.AddRange(listPNO);
            list.AddRange(listHB);
            list.AddRange(listI137);

            strsql = $@"select * from r_sap_as_bom where wo ='{strWO}'";
            var asBom = SFCDB.ORM.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == strWO).ToList();
            var asBomHb = SFCDB.ORM.Queryable<R_SAP_HB>().Where(t => t.WO == strWO).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if ((order_main.OFFERINGTYPE == "Premium Configurable Sys" || order_main.OFFERINGTYPE == "Customer-Specific CTO")
                    && i == 0)
                {
                    continue;
                }

                if (list[i].PN_7XX == "")
                {
                    continue;
                }

                if (list[i].TYPE == "SAP_HB")
                {
                    var bom = asBomHb.Find(t => t.PN == list[i].PN_7XX || t.PN == list[i].PN_7XX + "-FVN" ||
                    t.PN == list[i].PN_7XX + "-FJZ" || t.PN == list[i].PN_7XX + "-TAA");
                    if (bom == null)
                    {
                        throw new Exception($@"PN R_SAP_HB.pn {list[i].PN_7XX} Can't find 7XX Partno");
                    }
                    list[i].REV = bom.PNREV;

                    if (list[i].CLEI_CODE == null || list[i].CLEI_CODE == "")
                    {
                        continue;
                    }
                    var pnbom = asBomHb.Find(t =>  t.PN == list[i].PN );
                    if (pnbom == null)
                    {
                        throw new Exception($@"PN R_SAP_HB.pn {list[i].PN} Can't find PN ");
                    }

                    var agiledatas = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == list[i].PN)
                       .OrderBy(t => t.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList();
                    var agiledata = agiledatas.Find(t => t.REV == pnbom.PNREV);
                    if (agiledata == null)
                    {
                        agiledata = agiledatas.Find(t => t.REV.Substring(0,2) == pnbom.PNREV);
                    }
                    if (agiledata == null)
                    {
                        throw new Exception($@"PN O_AGILE_ATTR.ITEM_NUMBER {list[i].PN} Can't find rev {pnbom.PNREV}");
                    }
                    list[i].CLEI_CODE = agiledata.CLEI_CODE;
                }
                else
                {
                    var bom = asBom.Find(t => t.CUSTPARENTPN == list[i].CUST_PN && ( t.PN == list[i].PN_7XX || t.PN == list[i].PN_7XX + "-FVN" ||
                    t.PN == list[i].PN_7XX + "-FJZ" || t.PN == list[i].PN_7XX + "-TAA"));
                    if (bom == null)
                    {
                        bom = SFCDB.ORM.Queryable<R_SAP_AS_BOM, R_SAP_AS_BOM>((r1, r2) => r1.PN == r2.PARENTPN).Where((r1, r2) => r1.CUSTPARENTPN == list[i].CUST_PN && r2.WO == strWO
                         && (r2.PN == list[i].PN_7XX || r2.PN == SqlSugar.SqlFunc.MergeString(list[i].PN_7XX,"-FVN") ||
                         r2.PN == SqlSugar.SqlFunc.MergeString(list[i].PN_7XX ,"-FJZ") || r2.PN == SqlSugar.SqlFunc.MergeString(list[i].PN_7XX , "-TAA"))).Select((r1, r2) => r2).ToList().FirstOrDefault();
                        if (bom == null)
                        {
                            throw new Exception($@"PN R_SAP_AS_BOM.pn {list[i].PN_7XX} Can't find 7XX Partno");
                        }
                    }
                
                    list[i].REV = bom.PNREV;

                    if (list[i].CLEI_CODE == null || list[i].CLEI_CODE == "")
                    {
                        continue;
                    }
                    //var agiledata = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == list[i].PN && t.REV == bom.PARENTREV)
                    //   .OrderBy(t => t.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList();
                    //if (agiledata.Count == 0)
                    //{
                    //    throw new Exception($@"PN O_AGILE_ATTR.ITEM_NUMBER {list[i].PN} Can't find rev {bom.PARENTREV}");
                    //}
                    //list[i].CLEI_CODE = agiledata[0].CLEI_CODE;
                    var agiledatas = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == list[i].PN)
                      .OrderBy(t => t.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList();
                    var agiledata = agiledatas.Find(t => t.REV == bom.PNREV);
                    if (agiledata == null)
                    {
                        agiledata = agiledatas.Find(t => t.REV.Substring(0, 2) == bom.PARENTREV);
                    }
                    if (agiledata == null)
                    {
                        throw new Exception($@"PN O_AGILE_ATTR.ITEM_NUMBER {list[i].PN} Can't find rev {bom.PARENTREV}");
                    }
                    list[i].CLEI_CODE = agiledata.CLEI_CODE;
                }


            }

            JsonSave.SaveToDB(list, strWO, "JuniperAutoKPConfig", "", SFCDB, BU, true);
            return list;
        }

        DataTable GetConverWoList(OleExec SFCDB)
        {
            var sql = $@"SELECT AA.*,case when AA.convertdate<=sysdate then 'Y' ELSE 'N' END AS CONVERSTATUS  FROM (
                                select A.*,   case when TO_CHAR(A.cdd,'D')=1 then A.cdd-2
                                                                         when TO_CHAR(A.cdd,'D')=7 then A.cdd-1
                                                                           else  A.cdd end as convertdate     from (
                                                            select a.pono,a.poline,a.version,a.potype,a.qty,a.prewo,a.pid,a.plant,a.custpid,c.podeliverydate,c.custreqshipdate,
                                                            (case 
                                                                             when c.podeliverydate=c.custreqshipdate then c.custreqshipdate -5                                          
                                                                             when c.podeliverydate<c.custreqshipdate then c.podeliverydate -5                                      
                                                                             when c.podeliverydate-c.custreqshipdate<=4 then c.custreqshipdate -5 
                                                                             else c.podeliverydate-9
                                                                             end) cdd 
                                                          from o_order_main a, o_po_status b,o_i137_item c
                                                         where a.id = b.poid
                                                           and a.prewo is not null
                                                           and b.statusid = '8'
                                                           and a.ordertype<>'ZRMQ'
                                                           and b.validflag = '1'
                                                           and a.itemid=c.id ) A )AA   order by AA.convertdate,AA.pid,AA.pono,AA.poline asc";
            return SFCDB.ExecuteDataTable(sql, CommandType.Text);
        }
        /// <summary>
        /// 配合 LoadingSkuObjAndGroupIdByWo,JuniperLoadPackingByTransport Action使用
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="orderMain"></param>
        /// <param name="BU"></param>
        public static void AutoCalculateWOPackage(OleExec SFCDB, O_ORDER_MAIN orderMain, string BU)
        {
            var preWoDetail = SFCDB.ORM.Queryable<R_PRE_WO_DETAIL>().Where(r => r.WO == orderMain.PREWO && r.PARTNOTYPE == "PACKAGE").ToList().FirstOrDefault();
            if (preWoDetail == null)
            {
                throw new Exception($"WO=[{orderMain.PREWO}],PARTNOTYPE=[PACKAGE] no in R_PRE_WO_DETAIL");
            }
            var item = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == orderMain.ITEMID).ToList().FirstOrDefault();
            ENUM_O_SKU_PACKAGE_SCENARIO Scenariotype = string.IsNullOrEmpty(item.CARTONLABEL2) ? ENUM_O_SKU_PACKAGE_SCENARIO.Carton_Overpack : ENUM_O_SKU_PACKAGE_SCENARIO.Carton_Multipack;
            var skuPackge = SFCDB.ORM.Queryable<O_SKU_PACKAGE>().Where(o => o.SKUNO == orderMain.PID && o.PARTNO == preWoDetail.PARTNO
            && o.SCENARIO == Scenariotype.ExtValue() && o.TYPE == ENUM_O_SKU_PACKAGE_PACKTYPE.Optional.ExtValue()).ToList().FirstOrDefault();
            if (skuPackge == null)
            {
                throw new Exception($"SKUNO=[{orderMain.PID}],PARTNO=[{preWoDetail.PARTNO}]  no in O_SKU_PACKAGE");
            }
            if (!double.TryParse(orderMain.QTY, out double woQty))
            {
                throw new Exception($"QTY=[{orderMain.QTY}] is not a number in O_ORDER_MAIN");
            }
            if (!double.TryParse(skuPackge.TON, out double tNum))
            {
                throw new Exception($"TON=[{skuPackge.TON}] is not a number in O_SKU_PACKAGE");
            }

            var preWoHead = SFCDB.ORM.Queryable<R_PRE_WO_HEAD>().Where(r => r.WO == orderMain.PREWO).ToList().FirstOrDefault();
            if (preWoHead == null || string.IsNullOrEmpty(preWoHead.GROUPID))
            {
                throw new Exception($"[{orderMain.PREWO}] GroupID is null");
            }


            C_PACKING pidPacking = SFCDB.ORM.Queryable<C_PACKING>().Where(r => r.SKUNO == orderMain.PID && r.PACK_TYPE == "CARTON").ToList().FirstOrDefault();
            string snRule = pidPacking == null ? "DefCarton" : (string.IsNullOrWhiteSpace(pidPacking.SN_RULE) ? "DefCarton" : pidPacking.SN_RULE);
            C_PACKING cPacking = new C_PACKING();
            double maxQty = tNum;
            if (woQty % tNum != 0)
            {
                int totalBox = (int)woQty / (int)tNum + 1;
                maxQty = woQty % totalBox == 0 ? ((int)woQty / totalBox) : ((int)woQty / totalBox) + 1;
            }
            if (maxQty > tNum)
            {
                throw new Exception($"Calculation error,[MAX_QTY]{maxQty} >[TON]{tNum}");
            }
            cPacking.ID = MesDbBase.GetNewID<C_PACKING>(SFCDB.ORM, BU);
            cPacking.SKUNO = preWoHead.GROUPID;
            cPacking.PACK_TYPE = "CARTON";
            cPacking.TRANSPORT_TYPE = "DEFAULT";
            cPacking.INSIDE_PACK_TYPE = "SN";
            cPacking.MAX_QTY = maxQty;
            cPacking.DESCRIPTION = "AutoCalculateWOPackage";
            cPacking.EDIT_TIME = SFCDB.ORM.GetDate();
            cPacking.EDIT_EMP = "SYSTEM";
            cPacking.SN_RULE = snRule;
            SFCDB.ORM.Insertable<C_PACKING>(cPacking).ExecuteCommand();
        }
        MesFunctionRes ConvertWoByWo(OleExec SFCDB, string WO, string Bu)
        {
            var res = new MesFunctionRes();
            try
            {
                //throw new Exception($@"wo:{WO} =>test error!");
                #region ReleaseDueTime Wo and check lock
                SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(t => new R_MES_LOG() { DATA2 = MesBool.No.ExtValue() }).Where(t => t.DATA2 == MesBool.Yes.ExtValue() && t.EDIT_TIME < System.DateTime.Now.AddMinutes(-30)).ExecuteCommand();
                if (SFCDB.ORM.Queryable<R_MES_LOG>().Any(t => t.FUNCTION_NAME == "ConverWoLock" && t.DATA1 == WO && t.DATA2 == MesBool.Yes.ExtValue()))
                    throw new Exception("Operate too frequently!");
                #endregion

                if(SFCDB.ORM.Queryable<R_WO_BASE>().Any(t=>t.WORKORDERNO==WO))
                    throw new Exception("The Workorder already Convered,Pls check!");

                #region lock
                SFCDB.ORM.Insertable(new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                    PROGRAM_NAME = "ConverWoLock",
                    FUNCTION_NAME = "ConverWoLock",
                    DATA1 = WO,
                    DATA2 = MesBool.Yes.ExtValue(),
                    EDIT_TIME = DateTime.Now,
                    EDIT_EMP = this.LoginUser.EMP_NO
                }).ExecuteCommand();
                #endregion
                SFCDB.BeginTrain();
                var C_SKU = new T_C_SKU(SFCDB, DB_TYPE_ENUM.Oracle);
                var synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                var C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(SFCDB, DB_TYPE_ENUM.Oracle);

                var R_WO_HEADER = new T_R_WO_HEADER(SFCDB, DB_TYPE_ENUM.Oracle);
                var R_WO_ITEM = new T_R_WO_ITEM(SFCDB, DB_TYPE_ENUM.Oracle);
                var R_WO_TEXT = new T_R_WO_TEXT(SFCDB, DB_TYPE_ENUM.Oracle);
                var RouteDetail = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                var WOType = new T_R_WO_TYPE(SFCDB, DB_TYPE_ENUM.Oracle);
                var Keypart = new T_C_KEYPART(SFCDB, DB_TYPE_ENUM.Oracle);
                var C_ROUTE = new T_C_ROUTE(SFCDB, DB_TYPE_ENUM.Oracle);
                var R_WO_BASE = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                var T_Series = new T_C_SERIES(SFCDB, DB_TYPE_ENUM.Oracle);
                var t_c_kp_list = new T_C_KP_LIST(SFCDB, DB_TYPE_ENUM.Oracle);
                string series = "";
                C_SERIES C_Series;
                C_SERIES C_SeriesC;
                Row_C_ROUTE rowRoute;
                Row_C_ROUTE rowRouteC;
                R_WO_TYPE rowWOType;
                Row_R_WO_BASE rowWOBase;
                Row_R_WO_BASE rowWOBaseC;
                List<C_ROUTE_DETAIL> routeDetailList;
                List<C_ROUTE_DETAIL> routeDetailListC;
                List<C_KEYPART> keypartList;
                List<string> keypartIDList;
                List<string> keypartIDListC;
                List<C_ROUTE_DETAIL> linkStationList;

                var db = SFCDB.ORM;
                var OOM = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID).Where((m, s) => m.PREWO == WO && s.STATUSID == ENUM_O_PO_STATUS.DownloadWo.ExtValue()
                      && s.VALIDFLAG == MesBool.Yes.ExtValue()).Select((m, s) => m).First();
                if (OOM == null)
                {
                    throw new Exception("Can't find prewo on O_ORDER_MAIN");
                }
                var flag = db.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).First();
                if (flag != null)
                {
                    throw new Exception("Work order already exists");
                }
                #region lock Check, ECO_FCO or POLine Manual Lock
                var lockConvert = LockManager.CheckLock(WO, "WO", "CONVERTWO", SFCDB);
                if (lockConvert.Count != 0)
                {
                    throw new Exception(lockConvert[0].LOCK_REASON);
                }
                #endregion

                var SKUNO = OOM.PID;
                var WO_NO = OOM.PREWO;
                var QTY = (int)float.Parse(OOM.QTY.ToString());

                var Sku = C_SKU.GetSku(SKUNO, SFCDB);
                if (Sku == null)
                {
                    throw new Exception("(PE) SKU " + SKUNO + " has not been setup in CloudMES.");
                }
                //if (!string.Equals(rowSku.VERSION.ToString(), row["REVLV"].ToString()))
                //{
                //    throw new Exception(" The sku version is not the same," + rowSku.VERSION.ToString() + "," + row["REVLV"].ToString());
                //}


                if (Sku != null && Sku.C_SERIES_ID != null && Sku.C_SERIES_ID.ToString() != "")
                {
                    C_Series = T_Series.GetDetailById(SFCDB, Sku.C_SERIES_ID);
                    if (C_Series == null)
                    {
                        throw new Exception(" (PE)The series of " + SKUNO + " has not been setup in CloudMES.");
                    }
                    series = C_Series.SERIES_NAME;
                }
                else
                {
                    series = "JUNIPER_DEFAULT";
                }
                //rowWOType = WOType.GetWOTypeByWO(SFCDB, row["AUART"].ToString());
                rowWOType = new R_WO_TYPE() { WORKORDER_TYPE = "REGULAR", CATEGORY = "MODEL", PRODUCT_TYPE = "GA" };
                if (rowWOType == null)
                {
                    throw new Exception("get wo type fail");
                }
                rowRoute = (Row_C_ROUTE)C_ROUTE.GetRouteBySkuno(Sku.ID, SFCDB, DB_TYPE_ENUM.Oracle);
                if (rowRoute == null)
                {
                    throw new Exception("(PE) The route of " + SKUNO + " has not been setup in CloudMES.");
                }
                routeDetailList = RouteDetail.GetByRouteIdOrderBySEQASC(rowRoute.ID, SFCDB);
                if (routeDetailList == null || routeDetailList.Count == 0)
                {
                    throw new Exception("(PE) Get route detail fail by " + rowRoute.ID);
                }
                keypartIDList = t_c_kp_list.GetListIDBySkuno(Sku.SKUNO, SFCDB);
                var ediorder = SFCDB.ORM.Queryable<O_ORDER_MAIN, I137_I>((m, i) => m.ITEMID == i.ID).Where((m, i) => m.PREWO == WO_NO).Select((m, i) => new { m, i }).ToList().FirstOrDefault();
                #region check JNP order
                if (ediorder == null)
                    throw new Exception("It's not Jnp Order,Pls check!");

                #region IP/CSIP/SIP 用戶要求開工單但是不能CONVER;
                if ("IP/CSIP/SIP".Split('/').Contains(ediorder.i.LINESCHEDULINGSTATUS))
                    throw new Exception($@" {ediorder.m.PREWO } LineSchedulingStatus is {ediorder.i.LINESCHEDULINGSTATUS}!");
                #endregion

                #region hold
                var holdobj = JuniperOmBase.JuniperHoldCheck(ediorder.m.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.CREATEWO, db);
                if (holdobj.HoldFlag)
                    throw new Exception($@" {ediorder.m.PREWO } Order is OnHold ,ReasonCode is {holdobj.HoldReason}!");
                #endregion

                if (SFCDB.ORM.Queryable<R_ORDER_WO>().Where(t => t.WO == WO_NO && t.VALID == MesBool.Yes.ExtValue()).Any())
                    throw new Exception($@"WO: {WO_NO} is wait Dismantle,pls check!");

                if (db.Queryable<I137_H, I137_I>((h, i) => h.TRANID == i.TRANID).Where((h, i) => i.ID == ediorder.m.ITEMID && h.PODOCTYPE.Equals(ENUM_I137_PoDocType.ZRMQ.ToString())).Select((h, i) => i).Any())
                    throw new Exception($@"WO: {WO_NO} is RMQ type,pls check!");
                #endregion

                #region JuniperBomExplosion 
                if (!SFCDB.ORM.Queryable<R_F_CONTROL>().Any(t => t.FUNCTIONNAME == "ConverWoByPass" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue() && t.VALUE == WO_NO))
                {
                    var ExplosionRes = JuniperBase.JuniperBomExplosionForConverWo(OOM, this.BU, SFCDB.ORM);
                    if (!ExplosionRes.issuccess)
                        throw new Exception(ExplosionRes.msg);
                }
                #endregion    

                #region Rel Wo with sap and Download from sap
                var relSapWores = JuniperBase.RelSapWo(this.BU, WO_NO);
                if (!relSapWores.issuccess)
                    throw new Exception(relSapWores.msg);
                DownloadSapWo(SFCDB, this.BU, WO_NO);
                #endregion

                if (BU == "VNJUNIPER")
                {
                    //VNJUNIPER Check download workorderno from sap into r_wo_header,The test environment does not check
                    var wo_header = SFCDB.ORM.Queryable<R_WO_HEADER>().Where(r => r.AUFNR == WO).ToList().FirstOrDefault();
                    if (wo_header == null)
                    {
                        throw new Exception($@"{WO}  hasn't been downloaded from SAP yet");
                    }
                    if (string.IsNullOrEmpty(wo_header.MATNR))
                    {
                        throw new Exception($@"{WO} group id in SAP is null!");
                    }
                    var wo_groupid = SFCDB.ORM.Queryable<R_WO_GROUPID>().Where(r => r.WO == WO).ToList().FirstOrDefault();
                    if (wo_groupid == null)
                    {
                        throw new Exception($@"{WO} group id in OM is null!");
                    }
                    if (wo_groupid.GROUPID != wo_header.MATNR)
                    {
                        throw new Exception($@"{WO} group id in OM not equals SAP!");
                    }
                    var bSku = SFCDB.ORM.Queryable<C_SKU>().Where(c => c.SKUNO == wo_groupid.SKUNO).Any();
                    if (!bSku)
                    {
                        throw new Exception($@"{wo_groupid.SKUNO} not setting in OM,Please call PE setting!");
                    }
                }

                if (keypartIDList.Count > 0 && keypartIDList.Count != 1)
                {
                    throw new Exception("Skuno:" + SKUNO + " have multiple keypart list.");
                }

                rowWOBase = (Row_R_WO_BASE)R_WO_BASE.NewRow();
                rowWOBase.ID = R_WO_BASE.GetNewID(BU, SFCDB);
                rowWOBase.WORKORDERNO = WO_NO;
                rowWOBase.PLANT = OOM.PLANT;
                rowWOBase.RELEASE_DATE = DateTime.Now;
                rowWOBase.DOWNLOAD_DATE = DateTime.Now;
                rowWOBase.PRODUCTION_TYPE = Sku.SKU_TYPE; //20190326 Patty modified.
                rowWOBase.WO_TYPE = rowWOType.WORKORDER_TYPE;
                rowWOBase.SKUNO = SKUNO;
                //rowWOBase.SKU_VER = Sku.VERSION;
                rowWOBase.SKU_VER = new Func<string>(() =>
                {
                    var pidver = SFCDB.ORM.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == WO_NO && t.PARENTPN == SKUNO).ToList().FirstOrDefault();
                    if (pidver != null)
                        return pidver.PNREV;
                    else
                        return Sku.VERSION;
                })();
                rowWOBase.SKU_SERIES = series;
                rowWOBase.SKU_NAME = Sku.SKU_NAME;
                rowWOBase.SKU_DESC = Sku.DESCRIPTION;
                rowWOBase.CUST_PN = Sku.CUST_PARTNO;
                rowWOBase.CUST_PN_VER = "";
                rowWOBase.CUSTOMER_NAME = Sku.CUST_SKU_CODE;
                rowWOBase.ROUTE_ID = rowRoute.ID;
                rowWOBase.START_STATION = routeDetailList[0].STATION_NAME;
                rowWOBase.KP_LIST_ID = (keypartIDList != null && keypartIDList.Count > 0) ? keypartIDList[0] : "";
                rowWOBase.CLOSED_FLAG = "0";
                rowWOBase.WORKORDER_QTY = Convert.ToDouble(OOM.QTY);
                rowWOBase.INPUT_QTY = 0;
                rowWOBase.FINISHED_QTY = 0;
                rowWOBase.SCRAPED_QTY = 0;
                rowWOBase.STOCK_LOCATION = "";
                rowWOBase.PO_NO = "";
                rowWOBase.CUST_ORDER_NO = "";
                rowWOBase.ROHS = "";
                rowWOBase.EDIT_EMP = "interface";
                rowWOBase.EDIT_TIME = DateTime.Now;
                SFCDB.ThrowSqlExeception = true;
                var sql = rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle);
                SFCDB.ExecSQL(rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle));

                var AutoKps = MakeAutoKPConfigNew(WO_NO, SFCDB, BU);
                //Coo Config Check
                if(BU == "FJZ")
                for (int i = 0; i < AutoKps.Count; i++)
                {
                    var Pn = AutoKps[i].CUST_PN;
                    var count = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "AutoKP" && t.CATEGORY == "COO"
                                  && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == Pn).Count();
                    if (count == 0)
                    {
                            throw new Exception($@"{Pn} no setting Coo!");
                    }
                }


                #region Juniper PoStatus   
                SFCDB.ORM.Insertable(new R_ORDER_WO()
                {
                    ID = MesDbBase.GetNewID<R_ORDER_WO>(SFCDB.ORM, Customer.JUNIPER.ExtValue()),
                    WO = WO_NO,
                    UPOID = ediorder.m.UPOID,
                    ORIGINALID = ediorder.m.ID,
                    DISMANTLE = MesBool.No.ExtValue(),
                    VALID = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now
                }).ExecuteCommand();
                SFCDB.ORM.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == ediorder.m.ID && t.STATUSID == ENUM_O_PO_STATUS.DownloadWo.ExtValue()).ExecuteCommand();
                SFCDB.ORM.Insertable(new O_PO_STATUS()
                {
                    ID = MesDbBase.GetNewID<O_PO_STATUS>(SFCDB.ORM, Customer.JUNIPER.ExtValue()),
                    STATUSID = ENUM_O_PO_STATUS.Production.ExtValue(),
                    VALIDFLAG = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    POID = ediorder.m.ID
                }).ExecuteCommand();
                #endregion


                SFCDB.CommitTrain();
                res.issuccess = true;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                res.issuccess = false;
                res.msg = ee.Message;
            }
            finally
            {
                #region unlock
                SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(t => new R_MES_LOG() { DATA2 = MesBool.No.ExtValue() })
                    .Where(t => t.FUNCTION_NAME == "ConverWoLock" && t.DATA1 == WO && t.DATA2 == MesBool.Yes.ExtValue()).ExecuteCommand();
                #endregion
            }
            return res;
        }
        public static MesFunctionRes DownloadSapWo(OleExec SFCDB, string BU, string wo)
        {
            try
            {
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                string msg = "";
                T_R_SN t_r_sn = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
                DataTable tableLoading = t_r_sn.GetSNByWo(wo, SFCDB);
                if (tableLoading.Rows.Count > 0)
                {
                    throw new Exception($@"{wo} Have Been Loading!");
                }
                ZRFC_SFC_NSG_0001HW zrfc_sfc_nsg_0001hw = new ZRFC_SFC_NSG_0001HW(BU);
                zrfc_sfc_nsg_0001hw.SetValue(wo, wo, "", "ALL", "", "", "", "");
                zrfc_sfc_nsg_0001hw.CallRFC();
                DataTable tableWOHeader = zrfc_sfc_nsg_0001hw.GetTableValue("WO_HEADER");
                DataTable tableWOItem = zrfc_sfc_nsg_0001hw.GetTableValue("WO_ITEM");
                DataTable tableWOText = zrfc_sfc_nsg_0001hw.GetTableValue("WO_TEXT");

                if (tableWOHeader.Rows[0]["MATNR"].ToString().Trim() == "")
                {
                    throw new Exception($@"Get Skuno[MATNR] From SAP Error!");
                }
                //SaveWOHeader
                string saveMsg = "";
                new DownLoad_WO().SaveWOHeaderJnp(tableWOHeader, tableWOItem, SFCDB, DB_TYPE_ENUM.Oracle, BU, wo, "", "SYSTEM", false, ref saveMsg);

                if (saveMsg != "")
                    throw new Exception(saveMsg);

                return new MesFunctionRes() { issuccess = true };
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
