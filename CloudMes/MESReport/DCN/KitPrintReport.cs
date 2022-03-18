using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESReport.DCN
{
    /// <summary>
    /// 用於查詢KIT_PRINT工站打印的條碼
    /// </summary>
    public class KitPrintReport : MesAPIBase
    {
        private APIInfo _GetKitPrint = new APIInfo
        {
            FunctionName = "GetKitPrint",
            Description ="獲取KIT打印條碼",
            Parameters =new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        private APIInfo _DeleteKitPrint = new APIInfo
        {
            FunctionName = "DeleteKitPrint",
            Description = "刪除KIT打印條碼",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo{ InputName = "WO", InputType = "string", DefaultValue = ""},
                new APIInputInfo{ InputName = "SN", InputType = "string", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>() { }
        };

        public KitPrintReport()
        {
            this.Apis.Add(_GetKitPrint.FunctionName, _GetKitPrint);
            this.Apis.Add(_DeleteKitPrint.FunctionName, _DeleteKitPrint);
        }

        public void GetKitPrint(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            string wo = Data["WO"].ToString();
            string skuno = Data["SKUNO"].ToString();
            string isLoad = Data["ISLOAD"].ToString();
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                string sql = $@"
                select a.workorderno as wo,
                       a.skuno,
                       a.workorder_qty as qty,
                       b.sn,
                       case when exists (select 1 from r_sn where sn=b.sn and valid_flag=1) then 'Y' else 'N' end isLoad,
                       b.station_name as station,
                       b.edit_emp,
                       b.edit_time
                  from r_wo_base a, r_sn_station_detail b
                 where a.workorderno = b.workorderno
                   and b.station_name = 'KIT_PRINT'
                   and b.workorderno not like '*%'
                   and b.sn not like '*%'";
                sql += string.IsNullOrEmpty(wo) ? "" : $@" and b.workorderno = '{wo}' ";
                sql += string.IsNullOrEmpty(skuno) ? "" : $@" and a.skuno = '{skuno}' ";
                sql += isLoad.Equals("ALL") ? "" : $@" and case when exists (select 1 from r_sn where sn=b.sn and valid_flag=1) then 'Y' else 'N' end = '{isLoad}' ";
                sql += $@"order by b.sn, b.edit_time";

                DataTable resDT = SFCDB.RunSelect(sql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000016";
                StationReturn.Data = resDT;

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }

        public void DeleteKitPrint(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            Newtonsoft.Json.Linq.JArray woArray = (Newtonsoft.Json.Linq.JArray)Data["WO"];
            Newtonsoft.Json.Linq.JArray snArray = (Newtonsoft.Json.Linq.JArray)Data["SN"];
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                if (!CheckPermission(LoginUser.EMP_NO, SFCDB))
                {
                    DBPools["SFCDB"].Return(SFCDB);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "You have no permission to delete, Please call PE!";
                    return;
                }

                bool isOk = true;
                SFCDB.BeginTrain();

                for (int i = 0; i < snArray.Count; i++)
                {
                    string wo = woArray[i].ToString();
                    string sn = snArray[i].ToString();

                    string sql = $@"
                    update r_sn_station_detail
                       set sn          = '*' || sn,
                           workorderno = '*' || workorderno,
                           edit_emp = '{LoginUser.EMP_NO}',
                           edit_time   = sysdate
                     where station_name = 'KIT_PRINT'
                       and workorderno = '{wo}'
                       and sn = '{sn}'";

                    var result = SFCDB.ExecuteNonQuery(sql, CommandType.Text, null);
                    if (result <= 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000023";
                        StationReturn.MessagePara.Add("WO:" + wo + ", SN:" + sn);
                        isOk = false;
                        break;
                    }
                }

                if (isOk)
                {
                    SFCDB.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    SFCDB.RollbackTrain();
                }

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }

        private bool CheckPermission(string emp_no, OleExec SFCDB)
        {
            string sql = $@"
            select 1
              from c_user a, c_user_role b, c_role c, c_role_privilege d, c_privilege e
             where a.id = b.user_id
               and b.role_id = c.id
               and c.id = d.role_id
               and d.privilege_id = e.id
               and a.dpt_name = 'PE'
               and c.role_name = 'PE'
               and e.privilege_name = 'DELETE_KIT_PRINT'
               and a.emp_no = '{emp_no}'";

            if (SFCDB.RunSelect(sql).Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
}
