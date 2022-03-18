using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MESDataObject.Constants;
using MESPubLab.Common;
using System.Text.RegularExpressions;
using static MESDataObject.Common.EnumExtensions;
using MES_DCN.Schneider;
using MESDataObject.Module.OM;

namespace MESStation.Config
{
    public class ConvertWorkorder : MesAPIBase
    {
        #region
        protected APIInfo _GetWoConvertList = new APIInfo()
        {
            FunctionName = "GetWoConvertList",
            Description = "Get Wo Convert List",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoInfoByWo = new APIInfo()
        {
            FunctionName = "GetWoInfoByWo",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "WO", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoInfoById = new APIInfo()
        {
            FunctionName = "GetWoInfoById",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "ID", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoType = new APIInfo()
        {
            FunctionName = "GetWoType",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo()
                //{
                //    InputName = "ID", InputType = "string", DefaultValue = ""
                //}
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetStationBySkuno = new APIInfo()
        {
            FunctionName = "GetStationBySkuno",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "skuno", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetRouteBySkuno = new APIInfo()
        {
            FunctionName = "GetRouteBySkuno",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "skuno", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo _GetRouteBySkunoAndVer = new APIInfo()
        {
            FunctionName = "GetRouteBySkunoAndVer",
            Description = "GetRouteBySkunoAndVer",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "SKUNO", InputType = "string", DefaultValue = ""
                },
                new APIInputInfo()
                {
                    InputName = "VERSION", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetKeyPartBySkuno = new APIInfo()
        {
            FunctionName = "GetKeyPartBySkuno",
            Description = "_GetKeyPartBySkuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "skuno", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _SubmitWoInfo = new APIInfo()
        {
            FunctionName = "SubmitWoInfo",
            Description = "_SubmitWoInfo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "wo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "factory", InputType = "string" },
                new APIInputInfo() { InputName = "date", InputType = "string" },
                new APIInputInfo() { InputName = "qty", InputType = "string" },
                new APIInputInfo() { InputName = "wo_type", InputType = "string" },
                new APIInputInfo() { InputName = "station", InputType = "string" },
                new APIInputInfo() { InputName = "skuno", InputType = "string" },
                new APIInputInfo() { InputName = "sku_ver", InputType = "string" },
                new APIInputInfo() { InputName = "route_name", InputType = "string" },
                new APIInputInfo() { InputName = "kp_list_id", InputType = "string" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoDeviation = new APIInfo()
        {
            FunctionName = "GetWoDeviation",
            Description = "Get Wo Deviation",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetKPBySkuno = new APIInfo()
        {
            FunctionName = "GetKPBySkuno",
            Description = "GetKPBySkuno",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoDeviationByFuzzySearch = new APIInfo()
        {
            FunctionName = "GetWoDeviationByFuzzySearch",
            Description = "Get Wo Deviation By Fuzzy Search",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="SearchValue",InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _DeleteWoDeviationByIds = new APIInfo()
        {
            FunctionName = "DeleteWoDeviationByIds",
            Description = "Delete Wo Deviation By Ids",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="Ids",InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _AddNewWoDeviation = new APIInfo()
        {
            FunctionName = "AddNewWoDeviation",
            Description = "Add a new wo deviation",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="WO",InputType="string" },
                new APIInputInfo() { InputName="DEVIATION",InputType="string" }
            },
        };
        protected APIInfo _UploadExcelWO = new APIInfo()
        {
            FunctionName = "UploadExcelWO",
            Description = "UploadExcelWO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SKUNO",InputType="string" },
                new APIInputInfo() { InputName="SKUVERSION",InputType="string" },
                new APIInputInfo() { InputName="WO",InputType="string" },
                new APIInputInfo() { InputName="QTY",InputType="string" }
            },
        };
        protected APIInfo _ShowFailRecord = new APIInfo()
        {
            FunctionName = "ShowFailRecord",
            Description = "ShowFailRecord",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SKUNO",InputType="string" }

            },
        };
        #endregion

        public ConvertWorkorder()
        {
            Apis.Add(_GetWoConvertList.FunctionName, _GetWoConvertList);
            Apis.Add(_GetWoInfoByWo.FunctionName, _GetWoInfoByWo);
            Apis.Add(_GetWoInfoById.FunctionName, _GetWoInfoById);
            Apis.Add(_GetWoType.FunctionName, _GetWoType);
            Apis.Add(_GetStationBySkuno.FunctionName, _GetStationBySkuno);
            Apis.Add(_GetRouteBySkuno.FunctionName, _GetRouteBySkuno);
            Apis.Add(_GetKeyPartBySkuno.FunctionName, _GetKeyPartBySkuno);
            Apis.Add(_SubmitWoInfo.FunctionName, _SubmitWoInfo);
            Apis.Add(_GetWoDeviation.FunctionName, _GetWoDeviation);
            Apis.Add(_GetWoDeviationByFuzzySearch.FunctionName, _GetWoDeviationByFuzzySearch);
            Apis.Add(_DeleteWoDeviationByIds.FunctionName, _DeleteWoDeviationByIds);
            Apis.Add(_AddNewWoDeviation.FunctionName, _AddNewWoDeviation);
            Apis.Add(_GetKPBySkuno.FunctionName, _GetKPBySkuno);
            Apis.Add(_UploadExcelWO.FunctionName, _UploadExcelWO);
            Apis.Add(_ShowFailRecord.FunctionName, _ShowFailRecord);
            Apis.Add(_GetRouteBySkunoAndVer.FunctionName, _GetRouteBySkunoAndVer);

        }

        public void GetWoConvertList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_HEADER(oleDB, DB_TYPE_ENUM.Oracle);
                //dt = t_header.GetConvertWoList(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetWoSpecialVar(oleDB, new string[0], this.BU);
                if (dt == null || dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    //StationReturn.Data = dt;
                    StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void GetWoInfoByWo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            string wo = Data["WO"].ToString();
            if (string.IsNullOrEmpty(wo))
            {
                //throw new Exception();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Workorder" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_HEADER(oleDB, DB_TYPE_ENUM.Oracle);
                //dt = t_header.GetConvertWoList(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetConvertWoTableByWO(oleDB, DB_TYPE_ENUM.Oracle, wo);
                if (dt == null || dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void GetWoInfoById(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            string wo = Data["ID"].ToString();
            if (string.IsNullOrEmpty(wo))
            {
                //throw new Exception();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "ID" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_HEADER(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetConvertWoTableById(oleDB, wo);
                if (dt == null || dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void GetWoType(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_TYPE t_header = null;
            List<string> dt = null;
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_TYPE(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetAllType(oleDB);
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void GetStationBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_SKU t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_SKU(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetStationBySku(oleDB, skuno);

                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void GetRouteBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_ROUTE t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_ROUTE(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetRouteBySkuno(oleDB, skuno);
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }
        public void GetRouteBySkunoAndVer(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_ROUTE t_header = null;
            List<string> dt = null;
            string skuno = Data["SKUNO"].ToString();
            string version = Data["VERSION"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                dt = oleDB.ORM.Queryable<C_SKU, R_SKU_ROUTE, C_ROUTE>((c, r, t) => c.ID == r.SKU_ID && r.ROUTE_ID == t.ID)
                    .Where((c, r, t) => c.SKUNO == skuno && c.VERSION == version).Select((c, r, t) => t.ROUTE_NAME).ToList();
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void GetKPBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_KP_LIST t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }

            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_KP_LIST(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetListNameBySkuno(skuno, oleDB); ;

                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void GetWoFactory(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_KP_LIST t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }

            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_KP_LIST(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetListNameBySkuno(skuno, oleDB); ;

                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetKeyPartBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_KEYPART t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "skuno" }));
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_KEYPART(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetListBySkuno(oleDB, skuno);
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }

                throw ex;
            }
        }

        public void SubmitWoInfo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            //wo
            string wo = Data["wo"].ToString();
            if (string.IsNullOrEmpty(wo))
            {
                //StationReturn.Status = StationReturnStatusValue.Fail;
                //StationReturn.MessageCode = "MES00000006";
                //StationReturn.MessagePara = new List<object>() { "Skuno" };
                //StationReturn.Data = "";
                //return;
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "WO" }));
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                T_R_WO_BASE t_wo = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);

                if (t_wo.CheckDataExist(wo, sfcdb))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { wo }));
                }

                R_WO_HEADER wo_header = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailByWo(sfcdb, wo);

                //sku info
                string skuno = Data["skuno"].ToString();//wo_header.MATNR
                if (string.IsNullOrEmpty(skuno))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKUNO" }));
                }
                string skuver = Data["sku_ver"].ToString();//wo_header.REVLV

                C_SKU c_sku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle).GetSku(skuno, sfcdb);
                if (c_sku == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { skuno }));
                }


                //route exchange from name
                string route_name = Data["route_name"].ToString();
                if (string.IsNullOrEmpty(route_name))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "ROUTE" }));
                }
                C_ROUTE c_route = new T_C_ROUTE(sfcdb, DB_TYPE_ENUM.Oracle).GetByRouteName(route_name, sfcdb);
                if (c_route == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { route_name }));
                }

