using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MESStation.Config
{
    public class MaterialStageConfig : MesAPIBase
    {
        protected APIInfo FGetMaterialStageConfirmList = new APIInfo
        {
            FunctionName = "GetMaterialStageConfirmList",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FInitMaterialStageHead = new APIInfo
        {
            FunctionName = "InitMaterialStageHead",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateMaterialStageHead = new APIInfo
        {
            FunctionName = "UpdateMaterialStageHead",
            Description = "updateMaterialStageHead",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetMaterialStageHeadList = new APIInfo
        {
            FunctionName = "GetMaterialStageHeadList",
            Description = "Get MaterialStageHead List",
            Parameters = new List<APIInputInfo>() {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddMaterialStageConfirm = new APIInfo
        {
            FunctionName = "AddMaterialStageConfirm",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddMaterialStageDetail = new APIInfo
        {
            FunctionName = "AddMaterialStageDetail",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEditMaterialStageConfirm = new APIInfo
        {
            FunctionName = "EditMaterialStageConfirm",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetMaterialStageDetailList = new APIInfo
        {
            FunctionName = "GetMaterialStageDetailList",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetMaterialStageMrb = new APIInfo
        {
            FunctionName = "GetMaterialStageMrb",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetMaterialStageSkunoDetail = new APIInfo
        {
            FunctionName = "GetMaterialStageSkunoDetail",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        
        protected APIInfo FGetMaterialStageMrbSnDetail = new APIInfo
        {
            FunctionName = "GetMaterialStageMrbSnDetail",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetStationMaterialStageDetail = new APIInfo
        {
            FunctionName = "GetStationMaterialStageDetail",
            Description = "Init",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteMaterialConfirmById = new APIInfo
        {
            FunctionName = "DeleteMaterialConfirmById",
            Description = "刪除",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName = "ConfirmID" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public MaterialStageConfig()
        {
            this.Apis.Add(FUpdateMaterialStageHead.FunctionName, FUpdateMaterialStageHead);
            this.Apis.Add(FGetMaterialStageHeadList.FunctionName, FGetMaterialStageHeadList);
            this.Apis.Add(FInitMaterialStageHead.FunctionName, FInitMaterialStageHead);
            this.Apis.Add(FGetMaterialStageConfirmList.FunctionName, FGetMaterialStageConfirmList);
            this.Apis.Add(FAddMaterialStageConfirm.FunctionName, FAddMaterialStageConfirm);
            this.Apis.Add(FAddMaterialStageDetail.FunctionName, FAddMaterialStageDetail);
            this.Apis.Add(FEditMaterialStageConfirm.FunctionName, FEditMaterialStageConfirm);
            this.Apis.Add(FGetMaterialStageDetailList.FunctionName, FGetMaterialStageDetailList);
            this.Apis.Add(FGetMaterialStageMrb.FunctionName, FGetMaterialStageMrb);
            this.Apis.Add(FGetMaterialStageSkunoDetail.FunctionName, FGetMaterialStageSkunoDetail);
            this.Apis.Add(FGetMaterialStageMrbSnDetail.FunctionName, FGetMaterialStageMrbSnDetail);
            this.Apis.Add(FGetStationMaterialStageDetail.FunctionName, FGetStationMaterialStageDetail);
        }

        public void UpdateMaterialStageHead(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ID = Data["ID"].ToString().Trim().ToUpper();
                string ISMATERIAL = Data["ISMATERIAL"].ToString().Trim().ToUpper();
                var res = sfcdb.ORM.Updateable<R_MATERIAL_STAGE_HEAD>().UpdateColumns(t => new R_MATERIAL_STAGE_HEAD()
                {
                    ISMATERIAL = ISMATERIAL,
                    EDITTIME = DateTime.Now,
                    EDITBY = LoginUser.EMP_NO
                }).Where(t => t.ID == ID).ExecuteCommand(); 

                if (res > 0)
                {
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.MessagePara = new List<object>(){""};
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        /// <summary>
        /// 若機種還未有數據則自動新增,返回機種數據列表
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void InitSkuMaterialStageHead(Newtonsoft.Json.Linq.JObject requestValue,Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string Sku = Data["SKUNO"].ToString().Trim().ToUpper();
                var list = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_HEAD>().Where(t => t.SKUNO == Sku)
                    .OrderBy(t => SqlFunc.ToInt32(t.GROUPNAME), OrderByType.Asc).ToList();
                var skuobj = sfcdb.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == Sku).ToList().FirstOrDefault();
                if (skuobj == null)
                {
                    StationReturn.MessageCode = "MES00000172";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }

                var routelist = sfcdb.ORM.Queryable<C_ROUTE_DETAIL, C_SKU, R_SKU_ROUTE>((crd, cs, rsr) =>
                        crd.ROUTE_ID == rsr.ROUTE_ID && cs.ID == rsr.SKU_ID && cs.SKUNO == Sku)
                    .OrderBy((crd, cs, rsr) => crd.SEQ_NO, SqlSugar.OrderByType.Asc)
                    .GroupBy((crd, cs, rsr) => new {crd.SEQ_NO, crd.STATION_NAME})
                    .Select((crd, cs, rsr) => new {crd.SEQ_NO, crd.STATION_NAME}).ToList();
                sfcdb.ORM.Ado.BeginTran();
                if (list.Count > 0)
                    sfcdb.ORM.Deleteable(list).ExecuteCommand();
                var newlist = new List<R_MATERIAL_STAGE_HEAD>();
                for (int i = 0; i < routelist.Count; i++)
                {
                    if (newlist.FindAll(t => t.EVENTNAME == routelist[i].STATION_NAME).Count > 0)
                        continue;
                    var addobj = new R_MATERIAL_STAGE_HEAD()
                    {
                        ID = MesDbBase.GetNewID(sfcdb.ORM, skuobj.BU, "R_MATERIAL_STAGE_HEAD"),
                        SKUNO = skuobj.SKUNO,
                        GROUPNAME = (i + 1).ToString(),
                        EVENTNAME = routelist[i].STATION_NAME,
                        ISMATERIAL =
                            list.FindAll(t => t.EVENTNAME == routelist[i].STATION_NAME && t.ISMATERIAL == "Y").Count > 0
                                ? "Y"
                                : "N",
                        SEQNO = routelist[i].SEQ_NO.ToString(),
                        EDITBY = LoginUser.EMP_NO,
                        EDITTIME = GetDBDateTime()
                    };
                    newlist.Add(addobj);
                }

                sfcdb.ORM.Insertable(newlist).ExecuteCommand();
                sfcdb.ORM.Ado.CommitTran();

                if (newlist.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = newlist;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                sfcdb.ORM.Ado.RollbackTran();
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetMaterialStageHeadList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb =this.DBPools["SFCDB"].Borrow();
                string Sku = Data["SKUNO"].ToString().Trim().ToUpper();
                var list = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_HEAD>().OrderBy(t=>t.SKUNO,OrderByType.Asc).OrderBy(t=> SqlFunc.ToInt32(t.GROUPNAME), OrderByType.Asc).ToList();
                if (!Sku.Trim().Equals(""))
                    list = list.FindAll(t => t.SKUNO == Sku);
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetMaterialStageConfirmList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string Sku = Data["SKUNO"].ToString().Trim().ToUpper();
                var list = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_CONFIRM>()
                    .OrderBy(t => t.SKUNO, OrderByType.Asc).OrderBy(t => t.VERSION, OrderByType.Asc)
                    .OrderBy(t => t.STATION, OrderByType.Asc).ToList();
                if (!Sku.Trim().Equals(""))
                    list = list.FindAll(t => t.SKUNO == Sku);
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void AddMaterialStageConfirm(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string SKUNO = Data["SKUNO"].ToString().Trim().ToUpper();
                string VERSION = Data["VERSION"].ToString().Trim().ToUpper();
                string STATION = Data["STATION"].ToString().Trim().ToUpper();
                string FILENAME = Data["FILENAME"].ToString().Trim().ToUpper();
                string DESCRIPTION = Data["DESCRIPTION"].ToString().Trim().ToUpper();
                //string REASON = Data["REASON"].ToString().Trim().ToUpper();
                string FLAG = Data["FLAG"].ToString().Trim().ToUpper();
                if(sfcdb.ORM.Queryable<R_MATERIAL_STAGE_CONFIRM>().Where(t=>t.SKUNO== SKUNO && t.VERSION== VERSION&& t.STATION == STATION).Any())
                {
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else
                {
                    var res = sfcdb.ORM.Insertable(new R_MATERIAL_STAGE_CONFIRM()
                    {
                        ID = MesDbBase.GetNewID(sfcdb.ORM, LoginUser.BU, "R_MATERIAL_STAGE_CONFIRM"),
                        SKUNO= SKUNO,
                        VERSION = VERSION,
                        STATION = STATION,
                        FILENAME = FILENAME,
                        DESCRIPTION = DESCRIPTION,
                        //REASON = REASON,
                        FLAG = FLAG,
                        UPLOADBY = LoginUser.EMP_NO,
                        UPLOADTIME = DateTime.Now,
                        EDITBY = LoginUser.EMP_NO,
                        EDITTIME = DateTime.Now
                    }).ExecuteCommand();

                    if (res > 0)
                    {
                        StationReturn.MessageCode = "MES00000002";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara = new List<object>() { "" };
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void EditMaterialStageConfirm(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string TYPE = Data["TYPE"].ToString().Trim().ToUpper();
                string ID = Data["ID"].ToString().Trim().ToUpper();
                var obj = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_CONFIRM>().Where(t => t.ID == ID).ToList()
                    .FirstOrDefault();

                if (obj==null)
                {
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else
                {

                    if (TYPE.Equals("PE"))
                    {
                        obj.DESCRIPTION = Data["DESCRIPTION"].ToString().Trim().ToUpper();
                        obj.FLAG = "0";
                        obj.EDITBY = LoginUser.EMP_NO;
                        obj.EDITTIME = DateTime.Now;
                    }
                    else if(TYPE.Equals("QE"))
                    {
                        sfcdb.ORM.Updateable<R_MATERIAL_STAGE_DETAIL>().UpdateColumns(t => new R_MATERIAL_STAGE_DETAIL()
                        {
                            VAILD="1"
                        }).Where(t => t.SKUNO == obj.SKUNO&& t.VERSION== obj.VERSION && t.EVENTPOINT== obj.STATION).ExecuteCommand();
                        obj.REASON = Data["REASON"].ToString().Trim().ToUpper();
                        obj.FLAG = Data["FLAG"].ToString().Trim().ToUpper();
                        obj.SIGNBY = LoginUser.EMP_NO;
                        obj.SIGNTIME = DateTime.Now;
                    }
                    var res = sfcdb.ORM.Updateable(obj).ExecuteCommand();
                    if (res > 0)
                    {
                        StationReturn.MessageCode = "MES00000035";
                        StationReturn.MessagePara = new List<object>(){ "1" };
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000025";
                        StationReturn.MessagePara = new List<object>() { "" };
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }
        
        public void AddMaterialStageDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string skuno = Data["fileSkuno"].ToString().Trim().ToUpper();
                string desc = Data["fileDes"].ToString().Trim().ToUpper();
                string station = Data["fileStation"].ToString().Trim().ToUpper();
                string version = Data["fileversion"].ToString().Trim().ToUpper();
                if(!sfcdb.ORM.Queryable<R_MATERIAL_STAGE_HEAD>().Any(t=>t.SKUNO== skuno&& t.EVENTNAME==station && t.ISMATERIAL=="Y"))
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200323030907", new string[] { station }));
                if (!sfcdb.ORM.Queryable<R_Station>().Any(t=>t.STATION_NAME== station))
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200318042004", new string[]{ station }));
                List<R_MATERIAL_STAGE_DETAIL> rmsdList = new List<R_MATERIAL_STAGE_DETAIL>();
                //已签核完毕不准编辑;
                if (sfcdb.ORM.Queryable<R_MATERIAL_STAGE_CONFIRM>().Any(t => t.SKUNO == skuno && t.STATION == station && t.VERSION == version && t.FLAG == "1"))
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200320212916", new string[] {}));
                foreach (var item in Data["filePartNoList"])
                    if (item.Count() < 7)
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200318045439"));
                    else
                        rmsdList.Add(new R_MATERIAL_STAGE_DETAIL()
                        {
                            ID = MesDbBase.GetNewID(sfcdb.ORM, LoginUser.BU, "R_MATERIAL_STAGE_DETAIL"),
                            SKUNO = skuno.Trim(),
                            VERSION = version.Trim(),
                            EVENTPOINT = station.Trim(),
                            CUSTPARTNO = item[0].ToString().Trim(),
                            CUSTPNDESC = item[1].ToString().Trim(),
                            QTY = double.Parse(item[2].ToString().Trim()),
                            SOPPAGE = item[3].ToString().Trim(),
                            SOPEVENTPN = item[4].ToString().Trim(),
                            NOTE = item[5].ToString().Trim(),
                            TOTALQTY = item[6].ToString().Trim(),
                            EDITBY = LoginUser.EMP_NO,
                            EDITTIME = DateTime.Now,
                            VAILD = "0"
                        });
                sfcdb.ORM.Deleteable<R_MATERIAL_STAGE_DETAIL>().Where(t=>t.SKUNO== skuno&&t.VERSION == version&& t.VAILD !="1"&& t.EVENTPOINT==station).ExecuteCommand();
                sfcdb.ORM.Insertable(rmsdList).ExecuteCommand();

                StationReturn.Message = "OK!";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void GetMaterialStageDetailList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string sku = Data["SKUNO"].ToString().Trim().ToUpper();
                string version = Data["VERSION"].ToString().Trim().ToUpper();
                string station = Data["STATION"].ToString().Trim().ToUpper();
                var list = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_DETAIL>().Where(t =>
                        t.SKUNO == sku && t.VERSION == version && t.EVENTPOINT == station)
                    .OrderBy(t => t.CUSTPARTNO, OrderByType.Asc).ToList();
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetMaterialStageMrb(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                DateTime stime = Convert.ToDateTime(Data["stime"].ToString().Trim().ToUpper());
                DateTime etime = Convert.ToDateTime(Data["etime"].ToString().Trim().ToUpper()+" 23:59:59");
                var list = sfcdb.ORM.Queryable<R_MRB, R_WO_BASE>((rm, rwb) => rm.WORKORDERNO == rwb.WORKORDERNO)
                    //.Where((rm, rwb) => SqlFunc.Between(rm.CREATE_TIME, stime, etime) && rm.NEXT_STATION != "SHIPOUT" && rm.NEXT_STATION != "SHIPFINISH")
                    //add condition:rework_wo is null  asked by PC lushubing 2021-12-31
                    .Where((rm, rwb) => SqlFunc.Between(rm.CREATE_TIME, stime, etime) && rm.NEXT_STATION != "SHIPOUT" && rm.NEXT_STATION != "SHIPFINISH" && SqlFunc.IsNullOrEmpty(rm.REWORK_WO))
                    .GroupBy((rm, rwb) => new {rm.SKUNO, rwb.SKU_VER}).Select((rm, rwb) =>
                        new {skuno = rm.SKUNO, version = rwb.SKU_VER, mrbnum = SqlFunc.AggregateCount(1)})
                    .OrderBy("skuno asc").ToList();
                    
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetMaterialStageSkunoDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var sku = Data["skuno"].ToString().Trim().ToUpper();
                var version = Data["version"].ToString().Trim().ToUpper();
                DateTime stime = Convert.ToDateTime(Data["stime"].ToString().Trim().ToUpper());
                DateTime etime = Convert.ToDateTime(Data["etime"].ToString().Trim().ToUpper() + " 23:59:59");
                var head = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_HEAD>().Where(t => t.SKUNO == sku)
                    .OrderBy(t => SqlFunc.ToInt32(t.GROUPNAME), OrderByType.Asc).ToList();
                var mrb = sfcdb.ORM.Queryable<R_MRB, R_WO_BASE>((rm, rwb) => rm.WORKORDERNO == rwb.WORKORDERNO).Where(
                        (rm, rwb) => rm.SKUNO == sku && rwb.SKU_VER == version && SqlFunc.Between(rm.CREATE_TIME, stime, etime) && rm.NEXT_STATION != "SHIPOUT" && rm.NEXT_STATION != "SHIPFINISH" && SqlFunc.IsNullOrEmpty(rm.REWORK_WO))
                    .GroupBy((rm, rwb) => new {rm.TO_STORAGE, rm.NEXT_STATION})
                    .Select((rm, rwb) => new
                    {
                        TO_STORAGE = rm.TO_STORAGE,
                        NEXT_STATION = rm.NEXT_STATION,
                        STAGENUM = SqlFunc.AggregateCount(1)
                    }).ToList();
                var res = new List<object>();
                var storageArray = new string[] { "06RM", "B79M", "B69M" };//增加數組通用性
                foreach (var item in head)
                {
                    //var itemres = mrb.FindAll(t => t.NEXT_STATION == item.EVENTNAME && t.TO_STORAGE == "06RM").FirstOrDefault();
                    var itemres = mrb.FindAll(t => t.NEXT_STATION == item.EVENTNAME && storageArray.Contains(t.TO_STORAGE)).FirstOrDefault();
                    res.Add(new
                    {
                        SKUNO= item.SKUNO,
                        VERSION = version,
                        GROUPNAME = item.GROUPNAME,
                        EVENTNAME = item.EVENTNAME,
                        ISMATERIAL = item.ISMATERIAL,
                        //STORAGE = itemres!=null? itemres.TO_STORAGE: "06RM",
                        STORAGE = itemres != null ? itemres.TO_STORAGE : storageArray[Array.IndexOf(storageArray, mrb[0].TO_STORAGE)],
                        MRBNUM = itemres != null ? itemres.STAGENUM : 0
                    });
                }

                if (res.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = res;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetMaterialStageMrbSnDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var skuno = Data["skuno"].ToString().Trim().ToUpper();
                var version = Data["version"].ToString().Trim().ToUpper();
                var eventname = Data["eventname"].ToString().Trim().ToUpper();
                DateTime stime = Convert.ToDateTime(Data["stime"].ToString().Trim().ToUpper());
                DateTime etime = Convert.ToDateTime(Data["etime"].ToString().Trim().ToUpper() + " 23:59:59");

                var mrbsnList = sfcdb.ORM.Queryable<R_MRB, R_WO_BASE>((rm, rwb) => rm.WORKORDERNO == rwb.WORKORDERNO)
                    .Where((rm, rwb) => rm.SKUNO == skuno && rwb.SKU_VER == version && rm.NEXT_STATION == eventname &&
                                        SqlFunc.Between(rm.CREATE_TIME, stime, etime)
                                        && rm.NEXT_STATION != "SHIPOUT" && rm.NEXT_STATION != "SHIPFINISH" && SqlFunc.IsNullOrEmpty(rm.REWORK_WO))
                    .OrderBy((rm, rwb) => rm.CREATE_TIME, OrderByType.Asc).Select((rm, rwb) => new
                    {
                        SN = rm.SN,
                        SKUNO = rm.SKUNO,
                        rwb.SKU_VER,
                        NEXT_STATION = rm.NEXT_STATION,
                        FROM_STORAGE = rm.FROM_STORAGE,
                        TO_STORAGE = rm.TO_STORAGE,
                        REWORK_WO = rm.REWORK_WO,
                        CREATE_TIME = rm.CREATE_TIME,
                        CREATE_EMP = rm.CREATE_EMP,
                        SAP_FLAG = rm.SAP_FLAG
                    }).ToList();

                if (mrbsnList.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = mrbsnList;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetStationMaterialStageDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var skuno = Data["skuno"].ToString().Trim().ToUpper();
                var version = Data["version"].ToString().Trim().ToUpper();
                var groupname = Data["groupname"].ToString().Trim().ToUpper();
                var calc = 0; 
                try{calc = Convert.ToInt32(Data["calcqty"].ToString().Trim().ToUpper());}catch{calc = 0;}
                var materialdetail = sfcdb.ORM
                    .Queryable<R_MATERIAL_STAGE_DETAIL, R_MATERIAL_STAGE_CONFIRM, R_MATERIAL_STAGE_HEAD>(
                        (rmsd, rmsc, rmsh) => rmsd.EVENTPOINT == rmsc.STATION
                                              && rmsd.SKUNO == rmsc.SKUNO && rmsd.VERSION == rmsc.VERSION &&
                                              rmsc.SKUNO == rmsh.SKUNO && rmsd.EVENTPOINT == rmsh.EVENTNAME).Where(
                        (rmsd, rmsc, rmsh) => rmsd.SKUNO == skuno //&& rmsd.VERSION == version /*PE上傳的料表裡面的版本是SOP版本,這裡把版本條件屏蔽*/
                                                        && rmsd.VAILD == "1" &&
                                                        SqlFunc.ToInt32(rmsh.GROUPNAME) >=
                                                        SqlFunc.ToInt32(groupname) && rmsh.ISMATERIAL == "Y")
                    .Select((rmsd, rmsc, rmsh) => new
                    {
                        SKUNO=rmsd.SKUNO,
                        VERSION=rmsd.VERSION,
                        EVENTPOINT=rmsd.EVENTPOINT,
                        CUSTPARTNO=rmsd.CUSTPARTNO,
                        CUSTPNDESC=rmsd.CUSTPNDESC,
                        NOTE=rmsd.NOTE,
                        QTY=rmsd.QTY,
                        EDITTIME=rmsd.EDITTIME,
                        EDITBY=rmsd.EDITBY,
                        MRBQTY = calc,
                        TOTALQTY = SqlFunc.ToDouble(rmsd.QTY) * calc
                    }).ToList();
                if (materialdetail.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = materialdetail;
                }
                else
                {
                    //增加判斷是否不需帶料還是真的沒料表
                    var isMaterial = sfcdb.ORM.Queryable<R_MATERIAL_STAGE_HEAD>().Where(t => t.SKUNO == skuno && t.GROUPNAME == groupname && t.ISMATERIAL == "N").First();
                    if (isMaterial != null)
                    {
                        StationReturn.Message = $@"SKUNO：{skuno} 在 {isMaterial.EVENTNAME} 無需帶料！";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MSGCODE20200323062927";
                    }                    
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DeleteMaterialConfirmById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ConfirmID = Data["ConfirmID"].ToString().Trim().ToUpper();
                var CONFRIM_OBJ = SFCDB.ORM.Queryable<R_MATERIAL_STAGE_CONFIRM>().Where(t => t.ID == ConfirmID).ToList().FirstOrDefault();
                if (CONFRIM_OBJ == null)
                {
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else
                {
                    var userFunction = SFCDB.ORM.Queryable<C_USER_FUNCTION>().Where(t => t.USERID == LoginUser.ID && t.FUNCTIONNAME == "DELETE_MATERIALSTAGE").ToList().FirstOrDefault();
                    if (userFunction == null)
                    {
                        StationReturn.Message = $@"用戶：{LoginUser.EMP_NO}無刪除PE料表權限！";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                    else
                    {
                        var res = 0;
                        //刪除表：R_MATERIAL_STAGE_CONFIRM
                        var res1 = SFCDB.ORM.Deleteable<R_MATERIAL_STAGE_CONFIRM>().Where(t => t.ID == CONFRIM_OBJ.ID).ExecuteCommand();
                        //刪除表：R_MATERIAL_STAGE_DETAIL
                        var res2 = SFCDB.ORM.Deleteable<R_MATERIAL_STAGE_DETAIL>().Where(t => t.SKUNO == CONFRIM_OBJ.SKUNO && t.VERSION == CONFRIM_OBJ.VERSION && t.EVENTPOINT == CONFRIM_OBJ.STATION).ExecuteCommand();
                        //刪除表：R_FILE
                        var res3 = SFCDB.ORM.Deleteable<R_FILE>().Where(t => t.USETYPE == "Material" && t.NAME.ToUpper() == CONFRIM_OBJ.FILENAME).ExecuteCommand();
                        //寫LOG
                        MESPubLab.WriteLog.WriteIntoMESLog(SFCDB, BU, "CloudMES", "MESStation.Config.MaterialStageConfig", "DeleteMaterialConfirmById", CONFRIM_OBJ.SKUNO, "", LoginUser.EMP_NO, CONFRIM_OBJ.VERSION, CONFRIM_OBJ.STATION);

                        res = res1 + res2 + res3;
                        if (res > 0)
                        {
                            StationReturn.MessageCode = "MES00000002";
                            StationReturn.Status = StationReturnStatusValue.Pass;
                        }
                        else
                        {
                            StationReturn.MessageCode = "MES00000021";
                            StationReturn.MessagePara = new List<object>() { "" };
                            StationReturn.Status = StationReturnStatusValue.Fail;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
