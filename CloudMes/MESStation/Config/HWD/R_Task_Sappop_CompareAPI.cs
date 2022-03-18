using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using System.Data;
using System.Reflection;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module.Juniper;
using MESJuniper.Base;
using SqlSugar;
using MESDataObject.Module.OM;
using MESDataObject.Module.HWD;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.Common;
using Newtonsoft.Json.Linq;
using static MESJuniper.Base.SynAck;
using static MESDataObject.Common.EnumExtensions;
using System.Threading;
using MESPubLab.Json;
using MESPubLab.MesException;

namespace MESStation.Config.HWD
{
    public class R_Task_Sappop_CompareAPI : MesAPIBase
    {
        public string Factory = "";
        public R_Task_Sappop_CompareAPI()
        {
             
        }

        public void GetR_Task_SAPPOP_Compare(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                var TASK_NO = Data["TASK_NO"].ToString().Trim();
                var WO = Data["WO"].ToString().Trim();
                var SKUNO = Data["SKUNO"].ToString().Trim();
                var RESULT = Data["RESULT"].ToString().Trim();
                var txtDATE_FROM = Data["DATE_FROM"].ToString().Trim();
                var txtDATE_TO = Data["DATE_TO"].ToString().Trim();
                var DATE_FROM=DateTime.Now;
                var DATE_TO = DateTime.Now;
                if (!string.IsNullOrEmpty(txtDATE_FROM))
                {
                    if (string.IsNullOrEmpty(txtDATE_TO))
                    {
                        throw new Exception("Please input (DATE_TO).");
                    }
                    DATE_FROM = Convert.ToDateTime(txtDATE_FROM +" 00:00:00");
                }
                if (!string.IsNullOrEmpty(txtDATE_TO))
                {
                    if (string.IsNullOrEmpty(txtDATE_FROM))
                    {
                        throw new Exception("Please input (txtDATE_FROM).");
                    }
                    DATE_TO = Convert.ToDateTime(txtDATE_TO + " 23:59:59");
                    Int32 daycount = Convert.ToInt32(txtDATE_TO.Replace("-", "")) - Convert.ToInt32(txtDATE_FROM.Replace("-", ""));
                    if (daycount>7)
                    {
                        throw new Exception("Please input (You can only search 7 days data).");
                    }
                }
               

                if (RESULT=="ALL"|| RESULT == "OK")
                {
                    if (string.IsNullOrEmpty(TASK_NO) && string.IsNullOrEmpty(WO) && string.IsNullOrEmpty(SKUNO) && (string.IsNullOrEmpty(txtDATE_FROM) || string.IsNullOrEmpty(txtDATE_TO)))
                    {
                        throw new Exception("Please input At least one input (TASK_NO,WO,SKUNO,TIME).");
                    }
                }

                if (!string.IsNullOrEmpty(WO))
                {
                    if (WO.Length<12)
                    {
                        throw new Exception("WO Length must bigger than 11 !");
                    }
                }
               List<R_TASK_SAPPOP_COMPARE> listTemp;
                if (RESULT == "RELEASE")
                {

                      listTemp = sfcdb.ORM.Ado.SqlQuery<R_TASK_SAPPOP_COMPARE>(@"
                        SELECT DISTINCT task_no, skuno, wo, '' fox_item_sap, '' hw_item_sap,
                                            0 wo_unit_sap, 0 qty_sap, '' hw_item_pop, 0 qty_pop,
                                            0 unit_pop, '' version_date, 0 version_num, '' pop_num,
                                            'Release' RESULT, '可釋放工單' diff_remark, create_time,
                                            edit_time, data1 wo_flag
                                      FROM r_task_sappop_compare a
                                     WHERE 1 = 1
                                       AND RESULT <> 'OVER'
                                       AND NOT EXISTS (SELECT *
                                              FROM (SELECT DISTINCT task_no
                                                       FROM r_task_sappop_compare b
                                                      WHERE 1 = 1
                                                        AND b.result = 'NG')
                                             WHERE 1 = 1
                                               AND a.task_no = task_no)
                                     ORDER BY task_no ", new { Factory }).ToList();



                }
                else
                { 
                  listTemp = sfcdb.ORM.Queryable<R_TASK_SAPPOP_COMPARE>().WhereIF(!string.IsNullOrEmpty(TASK_NO), 
                    r => r.TASK_NO == TASK_NO).WhereIF(!string.IsNullOrEmpty(WO), 
                    r => r.WO.Contains(WO) ).WhereIF(!string.IsNullOrEmpty(SKUNO), 
                    r =>r.SKUNO == SKUNO).WhereIF(!string.IsNullOrEmpty(RESULT) && RESULT != "ALL", 
                    r => r.RESULT == RESULT).WhereIF(!string.IsNullOrEmpty(txtDATE_FROM) ,
                    r => r.CREATE_TIME >= DATE_FROM).WhereIF(!string.IsNullOrEmpty(txtDATE_TO),
                    r => r.CREATE_TIME <= DATE_TO).ToList();
                }
                StationReturn.Data = listTemp;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void UpdateR_Task_SAPPOP_Compare(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(sfcdb, DBTYPE);
                if (!TCUP.CheckpPivilegeByName(sfcdb, "BOM_Compare_UNLOCK", this.LoginUser.EMP_NO))
                {
                    throw new Exception("No Pivilege!");
                }

                var WO = Data["WO"].ToString().Trim();
                if (string.IsNullOrEmpty(WO))
                {
                    throw new Exception("Please input compare WO!");
                }

              var row=  sfcdb.ORM.Queryable<R_TASK_SAPPOP_COMPARE>().Where(t => t.WO == WO && t.RESULT == "NG").Count();

                if (row==0)
                {
                    throw new Exception("沒有NG狀態,無需解鎖!");
                }

                sfcdb.ORM.Updateable<R_TASK_SAPPOP_COMPARE>().SetColumns(t => new R_TASK_SAPPOP_COMPARE() { RESULT = "UNLOCK" , EDIT_TIME = DateTime.Now }).Where(t => t.WO == WO&&t.RESULT== "NG").ExecuteCommand();

                
             //   var listTemp = sfcdb.ORM.Queryable<R_TASK_SAPPOP_COMPARE>().Where(r => r.WO == WO).ToList();
                //不直接顯示查詢結果，到前台先顯示更新結果，在自動點擊查詢; 

                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetSAPPOPTIME(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
               
                Dictionary<string, object> dictionaryReturn = new Dictionary<string, object>();

                string dtSys = DateTime.Now.ToString("yyyy-MM-dd");
                // T_R_7B5_WO TRW = new T_R_7B5_WO(SFCDB, DBTYPE);
                //string dtSys = TRW.GetDBDateTime(SFCDB).ToString("yyyy-MM-dd");

                dictionaryReturn.Add("DATE_FROM", dtSys );
                dictionaryReturn.Add("DATE_TO", dtSys );
 

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dictionaryReturn;
                StationReturn.MessagePara.Add(dictionaryReturn.Count);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

    }
}

