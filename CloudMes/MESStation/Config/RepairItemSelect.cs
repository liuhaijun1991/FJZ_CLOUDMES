using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESDBHelper;
using System.Text.RegularExpressions;

namespace MESStation.Config
{
  public  class RepairItemSelect : MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetRepairItems = new APIInfo()
        {
            FunctionName = "GetRepairItems",
            Description = "獲取C_REPAIR_ITEMS的維修大項信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ItemName" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetRepairItemsSon = new APIInfo()
        {
            FunctionName = "GetRepairItemsSon",
            Description = "獲取C_REPAIR_ITEMS_SON的維修小項信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName= "ItemSon" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetRepairActionList = new APIInfo()
        {
            FunctionName = "GetRepairActionList",
            Description = "Get Repair Action List",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()
        };       
        #endregion 方法信息集合 end

        public RepairItemSelect()
        {
            this.Apis.Add(FGetRepairItems.FunctionName, FGetRepairItems);
            this.Apis.Add(FGetRepairItemsSon.FunctionName, FGetRepairItemsSon);
        }


        /// <summary>
        ///獲取C_REPAIR_ITEMS的維修大項信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairItems(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ITEM_NAME = Data["ItemName"].ToString(); ;
                List<string> RepairItemsList = new List<string>();
                T_C_REPAIR_ITEMS TC_REPAIR_ITEM = new T_C_REPAIR_ITEMS(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                RepairItemsList = TC_REPAIR_ITEM.GetRepairItemsList(ITEM_NAME, sfcdb);
                StationReturn.Data = RepairItemsList;
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
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }


        // <summary>
        ///獲取C_REPAIR_ITEMS_SON的維修小項信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairItemsSon(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ITEMS_SON = Data["ItemSon"].ToString();
                List<string> RepairItemsSonList = new List<string>();
                T_C_REPAIR_ITEMS_SON TC_REPAIR_ITEM_SON = new T_C_REPAIR_ITEMS_SON(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_REPAIR_ITEMS RepairItems = new T_C_REPAIR_ITEMS(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_REPAIR_ITEMS RowItems;
                RowItems = RepairItems.GetIDByItemName(ITEMS_SON, sfcdb);
                RepairItemsSonList = TC_REPAIR_ITEM_SON.GetRepairItemsSonList(RowItems.ID, sfcdb);
                StationReturn.Data = RepairItemsSonList;
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
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// Get All Repair Action Code
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairActionList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<string> listAction = new List<string>() { "" };
                List<string> list = sfcdb.ORM.Queryable<C_ACTION_CODE>().Select(r => r.ACTION_CODE).ToList().Distinct().ToList();
                listAction.AddRange(list);
                StationReturn.Data = listAction;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {               
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        /// <summary>
        /// Get Panel Fail Code List For Scan Fail
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetPanelFailCodeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string fail_station = Data["FailStation"].ToString().Trim();
                string category = "";
                List<string> listAction = new List<string>() { "" };
                List<string> list = new List<string>();
                if (!string.IsNullOrEmpty(fail_station))
                {
                    Regex re = new Regex(@"[a-zA-Z]+");
                    Match station = re.Match(fail_station);
                    if (!string.IsNullOrEmpty(station.Value))
                    {
                        category = "PANEL_" + station.Value.ToUpper();
                        list = sfcdb.ORM.Queryable<C_ERROR_CODE>().Where(r => SqlSugar.SqlFunc.Contains(r.ERROR_CATEGORY, category)).Select(r => r.ERROR_CODE).ToList().Distinct().ToList();
                    }                    
                }
                if (list.Count > 0)
                {
                    listAction.AddRange(list);
                }
                StationReturn.Data = listAction;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }

   
}
