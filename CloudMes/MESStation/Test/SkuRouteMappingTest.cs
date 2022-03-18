using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDBHelper;

namespace MESStation.Test
{
    class SkuRouteMappingTest : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        private APIInfo _AddSkuRouteMapping = new APIInfo()
        {
            FunctionName="AddSKuRouteMapping",
            Description="添加機種路由映射關係",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="MappingObject",InputType="string",DefaultValue=""}
            },
            Permissions=new List<MESPermission>()
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
            FunctionName="DeleteSkuRouteMapping",
            Description="刪除機種路由映射關係",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="MappingID",InputType="string",DefaultValue="" }
            },
            Permissions=new List<MESPermission>()
            { }
        };

        private APIInfo _GetRouteMappingsBySkuName = new APIInfo()
        {
            FunctionName= "GetMappingsBySkuOrRoute",
            Description="根據機種ID獲取所有的路由",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuOrRoute",InputType="string",DefaultValue=""}
            },
            Permissions=new List<MESPermission>()
            { }
        };

        private APIInfo _GetAllMappings = new APIInfo()
        {
            FunctionName="GetAllMappings",
            Description="獲取所有的路由機種映射",
            Parameters=new List<APIInputInfo>()
            {
                
            },
            Permissions=new List<MESPermission>()
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

        public SkuRouteMappingTest()
        {
            this.Apis.Add(_AddSkuRouteMapping.FunctionName, _AddSkuRouteMapping);
            this.Apis.Add(_DeleteSkuRouteMapping.FunctionName, _DeleteSkuRouteMapping);
            this.Apis.Add(_UpdateSkuRouteMapping.FunctionName, _UpdateSkuRouteMapping);
            this.Apis.Add(_GetRouteMappingsBySkuName.FunctionName, _GetRouteMappingsBySkuName);
            this.Apis.Add(_GetAllMappings.FunctionName, _GetAllMappings);
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
                    mapping.EDIT_EMP = LoginUser.EMP_NO;
                    mapping.EDIT_TIME = GetDBDateTime();
                    result = table.AddMapping(mapping, BU, sfcdb);
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
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
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
            catch(Exception e)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

                throw e;
            }
        }

        public void DeleteSkuRouteMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            OleExec sfcdb = null;
            string result = string.Empty;
            string MappingID = string.Empty;
            //MappingID = @"HWD000000000000000000000000000003";

            try
            {
                if (Data["MappingID"] != null && !string.IsNullOrEmpty(Data["MappingID"].ToString()))
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    MappingID = Data["MappingID"].ToString();
                    result = table.DeleteMapping(MappingID, sfcdb);
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
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }

                throw e;
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
                throw e;
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
                throw e;
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

                    if (Routes==null || Routes.Count() == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(Routes.Count().ToString());
                        StationReturn.Data = Routes;
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
                throw e;
            }
        }
    }
}
