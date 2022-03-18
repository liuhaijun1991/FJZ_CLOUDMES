using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab;

namespace MESInterface.JUNIPER
{
    public class DownloadWO : taskBase
    {
        #region 外部調用時，請給以下賦值       
        public string BU = "";
        public string Plant = "";
        public string DB = "";
        public string CUST = "";
        public string COUNT = "";
        public string _downloadWO = "";
        public string[] arrayConvertWO;
        public string downLoadDate = "";
        public string ip = "";
        public OleExec SFCDB = null;
        public T_C_TAB_COLUMN_MAP C_TAB_COLUMN_MAP;
        public T_R_WO_HEADER R_WO_HEADER;
        public T_R_WO_ITEM R_WO_ITEM;
        public T_R_WO_TEXT R_WO_TEXT;
        public T_R_WO_DEVIATION R_WO_DEVIATION;
        public T_C_SKU C_SKU;
        public T_R_SYNC_LOCK synLock;
        public T_C_ROUTE_DETAIL RouteDetail;
        public T_R_WO_TYPE WOType;
        public T_C_KEYPART Keypart;
        public T_C_ROUTE C_ROUTE;
        public T_R_WO_BASE R_WO_BASE;
        public T_C_SERIES T_Series;
        public T_C_KP_LIST t_c_kp_list;
        #endregion

        private ZRFC_SFC_NSG_0001B ZRFC_SFC_NSG_0001B;

        private bool woIsExist = false;        
        private bool autoDownLoad = true;

        public override void init()
        {
            try
            {
                BU = ConfigGet("BU");
                Plant = ConfigGet("PLANT");
                DB = ConfigGet("DB");
                CUST = ConfigGet("CUST");
                COUNT = ConfigGet("COUNT");
                arrayConvertWO = ConfigGet("CONVERTWO").Split(',');
                _downloadWO = ConfigGet("DOWNLOADWO");
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                Output.UI = new DownloadWO_UI(this);
                SFCDB = new OleExec(DB, false);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(SFCDB, DB_TYPE_ENUM.Oracle);
                C_SKU = new T_C_SKU(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_HEADER = new T_R_WO_HEADER(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_ITEM = new T_R_WO_ITEM(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_TEXT = new T_R_WO_TEXT(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_DEVIATION = new T_R_WO_DEVIATION(SFCDB, DB_TYPE_ENUM.Oracle);
                RouteDetail = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                WOType = new T_R_WO_TYPE(SFCDB, DB_TYPE_ENUM.Oracle);
                Keypart = new T_C_KEYPART(SFCDB, DB_TYPE_ENUM.Oracle);
                C_ROUTE = new T_C_ROUTE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_BASE = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                T_Series = new T_C_SERIES(SFCDB, DB_TYPE_ENUM.Oracle);
                t_c_kp_list = new T_C_KP_LIST(SFCDB, DB_TYPE_ENUM.Oracle);
                ZRFC_SFC_NSG_0001B = new ZRFC_SFC_NSG_0001B(BU);
            }
            catch (Exception e)
            {

                throw new Exception("Init DownLoadWO Fail" + e.Message);
            }
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("ITAB"));
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("WO_HEADER"));
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("WO_ITEM"));
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("WO_TEXT"));
        }

        public override void Start()
        {
            SFCDB = new OleExec(DB, false);
            string lockIp = string.Empty;
            var IsRuning = synLock.IsLock("DownLoadWO", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("DownLoadWO interface is running on " + lockIp + ",Please try again later");
            }
            try
            {

                synLock.SYNC_Lock(BU, ip, "DownLoadWO", "DownLoadWO", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
                SFCDB.CommitTrain();
                CallRFC();
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                throw new Exception("Start DownLoadWO Fail" + ex.Message);
            }
            finally
            {
                synLock.SYNC_UnLock(BU, ip, "DownLoadWO", "DownLoadWO", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
                SFCDB.CommitTrain();
                SFCDB.CloseMe();
            }

        }
        /// <summary>
        ///外部調用時請賦值public屬性
        /// </summary>
        private void CallRFC()
        {
            string msg = "";
            DataTable dtITAB = new DataTable();
            DataTable dtWOHeader = new DataTable();
            DataTable dtWOItem = new DataTable();
            DataTable dtWOText = new DataTable();
            DataTable dtConvertWO = new DataTable();

            try
            {
                List<string> plantList = this.Plant.Split(',').ToList();
                foreach(var plant in plantList)
                {
                    if (string.IsNullOrEmpty(plant))
                    {
                        continue;
                    }
                    ZRFC_SFC_NSG_0001B.SetValue("PLANT", plant);
                    ZRFC_SFC_NSG_0001B.SetValue("COUNT", COUNT);
                    ZRFC_SFC_NSG_0001B.SetValue("CUST", CUST);
                    ZRFC_SFC_NSG_0001B.SetValue("IN_CNF", "");
                    if (downLoadDate != "")
                    {
                        ZRFC_SFC_NSG_0001B.SetValue("SCHEDULED_DATE", downLoadDate);
                        ZRFC_SFC_NSG_0001B.SetValue("RLDATE", downLoadDate);
                        autoDownLoad = false;
                    }
                    else
                    {
                        ZRFC_SFC_NSG_0001B.SetValue("SCHEDULED_DATE", DateTime.Now.ToString("yyyy-MM-dd"));
                        ZRFC_SFC_NSG_0001B.SetValue("RLDATE", DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    ZRFC_SFC_NSG_0001B.CallRFC();
                    dtITAB = ZRFC_SFC_NSG_0001B.GetTableValue("ITAB");
                    dtWOHeader = ZRFC_SFC_NSG_0001B.GetTableValue("WO_HEADER");
                    dtWOItem = ZRFC_SFC_NSG_0001B.GetTableValue("WO_ITEM");
                    dtWOText = ZRFC_SFC_NSG_0001B.GetTableValue("WO_TEXT");

                    Output.Tables.Add(dtITAB);
                    Output.Tables.Add(dtWOHeader);
                    Output.Tables.Add(dtWOItem);
                    Output.Tables.Add(dtWOText);
                    if (dtITAB.Rows.Count > 0)
                    {
                        throw new Exception(dtITAB.Rows[0][1].ToString());
                    }
                    //SaveWOHeader();
                    //ConvertWO();
                    //ConvertWOText();
                    //調用MESStation.Interface.DownLoad_WO 中轉工單的方法 2020.3.11
                    string saveMsg = "";
                    MESStation.Interface.DownLoad_WO stationInterFace = new MESStation.Interface.DownLoad_WO();
                    stationInterFace.SaveWOHeader(dtWOHeader, dtWOItem, SFCDB, DB_TYPE_ENUM.Oracle, BU, _downloadWO, ip, "interface", autoDownLoad, ref saveMsg);
                    stationInterFace.SaveWOText(dtWOText, SFCDB, DB_TYPE_ENUM.Oracle, BU, ip, "interface");                    
                    downLoadDate = "";
                }
            }
            catch (Exception ex)
            {
                //write log
                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.JUNIPER.DownLoadWO", "CallRFC", ip + ";" + ex.Message.ToString(), "", "interface");
            }

        }
    }
}
