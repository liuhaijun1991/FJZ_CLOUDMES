using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESPubLab;
using MESStation.Interface.SAPRFC;
using MES_DCN.Broadcom;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using System.IO;
using System.Reflection;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.Json;

namespace MESStation.Config
{
    public class DebugConfig : MesAPIBase
    {
        protected APIInfo FProJnpPreWoHead = new APIInfo()
        {
            FunctionName = "ProJnpPreWoHead",
            Description = "ProJnpPreWoHead",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetJsonByName = new APIInfo()
        {
            FunctionName = "GetJsonByName",
            Description = "GetJsonByName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },                
            },
            Permissions = new List<MESPermission>() { }
        };

        public DebugConfig()
        {
            this.Apis.Add(FProJnpPreWoHead.FunctionName, FProJnpPreWoHead);
            this.Apis.Add(FGetJsonByName.FunctionName, FGetJsonByName);
        }

        public void ProJnpPreWoHead(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                var plantCode = "VUEA";
                oleDB = this.DBPools["SFCDB"].Borrow();
                var targetlist = oleDB.ORM.Queryable<R_PRE_WO_HEAD>().ToList();
                foreach (var item in targetlist)
                {
                    var skunewplant = oleDB.ORM.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                    item.PLANT = skunewplant != null ? skunewplant.PLANTCODE : plantCode;
                    var agile = oleDB.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item.PID).OrderBy(t => t.RELEASE_DATE, OrderByType.Desc).ToList().FirstOrDefault();
                    item.CUSTPN = agile?.CUSTPARTNO;
                    oleDB.ORM.Updateable(item).ExecuteCommand();
                }


                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = "OK";
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
        
        public void GetJsonByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var id = Data["ID"].ToString();
                var res = JsonSave.GetFromDB(id, oleDB);
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

    }
}
