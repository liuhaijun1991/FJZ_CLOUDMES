using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;
using MESPubLab.MESStation.SNMaker;
using System.Threading;

namespace MESStation.Test
{
    public class NNDoorCard : MESPubLab.MESStation.MesAPIBase
    {
        protected APIInfo FGET_ICIVENT_LOCK_DATA = new APIInfo()
        {
            FunctionName = "GET_ICIVENT_LOCK_DATA",
            Description = "FGET_ICIVENT_LOCK_DATA",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "ALL" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSET_ICIVENT_LOCK_DISABLE = new APIInfo()
        {
            FunctionName = "SET_ICIVENT_LOCK_DISABLE",
            Description = "FSET_ICIVENT_LOCK_DISABLE",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESC", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        public NNDoorCard()
        {
            this.Apis.Add(FGET_ICIVENT_LOCK_DATA.FunctionName, FGET_ICIVENT_LOCK_DATA);
            this.Apis.Add(FSET_ICIVENT_LOCK_DISABLE.FunctionName, FSET_ICIVENT_LOCK_DISABLE);

        }
        public void SET_ICIVENT_LOCK_DISABLE(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec db = null;
            try
            {
                var EMP_NO = Data["EMP_NO"].ToString();
                var DESC = Data["DESC"].ToString();
                // OleExec db = new OleExec("Data Source = 10.120.198.107:1527 / nnhr; User ID = apcardtime; Password = apcardtime123!");
                db = new OleExec("Data Source = 10.120.198.107:1527 / nnhr; User ID = apcardtime; Password = apcardtime123!");
                //var a = db.ExecSelect("select * from cqhr.icivet_lock where lock_state = 1");
                //StationReturn.Data = a.Tables[0];
                var strSQL = $@"update cqhr.icivet_lock set lock_state = 0 , UNLOCK_TIME = sysdate , UNLOCK_EMP='{LoginUser.EMP_NO}'
                                ,UNLOCK_DESC='{DESC.Replace("'","''")}' where EMP_NO='{EMP_NO}'";
                db.ExecSQL(strSQL);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception ee)
            {
                StationReturn.Data = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "ERROR:" + ee.Message;
            }
            finally
            {
                if (db != null)
                {
                    db.CloseMe();
                }
            }

        }
        public void GET_ICIVENT_LOCK_DATA(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec db = null;
            try
            {
               // OleExec db = new OleExec("Data Source = 10.120.198.107:1527 / nnhr; User ID = apcardtime; Password = apcardtime123!");
                db = new OleExec("Data Source = 10.120.198.107:1527 / nnhr; User ID = apcardtime; Password = apcardtime123!");
                var a = db.ExecSelect("select * from cqhr.icivet_lock where lock_state = 1");
                StationReturn.Data = a.Tables[0];
                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception ee)
            {
                StationReturn.Data = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "ERROR:"+ee.Message;
            }
            finally
            {
                if (db != null)
                {
                    db.CloseMe();
                }

            }

        }
    }
}
