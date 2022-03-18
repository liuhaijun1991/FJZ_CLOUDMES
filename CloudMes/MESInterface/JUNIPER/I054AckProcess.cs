using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESJuniper.OrderManagement;
using MESJuniper.SendData;
using MESPubLab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MESInterface.JUNIPER
{
    public class I054AckProcess : taskBase
    {
        public string BU = "";
        public string SFCDBstr = "";
        //public string OMDBstr = "";
        public string Factory = "";
        public bool IsRuning = false;
        OleExec SFCDB;
        //OleExec OMDB;
        public string BuildSite
        {
            get
            {
                return Factory == "FVN" ? "Foxconn Vietnam" : (Factory == "FJZ" ? "Foxconn Juarez" : "");
            }
        }

        public DataTable ASBDATA = null;

        public override void init()
        {
            ASBDATA = new DataTable();
            ASBDATA.TableName = "RES";
            DataColumn dc = null;
            dc = ASBDATA.Columns.Add("TranID", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("StartTime", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("Qty", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("FinishQty", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("Status", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("Message", Type.GetType("System.String"));
            this.Output.Tables.Add(ASBDATA);

            try
            {
                SFCDBstr = ConfigGet("SFCDB");
                //OMDBstr = ConfigGet("OMDB");
                Factory = ConfigGet("FACTORY");
                SFCDB = new OleExec(SFCDBstr, false);
                //OMDB = new OleExec(OMDBstr, false);
            }
            catch (Exception ex)
            {
                AddMessage("init", "", "", "Fail", ex.Message);
            }
            Output.UI = new I054AckProcess_UI(this);
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("The task is in progress. Please try again later...");
            }
            IsRuning = true;
            try
            {
                CaptrueASBData();
            }
            catch
            {
                throw;
            }
            finally
            {
                IsRuning = false;
            }
        }
        
        /// <summary>
        /// 抓数据
        /// </summary>
        public void CaptrueASBData()
        {
            var pendingList = GetPendingI054();
            var obj = new JuniperASBuildObj(SFCDB.ORM, Factory, BuildSite);
            var As_Build_Data = obj.GenerateAS_Build_Data(pendingList);
            //获取不同的MESSAGEID
            var idlist = As_Build_Data.Select(t => t.TRANID).Distinct().ToList();
            //按照不同的MESSAGEID循环插入
            for (int i = 0; i < idlist.Count; i++)
            {
                var data = As_Build_Data.FindAll(t => t.TRANID == idlist[i]);
                try
                {
                    SFCDB.BeginTrain();
                    //OMDB.BeginTrain();

                    //AS-BUILD数据插入MES
                    SFCDB.ORM.Insertable<R_I054>(data).ExecuteCommand();
                    //AS-BUILD数据插入OM
                    //OMDB.ORM.Insertable<R_I054>(data).ExecuteCommand();

                    #region 更新状态
                    var snlist = data.FindAll(t => t.PNTYPE == "Parent").Select(t => t.PARENTSN).ToList();
                    var rsn = SFCDB.ORM.Queryable<R_SN>().Where(t => snlist.Contains(t.SN) && t.VALID_FLAG == "1").ToList();
                    var sdts = new List<R_SN_STATION_DETAIL>();
                    var logs = new List<R_MES_LOG>();
                    for (int n = 0; n < rsn.Count; n++)
                    {
                        #region 过站记录
                        DateTime dt = DateTime.Now;
                        R_SN_STATION_DETAIL SDT = new R_SN_STATION_DETAIL()
                        {
                            ID = MESDataObject.MesDbBase.GetNewID<R_SN_STATION_DETAIL>(SFCDB.ORM, Factory),
                            R_SN_ID = rsn[n].ID,
                            SN = rsn[n].SN,
                            SKUNO = rsn[n].SKUNO,
                            WORKORDERNO = rsn[n].WORKORDERNO,
                            PLANT = rsn[n].PLANT,
                            ROUTE_ID = rsn[n].ROUTE_ID,
                            LINE = "LINE1",
                            STARTED_FLAG = rsn[n].STARTED_FLAG,
                            START_TIME = rsn[n].START_TIME,
                            PACKED_FLAG = rsn[n].PACKED_FLAG,
                            PACKED_TIME = rsn[n].PACKDATE,
                            COMPLETED_FLAG = rsn[n].COMPLETED_FLAG,
                            COMPLETED_TIME = rsn[n].COMPLETED_TIME,
                            SHIPPED_FLAG = rsn[n].SHIPPED_FLAG,
                            SHIPDATE = rsn[n].SHIPDATE,
                            REPAIR_FAILED_FLAG = rsn[n].REPAIR_FAILED_FLAG,
                            CURRENT_STATION = rsn[n].CURRENT_STATION,
                            NEXT_STATION = rsn[n].NEXT_STATION,
                            KP_LIST_ID = rsn[n].KP_LIST_ID,
                            PO_NO = rsn[n].PO_NO,
                            CUST_ORDER_NO = rsn[n].CUST_ORDER_NO,
                            CUST_PN = rsn[n].CUST_PN,
                            BOXSN = rsn[n].BOXSN,
                            DEVICE_NAME = "Interface",
                            STATION_NAME = "GENERATE_I054",
                            SCRAPED_FLAG = "0",
                            SCRAPED_TIME = dt,
                            PRODUCT_STATUS = rsn[n].PRODUCT_STATUS,
                            REWORK_COUNT = rsn[n].REWORK_COUNT,
                            VALID_FLAG = rsn[n].VALID_FLAG,
                            EDIT_EMP = "Interface_I054",
                            EDIT_TIME = dt
                        };
                        sdts.Add(SDT);
                        #endregion
                        var log = SFCDB.ORM.Queryable<R_MES_LOG>()
                            .Where(t => t.PROGRAM_NAME == "MESInterface" && t.FUNCTION_NAME == "ReGenerateAS_Build_Data" && t.DATA1 == rsn[n].SN && t.DATA4 == "N")
                            .First();
                        if (log != null)
                        {
                            logs.Add(log);
                        }
                    }
                    SFCDB.ORM.Insertable<R_SN_STATION_DETAIL>(sdts).ExecuteCommand();
                    if (logs.Count > 0)
                    {
                        var IDS = logs.Select(t => t.ID).ToList();
                        SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(t => new R_MES_LOG { DATA4 = "Y" }).Where(t => IDS.Contains(t.ID)).ExecuteCommand();
                    }
                    #endregion

                    SFCDB.CommitTrain();
                    //OMDB.CommitTrain();

                    AddMessage(idlist[i], "", "", "Success", "");
                }
                catch (Exception ex)
                {
                    SFCDB.RollbackTrain();
                    //OMDB.RollbackTrain();
                    AddMessage(idlist[i], "", "", "Fail", ex.Message);
                }

            }
        }

        /// <summary>
        /// Get Pending AS build SN List
        /// </summary>
        /// <returns>R_SN List,Does Not Contain Locked</returns>
        private List<R_SN> GetPendingI054()
        {
            List<R_SN> SNList = new List<R_SN>();
            #region CTO-ODM
            //var locklist_cto = SFCDB.ORM.Queryable<R_SN_LOCK, O_ORDER_MAIN, C_SKU, C_SERIES>((sn, wo, sku, ser) => sn.WORKORDERNO == wo.PREWO && wo.PID == sku.SKUNO && sku.C_SERIES_ID == ser.ID)
            //    .Where((sn, wo, sku, ser) => sn.LOCK_STATUS == "1" && ser.SERIES_NAME.StartsWith("JNP-ODM"))
            //    .Select((sn, wo, sku, ser) => sn)
            //    .ToList();

            //var locksn_cto = locklist_cto.FindAll(t => t.TYPE == "SN").Select(t => t.SN).ToList();
            //var lockwo_cto = locklist_cto.FindAll(t => t.TYPE == "WO").Select(t => t.WORKORDERNO).ToList();
            //var locksku_cto = locklist_cto.FindAll(t => t.TYPE == "SKU").Select(t => t.WORKORDERNO).ToList();

            var temp_cto = SFCDB.ORM.Ado.SqlQuery<R_SN>(@"
                        SELECT *
                          FROM r_sn sn
                         WHERE EXISTS (SELECT 1
                                  FROM o_order_main wo, c_sku sku, c_series ser
                                 WHERE sn.workorderno = wo.prewo
                                   AND wo.pid = sku.skuno
                                   AND sku.c_series_id = ser.ID
                                   AND wo.plant = @Factory
                                   AND ser.SERIES_NAME like 'JNP-ODM%')
                           AND EXISTS (SELECT 1
                                  FROM r_sn_station_detail s
                                 WHERE sn.sn = s.sn
                                   AND s.station_name = 'GENERATE_I054')
                           AND EXISTS
                         (SELECT *
                                  FROM R_I054
                                 WHERE PNTYPE = 'PARENT'
                                   AND PARENTSN = sn.sn
                                   AND TRANID IN (SELECT DATA3
                                                    FROM r_mes_log l
                                                   WHERE l.PROGRAM_NAME = 'MESInterface'
                                                     AND l.FUNCTION_NAME = 'ReGenerateAS_Build_Data'
                                                     AND l.data4 = 'N'))
                           AND sn.valid_flag = 1",
                            new { Factory })
                        .ToList();
            //var temp1 = temp_cto.FindAll(t => !locksn_cto.Contains(t.SN) && !lockwo_cto.Contains(t.WORKORDERNO) && !locksku_cto.Contains(t.SKUNO));
            SNList.AddRange(temp_cto);
            #endregion

            #region BTS & BNDL
            //var locklist = SFCDB.ORM.Queryable<R_SN_LOCK, O_ORDER_MAIN, O_SKU_CONFIG>((sn, wo, sku) => sn.WORKORDERNO == wo.PREWO && wo.USERITEMTYPE == sku.USERITEMTYPE && wo.POTYPE == sku.PRODUCTTYPE && wo.OFFERINGTYPE == sku.OFFERINGTYPE)
            //    .Where((sn, wo, sku) => sn.LOCK_STATUS == "1" && sku.I054 == "Y")
            //    .Select((sn, wo, sku) => sn)
            //    .ToList();

            //var locksn = locklist.FindAll(t => t.TYPE == "SN").Select(t => t.SN).ToList();
            //var lockwo = locklist.FindAll(t => t.TYPE == "WO").Select(t => t.WORKORDERNO).ToList();
            //var locksku = locklist.FindAll(t => t.TYPE == "SKU").Select(t => t.WORKORDERNO).ToList();

            var temp = SFCDB.ORM.Ado.SqlQuery<R_SN>(@"
                        SELECT *
                          FROM r_sn sn
                         WHERE EXISTS (SELECT 1
                                  FROM o_order_main wo, o_sku_config sku
                                 WHERE sn.workorderno = wo.prewo
                                   AND wo.useritemtype = sku.useritemtype
                                   AND wo.potype = sku.producttype
                                   AND wo.offeringtype = sku.offeringtype
                                   AND wo.plant = @Factory
                                   AND sku.i054 = 'Y')
                           AND EXISTS (SELECT 1
                                  FROM r_sn_station_detail s
                                 WHERE sn.sn = s.sn
                                   AND s.station_name = 'GENERATE_I054')
                           AND EXISTS
                         (SELECT *
                                  FROM R_I054
                                 WHERE PNTYPE = 'PARENT'
                                   AND PARENTSN = sn.sn
                                   AND TRANID IN (SELECT DATA3
                                                    FROM r_mes_log l
                                                   WHERE l.PROGRAM_NAME = 'MESInterface'
                                                     AND l.FUNCTION_NAME = 'ReGenerateAS_Build_Data'
                                                     AND l.data4 = 'N'))
                           AND sn.valid_flag = 1",
                            new { Factory })
                        .ToList();
            //var temp2 = temp.FindAll(t => !locksn.Contains(t.SN) && !lockwo.Contains(t.WORKORDERNO) && !locksku.Contains(t.SKUNO));
            SNList.AddRange(temp);
            #endregion

            var wolist = SNList.Select(t => t.WORKORDERNO).ToList();
            var orderlist = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => wolist.Contains(t.PREWO)).ToList();

            List<R_SN> res = new List<R_SN>();
            for (int i = 0; i < orderlist.Count; i++)
            {
                if (!JuniperOmBase.JuniperHoldCheck(orderlist[i].ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PRODUCTION, SFCDB.ORM).HoldFlag)
                {
                    var temp3 = SNList.FindAll(t => t.WORKORDERNO == orderlist[i].PREWO).ToList();
                    res.AddRange(temp3);
                }
            }
            return res;
        }



        private void AddMessage(string TranID, string Qty, string FinishQty, string Status, string Message)
        {
            if (ASBDATA.Rows.Count > 200)
            {
                ASBDATA.Clear();
            }
            using (ASBDATA)
            {
                DataRow dr = ASBDATA.NewRow();
                dr["TranID"] = TranID;
                dr["StartTime"] = DateTime.Now;
                dr["Qty"] = Qty;
                dr["FinishQty"] = FinishQty;
                dr["Status"] = Status;
                dr["Message"] = Message;
                ASBDATA.Rows.Add(dr);
            }
        }
    }
}
