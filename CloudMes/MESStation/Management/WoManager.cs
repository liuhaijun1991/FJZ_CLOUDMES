using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Management
{
    public class WoManager: MesAPIBase
    {

        public APIInfo FCloseWorkorder = new APIInfo() {
             FunctionName="CloseWorkOrder",
             Parameters=new List<APIInputInfo> { new APIInputInfo() {  InputName="WorkOrder"} },
             Permissions=new List<MESPermission>(),
             Description= "Close the ticket manually"
        };

        public APIInfo FGetAllWo = new APIInfo()
        {
            FunctionName = "GetAllWo",
            Parameters = new List<APIInputInfo> { new APIInputInfo() { InputName = "SKU" }, new APIInputInfo() { InputName = "WorkOrder" } },
            Permissions = new List<MESPermission>(),
            Description = "Search All Data from Sku or WO"
        };

        public WoManager()
        {
            this.Apis.Add(FCloseWorkorder.FunctionName, FCloseWorkorder);
            this.Apis.Add(FGetAllWo.FunctionName, FGetAllWo);
        }

        public void CloseOrOpenWorkOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string result = string.Empty ;
            T_R_WO_BASE TRWB = null;
            string WorkOrder = string.Empty;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                TRWB = new T_R_WO_BASE(sfcdb, DBTYPE);
                WorkOrder = Data["WorkOrder"].ToString();
                if (TRWB.GetWoByWoNo(WorkOrder, sfcdb) != null)
                {
                    result = TRWB.CloseOrOpenWorkOrder(WorkOrder, sfcdb);

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.MessagePara.Add(WorkOrder);
                    StationReturn.MessagePara.Add(result);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000164";
                    StationReturn.MessagePara.Add(WorkOrder);
                }

                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetAllWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_R_WO_BASE TRWB = null;
            string WorkOrder = string.Empty, SkuNo = string.Empty;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                TRWB = new T_R_WO_BASE(sfcdb, DBTYPE);
                SkuNo = Data["SKU"].ToString();
                WorkOrder = Data["WorkOrder"].ToString();

                List<R_WO_BASE> lstWOWorkOrder = TRWB.GetSkunoByWO(WorkOrder, SkuNo, sfcdb);

                if (lstWOWorkOrder.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = lstWOWorkOrder;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(string.IsNullOrEmpty(WorkOrder)?SkuNo: WorkOrder);
                }

                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000000";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
