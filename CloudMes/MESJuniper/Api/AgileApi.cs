using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using System.Data;
using System.Reflection;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module.Juniper;
using MESJuniper.Base;
using SqlSugar;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.MesBase;

namespace MESJuniper.Api
{
    public class AgileApi : MesAPIBase
    {
        protected APIInfo FAddNewAgileItemForSingle = new APIInfo
        {
            FunctionName = "AddNewAgileItemForSingle",
            Description = "AddNewAgileItemForSingle",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddNewAgileItemForMultiple = new APIInfo
        {
            FunctionName = "AddNewAgileItemForMultiple",
            Description = "AddNewAgileItemForMultiple",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetAgileItemFromOm = new APIInfo
        {
            FunctionName = "GetAgileItemFromOm",
            Description = "FGetAgileItemFromOm",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };


        protected APIInfo FGetEcnPageFromOm = new APIInfo
        {
            FunctionName = "GetEcnPageFromOm",
            Description = "GetEcnPageFromOm",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };


        protected APIInfo FAddEcnPageForMultiple = new APIInfo
        {
            FunctionName = "AddEcnPageForMultiple",
            Description = "AddEcnPageForMultiple",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
               
        protected APIInfo FGetAllRevAgileDataByFoxPn = new APIInfo
        {
            FunctionName = "GetAllRevAgileDataByFoxPn",
            Description = "GetAllRevAgileDataByFoxPn",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetEcnPage = new APIInfo
        {
            FunctionName = "GetEcnPage",
            Description = "GetEcnPage",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        public AgileApi()
        {
            this.Apis.Add(FAddNewAgileItemForSingle.FunctionName, FAddNewAgileItemForSingle);
            this.Apis.Add(FAddNewAgileItemForMultiple.FunctionName, FAddNewAgileItemForMultiple);
            this.Apis.Add(FGetAgileItemFromOm.FunctionName, FGetAgileItemFromOm);
            this.Apis.Add(FGetEcnPageFromOm.FunctionName, FGetEcnPageFromOm);
            this.Apis.Add(FAddEcnPageForMultiple.FunctionName, FAddEcnPageForMultiple);
            this.Apis.Add(FGetAllRevAgileDataByFoxPn.FunctionName, FGetAllRevAgileDataByFoxPn);
            this.Apis.Add(FGetEcnPage.FunctionName, FGetEcnPage);
        }


        public void GetEcnPageFromOm(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var CHANGENO = Data["CHANGENO"].ToString();
                var res = SFCDB.ORM.Queryable<R_JNP_ECNPAGE>().Where(t => t.CHANGENO == CHANGENO).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void AddEcnPageForMultiple(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var models = Data["JsonData"].ToObject<R_JNP_ECNPAGE[]>().ToList();
                var plant = Data["PLANT"].ToString();
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    var CS = models.Select(t=>t.CHANGENO).Distinct();
                    foreach (var cno in CS)
                    {
                        var csitems = models.FindAll(t => t.CHANGENO == cno).ToList();
                        var msgid = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.yyyyMMddHHmmssfff.ExtValue());
                        foreach (var item in csitems)
                        {
                            if (!item.CUSTREQUIRDATE.Equals(""))
                            {
                                try
                                {
                                    Convert.ToDateTime(item.CUSTREQUIRDATE);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception($@"CUSTREQUIRDATE must be in datetime format(yyyy-MM-dd HH:mm:ss)");
                                }
                            }

                            item.MSGID = msgid;
                            item.ID = MesDbBase.GetNewID<R_JNP_ECNPAGE>(SFCDB.ORM, this.BU);
                            item.CREATEDATE = DateTime.Now;
                            item.CREATEBY = string.Empty;
                            item.PLANT = plant;
                            item.ACTIVED = MesBool.No.ExtValue();
                            //SFCDB.ORM.Updateable<R_JNP_ECNPAGE>().SetColumns(t => new R_JNP_ECNPAGE() { ACTIVED = MesBool.No.ExtValue() })
                            //    .Where(t => t.CHANGENO == item.CHANGENO && t.ITEMNUMBER == item.ITEMNUMBER && t.PLANT == plant && t.ACTIVED == MesBool.Yes.ExtValue()).ExecuteCommand();
                            SFCDB.ORM.Insertable(item).ExecuteCommand();
                        }
                    }
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        //public void AddNewEcnPageForSingle(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        //{
        //    var SFCDB = this.DBPools["SFCDB"].Borrow();
        //    List<string> ret = new List<string>();
        //    try
        //    {
        //        var model = Data["JsonData"].ToObject<R_JNP_ECNPAGE>();
        //        var plant = Data["PLANT"].ToString();
        //        model.ID = MesDbBase.GetNewID<R_JNP_ECNPAGE>(SFCDB.ORM, this.BU);
        //        model.CREATEDATE = DateTime.Now;
        //        model.CREATEBY = string.Empty;
        //        model.ACTIVED = MesBool.Yes.ExtValue();
        //        model.PLANT = plant;
        //        var res = SFCDB.ORM.Ado.UseTran(() => {
        //            SFCDB.ORM.Updateable<R_JNP_ECNPAGE>().SetColumns(t => new R_JNP_ECNPAGE() { ACTIVED = MesBool.No.ExtValue() })
        //                .Where(t => t.CHANGENO == model.CHANGENO && t.ITEMNUMBER == model.ITEMNUMBER && t.ACTIVED == MesBool.Yes.ExtValue()).ExecuteCommand();
        //            SFCDB.ORM.Insertable(model).ExecuteCommand();
        //        });
        //        if (res.IsSuccess)
        //            StationReturn.Status = StationReturnStatusValue.Pass;
        //        else
        //            throw new Exception(res.ErrorMessage);
        //    }
        //    catch (Exception ee)
        //    {
        //        StationReturn.Status = StationReturnStatusValue.Fail;
        //        StationReturn.Message = ee.Message;
        //    }
        //    finally
        //    {
        //        StationReturn.Data = ret;
        //        this.DBPools["SFCDB"].Return(SFCDB);
        //    }
        //}

        public void AddNewAgileItemForSingle(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var model = Data["JsonData"].ToObject<O_AGILE_ATTR>();
                var plant = Data["PLANT"].ToString();
                model.ID = MesDbBase.GetNewID<O_AGILE_ATTR>(SFCDB.ORM,this.BU);
                model.DATE_CREATED = DateTime.Now;
                model.PLANT = plant;
                model.ACTIVED = MesBool.Yes.ExtValue();
                var res = SFCDB.ORM.Ado.UseTran(()=> {
                    SFCDB.ORM.Updateable<O_AGILE_ATTR>().SetColumns(t => new O_AGILE_ATTR() { ACTIVED = MesBool.No.ExtValue() })
                        .Where(t => t.ITEM_NUMBER == model.ITEM_NUMBER && t.PLANT == plant && t.ACTIVED == MesBool.Yes.ExtValue()).ExecuteCommand();
                    SFCDB.ORM.Insertable(model).ExecuteCommand();
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void AddNewAgileItemForMultiple(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var models = Data["JsonData"].ToObject<O_AGILE_ATTR[]>();
                var plant = Data["PLANT"].ToString();
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    foreach (var item in models)
                    {
                        item.ID = MesDbBase.GetNewID<O_AGILE_ATTR>(SFCDB.ORM, this.BU);
                        item.DATE_CREATED = DateTime.Now;
                        item.PLANT = plant;
                        item.ACTIVED = MesBool.Yes.ExtValue();
                        SFCDB.ORM.Updateable<O_AGILE_ATTR>().SetColumns(t => new O_AGILE_ATTR() { ACTIVED = MesBool.No.ExtValue() })
                            .Where(t => t.ITEM_NUMBER == item.ITEM_NUMBER && t.PLANT == plant && t.ACTIVED == MesBool.Yes.ExtValue()).ExecuteCommand();
                        SFCDB.ORM.Insertable(item).ExecuteCommand();
                    }
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
                //SFCDB.ORM.Insertable(models).ExecuteCommand();
                //StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetAgileItemFromOm(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ITEM_NUMBER = Data["ITEM_NUMBER"].ToString();
                var REV = Data["REV"].ToString();
                var PLANT = Data["PLANT"].ToString();
                var res = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == ITEM_NUMBER && t.REV == REV && t.PLANT == PLANT).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetAllAgileItemWithActiveFromOm(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                //var res = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ACTIVED == MesBool.Yes.ExtValue()).Select(t=>new { t.ID, t.ITEM_NUMBER, t.REV, t.CUSTPARTNO, t.DESCRIPTION, t.CHANGE_NUMBER, t.USER_ITEM_TYPE,t.OFFERING_TYPE, t.EFFECTIVE_DATE, t.RELEASE_DATE, t.DATE_CREATED,t.CLEI_CODE, t.CS_FLAG, t.HIDDEN_BOM }).ToList();
                //var res = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ACTIVED == MesBool.Yes.ExtValue()).ToList();
                var res = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ACTIVED == MesBool.Yes.ExtValue()).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void CreateNewAgileItemForSingle(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var model = Data.ToObject<O_AGILE_ATTR>();
                var defaultplantcodeobj = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "DefaultPlantCode" && t.CATEGORY == "PlantCode" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList().FirstOrDefault();
                if (defaultplantcodeobj == null)
                    throw new Exception("pls config default plantcode!");
                var plantCode = defaultplantcodeobj.VALUE.ToString();
                model.ID = MesDbBase.GetNewID<O_AGILE_ATTR>(SFCDB.ORM, this.BU);
                model.PLANT = plantCode;
                model.DATE_CREATED = DateTime.Now;
                model.ACTIVED = MesBool.Yes.ExtValue();
                var res = SFCDB.ORM.Ado.UseTran(() => {
                    SFCDB.ORM.Updateable<O_AGILE_ATTR>().SetColumns(t => new O_AGILE_ATTR() { ACTIVED = MesBool.No.ExtValue() })
                        .Where(t => t.ITEM_NUMBER == model.ITEM_NUMBER && t.PLANT == plantCode && t.ACTIVED == MesBool.Yes.ExtValue()).ExecuteCommand();
                    SFCDB.ORM.Insertable(model).ExecuteCommand();
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetAllRevAgileDataByFoxPn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ITEM_NUMBER = Data["ITEM_NUMBER"].ToString();
                var res = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == ITEM_NUMBER ).OrderBy(t=>t.DATE_CREATED,OrderByType.Asc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetEcnPage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = SFCDB.ORM.SqlQueryable<R_JNP_ECNPAGE>($@"SELECT * FROM (
                                                    select A.*,RANK() OVER (PARTITION BY ITEMNUMBER ORDER BY REV DESC,CREATEDATE DESC) ROW_NUMBER from r_jnp_ecnpage A ) WHERE ROW_NUMBER=1").ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetEcnPageByPn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ITEMNUMBER = Data["ITEMNUMBER"].ToString();
                var res = SFCDB.ORM.Queryable<R_JNP_ECNPAGE>().Where(t=>t.ITEMNUMBER == ITEMNUMBER).OrderBy(t=>t.REV,OrderByType.Desc).OrderBy(t=>t.CREATEDATE,OrderByType.Desc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetLockDataWithEcnPage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var PN = Data["PN"].ToString();
                var revs = new Func<string>(()=> {
                    var locks = SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.LOCK_EMP == "EcnLock" && t.TYPE == "SKU" && t.WORKORDERNO == PN).Select(t=>t.SN).Distinct().ToList();
                    var ress = "''";
                    foreach (var rev in locks)
                        ress += $@",'{rev}'";                    
                    return ress;
                })();

                var res = SFCDB.ORM.Ado.GetDataTable($@"
                             select sn,a.workorderno,a.skuno,b.sku_ver,next_station,'' SSN,'' SSN_WO,'' SSN_SKU,'' SSN_NEXTSTATION from r_sn a,r_wo_base b
                              where a.skuno='{PN}' and a.workorderno=b.workorderno  and b.sku_ver IN ({revs}) and completed_flag='1' AND A.shipped_flag='0'  AND VALID_FLAG='1'
                              UNION ALL
                             select sn,a.workorderno,a.skuno,b.sku_ver,next_station,'' SSN,'' SSN_WO,'' SSN_SKU,'' SSN_NEXTSTATION from r_sn a,r_wo_base b
                              where a.skuno='{PN}' and a.workorderno=b.workorderno  and b.sku_ver IN ({revs}) and completed_flag='0'  AND VALID_FLAG='1'
                              UNION ALL
                              SELECT * FROM (
                              select distinct BBB.SN,BBB.WORKORDERNO,BBB.SKUNO,CCC.SKU_VER,BBB.NEXT_STATION,AAA.SN SSN,
                              AAA.workorderno SSN_WO ,AAA.SKUNO SSN_SKU,AAA.next_station SSN_NEXTSTATION
                                from (
                              select aa.sn,bb.skuno,bb.workorderno,bb.next_station,aa.value from r_sn_kp aa, r_sn bb where value in (
                             select sn from r_sn a,r_wo_base b
                              where a.skuno='{PN}' and a.workorderno=b.workorderno and b.sku_ver IN ({revs}) ) and aa.sn=bb.sn and bb.skuno <>'{PN}' 
                              and bb.shipped_flag='0' and bb.valid_flag='1'
                              union all
                              select distinct dd.sn,dd.skuno,dd.workorderno,dd.next_station,aa.value from r_sn_kp aa, r_sn bb,r_sn_kp cc,r_sn dd where aa.value in (
                             select sn from r_sn a,r_wo_base b
                              where a.skuno='{PN}' and a.workorderno=b.workorderno and b.sku_ver IN  ({revs}) ) and aa.sn=bb.sn and bb.skuno <>'{PN}' 
                             and aa.sn=cc.value and cc.partno<>dd.skuno and cc.sn=dd.sn and dd.valid_flag='1'   and dd.shipped_flag='0'
                               union all
                              select distinct FF.sn,FF.skuno,FF.workorderno,FF.next_station,aa.value from r_sn_kp aa, r_sn bb,r_sn_kp cc,r_sn dd , R_SN_KP EE,R_SN FF where aa.value in (
                             select sn from r_sn a,r_wo_base b
                              where a.skuno='{PN}' and a.workorderno=b.workorderno and b.sku_ver IN  ({revs}) ) and aa.sn=bb.sn and bb.skuno <>'{PN}' 
                             and aa.sn=cc.value and cc.partno<>dd.skuno and cc.sn=dd.sn and  DD.SN=EE.VALUE AND EE.SN=FF.SN AND EE.PARTNO<>FF.SKUNO AND FF.VALID_FLAG='1'   and FF.shipped_flag='0'
                             )  AAA ,R_SN BBB,R_Wo_BASE CCC WHERE AAA.VALUE=BBB.SN AND BBB.SKUNO='{PN}' AND BBB.WORKORDERNO=CCC.WORKORDERNO  AND BBB.VALID_FLAG='1' AND CCC.sku_ver IN  ({revs}) 
                             ) EEE WHERE EEE.WORKORDERNO<>EEE.SSN_WO");
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
