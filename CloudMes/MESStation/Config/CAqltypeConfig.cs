using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class CAqltypeConfig : MesAPIBase
    {
        protected APIInfo FAddCAqltype = new APIInfo()
        {
            FunctionName = "AddCAqltype",
            Description = "添加CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "GL_LEVEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SAMPLE_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ACCEPT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REJECT_QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCAqltype = new APIInfo()
        {
            FunctionName = "DeleteCAqltype",
            Description = "刪除CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateCAqltype = new APIInfo()
        {
            FunctionName = "UpdateCAqltype",
            Description = "更新CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "GL_LEVEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SAMPLE_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ACCEPT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REJECT_QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectCAqltype = new APIInfo()
        {
            FunctionName = "SelectCAqltype",
            Description = "查询CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllCAqltype = new APIInfo()
        {
            FunctionName = "GetAllCAqltype",
            Description = "查询AllCAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllAqlName = new APIInfo()
        {
            FunctionName = "GetAllAqlName",
            Description = "查询GetAllAqlName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddSkuAql = new APIInfo()
        {
            FunctionName = "AddSkuAql",
            Description = "AddSkuAql",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SkuId", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AqlType", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "aqlLevel", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAqlLevel = new APIInfo()
        {
            FunctionName = "GetAqlLevel",
            Description = "GetAqlLevel",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSkuAqlData = new APIInfo()
        {
            FunctionName = "GetSkuAqlData",
            Description = "取機種AQL詳細數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SkuId", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSkuAql = new APIInfo()
        {
            FunctionName = "GetSkuAql",
            Description = "取機種對應AQLTYPE",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SkuId", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CAqltypeConfig()
        {
            this.Apis.Add(FAddCAqltype.FunctionName, FAddCAqltype);
            this.Apis.Add(FDeleteCAqltype.FunctionName, FDeleteCAqltype);
            this.Apis.Add(FUpdateCAqltype.FunctionName, FUpdateCAqltype);
            this.Apis.Add(FSelectCAqltype.FunctionName, FSelectCAqltype);
            this.Apis.Add(FGetAllCAqltype.FunctionName, FGetAllCAqltype);
            this.Apis.Add(FGetAllAqlName.FunctionName, FGetAllAqlName);
            this.Apis.Add(FAddSkuAql.FunctionName, FAddSkuAql);
            this.Apis.Add(FGetAqlLevel.FunctionName, FGetAqlLevel);
            this.Apis.Add(FGetSkuAqlData.FunctionName, FGetSkuAqlData);
            this.Apis.Add(FGetSkuAql.FunctionName, FGetSkuAql);
        }

        public void GetSkuAqlData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = cAqultype.GetAqlTypeBySkunoId(Data["SkuId"].ToString(), sfcdb); ;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetSkuAql(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_AQL tCSkuAql = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                tCSkuAql = new T_C_SKU_AQL(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = tCSkuAql.GetSkuAqlBySkuId(sfcdb, Data["SkuId"].ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetAqlLevel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = cAqultype.GetAqlLevelByType(Data["AQLTYPE"].ToString(), sfcdb); ;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void AddSkuAql(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_AQL tCSkuAql = null;
            OleExec sfcdb = null;
            T_C_SKU tCSku = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                tCSkuAql = new T_C_SKU_AQL(sfcdb, DB_TYPE_ENUM.Oracle);
                tCSku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                SkuObject sku = tCSku.GetSkuByID(Data["SkuId"].ToString().Trim(), sfcdb);
                if (sku==null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000245";
                    StationReturn.MessagePara = new List<object>() { Data["SKUNO"].ToString().Trim() };
                    return;
                }
                tCSkuAql.DeleteBySkuno(sfcdb, sku.SkuNo);
                Row_C_SKU_AQL r = (Row_C_SKU_AQL)tCSkuAql.NewRow();
                r.ID = tCSkuAql.GetNewID(this.BU, sfcdb);
                r.SKUNO = sku.SkuNo;
                r.AQLTYPE = Data["AqlType"].ToString().Trim();
                r.DEFAULLEVEL = Data["aqlLevel"].ToString().Trim();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();


                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetAllAqlName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = cAqultype.GetAql(sfcdb); ;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void AddCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_AQLTYPE r = (Row_C_AQLTYPE)cAqultype.NewRow();
                r.ID = cAqultype.GetNewID(this.BU, sfcdb);
                r.AQL_TYPE = (Data["AQL_TYPE"].ToString()).Trim();
                r.LOT_QTY = Convert.ToDouble((Data["LOT_QTY"].ToString()).Trim());
                r.GL_LEVEL = (Data["GL_LEVEL"].ToString()).Trim();
                r.SAMPLE_QTY = Convert.ToDouble((Data["SAMPLE_QTY"].ToString()).Trim());
                r.ACCEPT_QTY = Convert.ToDouble((Data["ACCEPT_QTY"].ToString()).Trim());
                r.REJECT_QTY = Convert.ToDouble((Data["REJECT_QTY"].ToString()).Trim());
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                Row_C_AQLTYPE r = (Row_C_AQLTYPE)cAqultype.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                string strRet= sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MES00000004"; 
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void UpdateCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_AQLTYPE r = (Row_C_AQLTYPE)cAqultype.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                r.AQL_TYPE = (Data["AQL_TYPE"].ToString()).Trim();
                r.LOT_QTY = Convert.ToDouble((Data["LOT_QTY"].ToString()).Trim());
                r.GL_LEVEL = (Data["GL_LEVEL"].ToString()).Trim();
                r.SAMPLE_QTY = Convert.ToDouble((Data["SAMPLE_QTY"].ToString()).Trim());
                r.ACCEPT_QTY = Convert.ToDouble((Data["ACCEPT_QTY"].ToString()).Trim());
                r.REJECT_QTY = Convert.ToDouble((Data["REJECT_QTY"].ToString()).Trim());
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814163301";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void GetAllCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cSkuDetail = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_AQLTYPE> list = cSkuDetail.GetAllAql(sfcdb);

                if (list.Count > 0)
                {            
                   StationReturn.MessageCode = "MSGCODE20210814161629";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void SelectCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cSkuDetail = null;
            OleExec sfcdb = null;
            List<C_AQLTYPE> list = new List<C_AQLTYPE>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                if (Data["AQL_TYPE"].ToString()!="")
                    list = cSkuDetail.GetAqlBySkuno(Data["AQL_TYPE"].ToString().Trim(), sfcdb);
                else
                    list = cSkuDetail.GetAllAql(sfcdb);

                if (list.Count > 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629"; 
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
    }
}
