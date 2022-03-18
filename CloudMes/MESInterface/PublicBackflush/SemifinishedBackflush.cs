using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.SAP_RFC;
using MESPubLab.MESInterface;
using MESPubLab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.PublicBackflush
{
    public class SemifinishedBackflush : taskBase
    {
        private bool IsRuning = false;
        private string lockIp = "";
        private string gt_id = "";
        private string sql = "";
        private string result = "";
        private List<R_SN_STATION_DETAIL> listDetail;
        private string postDate;

        public string ip = "";
        public string BU = "";
        public string Plant = "";
        public string DB = "";
        public string fromStorage;
        public string toStorage;
        public string debugWo;
        public OleExec SFCDB = null;
        public ZRFC_SFC_NSG_0011 zfrc_sfc_nsg_0011 = null;
        public T_R_SYNC_LOCK synLock;
        public T_R_STOCK t_r_stock;
        public T_R_STOCK_GT t_r_stock_gt;
        public T_R_SN_STATION_DETAIL t_r_sn_station_detail;

        public override void init()
        {
            //base.init(); 
            try
            {
                BU = ConfigGet("BU");
                Plant = ConfigGet("PLANT");
                DB = ConfigGet("DB");
                fromStorage = ConfigGet("FROMSTORAGE");
                toStorage = ConfigGet("TOSTORAGE");
                debugWo = ConfigGet("DEBUGWO");
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                listDetail = new List<R_SN_STATION_DETAIL>();
                SFCDB = new OleExec(DB, false);
                SFCDB.ThrowSqlExeception = true;
                zfrc_sfc_nsg_0011 = new ZRFC_SFC_NSG_0011(BU);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                t_r_stock = new T_R_STOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                t_r_stock_gt = new T_R_STOCK_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                Output.UI = new SemifinishedBackflush_UI(this);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Start()
        {
            //base.Start();
            try
            {
                GetBackFlustData();
                GetPassCBSQty();
                CallRFCBackFlush();
            }
            catch (Exception ex)
            {
                throw new Exception("Start SemifinishedBackflush Fail" + ex.Message);
            }
        }
        public void GetBackFlustData()
        {            
            Row_R_STOCK_GT rowStockGT;
            Row_R_BACKFLUSH_HISTORY rowBackflushHistory;
            T_R_BACKFLUSH_HISTORY t_r_backflush_history = new T_R_BACKFLUSH_HISTORY(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_BACKFLUSH_HISTORY> backflushList = new List<R_BACKFLUSH_HISTORY>();           

            if (debugWo.ToString() != "")
            {
                backflushList = SFCDB.ORM.Queryable<R_BACKFLUSH_HISTORY>().Where(r => r.WORKORDERNO == debugWo && r.SFC_STATION == "CBS" && r.RESULT == "Y" && r.REC_TYPE == null).ToList();
            }
            else
            {
                backflushList = SFCDB.ORM.Queryable<R_BACKFLUSH_HISTORY>().Where(r => r.SFC_STATION == "CBS" && r.RESULT == "Y" && r.REC_TYPE == null).ToList();
            }
            foreach (R_BACKFLUSH_HISTORY  r in backflushList)
            {                
                SFCDB.BeginTrain();
                try
                {
                    //VN 排除掉RMA工單倉碼間轉賬 20211027
                    var isRmaWo = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == r.WORKORDERNO && SqlSugar.SqlFunc.Contains(t.WO_TYPE, "RMA")).Any();
                    if (isRmaWo)
                    {
                        continue;
                    }

                    //confirmed_flag=1 表示只轉倉
                    //stock_type=0 表示從半成品轉到成品倉
                    R_STOCK_GT objGT = t_r_stock_gt.GetNotGTbjByWOAndStockType(r.WORKORDERNO, "1", "0", SFCDB);
                    if (objGT == null)
                    {
                        gt_id = t_r_stock_gt.GetNewID(BU, SFCDB);
                        rowStockGT = (Row_R_STOCK_GT)t_r_stock_gt.NewRow();
                        rowStockGT.ID = gt_id;
                        rowStockGT.WORKORDERNO = r.WORKORDERNO;
                        rowStockGT.SKUNO = r.SKUNO;
                        rowStockGT.TOTAL_QTY = r.DIFF_QTY;
                        rowStockGT.FROM_STORAGE = fromStorage;
                        rowStockGT.TO_STORAGE = toStorage;
                        rowStockGT.SAP_FLAG = "0";
                        rowStockGT.CONFIRMED_FLAG = "1";
                        rowStockGT.SAP_STATION_CODE = r.SAP_STATION;
                        rowStockGT.EDIT_EMP = "Interface";
                        rowStockGT.STOCK_TYPE = "0";
                        rowStockGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowStockGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                    else
                    {
                        rowStockGT = (Row_R_STOCK_GT)t_r_stock_gt.GetObjByID(objGT.ID, SFCDB);
                        rowStockGT.TOTAL_QTY = rowStockGT.TOTAL_QTY + r.DIFF_QTY;
                        rowStockGT.EDIT_EMP = "Interface";
                        rowStockGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowStockGT.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    }

                    sql = $@"select * from r_backflush_history where skuno='{r.SKUNO}' and workorderno='{r.WORKORDERNO}' and sap_station='{r.SAP_STATION}' and workorder_qty='{r.WORKORDER_QTY}' 
                             and sfc_qty='{r.SFC_QTY}'and diff_qty='{r.DIFF_QTY}' and sfc_station='{r.SFC_STATION}' and last_sfc_qty='{r.LAST_SFC_QTY}' and diff_qty1='{r.DIFF_QTY1}' 
                             and diff_qty2='{r.DIFF_QTY2}' and mrb_qty='{r.MRB_QTY}' and back_date=to_date('{((DateTime)r.BACK_DATE).ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss') and result='{r.RESULT}' and times='{r.TIMES}'";
                    
                    rowBackflushHistory = (Row_R_BACKFLUSH_HISTORY)t_r_backflush_history.GetObjBySelect(sql, SFCDB, DB_TYPE_ENUM.Oracle);
                    rowBackflushHistory.REC_TYPE = "SEMI_FINISHED_BACKFLUSH";
                    result = SFCDB.ExecSQL(rowBackflushHistory.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    if (Convert.ToInt32(result) <= 0)
                    {
                        throw new Exception(" update r_backflush_history fail ");
                    }                    
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    SFCDB.RollbackTrain();
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.PublicBackflush.SemifinishedBackflush", "GetBackFlustData", ip
                        + ";" + r.WORKORDERNO + ";" + ex.Message, "", "interface");               
                }                
            }

        }
        public void CallRFCBackFlush()
        {
            IsRuning = synLock.IsLock("SemifinishedBackflush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("SemifinishedBackflush interface is running on " + lockIp + ",Please try again later");
            }
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                //月結不給拋賬
                throw new Exception("This time is monthly,can't BackFlush");
            }

            synLock.SYNC_Lock(BU, ip, "SemifinishedBackflush", "SemifinishedBackflush", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
           
            //confirmed_flag=1 表示只轉倉
            //stock_type=0 表示從半成品轉到成品倉
            List<R_STOCK_GT> gtList = t_r_stock_gt.GetNotGTList("1", "0", SFCDB);
            List<R_STOCK_GT> backflushList = new List<R_STOCK_GT>();
            Row_R_STOCK_GT rowStockGT;
            postDate = InterfacePublicValues.GetPostDate(SFCDB);
            if (gtList != null)
            {
                if (debugWo.ToString() != "")
                {
                    backflushList = gtList.FindAll(g => g.WORKORDERNO == debugWo);
                }
                else
                {
                    backflushList = gtList;
                }

                foreach (R_STOCK_GT gt in backflushList)
                {
                    rowStockGT = (Row_R_STOCK_GT)t_r_stock_gt.GetObjByID(gt.ID, SFCDB);
                    try
                    {
                        zfrc_sfc_nsg_0011.SetValue("I_BKTXT","");
                        zfrc_sfc_nsg_0011.SetValue("I_BUDAT", postDate);
                        zfrc_sfc_nsg_0011.SetValue("I_ERFMG", gt.TOTAL_QTY.ToString());
                        zfrc_sfc_nsg_0011.SetValue("I_MATNR", gt.SKUNO);
                        zfrc_sfc_nsg_0011.SetValue("I_FROM", fromStorage);
                        zfrc_sfc_nsg_0011.SetValue("I_TO", toStorage);
                        zfrc_sfc_nsg_0011.SetValue("PLANT", Plant);
                        zfrc_sfc_nsg_0011.CallRFC();
                        if (zfrc_sfc_nsg_0011.GetValue("O_FLAG").Equals("0"))
                        {
                            rowStockGT.SAP_FLAG = "1";                                                       
                        }      
                        else
                        {
                            rowStockGT.SAP_FLAG = "2";
                        }
                        rowStockGT.BACKFLUSH_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        rowStockGT.SAP_MESSAGE = zfrc_sfc_nsg_0011.GetValue("O_MESSAGE");
                        result = SFCDB.ExecSQL(rowStockGT.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        if (Convert.ToInt32(result) <= 0)
                        {
                            throw new Exception(" update t_stock_gt fail ");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.PublicBackflush.SemifinishedBackflush", "CallRFCBackFlush", ip + ";"
                            + gt.WORKORDERNO + ";" + ex.Message, "", "interface");
                    }                    
                }
            }
            synLock.SYNC_UnLock(BU, ip, "SemifinishedBackflush", "SemifinishedBackflush", "interface", SFCDB, DB_TYPE_ENUM.Oracle);           
        }

        //用於收集最後一個拋賬點不是CBS的機種的要轉倉的數據
        public void GetPassCBSQty()
        {
            string skuno = "";
            string workorderno = "";
            int pass_cbs_qty = 0;
            int last_total_backflush_qty = 0;
            int wo_stock_qty = 0;
            int wait_backflush_qty = 0;

            string pass_qty = "";
            string stock_qty = "";
            string total_backflush_qty = "";
            C_SAP_STATION_MAP sapMap = null;
            R_STOCK_GT objGT = null;
            if (debugWo.ToString() != "")
            {
                sql = $@"select distinct workorderno,skuno from r_sn_station_detail where station_name = 'CBS' and workorderno='{debugWo}'";
            }
            else
            {
                sql = "select distinct workorderno,skuno from r_sn_station_detail where station_name = 'CBS' and edit_time> sysdate - 7";
            }
            DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        pass_qty = "0";
                        stock_qty = "0";
                        total_backflush_qty = "0";
                        skuno = row["SKUNO"].ToString();
                        workorderno = row["WORKORDERNO"].ToString();
                        if (string.IsNullOrEmpty(skuno) || string.IsNullOrEmpty(workorderno))
                        {
                            continue;
                        }
                        sapMap = SFCDB.ORM.Queryable<C_SAP_STATION_MAP>()
                            .Where(r => r.SKUNO == skuno).OrderBy(r => r.SAP_STATION_CODE, SqlSugar.OrderByType.Desc)
                            .ToList().FirstOrDefault();
                        if (sapMap == null)
                        {
                            continue;
                        }
                        if (sapMap.STATION_NAME == "CBS")
                        {
                            continue;
                        }
                        sql = $@"select count(distinct(sn)) as pass_qty from r_sn_station_detail where station_name = 'CBS' and workorderno='{workorderno}'";
                        dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                        pass_qty = dt.Rows[0]["PASS_QTY"].ToString() == "" ? "0" : dt.Rows[0]["PASS_QTY"].ToString();
                        pass_cbs_qty = Convert.ToInt32(pass_qty);
                        if (pass_cbs_qty == 0)
                        {
                            continue;
                        }

                        sql = $@"select sum(diff_qty) as stock_qty from r_backflush_history where workorderno='{workorderno}' and sap_station='{sapMap.SAP_STATION_CODE}' 
                                and result='Y' and sfc_station not in ('CBS')";

                        dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                        stock_qty = dt.Rows[0]["STOCK_QTY"].ToString() == "" ? "0" : dt.Rows[0]["STOCK_QTY"].ToString();
                        wo_stock_qty = Convert.ToInt32(stock_qty);
                        if (wo_stock_qty == 0)
                        {
                            throw new Exception($@"{workorderno},{sapMap.SAP_STATION_CODE},Backflush Qty Is 0!");
                        }
                        if (wo_stock_qty < pass_cbs_qty)
                        {
                            throw new Exception($@"{workorderno},{sapMap.SAP_STATION_CODE},Backflush Qty({wo_stock_qty}) Less Then CBS Pass Qty({pass_cbs_qty})!");
                        }                        
                        sql = $@"select sum(total_qty) as last_total_backflush_qty from r_stock_gt where workorderno='{workorderno}' and skuno='{skuno}' and from_storage='{fromStorage}' and to_storage='{toStorage}' and stock_type='0'";
                        dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                        total_backflush_qty = dt.Rows[0]["LAST_TOTAL_BACKFLUSH_QTY"].ToString() == "" ? "0" : dt.Rows[0]["LAST_TOTAL_BACKFLUSH_QTY"].ToString();
                        last_total_backflush_qty = Convert.ToInt32(total_backflush_qty);
                        if (last_total_backflush_qty == pass_cbs_qty)
                        {
                            //CBS 過站數量等於已寫進拋賬表的數據,則本次不用拋賬
                            continue;
                        }
                        wait_backflush_qty = pass_cbs_qty - last_total_backflush_qty;
                        objGT = t_r_stock_gt.GetNotGTbjByWOAndStockType(workorderno, "1", "0", SFCDB);
                        if (objGT == null)
                        {
                            objGT = new R_STOCK_GT();
                            objGT.ID = t_r_stock_gt.GetNewID(BU, SFCDB);
                            objGT.WORKORDERNO = workorderno;
                            objGT.SKUNO = skuno;
                            objGT.TOTAL_QTY = wait_backflush_qty;
                            objGT.FROM_STORAGE = fromStorage;
                            objGT.TO_STORAGE = toStorage;
                            objGT.SAP_FLAG = "0";
                            objGT.CONFIRMED_FLAG = "1";
                            objGT.SAP_STATION_CODE = sapMap.SAP_STATION_CODE;
                            objGT.EDIT_EMP = "Interface";
                            objGT.STOCK_TYPE = "0";
                            objGT.EDIT_TIME = SFCDB.ORM.GetDate();
                            t_r_stock_gt.Insert(SFCDB, objGT);
                        }
                        else
                        {                            
                            objGT.TOTAL_QTY = objGT.TOTAL_QTY + wait_backflush_qty;
                            objGT.EDIT_EMP = "Interface";
                            objGT.EDIT_TIME = SFCDB.ORM.GetDate();
                            t_r_stock_gt.Update(SFCDB, objGT);
                        }                        
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.PublicBackflush.SemifinishedBackflush", "GetPassCBSQty", ip
                       + ";" + row["WORKORDERNO"].ToString() + ";" + ex.Message, "", "interface");
                    }                   
                }
            }
        }
    }
}
