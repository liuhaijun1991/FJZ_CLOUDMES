using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    class Lltconfig : MesAPIBase
    {
        protected APIInfo FSearch = new APIInfo()
        {
            FunctionName = "Search",
            Description = "Search",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "STATUS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDelete = new APIInfo()
        {
            FunctionName = "Delete",
            Description = "Delete",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ROW_OBJECT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDownload = new APIInfo()
        {
            FunctionName = "Download",
            Description = "Download",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "TYPE", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "SEARCH_TEXT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSeleteValue = new APIInfo()
        {
            FunctionName = "GetSeleteValue",
            Description = "GetSeleteValue",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        public Lltconfig()
        {
            this.Apis.Add(FSearch.FunctionName, FSearch);
            this.Apis.Add(FDelete.FunctionName, FDelete);
            this.Apis.Add(FDownload.FunctionName, FDownload);
            this.Apis.Add(FGetSeleteValue.FunctionName, FGetSeleteValue);
        }

        public void GetSeleteValue(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string total_sql = $@"select aa.ID,aa.SKUNO,aa.WORKORDERNO ,aa.SN ,aa.STATUS ,aa.LOCATION,aa.CREATETIME ," +
                    $"bb.skuno TOPSKU, bb.workorderno TOPWO, bb.SN TOPSN, bb.next_station STATION from(" +
                    $" select a.id, b.skuno, b.workorderno, a.sn, a.station, a.createtime, 'WAIT_TEST' status," +
                    $"case  when b.next_station='SHIPOUT' THEN '已入庫)' when b.next_station IN('SHIPFINISH' , 'JOBFINISH') then '已出貨(已綁定)' else '未入庫' end as LOCATION" +
                    $" from r_llt a, r_sn b where a.r_sn_id = b.id and b.valid_flag=1)aa" +
                    $" left join(select a.value, a.SN, b.skuno, b.workorderno, b.next_station from r_sn_kp a, r_sn b" +
                    $" where a.value = b.sn and b.valid_flag=1)bb on aa.sn = bb.value";
                DataTable total_dt = SFCDB.ExecSelect(total_sql, null).Tables[0];
                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");
                }

                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.Data = new { Total = total_dt.Rows.Count, Rows = total_dt };
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Search(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string total_sql = string.Empty;
                string temp_sql = string.Empty;
                string temp1_sql = string.Empty;
                string strstatus = string.Empty;
                string Inoutdata = Data["SEARCH_TEXT"].ToString();
                string flag = Data["STATUS"].ToString();
                switch (flag)
                {
                    case "0":
                        strstatus = " case  when aa.STATUS='0' then 'WAIT_TEST' end STATUS";
                        break;
                    case "1":
                        strstatus = " case  when aa.STATUS='1' then 'ON_TEST' end STATUS";
                        break;
                    case "2":
                        strstatus = " case  when aa.STATUS='2' then 'OFF_TEST' end STATUS";
                        break;
                    case "3":
                        strstatus = " case  when aa.STATUS='3' then 'CANCEL' end STATUS";
                        break;
                    default:
                        strstatus = "case when aa.STATUS = '0' then 'WAIT_TEST' when aa.STATUS = '1' then 'ON_TEST' when aa.STATUS = '2' then 'OFF_TEST' when aa.STATUS = '3' then 'CANCEL' end STATUS";
                        break;
                }
                total_sql=$@"select distinct aa.ID,aa.SKUNO,aa.WORKORDERNO ,aa.SN ,";

                total_sql = total_sql + strstatus+ ",aa.SHOULDTEST,cc.TotalTIME,aa.SHOULDTEST-cc.TotalTIME Remaintime,dd.lock_time,round(sysdate-dd.lock_time) LOCKDAY, aa.LOCATION,aa.CREATETIME ,bb.skuno TOPSKU, bb.workorderno TOPWO, bb.SN TOPSN, aa.CURRENT_STATION ,aa.NEXT_STATION from(" +
                            " select a.id, b.skuno, b.workorderno, a.sn, a.station, a.createtime,'1440' SHOULDTEST, a.status,case  when b.next_station='SHIPOUT' THEN '已入庫)' when b.next_station IN('SHIPFINISH' , 'JOBFINISH') then '已出貨(已綁定)' else '未入庫' end as LOCATION ,b.CURRENT_STATION,b.NEXT_STATION" +
                            " from r_llt a, r_sn b where a.sn = b.sn and b.valid_flag=1";

                if (flag == "" && Inoutdata == "")
                {
                    temp1_sql =  " and a.status in('0','1','2','3')";
                }
                else if (flag != "" && Inoutdata != "")
                {
                    temp1_sql = " and (b.skuno = '" + Inoutdata + "' or a.sn='" + Inoutdata + "') and a.status ='" + flag + "'";
                }
                else if (flag == "" && Inoutdata != "")
                {
                    temp1_sql = " and (b.skuno = '" + Inoutdata + "' or a.sn='" + Inoutdata + "') and a.status in('0','1','2','3')";
                }
                else if (flag != "" && Inoutdata == "")
                {
                    temp1_sql =  "  and a.status ='" + flag + "'";
                }
                temp_sql = " )aa " +
                        " left join(select a.value, a.SN, b.skuno, b.workorderno, b.next_station from r_sn_kp a, r_sn b where a.value = b.sn and b.valid_flag=1)bb on aa.sn = bb.value " +
                        " left join (select distinct sn, sum(BURNIN_TIME) TotalTIME from  r_llt_test group by sn)cc on aa.sn = cc.sn  left join r_sn_lock dd on aa.sn = dd.sn and dd.lock_status = '1'";

                total_sql = total_sql + temp1_sql + temp_sql;


                DataTable total_dt = SFCDB.ExecSelect(total_sql, null).Tables[0];
                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");
                }
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.Data = new { Total = total_dt.Rows.Count, Rows = total_dt };
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Delete(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                string SN = Data["SN"].ToString();
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);
                if (!TCUP.CheckpPivilegeByName(SFCDB, "DELETE_LLT", this.LoginUser.EMP_NO))
                {
                    throw new Exception("No Pivilege!");
                }
                T_R_LLT t_r_llt = new T_R_LLT(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                DateTime systemTime = t_r_llt.GetDBDateTime(SFCDB);
                R_LLT test_obj = SFCDB.ORM.Queryable<R_LLT>().Where(r => r.ID == id && r.STATUS == "0").ToList().FirstOrDefault();
                if (test_obj == null)
                {
                    throw new Exception($@"SN:{SN} Delete is not allowed in this status!");
                }
                int result = SFCDB.ORM.Updateable<R_LLT>().SetColumns(r => new R_LLT { STATUS="3",CANCELBY=LoginUser.EMP_NO,CANCELTIME=systemTime} )
                    .Where(r => r.ID == id).ExecuteCommand();

                if (result == 0)
                {
                    throw new Exception("Update Data Error!");
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add("OK");
                StationReturn.Data = "OK";
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void Download(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {

                string total_sql = string.Empty;
                string temp_sql = string.Empty;
                string temp1_sql = string.Empty;
                string strstatus = string.Empty;
                string Inoutdata = Data["SEARCH_TEXT"].ToString();
                string flag = Data["STATUS"].ToString();
                switch (flag)
                {
                    case "0":
                        strstatus = " case  when aa.STATUS='0' then 'WAIT_TEST' end STATUS";
                        break;
                    case "1":
                        strstatus = " case  when aa.STATUS='1' then 'ON_TEST' end STATUS";
                        break;
                    case "2":
                        strstatus = " case  when aa.STATUS='2' then 'OFF_TEST' end STATUS";
                        break;
                    case "3":
                        strstatus = " case  when aa.STATUS='3' then 'CANCEL' end STATUS";
                        break;
                    default:
                        strstatus = "case when aa.STATUS = '0' then 'WAIT_TEST' when aa.STATUS = '1' then 'ON_TEST' when aa.STATUS = '2' then 'OFF_TEST' when aa.STATUS = '3' then 'CANCEL' end STATUS";
                        break;
                }
                total_sql = $@"select aa.ID,aa.SKUNO,aa.WORKORDERNO ,aa.SN ,";

                total_sql = total_sql + strstatus + ",aa.SHOULDTEST,cc.TotalTIME,aa.SHOULDTEST-cc.TotalTIME Remaintime,dd.lock_time,round(sysdate-dd.lock_time) LOCKDAY, aa.LOCATION,aa.CREATETIME ,bb.skuno TOPSKU, bb.workorderno TOPWO, bb.SN TOPSN, bb.next_station STATION from(" +
                            " select a.id, b.skuno, b.workorderno, a.sn, a.station, a.createtime,'1440' SHOULDTEST, a.status,case  when b.next_station='SHIPOUT' THEN '已入庫)' when b.next_station IN('SHIPFINISH' , 'JOBFINISH') then '已出貨(已綁定)' else '未入庫' end as LOCATION" +
                            " from r_llt a, r_sn b where a.r_sn_id = b.id and b.valid_flag=1";

                if (flag == "" && Inoutdata == "")
                {
                    temp1_sql = " and a.status in('0','1','2','3')";
                }
                else if (flag != "" && Inoutdata != "")
                {
                    temp1_sql = " and (b.skuno = '" + Inoutdata + "' or a.sn='" + Inoutdata + "') and a.status ='" + flag + "'";
                }
                else if (flag == "" && Inoutdata != "")
                {
                    temp1_sql = " 1 and (b.skuno = '" + Inoutdata + "' or a.sn='" + Inoutdata + "') and a.status in('0','1','2','3')";
                }
                else if (flag != "" && Inoutdata == "")
                {
                    temp1_sql = "  and a.status ='" + flag + "'";
                }
                temp_sql = " )aa " +
                        " left join(select a.value, a.SN, b.skuno, b.workorderno, b.next_station from r_sn_kp a, r_sn b where a.value = b.sn and b.valid_flag=1)bb on aa.sn = bb.value " +
                        " left join (select distinct sn, sum(BURNIN_TIME) TotalTIME from  r_llt_test group by sn)cc on aa.sn = cc.sn  left join r_sn_lock dd on aa.sn = dd.sn and dd.lock_status = '1'";

                total_sql = total_sql + temp1_sql + temp_sql;

                DataTable total_dt = SFCDB.ExecSelect(total_sql, null).Tables[0];
                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");
                }
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(total_dt);
                string fileName = "LLTSNSTATUS_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.Data = new { FileName = fileName, FileContent = content };
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

    }
}
