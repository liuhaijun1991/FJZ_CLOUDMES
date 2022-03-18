using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESJuniper.Api
{
    public class SubModelMapping: MesAPIBase
    {
        protected APIInfo FGetList = new APIInfo
        {
            FunctionName = "GetList",
            Description = "GetList",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAdd = new APIInfo
        {
            FunctionName = "Add",
            Description = "Add",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="R_MODELSUBPN_MAP", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDelete = new APIInfo
        {
            FunctionName = "Delete",
            Description = "Delete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="IDS", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FEdit = new APIInfo
        {
            FunctionName = "Edit",
            Description = "Edit",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="R_MODELSUBPN_MAP", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        public SubModelMapping() {
            this.Apis.Add(FGetList.FunctionName, FGetList);
            this.Apis.Add(FAdd.FunctionName, FAdd);
            this.Apis.Add(FDelete.FunctionName, FDelete);
        }
        public void GetList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Queryable<R_MODELSUBPN_MAP>()
                    .ToList();
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void Add(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var model = Data["R_MODELSUBPN_MAP"].ToObject<R_MODELSUBPN_MAP>();
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    model.ID = MesDbBase.GetNewID<R_MODELSUBPN_MAP>(SFCDB.ORM, this.BU);
                    model.CREATETIME = DateTime.Now;
                    SFCDB.ORM.Insertable(model).ExecuteCommand();                    
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void Delete(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var ids = Data["IDS"].ToObject<string[]>();
                var datalist = SFCDB.ORM.Queryable<R_MODELSUBPN_MAP>().Where(t => ids.Contains(t.ID)).ToList();
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    SFCDB.ORM.Deleteable(datalist).ExecuteCommand();
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void Edit(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var model = Data["R_MODELSUBPN_MAP"].ToObject<R_MODELSUBPN_MAP>();
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    model.CREATETIME = DateTime.Now;
                    SFCDB.ORM.Updateable(model).ExecuteCommand();
                });
                if (res.IsSuccess)
                    StationReturn.Status = StationReturnStatusValue.Pass;
                else
                    throw new Exception(res.ErrorMessage);
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

    }
}