                //station route check
                string station_name = Data["station"].ToString();
                if (string.IsNullOrEmpty(station_name))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "STATION" }));
                }
                List<C_ROUTE_DETAIL> c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle).GetByRouteIdOrderBySEQASC(c_route.ID, sfcdb);
                if (c_route_detail != null && c_route_detail.Count > 0)
                {
                    C_ROUTE_DETAIL check = c_route_detail.Find(t => t.STATION_NAME == station_name);
                    if (check == null)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { station_name }));
                    }
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { route_name }));
                }
                T_R_WO_LOG TRWOLOG = new T_R_WO_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                bool CheckKp = TRWOLOG.CheckKpSetCount(wo, sfcdb);
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                bool checkBU = TRFC.CheckUserFunctionExist("WOKPComfirm", "WOKPComfirm", this.BU, sfcdb);
                if (!CheckKp && checkBU)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162658"));
                }

                #region c_kp_list
                var ckplist = sfcdb.ORM.Queryable<C_KP_LIST>()
                    .Where(t => t.LISTNAME == Data["KpListName"].ToString().Trim()).ToList().FirstOrDefault();
                #endregion

                //data record
                Row_R_WO_BASE row_wobase = (Row_R_WO_BASE)t_wo.NewRow();
                row_wobase.ID = t_wo.GetNewID(this.BU, sfcdb);
                row_wobase.WORKORDERNO = wo;
                row_wobase.PLANT = Data["factory"].ToString();
                row_wobase.RELEASE_DATE = DateTime.Now;
                row_wobase.DOWNLOAD_DATE = Convert.ToDateTime(Data["date"].ToString());
                row_wobase.PRODUCTION_TYPE = "BTO";
                row_wobase.WO_TYPE = Data["wo_type"].ToString();
                row_wobase.SKUNO = skuno;
                row_wobase.SKU_VER = skuver;
                row_wobase.SKU_NAME = c_sku.SKU_NAME;
                //row_wobase.SKU_SERIES = null;
                //row_wobase.SKU_DESC = null;
                row_wobase.INPUT_QTY = 0;   //add by fgg 2019.08.13 避免後續過站update這個值時因為空值導致異常
                row_wobase.FINISHED_QTY = 0;  //add by fgg 2019.08.13 避免後續過站update這個值時因為空值導致異常
                row_wobase.CUST_PN = c_sku.CUST_PARTNO;
                row_wobase.ROUTE_ID = c_route.ID;
                row_wobase.START_STATION = station_name;
                row_wobase.KP_LIST_ID = ckplist != null ? ckplist.ID : "";
                row_wobase.CLOSED_FLAG = "0";
                row_wobase.WORKORDER_QTY = Convert.ToDouble(Data["qty"].ToString());
                row_wobase.STOCK_LOCATION = wo_header.LGORT;
                row_wobase.CUST_ORDER_NO = wo_header.ABLAD;
                row_wobase.EDIT_EMP = this.LoginUser.EMP_NO;
                row_wobase.EDIT_TIME = DateTime.Now;

                string sql = row_wobase.GetInsertString(DB_TYPE_ENUM.Oracle);


                int res = sfcdb.ExecSqlNoReturn(sql, null);
                if (res == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { wo }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void SubmitWoInfoDcn(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            //wo
            string wo = Data["wo"].ToString();
            string skunoLog = null;

            if (string.IsNullOrEmpty(wo))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "WO" }));
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                T_R_WO_BASE t_wo = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);

                if (t_wo.CheckDataExist(wo, sfcdb))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { wo }));
                }

                R_WO_HEADER wo_header = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailByWo(sfcdb, wo);

                //sku info
                string skuno = Data["skuno"].ToString();//wo_header.MATNR

                skunoLog = skuno;

                if (string.IsNullOrEmpty(skuno))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKUNO" }));
                }
                string skuver = Data["sku_ver"].ToString();//wo_header.REVLV

                C_SKU c_sku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle).GetSku(skuno, sfcdb);
                if (c_sku == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { skuno }));
                }


                //route exchange from name
                string route_name = Data["route_name"].ToString();
                if (string.IsNullOrEmpty(route_name))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "ROUTE" }));
                }

                if (route_name.Contains("CTO"))
                {
                    bool ss = sfcdb.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((csk, css, ccu) => csk.ID == css.ID && css.CUSTOMER_ID == ccu.ID)
                        .Where((csk, css, ccu) => csk.SKUNO == skuno && ccu.DESCRIPTION == "NETGEAR ODM").Select((csk, css, ccu) => csk).Any();
                    if (ss)
                    {
                        string strSql = $@"select * from c_sku_label where skuno = '{skuno}' AND STATION = 'P-BOX' AND EDIT_TIME < sysdate -3";
                        DataTable dtt = sfcdb.RunSelect(strSql).Tables[0];
                        if (dtt.Rows.Count > 0)
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200717084550", new string[] { skuno }));
                        }
                    }
                }

                C_ROUTE c_route = new T_C_ROUTE(sfcdb, DB_TYPE_ENUM.Oracle).GetByRouteName(route_name, sfcdb);
                if (c_route == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { route_name }));
                }

                //station route check
                string station_name = "";
                List<C_ROUTE_DETAIL> c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle).GetByRouteIdOrderBySEQASC(c_route.ID, sfcdb);
                if (c_route_detail != null && c_route_detail.Count > 0)
                {
                    if (Data["wo_type"].ToString().ToUpper().StartsWith("REWORK") || Data["wo_type"].ToString().ToUpper() == "RMA")
                    {
                        station_name = Data["station"].ToString();
                        C_ROUTE_DETAIL check = c_route_detail.Find(t => t.STATION_NAME == station_name);
                        if (check == null)
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { station_name }));
                        }
                    }
                    else
                    {
                        station_name = c_route_detail.FirstOrDefault().STATION_NAME;
                    }
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { route_name }));
                }
                if (string.IsNullOrEmpty(station_name))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "STATION" }));
                }

                bool _check = sfcdb.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((csk, css, ccu) => csk.C_SERIES_ID == css.ID && css.CUSTOMER_ID == ccu.ID)
                 .Where((csk, css, ccu) => csk.SKUNO == skuno && ccu.CUSTOMER_NAME == "NETGEAR").Select((csk, css, ccu) => csk).Any();

                var ExistPBOX = c_route_detail.Find(t => t.STATION_NAME == "P-BOX");

                var _checklabel = sfcdb.ORM.Queryable<C_SKU_Label>().Where(csl => csl.STATION == "P-BOX" && csl.SKUNO == skuno).Select((csl) => csl).Any();

                if (_check && ExistPBOX != null && _checklabel)
                {

                    string strSql = $@"select * from c_sku_detail where skuno = '{skuno}' AND CATEGORY = 'P-BOX_TA'";
                    DataTable dtt = sfcdb.RunSelect(strSql).Tables[0];
                    if (dtt.Rows.Count == 0)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20201002141256", new string[] { skuno }));
                    }

                    strSql = $@"select * from c_sku_detail where skuno='{skuno}' and CATEGORY='P-BOX_TA' and EDIT_TIME<sysdate-5";
                    DataTable _dtt = sfcdb.RunSelect(strSql).Tables[0];

                    if (_dtt.Rows.Count > 0)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20201002141627", new string[] { skuno }));
                    }

                }

                T_R_WO_LOG TRWOLOG = new T_R_WO_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                bool CheckKp = TRWOLOG.CheckKpSetCount(wo, sfcdb);
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                bool checkBU = TRFC.CheckUserFunctionExist("WOKPComfirm", "WOKPComfirm", this.BU, sfcdb);
                if (!CheckKp && checkBU)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162658"));
                }

                #region PTM TA/FA
                var customer = sfcdb.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((cs, cse, cc) => cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                    .Where((cs, cse, cc) => cs.SKUNO == skuno).Select((cs, cse, cc) => cc.CUSTOMER_NAME).ToList().FirstOrDefault();

                if (customer == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210911094908", new string[] { skuno }));
                }

                if (customer.ToUpper().Trim().Equals(Customer.NETGEAR.Ext<EnumValueAttribute>().Description))
                {
                    var sn_rule = sfcdb.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == skuno).Select(t => t.SN_RULE).First();

                    var rfunctioncontrolobj = sfcdb.ORM
                        .Queryable<R_F_CONTROL, R_F_CONTROL_EX
                        >((rfc, rfcx) => rfc.ID == rfcx.DETAIL_ID).Where((rfc, rfcx) =>
                            rfc.FUNCTIONNAME == "PTM_TA/FA" && rfc.CATEGORY == "PTM_TA/FA" &&
                            rfc.FUNCTIONTYPE == "NOSYSTEM" && rfc.VALUE == skuno)
                        .Select((rfc, rfcx) => new { rfc, rfcx }).ToList();
                    var PREXID = new Func<string>(() =>
                    {
                        var preobj = sfcdb.ORM
                            .Queryable<C_SN_RULE, C_SN_RULE_DETAIL>((csr, csrd) => csr.ID == csrd.C_SN_RULE_ID)
                            .Where((csr, csrd) => csr.NAME == sn_rule && csrd.INPUTTYPE == "PREFIX" && csrd.SEQ == '1')
                            .Select((csr, csrd) => csrd).ToList().FirstOrDefault();

                        if (preobj != null)
                            return preobj.CURVALUE;
                        else
                            throw new Exception("MIS WO PREXID");
                    })();

                    //NETGERA出貨階都需要檢查是否配置PTM_TA/FA  Edit By ZHB 2020年9月15日15:35:41
                    var shipoutRoute = c_route_detail.Find(t => t.STATION_NAME == "SHIPOUT");
                    if (shipoutRoute != null)   //如果是出貨階,一階的就不檢查也不寫R_PTM_TACONFIG表了
                    {
                        if (rfunctioncontrolobj == null)    //如果未配置PTM_TA/FA
                        {
                            throw new Exception("NETGEAR出貨階機種需要先配置PTM_TA/FA!");
                        }
                        else    //如果已配置PTM_TA/FA
                        {
                            //wo release TA/FA
                            sfcdb.ORM.Insertable(new R_PTM_TACONFIG()
                            {
                                ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_PTM_TACONFIG"),
                                WO = wo,
                                SKUNO = skuno,
                                PREXID = PREXID,
                                TA_NUMBER = new Func<string>(() =>
                                {
                                    var faobj = rfunctioncontrolobj.FindAll(t =>
                                        t.rfc.EXTVAL == PREXID &&
                                        t.rfcx.NAME == "TA");
                                    if (faobj.Count == 0)
                                        throw new Exception("MIS_SKU_FA");
                                    return faobj.FirstOrDefault().rfcx.VALUE ?? "";
                                })(),
                            }).ExecuteCommand();
                        }
                    }
                }


                #endregion

                #region c_kp_list

                string KpListName = Data["KpListName"].ToString().Trim();

                //防止沒有KP 而一槍過站 2021年10月30日
                if (string.IsNullOrEmpty(KpListName))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162659"));
                }

                var ckplist = sfcdb.ORM.Queryable<C_KP_LIST>()
                    .Where(t => t.LISTNAME == KpListName && t.SKUNO == skuno && t.FLAG == "1").ToList().FirstOrDefault();
                var CheckWoType = sfcdb.ORM.Queryable<R_WO_TYPE>().Where(t => t.PREFIX == wo.Substring(0, 6)).ToList().FirstOrDefault();
                if (CheckWoType.ORDER_TYPE == "REGULAR" && (skuno.StartsWith("711") || skuno.StartsWith("750")))
                {
                    var MpnRule = sfcdb.ORM.Queryable<C_KP_LIST, C_KP_List_Item, C_KP_List_Item_Detail, C_SKU_MPN, C_KP_Rule>
                                  ((KL, KIT, KITD, MPN, RL) => new object[] {
                                            SqlSugar.JoinType.Left,(KL.ID==KIT.LIST_ID),
                                            SqlSugar.JoinType.Left,(KIT.ID==KITD.ITEM_ID),
                                            SqlSugar.JoinType.Left,(KIT.KP_PARTNO==MPN.PARTNO && KL.SKUNO==MPN.SKUNO),
                                            SqlSugar.JoinType.Left,(MPN.PARTNO==RL.PARTNO && MPN.MPN==RL.MPN && KITD.SCANTYPE==RL.SCANTYPE)
                                  })
                                  .Where((KL, KIT, KITD, MPN, RL) => KL.LISTNAME == KpListName)
                                  .OrderBy((KL, KIT, KITD, MPN, RL) => KIT.SEQ, SqlSugar.OrderByType.Asc)
                                  .OrderBy((KL, KIT, KITD, MPN, RL) => KITD.SEQ, SqlSugar.OrderByType.Asc)
                                  .Select((KL, KIT, KITD, MPN, RL) => new
                                  {
                                      SEQ = KIT.SEQ,
                                      SCANSEQ = KITD.SEQ,
                                      SKUNO = KL.SKUNO,
                                      LISTNAME = KL.LISTNAME,
                                      PARTNO = MPN.PARTNO,
                                      MPN = MPN.MPN,
                                      STATION = KIT.STATION,
                                      MFRCODE = MPN.MFRCODE,
                                      SCANTYPE = KITD.SCANTYPE,
                                      REGEX = RL.REGEX
                                  }).ToList();
                    foreach (var KPDetail in MpnRule)
                    {
                        if (KPDetail.SCANTYPE == "PN" && KPDetail.REGEX == "^Y$")
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162660"));
                        }
                        if (skuno == KPDetail.PARTNO && (KPDetail.SCANTYPE == "SN" || KPDetail.SCANTYPE.ToUpper() == "SYSTEMSN"))
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162661"));
                        }
                        var Bomdetail = sfcdb.ORM.Queryable<R_WO_ITEM>().Where(r => r.AUFNR == wo && (r.MATNR == KPDetail.PARTNO || r.BAUGR == KPDetail.PARTNO)).ToList();
                        if (Regex.IsMatch(KPDetail.PARTNO.Substring(1, 1), @"^[0-9]{1}$") && Bomdetail.Count == 0)
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162662", new string[] { KPDetail.PARTNO }));
                        }
                    }
                }

                //keypart partno is 711/750 must scan SystemSN/SN(buy part)
                if ((this.BU == "FJZ" || this.BU == "VNJUNIPER")
                    && !sfcdb.ORM.Queryable<R_WO_TYPE>()
                    .Where(t => wo.Contains(t.PREFIX)
                    && (t.WORKORDER_TYPE.StartsWith("REWORK") || t.WORKORDER_TYPE.StartsWith("RMA")))
                    .Any()
                    )
                {
                    var kpset = sfcdb.ORM.Ado.GetDataTable($@"SELECT L.LISTNAME,
                                                               L.SKUNO,
                                                               I.KP_NAME,
                                                               I.KP_PARTNO,
                                                               I.STATION,
                                                               I.QTY,
                                                               I.SEQ,
                                                               D.SCANTYPE,
                                                               D.SEQ
                                                          FROM C_KP_LIST L, C_KP_LIST_ITEM I, C_KP_LIST_ITEM_DETAIL D
                                                         WHERE L.ID = I.LIST_ID
                                                           AND L.FLAG=1
                                                           AND I.ID = D.ITEM_ID
                                                           AND L.SKUNO = '{skuno}'
                                                           AND D.SCANTYPE IN('SN','SystemSN','PCBA S/N')
                                                        UNION ALL
                                                        SELECT 'LINK_CONTORL',
                                                               MAIN_ITEM AS SKUNO,
                                                               'KEEP_SN',
                                                               SUB_ITEM AS KP_PARTNO,
                                                               'SILOADING',
                                                               1,
                                                               0,
                                                               'SystemSN',
                                                               1
                                                          FROM R_LINK_CONTROL
                                                         WHERE MAIN_ITEM = '{skuno}'");
                    var sapbom = sfcdb.ORM.Ado.SqlQuery<R_WO_ITEM>($@"
                                                        SELECT *
                                                          FROM R_WO_ITEM
                                                         WHERE AUFNR = '{wo}'
                                                           AND (MATNR IN (SELECT DISTINCT SKUNO FROM C_SKU) OR MATNR LIKE '711-%' OR MATNR LIKE '750-%')")
                            .ToList();

                    if (!wo.StartsWith("0093") && !wo.StartsWith("0094"))
                    {
                        for (int i = 0; i < sapbom.Count; i++)
                        {
                            var dr = kpset.Select($@"KP_PARTNO='{sapbom[i].MATNR}'");
                            if (dr.Length == 0)
                            {
                                throw new Exception($@"Please add scantype is 'SN' or 'SystemSN' keypart configuretion of partno[{sapbom[i].MATNR}],Or if you need to inherit the serial number,please configure LinkContol with 'LOADING_KEEP_SN'.");
                            }
                        }
                    }

                    var Revs = MESJuniper.Base.EcnFunction.GetUsefulVer(skuno, sfcdb);

                    if (skuno.StartsWith("711"))
                    {
                        var mver = MESJuniper.Base.EcnFunction.GetLastMandatoryUsefulVers(skuno, sfcdb);
                        if (!mver.Contains(skuver))
                        {
                            throw new Exception($@"The latest mandatory ECO revision must be {mver[mver.Count-1]}");
                        }

                    }
                    if (skuno.EndsWith("-FJZ") || skuno.EndsWith("-FVN") || skuno.EndsWith("-TAA"))
                    {
                        var mver = MESJuniper.Base.EcnFunction.GetLastMandatoryUsefulVers(skuno, sfcdb);
                        if (!mver.Contains(skuver))
                        {
                            throw new Exception($@"The latest mandatory ECO revision must be {mver[mver.Count - 1]}");
                        }
                    }
                    else
                    {
                        if (!Revs.Contains(skuver))
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("REV_OF_PARTNO_ERROR", new string[] { skuno, skuver }));
                        }
                    }

                }

                #endregion

                //data record
                Row_R_WO_BASE row_wobase = (Row_R_WO_BASE)t_wo.NewRow();
                row_wobase.ID = t_wo.GetNewID(this.BU, sfcdb);
                row_wobase.WORKORDERNO = wo;
                row_wobase.PLANT = Data["factory"].ToString().ToUpper();
                row_wobase.RELEASE_DATE = MesDbBase.GetOraDbTime(sfcdb.ORM);
                row_wobase.DOWNLOAD_DATE = Convert.ToDateTime(Data["date"].ToString());
                row_wobase.PRODUCTION_TYPE = "BTO";
                row_wobase.WO_TYPE = Data["wo_type"].ToString();
                row_wobase.SKUNO = skuno;
                row_wobase.SKU_VER = skuver;
                row_wobase.SKU_NAME = c_sku.SKU_NAME;
                //row_wobase.SKU_SERIES = null;
                //row_wobase.SKU_DESC = null;
                row_wobase.INPUT_QTY = 0;   //add by fgg 2019.08.13 避免後續過站update這個值時因為空值導致異常
                row_wobase.FINISHED_QTY = 0;  //add by fgg 2019.08.13 避免後續過站update這個值時因為空值導致異常
                row_wobase.CUST_PN = c_sku.CUST_PARTNO;
                row_wobase.ROUTE_ID = c_route.ID;
                row_wobase.START_STATION = station_name;
                row_wobase.KP_LIST_ID = ckplist != null ? ckplist.ID : "";
                row_wobase.CLOSED_FLAG = "0";
                row_wobase.WORKORDER_QTY = Convert.ToDouble(Data["qty"].ToString());
                row_wobase.STOCK_LOCATION = wo_header.LGORT;
                row_wobase.CUST_ORDER_NO = wo_header.ABLAD;
                row_wobase.EDIT_EMP = this.LoginUser.EMP_NO;
                row_wobase.EDIT_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM);

                string sql = row_wobase.GetInsertString(DB_TYPE_ENUM.Oracle);


                int res = sfcdb.ExecSqlNoReturn(sql, null);
                if (res == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { wo }));
                }

                #region VNDCN Schneider 傳工單信息給客戶 自動線已休眠，等它活再開
                //其他開關是工站action， stockin  siloading
                var isCTL = sfcdb.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "SCHNEIDER_AUTOLINE" && t.CONTROL_TYPE == "Schneider_ConvertWO" && t.CONTROL_VALUE == "Y").Any();
                if (isCTL)
                {
                    var modelWoList = sfcdb.ORM.Queryable<R_WO_BASE, C_SKU, C_SERIES>((a, b, c) => a.SKUNO == b.SKUNO && a.SKU_VER == b.VERSION && b.C_SERIES_ID == c.ID)
                        .Where((a, b, c) => c.DESCRIPTION == "Schneider" && b.SKU_TYPE == "MODEL" && a.WORKORDERNO == row_wobase.WORKORDERNO)
                        .Select((a, b, c) => new { a.WORKORDERNO, a.SKUNO, a.WORKORDER_QTY, b.DESCRIPTION }).ToList().FirstOrDefault();
                    if (modelWoList != null)
                    {
                        SchneiderAction.Model_WO modelWo = new SchneiderAction.Model_WO()
                        {
                            Wo = modelWoList.WORKORDERNO,
                            Skuno = modelWoList.SKUNO,
                            SkunoDescription = modelWoList.DESCRIPTION,
                            Quantity = modelWoList.WORKORDER_QTY.ToString()
                        };
                        SchneiderAction Schneider = new SchneiderAction();
                        Schneider.SendModelWoToSchneiderDB(modelWo);
                    }
                }

                #endregion
                /*
                sfcdb.ORM.Insertable<R_MES_LOG>(new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_MES_LOG"),
                    PROGRAM_NAME = "ConvertWorkOrder",
                    CLASS_NAME = "MESStation.Config.ConvertWorkorder",
                    FUNCTION_NAME = "SubmitWoInfoDcn",
                    DATA1 = wo,
                    DATA2 = skuno,
                    LOG_MESSAGE = "Successfully",
                    EDIT_EMP = LoginUser.EMP_NO,
                    EDIT_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM)
                }).ExecuteCommand();
                */
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                /*
                sfcdb.ORM.Insertable<R_MES_LOG>(new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID(sfcdb.ORM, BU, "R_MES_LOG"),
                    PROGRAM_NAME = "ConvertWorkOrder",
                    CLASS_NAME = "MESStation.Config.ConvertWorkorder",
                    FUNCTION_NAME = "SubmitWoInfoDcn",
                    DATA1 = wo,
                    DATA2 = skunoLog,
                    LOG_MESSAGE = ex.ToString().Trim(),
                    EDIT_EMP = LoginUser.EMP_NO,
                    EDIT_TIME = MesDbBase.GetOraDbTime(sfcdb.ORM)
                }).ExecuteCommand();
                */
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }

        }


        public void GetWoDeviation(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_R_WO_DEVIATION TRWD = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TRWD = new T_R_WO_DEVIATION(DB, DB_TYPE_ENUM.Oracle);
                List<R_WO_DEVIATION> Deviations = TRWD.GetWoDeviations(DB);
                if (Deviations.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = Deviations;
                }

                if (DB != null)
                {
                    DBPools["SFCDB"].Return(DB);
                }
            }
            catch (Exception ex)
            {
                if (DB != null)
                {
                    DBPools["SFCDB"].Return(DB);
                }

                throw ex;
            }
        }

        public void GetWoDeviationByFuzzySearch(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_R_WO_DEVIATION TRWD = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TRWD = new T_R_WO_DEVIATION(DB, DB_TYPE_ENUM.Oracle);
                string SearchValue = Data["SearchValue"].ToString().ToUpper();
                List<R_WO_DEVIATION> Deviations = TRWD.GetWoDeviationByFuzzyQuery(SearchValue, DB);
                if (Deviations.Count > 0)
                {
                    StationReturn.Data = Deviations;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(SearchValue);
                }
                this.DBPools["SFCDB"].Return(DB);
            }
            catch (Exception e)
            {
                if (DB != null)
                {
                    this.DBPools["SFCDB"].Return(DB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }

        }

        public void DeleteWoDeviationByIds(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_R_WO_DEVIATION TRWD = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TRWD = new T_R_WO_DEVIATION(DB, DB_TYPE_ENUM.Oracle);
                string Ids = Data["Ids"].ToString();
                int result = TRWD.DeleteWoDeviationByIds(Ids, DB);

                if (result > 0)
                {
                    StationReturn.Data = result;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
                }
                this.DBPools["SFCDB"].Return(DB);

            }
            catch (Exception e)
            {
                if (DB != null)
                {
                    this.DBPools["SFCDB"].Return(DB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
        }

        public void AddNewWoDeviation(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_R_WO_DEVIATION TRWD = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TRWD = new T_R_WO_DEVIATION(DB, DB_TYPE_ENUM.Oracle);
                string WorkOrder = Data["WO"].ToString();
                string Deviation = Data["DEVIATION"].ToString();
                int result = 0;
                if (DB.ORM.Queryable<R_WO_DEVIATION>().Any(t => t.WORKORDERNO == WorkOrder))
                {
                    result = TRWD.UpdateWoDeviation(WorkOrder, Deviation, LoginUser.EMP_NO, DB);
                }
                else
                {
                    result = TRWD.AddWoDeviation(WorkOrder, Deviation, LoginUser.BU, LoginUser.EMP_NO, DB);
                }

                if (result > 0)
                {
                    StationReturn.Data = result;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
                }
                this.DBPools["SFCDB"].Return(DB);

            }
            catch (Exception e)
            {
                if (DB != null)
                {
                    this.DBPools["SFCDB"].Return(DB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
        }

        public void UpdateWoDeviationById(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_R_WO_DEVIATION TRWD = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TRWD = new T_R_WO_DEVIATION(DB, DB_TYPE_ENUM.Oracle);
                string Id = Data["Id"].ToString();
                string Deviation = Data["Deviation"].ToString();
                int result = TRWD.UpdateWoDeviationById(Id, Deviation, LoginUser.EMP_NO, DB);


                if (result > 0)
                {
                    StationReturn.Data = result;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
                }
                this.DBPools["SFCDB"].Return(DB);

            }
            catch (Exception e)
            {
                if (DB != null)
                {
                    this.DBPools["SFCDB"].Return(DB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
        }

        public void UploadExcelWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            var successCount = 0;
            var failCount = 0;
            string data = Data["DataList"].ToString();
            try
            {
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                Newtonsoft.Json.Linq.JToken dataList = Data["DataList"];

                string SKUNO = string.Empty;
                string SKUVERSION = string.Empty;
                string WO = string.Empty;
                string QTY = string.Empty;

                for (int i = 0; i < array.Count; i++)
                {
                    T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBTYPE);
                    T_R_WO_HEADER TRWH = new T_R_WO_HEADER(SFCDB, DBTYPE);
                    T_R_WO_ITEM TRWI = new T_R_WO_ITEM(SFCDB, DBTYPE);

                    SKUNO = array[i]["SKUNO"].ToString().Trim();
                    SKUVERSION = array[i]["SKUVERSION"].ToString().Trim();
                    WO = array[i]["WO"].ToString().Trim();
                    QTY = array[i]["QTY"].ToString().Trim();

                    List<C_KP_List_Item> itemList = SFCDB.
                     ORM.Queryable<C_KP_LIST, C_KP_List_Item>((a, b) => a.ID == b.LIST_ID)
                     .Where((a, b) => a.FLAG == "1" && a.SKUNO == SKUNO)
                     .Select((a, b) => b)
                     .ToList();

                    bool checkSku = SFCDB.ORM.Queryable<C_SKU>()
                        .Where(it => it.SKUNO == SKUNO && it.VERSION == SKUVERSION)
                        .Any();

                    R_WO_TYPE RWT = SFCDB.ORM.Queryable<R_WO_TYPE>()
                        .Where(it => it.PREFIX == WO.Substring(0, 6)).First();
                    if (!Regex.IsMatch(QTY, @"^[1-9][0-9]{0,}$"))
                    {
                        InsertFailLog("數量需為正整數", WO, QTY, "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }
                    if (!Regex.IsMatch(WO, @"^[0-9]{12}$"))
                    {
                        InsertFailLog("工單需為12位數字", WO, "", "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }
                    if (!checkSku)
                    {
                        InsertFailLog("該機種或機種版本不存在C_SKU", SKUNO, SKUVERSION, "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }

                    if (RWT == null)
                    {
                        InsertFailLog("該工單前綴不存在", WO, WO.Substring(0, 6), "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }

                    if (TRWH.CheckWoHeadByWo(WO, true, "", SFCDB, DB_TYPE_ENUM.Oracle))
                    {
                        InsertFailLog("該工單已存在R_WO_HEADER", WO, "", "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }

                    if (TRWB.CheckDataExist(WO, SFCDB))
                    {
                        InsertFailLog("該工單已存在R_WO_BASE", WO, "", "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }

                    try
                    {
                        Row_R_WO_HEADER rowRWH = null;
                        Row_R_WO_ITEM rowRWI = null;
                        rowRWH = (Row_R_WO_HEADER)TRWH.NewRow();
                        rowRWH.ID = TRWH.GetNewID(BU, SFCDB);
                        rowRWH.AUFNR = WO;
                        rowRWH.MATNR = SKUNO;
                        rowRWH.REVLV = SKUVERSION;
                        rowRWH.GAMNG = QTY + ".000";
                        rowRWH.AUART = RWT.ORDER_TYPE;
                        rowRWH.GSTRS = TRWH.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                        rowRWH.ERDAT = TRWH.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                        rowRWH.GLTRS = TRWH.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");
                        rowRWH.FTRMI = TRWH.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");

                        if (itemList.Count > 0)
                        {
                            foreach (var item in itemList)
                            {
                                rowRWI = (Row_R_WO_ITEM)TRWI.NewRow();
                                rowRWI.ID = TRWH.GetNewID(BU, SFCDB);
                                rowRWI.AUFNR = WO;
                                rowRWI.AUART = RWT.ORDER_TYPE;
                                rowRWI.MATNR = item.KP_PARTNO;
                                rowRWI.REVLV = SKUVERSION;
                                rowRWI.BAUGR = SKUNO;
                                SFCDB.ExecSQL(rowRWI.GetInsertString(DBTYPE));
                            }

                        }
                        SFCDB.ExecSQL(rowRWH.GetInsertString(DBTYPE));

                        successCount += 1;
                    }
                    catch (Exception ex)
                    {
                        InsertFailLog(ex.Message, WO, "", "UploadExcelWO", SFCDB);
                        failCount += 1;
                        continue;
                    }

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = failCount;
                StationReturn.Message = $@"配置數據成功[{successCount.ToString()}]筆,失敗[{failCount.ToString()}]筆!";
                SFCDB.CommitTrain();
            }
            catch (Exception e)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void ShowFailRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string FUNCTION_NAME = Data["FUNCTION_NAME"].ToString();
            string SearchTime = GetDBDateTime().AddSeconds(-8).ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TRML.GetMESLog("CloudMES", "MESStation.Config.ConvertWorkorder", FUNCTION_NAME, SearchTime, "", LoginUser.EMP_NO.ToString(), DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        private void InsertFailLog(string LOG_MESSAGE, string Data1, string Data2, string FUNCTION_NAME, MESDBHelper.OleExec DB)
        {
            T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DBTYPE);
            R_MES_LOG LOG = new R_MES_LOG
            {
                ID = TRML.GetNewID(BU, DB),
                DATA1 = Data1,
                DATA2 = Data2,
                FUNCTION_NAME = FUNCTION_NAME,
                CLASS_NAME = "MESStation.Config.ConvertWorkorder",
                PROGRAM_NAME = "CloudMES",
                EDIT_TIME = GetDBDateTime(),
                EDIT_EMP = LoginUser.EMP_NO,
                LOG_MESSAGE = LOG_MESSAGE
            };
            TRML.InsertMESLog(LOG, DB);
        }

        public void GetWoConvertListJuniper(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            try
            {
                string pono = Data["PONO"].ToString().ToUpper();
                string prewo = Data["PREWO"].ToString().ToUpper();
                string pid = Data["PID"].ToString().ToUpper();
                oleDB = DBPools["SFCDB"].Borrow();
                string sqlPono = "", sqlPrewo = "", sqlPid = "";
                if (!string.IsNullOrEmpty(pono))
                {
                    sqlPono = $@" and a.pono like '%{pono}%'";
                }
                if (!string.IsNullOrEmpty(prewo))
                {
                    sqlPrewo = $@" and a.prewo like '%{prewo}%'";
                }
                if (!string.IsNullOrEmpty(pid))
                {
                    sqlPid = $@" and a.pid like '%{pid}%'";
                }
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
                                                           and a.itemid=c.id {sqlPono} {sqlPrewo} {sqlPid} ) A )AA   order by AA.convertdate,AA.pid,AA.pono,AA.poline asc";
                dt = oleDB.ExecuteDataTable(sql, CommandType.Text);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception ex)
            {
                if (oleDB != null)
                {
                    DBPools["SFCDB"].Return(oleDB);
                }
                throw ex;
            }
        }



    }
}
