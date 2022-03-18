using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.DCN;
using MESDataObject;
using System.Data;

namespace MESStation.Config
{
    class Oba_Control : MesAPIBase
    {
        protected APIInfo FDeleteObaControlDetail = new APIInfo()
        {
            FunctionName = "DeleteObaControlDetail",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        

        protected APIInfo FOBA_ControlNew = new APIInfo()
        {
            FunctionName = "OBA_ControlNew",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        
        protected APIInfo FGetObaControlList = new APIInfo()
        {
            FunctionName = "GetObaControlList",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };


        public Oba_Control()
        {
            this.Apis.Add(FDeleteObaControlDetail.FunctionName, FDeleteObaControlDetail);
            this.Apis.Add(FOBA_ControlNew.FunctionName, FOBA_ControlNew);
            this.Apis.Add(FGetObaControlList.FunctionName, FGetObaControlList);

        }
        public void DeleteObaControlDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string ID = Data["ID"].ToString().Trim();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                sfcdb.ORM.Deleteable<R_OBA_CONTROL>().Where(o => o.ID == ID).ExecuteCommand();

                StationReturn.Message = "刪除成功";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void OBA_ControlNew(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string Phase = Data["Phase"].ToString().Trim();
            string SKUNO = Data["SKUNO"].ToString().Trim();
            string QTY = Data["QTY"].ToString();
            DateTime ControlDate =Convert.ToDateTime(Data["ControlDate"].ToString());
            string Address = Data["Address"].ToString();
            string Lotno ="V"+DateTime.Now.ToString("yyyyMMddHHmmss");
            Newtonsoft.Json.Linq.JArray idArray = (Newtonsoft.Json.Linq.JArray)Data["SerialNo"];
            if (Address.Length > 200)
            {
                StationReturn.Message = $@"地址長度大於200，請精簡！！";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
                return;
            }

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
            
                if (idArray.Count.ToString() != QTY)
                {
                    StationReturn.Message = $@"輸入的數量與實際SN數量不一致！！";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                T_R_OBA_CONTROL roc = new T_R_OBA_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                for (int i=0;i< idArray.Count; i++)
                {
                    string STRSN = idArray[i].ToString();
            
                    bool checkExist = sfcdb.ORM.Queryable<R_OBA_CONTROL>()
                   .Where(o => o.SN == STRSN && o.CONTROL_DATE == ControlDate ).Any();
                    if (checkExist)
                    {
                        StationReturn.Message = $@"序列號:{STRSN}--->管控失敗：該產品已經存在當天管控！！!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                    var checkoba = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == STRSN && t.STATION_NAME.Contains("OBA") && t.VALID_FLAG == "1").ToList();
                    if (checkoba.Count > 0)
                    {
                        StationReturn.Message = $@"序列號:{STRSN}--->管控失敗：該產品已經過了OBA！！!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
             
                    var checkroute = sfcdb.ORM.Queryable<R_SN, C_ROUTE_DETAIL>((r, c) => r.ROUTE_ID == c.ROUTE_ID).Where((r, c) => r.SN == STRSN && c.STATION_NAME.Contains("OBA")).Select((r, c) => c).ToList();
                    if (checkroute.Count == 0)
                    {
                        StationReturn.Message = $@"序列號:{STRSN}--->管控失敗：SN路由裡沒有OBA工站！！!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                    var checksn = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == STRSN && t.SKUNO == SKUNO && t.VALID_FLAG == "1").ToList();
                    if (checksn.Count == 0)
                    {
                        StationReturn.Message = $@"序列號:{STRSN}--->管控失敗：SN機種與輸入的機種：{SKUNO}不一致或SN不存在，請確認！！!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }

                    sfcdb.ORM.Insertable<R_OBA_CONTROL>(new R_OBA_CONTROL()
                    {
                        ID = roc.GetNewID(BU, sfcdb),
                        LOTNO = Lotno,
                        PHASE = Phase,
                        SKUNO = SKUNO,
                        SN = STRSN,
                        ADDRESS = Address,
                        CONTROL_DATE = DateTime.Now,
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_DATE = DateTime.Now
                    }).ExecuteCommand();

                    StationReturn.Message = "新增成功";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";

                }
               
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        
        public void GetObaControlList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MenuData = sfcdb.ORM.Queryable<R_OBA_CONTROL>().OrderBy(o => o.EDIT_DATE, SqlSugar.OrderByType.Desc).ToList();
                StationReturn.Data = MenuData;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
