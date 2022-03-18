
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config.VN
{
    public class SKUPackoutSizeConfig : MesAPIBase
    {
        protected APIInfo FGetMappingList = new APIInfo()
        {
            FunctionName = "GetMappingList",
            Description = "Get pack size setting",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewMapping = new APIInfo()
        {
            FunctionName = "AddNewMapping",
            Description = "add pack size",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PALLETSIZE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CARTONSIZE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BOXSIZE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEditNewMapping = new APIInfo()
        {
            FunctionName = "EditNewMapping",
            Description = "edit pack size",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PALLETSIZE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CARTONSIZE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BOXSIZE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public SKUPackoutSizeConfig()
        {
            this.Apis.Add(FGetMappingList.FunctionName, FGetMappingList);
            this.Apis.Add(FAddNewMapping.FunctionName, FAddNewMapping);
            this.Apis.Add(FEditNewMapping.FunctionName, FEditNewMapping);
        }

        public void GetMappingList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_PACKING_SIZE tcp = null;
            OleExec sfcdb = null;
            var sku = Data["SKUNO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                tcp = new T_C_PACKING_SIZE(sfcdb, DB_TYPE_ENUM.Oracle);

                List<C_PACKING_SIZE> packingSizeList = new List<C_PACKING_SIZE>();
                if (string.IsNullOrEmpty(sku))
                {
                    packingSizeList = sfcdb.ORM.Queryable<C_PACKING_SIZE>().OrderBy(t => t.SKUNO).OrderBy(t => t.EDIT_TIME).ToList();
                }
                else
                {
                    packingSizeList = sfcdb.ORM.Queryable<C_PACKING_SIZE>().Where(t => t.SKUNO == sku).OrderBy(t => t.SKUNO).OrderBy(t => t.EDIT_TIME).ToList();
                }
                if (packingSizeList.Count > 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = packingSizeList;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }
        public void AddNewMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_PACKING_SIZE tcp = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                bool bol = false;
                string sku = Data["SKUNO"].ToString().Trim().ToUpper();
                string palletSize = Data["PALLETSIZE"].ToString().Trim().ToUpper();
                string cartonSize = Data["CARTONSIZE"].ToString().Trim().ToUpper();
                string boxSize = Data["BOXSIZE"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tcp = new T_C_PACKING_SIZE(sfcdb, DB_TYPE_ENUM.Oracle);
                bol = sfcdb.ORM.Queryable<C_PACKING_SIZE>().Any(t => t.SKUNO == sku);
                if (bol)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { sku + "---" }));
                }
                Row_C_PACKING_SIZE rowPack= (Row_C_PACKING_SIZE)tcp.NewRow();
                rowPack.ID = tcp.GetNewID(this.BU, sfcdb);
                rowPack.SKUNO = sku;
                rowPack.PALLET_SIZE = palletSize;
                rowPack.CARTON_SIZE = cartonSize;
                rowPack.BOX_SIZE = boxSize;
                rowPack.EDIT_TIME = GetDBDateTime();
                rowPack.EDIT_EMP = this.LoginUser.EMP_NO;
                result = sfcdb.ExecSQL(rowPack.GetInsertString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000021";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000021";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }
        public void EditNewMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_PACKING_SIZE tcp = null;
            OleExec sfcdb = null;
            try
            {
                bool bol = false;
                string sku = Data["SKUNO"].ToString().Trim().ToUpper();
                string palletSize = Data["PALLETSIZE"].ToString().Trim().ToUpper();
                string cartonSize = Data["CARTONSIZE"].ToString().Trim().ToUpper();
                string boxSize = Data["BOXSIZE"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tcp = new T_C_PACKING_SIZE(sfcdb, DB_TYPE_ENUM.Oracle);
                bol = sfcdb.ORM.Queryable<C_PACKING_SIZE>().Any(t => t.SKUNO == sku);
                if (!bol)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { sku + "---" }));
                }
                sfcdb.ORM.Updateable<C_PACKING_SIZE>()
                    .Where(c => c.SKUNO == sku)
                    .SetColumns(c => new C_PACKING_SIZE {
                        PALLET_SIZE = palletSize,
                        CARTON_SIZE = cartonSize,
                        BOX_SIZE = boxSize,
                        EDIT_EMP=this.LoginUser.EMP_NO,
                        EDIT_TIME= GetDBDateTime()
                    })
                    .ExecuteCommand();
                StationReturn.Message = "";
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
               
            
                
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000021";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally{

                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
