using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class FoxconnSkuConfig : MesAPIBase
    {
        protected APIInfo _GetFoxconnSkuMapping = new APIInfo()
        {
            FunctionName= "GetFoxconnSkuMapping",
            Description= "Get all inner sku and foxconn sku mappings",
            Parameters=new List<APIInputInfo>(),
            Permissions=new List<MESPermission>()
        };

        protected APIInfo _GetFoxconnSkuMappingByFuzzySearch = new APIInfo()
        {
            FunctionName = "GetFoxconnSkuMappingByFuzzySearch",
            Description = "Get inner sku and foxconn sku mapping by fuzzy search",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SearchValue", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _DeleteFoxconnSkuMappingByIds = new APIInfo()
        {
            FunctionName= "DeleteFoxconnSkuMappingByIds",
            Description= "Delete inner sku and foxconn sku mapping by ids",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="Ids",InputType="string" }
            },
            Permissions=new List<MESPermission>()
        };

        protected APIInfo _AddFoxconnSkuMapping = new APIInfo()
        {
            FunctionName= "AddFoxconnSkuMapping",
            Description="Add inner sku and foxconn sku mapping",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="InnerSku",InputType="string" },
                new APIInputInfo() {InputName="InnerSkuVer",InputType="string" },
                new APIInputInfo() {InputName="FoxconnSku",InputType="string" },
                new APIInputInfo() {InputName="FoxconnSkuVer",InputType="string" }
            },
            Permissions=new List<MESPermission>()
        };

        protected APIInfo _UpdateFoxconnSkuMapping = new APIInfo()
        {
            FunctionName = "UpdateFoxconnSkuMapping",
            Description = "Update inner sku and foxconn sku mapping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputType="Id",InputName="string" },
                new APIInputInfo() {InputName="InnerSku",InputType="string" },
                new APIInputInfo() {InputName="InnerSkuVer",InputType="string" },
                new APIInputInfo() {InputName="FoxconnSku",InputType="string" },
                new APIInputInfo() {InputName="FoxconnSkuVer",InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _GetReworkSkuMapping = new APIInfo()
        {
            FunctionName = "GetReworkSkuMapping",
            Description = "Get all  Rework sku mappings",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _DeleteReworkSkuMappingByIds = new APIInfo()
        {
            FunctionName = "DeleteReworkSkuMappingByIds",
            Description = "Delete Rework sku mapping by ids",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="Ids",InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _AddReworkSkuMapping = new APIInfo()
        {
            FunctionName = "AddReworkSkuMapping",
            Description = "Add  Rework sku mapping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="Sku1",InputType="string" },
                new APIInputInfo() {InputName="Sku2",InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _UpdateReworkSkuMapping = new APIInfo()
        {
            FunctionName = "UpdateReworkSkuMapping",
            Description = "Update Rework sku mapping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputType="Id",InputName="string" },
                new APIInputInfo() {InputName="Sku1",InputType="string" },
                new APIInputInfo() {InputName="Sku2",InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        public FoxconnSkuConfig()
        {
            this.Apis.Add(_GetFoxconnSkuMapping.FunctionName, _GetFoxconnSkuMapping);
            this.Apis.Add(_GetFoxconnSkuMappingByFuzzySearch.FunctionName, _GetFoxconnSkuMappingByFuzzySearch);
            this.Apis.Add(_DeleteFoxconnSkuMappingByIds.FunctionName, _DeleteFoxconnSkuMappingByIds);
            this.Apis.Add(_AddFoxconnSkuMapping.FunctionName, _AddFoxconnSkuMapping);
            this.Apis.Add(_UpdateFoxconnSkuMapping.FunctionName, _UpdateFoxconnSkuMapping);

            this.Apis.Add(_GetReworkSkuMapping.FunctionName, _GetReworkSkuMapping);
            this.Apis.Add(_DeleteReworkSkuMappingByIds.FunctionName, _DeleteReworkSkuMappingByIds);
            this.Apis.Add(_AddReworkSkuMapping.FunctionName, _AddReworkSkuMapping);
            this.Apis.Add(_UpdateReworkSkuMapping.FunctionName, _UpdateReworkSkuMapping);
        }


        public void GetFoxconnSkuMapping(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_SKU_FOXCONN_MAPPING TCSFM = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCSFM = new T_C_SKU_FOXCONN_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                List<C_SKU_FOXCONN_MAPPING> Mappings = TCSFM.GetAllFoxconnSkuMappings(DB);
                if (Mappings.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = Mappings;
                }

                if (DB != null) DBPools["SFCDB"].Return(DB);
            }
            catch (Exception ex)
            {
                if (DB != null) DBPools["SFCDB"].Return(DB);
                throw ex;
            }
        }

        public void GetReworkSkuMapping(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_CONTROL TCC = null;
            string SearchValue = string.Empty;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                if (Data["SearchValue"] != null)
                {
                    SearchValue= Data["SearchValue"].ToString().ToUpper();
                }
                TCC = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
                List<C_REWORK_SKU_MAPPING> Mappings = TCC.GetReworkSkuMappings(SearchValue,DB);
                
                if (Mappings.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = Mappings;
                }

                if (DB != null) DBPools["SFCDB"].Return(DB);
            }
            catch (Exception ex)
            {
                if (DB != null) DBPools["SFCDB"].Return(DB);
                throw ex;
            }
        }

        public void GetFoxconnSkuMappingByFuzzySearch(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_SKU_FOXCONN_MAPPING TCSFM = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCSFM = new T_C_SKU_FOXCONN_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                string SearchValue = Data["SearchValue"].ToString().ToUpper();
                List<C_SKU_FOXCONN_MAPPING> Mappings = TCSFM.GetFoxconnSkuMappingByCondition(SearchValue, DB);
                if (Mappings.Count > 0)
                {
                    StationReturn.Data = Mappings;
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

        public void DeleteFoxconnSkuMappingByIds(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_SKU_FOXCONN_MAPPING TCSFM = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCSFM = new T_C_SKU_FOXCONN_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                string Ids = Data["Ids"].ToString();
                int result = TCSFM.DeleteFoxconnSkuMappingByIds(Ids, DB);

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

        public void DeleteReworkSkuMappingByIds(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_CONTROL TCC = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCC = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
                string Ids = Data["Ids"].ToString();
                int result = TCC.DeleteReworkSkuMappingByIds(Ids, DB);

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


        public void AddFoxconnSkuMapping(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_SKU_FOXCONN_MAPPING TCSFM = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCSFM = new T_C_SKU_FOXCONN_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                string InnerSku = Data["InnerSku"].ToString();
                string InnerSkuVer = Data["InnerSkuVer"].ToString();
                string FoxconnSku = Data["FoxconnSku"].ToString();
                string FoxconnSkuVer = Data["FoxconnSkuVer"].ToString();

                int result = 0;
                if (DB.ORM.Queryable<C_SKU_FOXCONN_MAPPING>().Any(t => t.SKUNO.Equals(InnerSku) && t.SKU_VER.Equals(InnerSkuVer)))
                {
                    result = TCSFM.UpdateFoxconnSkuMapping(InnerSku, InnerSkuVer, FoxconnSku, FoxconnSkuVer, LoginUser.EMP_NO, DB);
                }
                else
                {
                    result = TCSFM.AddFoxconnSkuMapping(InnerSku,InnerSkuVer,FoxconnSku,FoxconnSkuVer, LoginUser.BU, LoginUser.EMP_NO, DB);
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

        public void AddReworkSkuMapping(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_CONTROL TCC = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCC = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
                string Sku1 = Data["Sku1"].ToString();
                string Sku2 = Data["Sku2"].ToString();

                int result = 0;
  
                
                result = TCC.SaveReworkSkuMapping(Sku1, Sku2, LoginUser.BU,LoginUser.EMP_NO, DB);


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

        public void UpdateFoxconnSkuMapping(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_SKU_FOXCONN_MAPPING TCSFM = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCSFM = new T_C_SKU_FOXCONN_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                string Id = Data["Id"].ToString();
                string FoxconnSku = Data["FoxconnSku"].ToString();
                string FoxconnSkuVer = Data["FoxconnSkuVer"].ToString();
                int result = TCSFM.UpdateFoxconnSkuMappingById(Id, FoxconnSku, FoxconnSkuVer, LoginUser.EMP_NO, DB);


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
        public void UpdateReworkSkuMapping(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            T_C_CONTROL TCC = null;
            try
            {
                DB = DBPools["SFCDB"].Borrow();
                TCC = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
                string Id = Data["Id"].ToString();
                string Sku1 = Data["Sku1"].ToString();
                string Sku2 = Data["Sku2"].ToString();
                int result = TCC.UpdateReworkSkuMappingById(Id, Sku1, Sku2, LoginUser.EMP_NO, DB);


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
    }
}
