using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;

namespace MESStation.Config
{
    public class CRMADetail:MesAPIBase
    {
        protected APIInfo FGetRMACheckDetail = new APIInfo()
        {
            FunctionName = "GetRMACheckDetail",
            Description = "Get RMACheck Detail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FOBARMACheckSubmit = new APIInfo()
        {
            FunctionName = "OBARMACheckSubmit",
            Description = "OBA RMA Check Submit",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CRMADetail()
        {
            this.Apis.Add(FGetRMACheckDetail.FunctionName, FGetRMACheckDetail);
            this.Apis.Add(FOBARMACheckSubmit.FunctionName, FOBARMACheckSubmit);
        }


        public void GetRMACheckDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {           
            OleExec sfcdb = null;
            try
            {
                if (Data["SN"] == null || Data["SN"].ToString() == "")
                {
                    throw new Exception("Please input sn");
                }
                string sn = Data["SN"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_RMA_DETAIL TRRD = new T_R_RMA_DETAIL(sfcdb, DBTYPE);
                T_R_SN TRS = new T_R_SN(sfcdb, DBTYPE);
                R_SN objSN = TRS.LoadData(sn, sfcdb);
                if (objSN == null)
                {
                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000048";
                    StationReturn.MessagePara.Add(sn);
                    return;
                }
                R_RMA_DETAIL objRMA = TRRD.GetObjBySN(sfcdb, sn);

                if (objRMA != null)
                {
                    if (objRMA.DATA1 != "1")
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "此RMASN沒有維修確認動作,請聯繫RMA人員";
                        StationReturn.MessageCode = "";
                        StationReturn.Data = "";
                        return;
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(1);
                    StationReturn.Data = objRMA;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "此RMASN沒有維修確認動作,請聯繫RMA人員";
                    StationReturn.MessageCode = "";
                    StationReturn.Data = "";
                }
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception exception)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                return;
            }
        }
        /// <summary>
        /// OBA 工站RMACheck提交
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void OBARMACheckSubmit(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {           
            OleExec sfcdb = null;            
            try
            {
                if (Data["SN"] == null || Data["SN"].ToString() == "")
                {
                    throw new Exception("Please input sn");
                }
                string sn = Data["SN"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_RMA_DETAIL TRRD = new T_R_RMA_DETAIL(sfcdb, DBTYPE);
                T_R_SN TRS = new T_R_SN(sfcdb, DBTYPE);
                R_SN objSN = TRS.LoadData(sn, sfcdb);
                if (objSN == null)
                {
                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000048";
                    StationReturn.MessagePara.Add(sn);
                    return;
                }
                //把這個SN所在的棧板里所有在R_RMA_DETAIL 的SN都update
                string sql = $@"update r_rma_detail set data1='2' where sn in (
                                select sn from r_sn where id in (
                                select sn_id from r_sn_packing where pack_id in (
                                select id from r_packing where parent_pack_id in (
                                select parent_pack_id from r_packing where id in (
                                select pack_id from r_sn_packing where sn_id='{objSN.ID}')))))";
                sfcdb.ExecSQL(sql);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = "";
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception exception)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                return;
            }
        }
    }
}
