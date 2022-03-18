using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;

namespace MESStation.Config
{
    class SkuRouteMappingConfig : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        private APIInfo _AddSkuRouteMapping = new APIInfo()
        {
            FunctionName = "AddSKuRouteMapping",
            Description = "添加機種路由映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="MappingObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            {

            }

        };
        private APIInfo _AddFaiToSku = new APIInfo()
        {
            FunctionName = "AddFaiToSku",
            Description = "配置Fai幾種映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="MappingObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            {

            }

        };

        private APIInfo _UpdateSkuRouteMapping = new APIInfo()
        {
            FunctionName = "UpdateSkuRouteMapping",
            Description = "更新機種路由映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="MappingObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSkuRouteMapping = new APIInfo()
        {
            FunctionName = "DeleteSkuRouteMapping",
            Description = "刪除機種路由映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="MappingID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSkuRouteMappingByObject = new APIInfo()
        {
            FunctionName = "DeleteSkuRouteMappingByObject",
            Description = "根據映射對象刪除映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="MappingObject",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetRouteMappingsBySkuName = new APIInfo()
        {
            FunctionName = "GetMappingsBySkuOrRoute",
            Description = "根據機種ID獲取所有的路由",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuOrRoute",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetAllMappings = new APIInfo()
        {
            FunctionName = "GetAllMappings",
            Description = "獲取所有的路由機種映射",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetRouteBySkuId = new APIInfo()
        {
            FunctionName = "GetRoutesBySkuId",
            Description = "根據機種獲取到所有對應路由",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuId",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            {

            }
        };
        protected APIInfo FGetAvailableRoutesForSku = new APIInfo()
        {
            FunctionName = "GetAvailableRoutesForSku",
            Description = "獲取機種可用的路由，路由的默認機種為空或者默認機種是當前機種",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "SkuId" }
            },
            Permissions = new List<MESPermission>()
        };

        public SkuRouteMappingConfig()
        {
            this.Apis.Add(_AddSkuRouteMapping.FunctionName, _AddSkuRouteMapping);
            this.Apis.Add(_AddFaiToSku.FunctionName, _AddFaiToSku);
            this.Apis.Add(_DeleteSkuRouteMapping.FunctionName, _DeleteSkuRouteMapping);
            this.Apis.Add(_DeleteSkuRouteMappingByObject.FunctionName, _DeleteSkuRouteMappingByObject);
            this.Apis.Add(_UpdateSkuRouteMapping.FunctionName, _UpdateSkuRouteMapping);
            this.Apis.Add(_GetRouteMappingsBySkuName.FunctionName, _GetRouteMappingsBySkuName);
            this.Apis.Add(_GetAllMappings.FunctionName, _GetAllMappings);
            this.Apis.Add(_GetRouteBySkuId.FunctionName, _GetRouteBySkuId);
            this.Apis.Add(FGetAvailableRoutesForSku.FunctionName, FGetAvailableRoutesForSku);
        }

        public void AddSKuRouteMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            R_SKU_ROUTE mapping = null;
            OleExec sfcdb = null;
            string result = string.Empty;
            string MappingObject = string.Empty;
            //MappingObject = @"{
            //    'DEFAULT_FLAG': 'Y',
            //    'SKU': {
            //        'ID': '111',
            //        'SKUNO': '222',
            //        'VERSION':'333'
            //    },
            //    'ROUTE': {
            //        'ID':'HWD00000000000000000000000000001X',
            //        'ROUTE_NAME':'555'
            //    }
            //}";

            try
            {
                if (Data["MappingObject"] != null && !string.IsNullOrEmpty(Data["MappingObject"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    MappingObject = Data["MappingObject"].ToString();
                    mapping = (R_SKU_ROUTE)JsonConvert.Deserialize(MappingObject, typeof(R_SKU_ROUTE));
                    if (mapping.ROUTE_ID != null)
                    {
                        T_C_ROUTE ROUTE = new T_C_ROUTE(sfcdb, DBTYPE);
                        C_ROUTE C_ROUTE = ROUTE.GetByRouteName(mapping.ROUTE_ID, sfcdb);
                        mapping.ROUTE_ID = C_ROUTE.ID;
                        mapping.EDIT_EMP = LoginUser.EMP_NO;
                        mapping.EDIT_TIME = GetDBDateTime();
                        result = table.AddMapping(mapping, BU, sfcdb);
                        if (Int32.Parse(result) > 0)
                        {
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.MessageCode = "MES00000035";
                            StationReturn.MessagePara.Add(result);
                            StationReturn.Data = result;
                            //throw
                        }
                        else
                        {
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.MessageCode = "MES00000036";
                            StationReturn.Data = result;
                        }
                        if (sfcdb != null)
                        {
                            this.DBPools["SFCDB"].Return(sfcdb);
                        }
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000204";
                        StationReturn.Data = result;
                    }
                }
            }
            catch (Exception e)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;


                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                //throw e;
            }
        }

        public void AddFaiToSku(MESPubLab.MESStation.MESStationBase Station, Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            R_SKU_ROUTE mapping = null;
            C_SKU Rsku = null;
            OleExec sfcdb = null;
            string result = string.Empty;
            string result1 = string.Empty;
            string result2 = string.Empty;
            string MappingObject = string.Empty;
            try
            {
                if (Data["MappingObject"] != null && !string.IsNullOrEmpty(Data["MappingObject"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    MappingObject = Data["MappingObject"].ToString();
                    mapping = (R_SKU_ROUTE)JsonConvert.Deserialize(MappingObject, typeof(R_SKU_ROUTE));
                    if (mapping.ROUTE_ID != null)
                    {
                        T_C_SKU c_SKU = new T_C_SKU(sfcdb, DBTYPE);
                        T_C_ROUTE_DETAIL c_ROUTE = new T_C_ROUTE_DETAIL(sfcdb, DBTYPE);

                        //DCN檢查除了'NETGEAR ODM', 'D-LINK ODM' 兩個系列 都需要做OBAFAI
                        //HWD需要判斷在C_CONTROL是否有配置
                        if (!c_SKU.SeriesIsExist(sfcdb, mapping.ROUTE_ID))
                        {

                            Rsku = c_SKU.getskuversionname(mapping.ROUTE_ID, sfcdb);
                            //CrouteName = c_ROUTE.getroutename(mapping.ROUTE_ID, sfcdb);

                            T_R_FAI RFAI = new T_R_FAI(sfcdb, DBTYPE);
                            R_FAI rfai = new R_FAI();

                            //每個機種對應的版本只能有一筆數據
                            if (!RFAI.CheckExistSku(Rsku.SKUNO, Rsku.VERSION, sfcdb))
                            {
                                OleExec oleDB = null;
                                oleDB = Station.SFCDB;
                                //oleDB = this.DBPools["SFCDB"].Borrow();
                                oleDB.BeginTrain();  //以下執行 要么全成功，要么全失敗

                                rfai.ID = RFAI.GetNewID(BU, sfcdb);
                                rfai.FAITYPE = "SKU";
                                rfai.SKUNO = Rsku.SKUNO;
                                rfai.SKU_VER = Rsku.VERSION;
                                rfai.STATUS = "0";
                                rfai.CREATEBY = LoginUser.EMP_NO;
                                rfai.CREATETIME = GetDBDateTime();
                                result = sfcdb.ORM.Insertable<R_FAI>(rfai).ExecuteCommand().ToString();

                                T_R_FAI_STATION RFAIS = new T_R_FAI_STATION(sfcdb, DBTYPE);
                                R_FAI_STATION rfais = new R_FAI_STATION();
                                rfais.ID = RFAIS.GetNewID(BU, sfcdb);
                                rfais.FAIID = rfai.ID;
                                rfais.STARTSTATION = "";
                                rfais.FAISTATION = "";
                                rfais.CREATEBY = LoginUser.EMP_NO;
                                rfais.CREATETIME = GetDBDateTime();
                                result1 = sfcdb.ORM.Insertable<R_FAI_STATION>(rfais).ExecuteCommand().ToString();

                                T_R_FAI_STATION RFAISD = new T_R_FAI_STATION(sfcdb, DBTYPE);
                                R_FAI_DETAIL rfaisd = new R_FAI_DETAIL();
                                rfaisd.ID = RFAISD.GetNewID(BU, sfcdb);
                                rfaisd.FAISTATIONID = rfais.ID;
                                rfaisd.CREATEBY = LoginUser.EMP_NO;
                                rfaisd.CREATETIME = GetDBDateTime();
                                result2 = sfcdb.ORM.Insertable<R_FAI_DETAIL>(rfaisd).ExecuteCommand().ToString();

                                oleDB.CommitTrain();

                                if (sfcdb != null)
                                {
                                    this.DBPools["SFCDB"].Return(sfcdb);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                //throw e;
            }
        }

        public void UpdateSkuRouteMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            R_SKU_ROUTE mapping = null;
            OleExec sfcdb = null;
            string result = string.Empty;
            string MappingObject = string.Empty;
            //MappingObject = @"{
            //    'ID': 'HWD000000000000000000000000000003',
            //    'DEFAULT_FLAG': 'Y',
            //    'SKU': {
            //        'ID': '111',
            //        'SKUNO': '222',
            //        'VERSION':'333'
            //    },
            //    'ROUTE': {
            //        'ID':'666',
            //        'ROUTE_NAME':'777'
            //    },
            //    'EDIT_TIME': '2017/12/21 17:13:21'

            //}";

            try
            {
                if (Data["MappingObject"] != null && !string.IsNullOrEmpty(Data["MappingObject"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    MappingObject = Data["MappingObject"].ToString();
                    mapping = (R_SKU_ROUTE)JsonConvert.Deserialize(MappingObject, typeof(R_SKU_ROUTE));
                    result = table.UpdateMapping(mapping, sfcdb);
                    if (Int32.Parse(result) > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000035";
                        StationReturn.MessagePara.Add(result);
                        StationReturn.Data = result;
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000036";
                        StationReturn.Data = result;
                    }
                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                }
            }
            catch (Exception e)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;


                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

                //throw e;
            }
        }

        public void DeleteSkuRouteMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                JToken IDs = Data["MappingID"];
                int counts = 0;
                for (int i = 0; i < IDs.Count(); i++)
                {
                    string result = string.Empty;
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    result = table.DeleteMapping(IDs[i].ToString(), sfcdb);
                    if (Int32.Parse(result) > 0)
                    {
                        counts++;
                    }

                    sfcdb.ORM.Insertable<R_MES_LOG>(new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_MES_LOG"),
                        PROGRAM_NAME = "SKU SETTING",
                        CLASS_NAME = "MESStation.Config.SkuRouteMappingConfig",
                        FUNCTION_NAME = "DeleteSkuRouteMapping",
                        //DATA1 = "SKU_ID: " + mapping.SKU_ID,
                        //DATA2 = "ROUTE_ID: " + mapping.ROUTE_ID,
                        //LOG_MESSAGE = "Delete R_SKU_ROUTE where ID = '"+ IDs[i].ToString() + "'",
                        LOG_SQL = "Delete R_SKU_ROUTE where ID = '" + IDs[i].ToString() + "'",
                        EDIT_EMP = LoginUser.EMP_NO,
                        EDIT_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM)
                    }).ExecuteCommand();

                }
                if (counts > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(counts);
                    StationReturn.Data = counts;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = counts;
                }
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DeleteSkuRouteMappingByObject(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            OleExec sfcdb = null;
            string result = string.Empty;
            R_SKU_ROUTE mapping = null;

            try
            {
                if (Data["MappingObject"] != null && !string.IsNullOrEmpty(Data["MappingObject"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    mapping = (R_SKU_ROUTE)JsonConvert.Deserialize(Data["MappingObject"].ToString(), typeof(R_SKU_ROUTE));

                    result = table.DeleteMapping(mapping, sfcdb);
                    //R_SKU_ROUTE

                    sfcdb.ORM.Insertable<R_MES_LOG>(new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_MES_LOG"),
                        PROGRAM_NAME = "SKU SETTING",
                        CLASS_NAME = "MESStation.Config.SkuRouteMappingConfig",
                        FUNCTION_NAME = "DeleteSkuRouteMappingByObject",
                        DATA1 = "SKU_ID: " + mapping.SKU_ID,
                        DATA2 = "ROUTE_ID: "+ mapping.ROUTE_ID,
                        LOG_SQL = Data["MappingObject"].ToString(),
                        EDIT_EMP = mapping.EDIT_EMP,
                        EDIT_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM)
                    }).ExecuteCommand();

                    if (Int32.Parse(result) > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000035";
                        StationReturn.MessagePara.Add(result);
                        StationReturn.Data = result;
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000036";
                        StationReturn.Data = result;
                    }

                    

                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                }
            }
            catch (Exception e)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;


                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

                //throw e;
            }
        }

        public void GetMappingsBySkuOrRoute(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string sql = string.Empty;
            string SkuOrRoute = string.Empty;
            T_R_SKU_ROUTE table = null;
            OleExec sfcdb = null;
            List<R_SKU_ROUTE> mappings = new List<R_SKU_ROUTE>();

            try
            {
                if (Data["SkuOrRoute"] != null && !string.IsNullOrEmpty(Data["SkuOrRoute"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    SkuOrRoute = Data["SkuOrRoute"].ToString();
                    mappings = table.Get_SKU_ROUTE_Mappings(sfcdb, new string[] { SkuOrRoute });
                    if (mappings.Count() == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(mappings.Count().ToString());
                        StationReturn.Data = mappings;
                    }

                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                //throw e;
            }
        }

        public void GetAllMappings(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<R_SKU_ROUTE> mappings = new List<R_SKU_ROUTE>();
            OleExec sfcdb = null;
            T_R_SKU_ROUTE table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                mappings = table.Get_SKU_ROUTE_Mappings(sfcdb);
                if (mappings.Count() == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(mappings.Count().ToString());
                    StationReturn.Data = mappings;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                //throw e;
            }
        }

        public void GetRoutesBySkuId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<SKU_ROUTE> Routes = new List<SKU_ROUTE>();
            OleExec sfcdb = null;
            string SkuId = string.Empty;
            T_R_SKU_ROUTE table = null;

            try
            {
                if (Data["SkuId"] != null && !string.IsNullOrEmpty(Data["SkuId"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    SkuId = Data["SkuId"].ToString();
                    Routes = table.GetBySKU(sfcdb, SkuId);
                    StationReturn.Data = Routes;
                    if (Routes == null || Routes.Count() == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new List<SKU_ROUTE>();
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(Routes.Count().ToString());
                    }

                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = Routes;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                //throw e;
            }
        }
        /// <summary>
        /// 獲取機種可用的路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAvailableRoutesForSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<C_ROUTE> getRouteList = new List<C_ROUTE>();
                string strSKUID = Data["SkuId"].ToString();
                T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                getRouteList = TC_ROUTE.GetAvailableRoutesForSkuBySkuid(strSKUID, sfcdb);
                if (getRouteList == null)
                {
                    getRouteList = new List<C_ROUTE>();
                }
                StationReturn.Data = getRouteList;
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
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }
}
