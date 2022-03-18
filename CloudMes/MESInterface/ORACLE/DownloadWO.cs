using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.SAP_RFC;
using MESPubLab.MESInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab;
namespace MESInterface.ORACLE
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
        private string sql = "";
        private string lockIp = "";
        private string series = "";

        private bool woIsExist = false;
        private bool skuIsExist = false;
        private bool autoDownLoad = true;
        private bool IsRuning = false;


        //初始化SAP返回的DataTable
        private DataTable dtITAB = new DataTable();
        private DataTable dtWOHeader = new DataTable();
        private DataTable dtWOItem = new DataTable();
        private DataTable dtWOText = new DataTable();
        private DataTable dtConvertWO = new DataTable();
        private DataRow[] rowWOHeader;
        private DataRow[] rowWOItem;

        //private Row_C_SKU rowSku;
        private Row_C_ROUTE rowRoute;
        private Row_C_ROUTE rowRouteC;
        private R_WO_TYPE rowWOType;
        private Row_R_WO_BASE rowWOBase;
        private Row_R_WO_BASE rowWOBaseC;
        private List<C_ROUTE_DETAIL> routeDetailList;
        private List<C_ROUTE_DETAIL> routeDetailListC;
        private List<C_KEYPART> keypartList;
        private List<string> keypartIDList;
        private List<string> keypartIDListC;
        private List<C_ROUTE_DETAIL> linkStationList;
        private C_SERIES C_Series;
        private C_SERIES C_SeriesC;

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
            IsRuning = synLock.IsLock("HWD_DownLoadWO", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception("HWD DownLoadWO interface is running on " + lockIp + ",Please try again later");
            }
            try
            {

                synLock.SYNC_Lock(BU, ip, "HWD_DownLoadWO", "HWD_DownLoadWO", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
                SFCDB.CommitTrain();
                CallRFC();
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                throw new Exception("Start DownLoadWO Fail" + ex.Message);
            }
            synLock.SYNC_UnLock(BU, ip, "HWD_DownLoadWO", "HWD_DownLoadWO", "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            SFCDB.CommitTrain();

        }
        /// <summary>
        ///外部調用時請賦值public屬性
        /// </summary>
        private void CallRFC()
        {
            try
            {
                ZRFC_SFC_NSG_0001B.SetValue("PLANT", this.Plant);
                ZRFC_SFC_NSG_0001B.SetValue("COUNT", COUNT);
                ZRFC_SFC_NSG_0001B.SetValue("CUST", CUST);
                ZRFC_SFC_NSG_0001B.SetValue("IN_CNF", "");
                if (downLoadDate != "")
                {
                    ZRFC_SFC_NSG_0001B.SetValue("SCHEDULED_DATE", downLoadDate);
                    ZRFC_SFC_NSG_0001B.SetValue("RLDATE", ""); //patty 20181220 no need RLDATE 
                    autoDownLoad = false;
                }
                else
                {
                    ZRFC_SFC_NSG_0001B.SetValue("SCHEDULED_DATE", DateTime.Now.ToString("yyyy-MM-dd"));
                    ZRFC_SFC_NSG_0001B.SetValue("RLDATE", ""); //patty 20181220 no need RLDATE 
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
                SaveWOHeader();
                ConvertWO();
                ConvertWOText();
                downLoadDate = "";
            }
            catch (Exception ex)
            {
                //write log
                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "CallRFC", ip + ";" + ex.Message.ToString(), "", "interface");
            }

        }
        /// <summary>
        /// downlaod wo info into r_wo_header from sap
        /// </summary>
        protected void SaveWOHeader()
        {
            if (_downloadWO != "")
            {
                sql = "";
                woIsExist = false;
                rowWOHeader = null;
                autoDownLoad = false;
                try
                {
                    rowWOHeader = dtWOHeader.Select("AUFNR='" + _downloadWO + "'");
                    if (rowWOHeader.Length == 0)
                    {
                        throw new Exception("(SAP) WO " + _downloadWO + " does not exist in SAP.");
                    }
                    skuIsExist = C_SKU.CheckSku(rowWOHeader[0]["MATNR"].ToString(), SFCDB);
                    if (!skuIsExist)
                    {
                        throw new Exception("(PE) SKU " + rowWOHeader[0]["MATNR"].ToString() + " has not been setup in CloudMES.");
                    }
                    woIsExist = R_WO_HEADER.CheckWoHeadByWo(rowWOHeader[0]["AUFNR"].ToString(), true, SFCDB);
                    if (woIsExist)
                    {
                        return;
                    }
                    Row_R_WO_HEADER rowRWOHeader = (Row_R_WO_HEADER)R_WO_HEADER.NewRow();
                    rowRWOHeader.ID = R_WO_HEADER.GetNewID(BU, SFCDB);
                    rowRWOHeader.AUFNR = rowWOHeader[0]["AUFNR"].ToString();
                    rowRWOHeader.WERKS = rowWOHeader[0]["WERKS"].ToString();
                    rowRWOHeader.AUART = rowWOHeader[0]["AUART"].ToString();
                    rowRWOHeader.MATNR = rowWOHeader[0]["MATNR"].ToString();
                    rowRWOHeader.REVLV = rowWOHeader[0]["REVLV"].ToString();
                    rowRWOHeader.KDAUF = rowWOHeader[0]["KDAUF"].ToString();
                    rowRWOHeader.GSTRS = rowWOHeader[0]["GSTRS"].ToString();
                    rowRWOHeader.GAMNG = rowWOHeader[0]["GAMNG"].ToString();
                    rowRWOHeader.KDMAT = rowWOHeader[0]["KDMAT"].ToString();
                    rowRWOHeader.AEDAT = rowWOHeader[0]["AEDAT"].ToString();
                    rowRWOHeader.AENAM = rowWOHeader[0]["AENAM"].ToString();
                    rowRWOHeader.MATKL = rowWOHeader[0]["MATKL"].ToString();
                    rowRWOHeader.MAKTX = rowWOHeader[0]["MAKTX"].ToString();
                    rowRWOHeader.ERDAT = rowWOHeader[0]["ERDAT"].ToString();
                    rowRWOHeader.GSUPS = rowWOHeader[0]["GSUPS"].ToString();
                    rowRWOHeader.ERFZEIT = rowWOHeader[0]["ERFZEIT"].ToString();
                    rowRWOHeader.GLTRS = rowWOHeader[0]["GLTRS"].ToString();
                    rowRWOHeader.GLUPS = rowWOHeader[0]["GLUPS"].ToString();
                    rowRWOHeader.LGORT = rowWOHeader[0]["LGORT"].ToString();
                    rowRWOHeader.ABLAD = rowWOHeader[0]["ABLAD"].ToString();
                    rowRWOHeader.ROHS_VALUE = rowWOHeader[0]["ROHS_VALUE"].ToString();
                    rowRWOHeader.FTRMI = rowWOHeader[0]["FTRMI"].ToString();
                    rowRWOHeader.MVGR3 = rowWOHeader[0]["MVGR3"].ToString();
                    rowRWOHeader.WEMNG = rowWOHeader[0]["WEMNG"].ToString();
                    rowRWOHeader.BISMT = rowWOHeader[0]["BISMT"].ToString();
                    rowRWOHeader.CHARG = rowWOHeader[0]["CHARG"].ToString();
                    rowRWOHeader.SAENR = rowWOHeader[0]["SAENR"].ToString();
                    rowRWOHeader.AETXT = rowWOHeader[0]["AETXT"].ToString();
                    rowRWOHeader.GLTRP = rowWOHeader[0]["GLTRP"].ToString();
                    sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(sql);
                    rowWOItem = null;
                    rowWOItem = dtWOItem.Select("AUFNR='" + rowWOHeader[0]["AUFNR"].ToString() + "'");
                    SaveWOItem(rowWOItem);
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOHeader", ip + ";" + _downloadWO + "; Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                    string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='Download r_wo_header failed: " + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='" + _downloadWO + "'";
                    SFCDB.ExecSQL(strSQLWOError);
                }
            }
            else
            {
                for (int i = 0; i < dtWOHeader.Rows.Count; i++)
                {
                    sql = "";
                    woIsExist = false;
                    try
                    {
                        skuIsExist = C_SKU.CheckSku(dtWOHeader.Rows[i]["MATNR"].ToString(), SFCDB);
                        if (!skuIsExist)
                        {
                            throw new Exception(" sku " + dtWOHeader.Rows[i]["MATNR"].ToString() + " is not existing!");
                        }
                        woIsExist = R_WO_HEADER.CheckWoHeadByWo(dtWOHeader.Rows[i]["AUFNR"].ToString(), true, SFCDB);
                        if (woIsExist)
                        {
                            continue;
                        }
                        Row_R_WO_HEADER rowRWOHeader = (Row_R_WO_HEADER)R_WO_HEADER.NewRow();
                        rowRWOHeader.ID = R_WO_HEADER.GetNewID(BU, SFCDB);
                        rowRWOHeader.AUFNR = dtWOHeader.Rows[i]["AUFNR"].ToString();
                        rowRWOHeader.WERKS = dtWOHeader.Rows[i]["WERKS"].ToString();
                        rowRWOHeader.AUART = dtWOHeader.Rows[i]["AUART"].ToString();
                        rowRWOHeader.MATNR = dtWOHeader.Rows[i]["MATNR"].ToString();
                        rowRWOHeader.REVLV = dtWOHeader.Rows[i]["REVLV"].ToString();
                        rowRWOHeader.KDAUF = dtWOHeader.Rows[i]["KDAUF"].ToString();
                        rowRWOHeader.GSTRS = dtWOHeader.Rows[i]["GSTRS"].ToString();
                        rowRWOHeader.GAMNG = dtWOHeader.Rows[i]["GAMNG"].ToString();
                        rowRWOHeader.KDMAT = dtWOHeader.Rows[i]["KDMAT"].ToString();
                        rowRWOHeader.AEDAT = dtWOHeader.Rows[i]["AEDAT"].ToString();
                        rowRWOHeader.AENAM = dtWOHeader.Rows[i]["AENAM"].ToString();
                        rowRWOHeader.MATKL = dtWOHeader.Rows[i]["MATKL"].ToString();
                        rowRWOHeader.MAKTX = dtWOHeader.Rows[i]["MAKTX"].ToString();
                        rowRWOHeader.ERDAT = dtWOHeader.Rows[i]["ERDAT"].ToString();
                        rowRWOHeader.GSUPS = dtWOHeader.Rows[i]["GSUPS"].ToString();
                        rowRWOHeader.ERFZEIT = dtWOHeader.Rows[i]["ERFZEIT"].ToString();
                        rowRWOHeader.GLTRS = dtWOHeader.Rows[i]["GLTRS"].ToString();
                        rowRWOHeader.GLUPS = dtWOHeader.Rows[i]["GLUPS"].ToString();
                        rowRWOHeader.LGORT = dtWOHeader.Rows[i]["LGORT"].ToString();
                        rowRWOHeader.ABLAD = dtWOHeader.Rows[i]["ABLAD"].ToString();
                        rowRWOHeader.ROHS_VALUE = dtWOHeader.Rows[i]["ROHS_VALUE"].ToString();
                        rowRWOHeader.FTRMI = dtWOHeader.Rows[i]["FTRMI"].ToString();
                        rowRWOHeader.MVGR3 = dtWOHeader.Rows[i]["MVGR3"].ToString();
                        rowRWOHeader.WEMNG = dtWOHeader.Rows[i]["WEMNG"].ToString();
                        rowRWOHeader.BISMT = dtWOHeader.Rows[i]["BISMT"].ToString();
                        rowRWOHeader.CHARG = dtWOHeader.Rows[i]["CHARG"].ToString();
                        rowRWOHeader.SAENR = dtWOHeader.Rows[i]["SAENR"].ToString();
                        rowRWOHeader.AETXT = dtWOHeader.Rows[i]["AETXT"].ToString();
                        rowRWOHeader.GLTRP = dtWOHeader.Rows[i]["GLTRP"].ToString();
                        sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(sql);
                        rowWOItem = null;
                        rowWOItem = dtWOItem.Select("AUFNR='" + dtWOHeader.Rows[i]["AUFNR"].ToString() + "'");
                        SaveWOItem(rowWOItem);

                        SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOHeader", ip + ";" + dtWOHeader.Rows[i]["AUFNR"].ToString() + ";Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                        string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='Download r_wo_header failed: " + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='" + dtWOHeader.Rows[i]["AUFNR"].ToString() + "'";
                        SFCDB.ExecSQL(strSQLWOError);
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// downlaod wo info into r_wo_item from sap
        /// </summary>
        protected void SaveWOItem(DataRow[] rowDownloadItem)
        {
            for (int m = 0; m < rowDownloadItem.Length; m++)
            {
                sql = "";
                woIsExist = false;
                try
                {
                    woIsExist = R_WO_ITEM.CheckWoItemByWo(rowDownloadItem[m]["AUFNR"].ToString(), rowDownloadItem[m]["MATNR"].ToString(), true, SFCDB);
                    if (woIsExist)
                    {
                        sql = "Update R_WO_ITEM set PARTS= PARTS + " + rowDownloadItem[m]["PARTS"].ToString() + ",BDMNG = BDMNG + "+ rowDownloadItem[m]["BDMNG"].ToString() + " where AUFNR='" + rowDownloadItem[m]["AUFNR"].ToString() + "' and MATNR = '"+ rowDownloadItem[m]["MATNR"].ToString() + "'";

                    }
                    else
                    {
                        Row_R_WO_ITEM rowRWOItem = (Row_R_WO_ITEM)R_WO_ITEM.NewRow();
                        rowRWOItem.ID = R_WO_ITEM.GetNewID(BU, SFCDB);
                        rowRWOItem.AUFNR = rowDownloadItem[m]["AUFNR"].ToString();
                        rowRWOItem.POSNR = rowDownloadItem[m]["POSNR"].ToString();
                        rowRWOItem.MATNR = rowDownloadItem[m]["MATNR"].ToString();
                        rowRWOItem.PARTS = rowDownloadItem[m]["PARTS"].ToString();
                        rowRWOItem.KDMAT = rowDownloadItem[m]["KDMAT"].ToString();
                        rowRWOItem.BDMNG = rowDownloadItem[m]["BDMNG"].ToString();
                        rowRWOItem.MEINS = rowDownloadItem[m]["MEINS"].ToString();
                        rowRWOItem.REVLV = rowDownloadItem[m]["REVLV"].ToString();
                        rowRWOItem.BAUGR = rowDownloadItem[m]["BAUGR"].ToString();
                        rowRWOItem.REPNO = rowDownloadItem[m]["REPNO"].ToString();
                        rowRWOItem.REPPARTNO = rowDownloadItem[m]["REPPARTNO"].ToString();
                        rowRWOItem.AUART = rowDownloadItem[m]["AUART"].ToString();
                        rowRWOItem.AENAM = rowDownloadItem[m]["AENAM"].ToString();
                        rowRWOItem.AEDAT = rowDownloadItem[m]["AEDAT"].ToString();
                        rowRWOItem.MAKTX = rowDownloadItem[m]["MAKTX"].ToString();
                        rowRWOItem.MATKL = rowDownloadItem[m]["MATKL"].ToString();
                        rowRWOItem.WGBEZ = rowDownloadItem[m]["WGBEZ"].ToString();
                        rowRWOItem.ALPOS = rowDownloadItem[m]["ALPOS"].ToString();
                        rowRWOItem.ABLAD = rowDownloadItem[m]["ABLAD"].ToString();
                        rowRWOItem.MVGR3 = rowDownloadItem[m]["MVGR3"].ToString();
                        rowRWOItem.RGEKZ = rowDownloadItem[m]["RGEKZ"].ToString();
                        rowRWOItem.LGORT = rowDownloadItem[m]["LGORT"].ToString();
                        rowRWOItem.ENMNG = rowDownloadItem[m]["ENMNG"].ToString();
                        rowRWOItem.DUMPS = rowDownloadItem[m]["DUMPS"].ToString();
                        rowRWOItem.BISMT = rowDownloadItem[m]["BISMT"].ToString();
                        rowRWOItem.XLOEK = rowDownloadItem[m]["XLOEK"].ToString();
                        rowRWOItem.SHKZG = rowDownloadItem[m]["SHKZG"].ToString();
                        rowRWOItem.CHARG = rowDownloadItem[m]["CHARG"].ToString();
                        rowRWOItem.RSPOS = rowDownloadItem[m]["RSPOS"].ToString();
                        rowRWOItem.VORNR = rowDownloadItem[m]["VORNR"].ToString();
                        sql = rowRWOItem.GetInsertString(DB_TYPE_ENUM.Oracle);
                       
                    }
                    SFCDB.ExecSQL(sql);
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOItem", ip + ";" + rowDownloadItem[m]["AUFNR"].ToString() + "; Download r_wo_item  fail," + ex.Message.ToString(), sql, "interface");
                    string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='Download r_wo_item failed: " + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='" + rowDownloadItem[m]["AUFNR"].ToString() + "'";
                    SFCDB.ExecSQL(strSQLWOError);
                    continue;
                }
            }
        }
        /// <summary>
        /// downlaod wo info into r_wo_text from sap
        /// </summary>
        protected void SaveWOText()
        {
            for (int m = 0; m < dtWOText.Rows.Count; m++)
            {
                sql = "";
                woIsExist = false;
                try
                {
                    woIsExist = R_WO_TEXT.CheckWoTextByWo(dtWOText.Rows[m]["AUFNR"].ToString(), dtWOText.Rows[m]["LTXA1"].ToString(), true, SFCDB);
                    if (woIsExist)
                    {
                        continue;
                    }
                    Row_R_WO_TEXT rowRWOText = (Row_R_WO_TEXT)R_WO_TEXT.NewRow();
                    rowRWOText.ID = R_WO_TEXT.GetNewID(BU, SFCDB);
                    rowRWOText.AUFNR = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.MATNR = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.ARBPL = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.LTXA1 = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.ISAVD = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.VORNR = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.MGVRG = dtWOText.Rows[m]["AUFNR"].ToString();
                    rowRWOText.LMNGA = dtWOText.Rows[m]["AUFNR"].ToString();
                    sql = rowRWOText.GetInsertString(DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(sql);
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    //write log 
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SaveWOText", ip + ";" + dtWOText.Rows[m]["AUFNR"].ToString() + ";Down load r_wo_text fail," + ex.Message.ToString(), "", "interface");
                    continue;
                }
            }
        }
        /// <summary>
        /// convert wo
        /// </summary>    
        protected void ConvertWO()
        {
            C_SKU Sku = null;
            if (_downloadWO != "")
            {
                dtConvertWO = R_WO_HEADER.GetConvertWoTableByWO(SFCDB, DB_TYPE_ENUM.Oracle, _downloadWO);
            }
            else
            {
                dtConvertWO = R_WO_HEADER.GetConvertWoTable(SFCDB, DB_TYPE_ENUM.Oracle, arrayConvertWO);
            }
            if (dtConvertWO.Rows.Count > 0)
            {
                foreach (DataRow row in dtConvertWO.Rows)
                {
                    //rowSku = null;
                    rowRoute = null;
                    rowWOBase = null;
                    rowWOType = null;
                    routeDetailList = null;
                    keypartList = null;
                    keypartIDList = null;
                    linkStationList = null;
                    C_Series = null;
                    sql = "";
                    series = "";
                    try
                    {
                        Sku = C_SKU.GetSku(row["MATNR"].ToString(), SFCDB);
                        if (Sku == null)
                        {
                            throw new Exception("(PE) SKU " + row["MATNR"].ToString() + " has not been setup in CloudMES.");
                        }
                        //if (!string.Equals(rowSku.VERSION.ToString(), row["REVLV"].ToString()))
                        //{
                        //    throw new Exception(" The sku version is not the same," + rowSku.VERSION.ToString() + "," + row["REVLV"].ToString());
                        //}
                        

                        if (Sku.C_SERIES_ID.ToString() != "")
                        {
                            C_Series = T_Series.GetDetailById(SFCDB, Sku.C_SERIES_ID);
                            if (C_Series == null)
                            {
                                throw new Exception(" (PE)The series of " + row["MATNR"].ToString() + " has not been setup in CloudMES.");
                            }
                            series = C_Series.SERIES_NAME;
                        }
                        else
                        {
                            series = "VERTIV_DEFAULT";
                        }
                        rowWOType = WOType.GetWOTypeByWO(SFCDB, row["AUART"].ToString());
                        if (rowWOType == null)
                        {
                            throw new Exception("get wo type fail");
                        }
                        rowRoute = (Row_C_ROUTE)C_ROUTE.GetRouteBySkuno(Sku.ID, SFCDB, DB_TYPE_ENUM.Oracle);
                        if (rowRoute == null)
                        {
                            throw new Exception("(PE) The route of " + row["MATNR"].ToString() + " has not been setup in CloudMES.");
                        }
                        routeDetailList = RouteDetail.GetByRouteIdOrderBySEQASC(rowRoute.ID, SFCDB);
                        if (routeDetailList == null || routeDetailList.Count == 0)
                        {
                            throw new Exception("(PE) Get route detail fail by " + rowRoute.ID);
                        }
                        //20190326 Patty added PTO and ATO check.                        
                        if (Sku.SKU_NAME.Contains("-2C")|| Sku.SKU_NAME.Contains("ORACLE_RACK") || Sku.SKU_NAME == "ODA_HA" ||  (Sku.SKU_TYPE == "PTO" && Sku.SKU_NAME == "X8-8") || (Sku.SKU_TYPE == "PTO" && Sku.SKU_NAME == "ODA_X8-2"))
                        {
                            keypartIDList = t_c_kp_list.GetListIDBySkuno(Sku.SKUNO, SFCDB);
                        }
                        else
                        {

                            keypartIDList = t_c_kp_list.GetListIDBySkUNAME(Sku.SKU_NAME, SFCDB);
                        }
                        
                        if (keypartIDList.Count > 0 && keypartIDList.Count != 1)
                        {
                            throw new Exception("Skuno:" + row["MATNR"].ToString() + " have multiple keypart list.");
                        }

                        rowWOBase = (Row_R_WO_BASE)R_WO_BASE.NewRow();
                        rowWOBase.ID = R_WO_BASE.GetNewID(BU, SFCDB);
                        rowWOBase.WORKORDERNO = row["AUFNR"].ToString();
                        rowWOBase.PLANT = row["WERKS"].ToString();
                        rowWOBase.RELEASE_DATE = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        rowWOBase.DOWNLOAD_DATE = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        rowWOBase.PRODUCTION_TYPE = Sku.SKU_TYPE; //20190326 Patty modified.
                        rowWOBase.WO_TYPE = rowWOType.WORKORDER_TYPE;
                        rowWOBase.SKUNO = row["MATNR"].ToString();
                        rowWOBase.SKU_VER = row["REVLV"].ToString();
                        rowWOBase.SKU_SERIES = series;
                        rowWOBase.SKU_NAME = Sku.SKU_NAME;
                        rowWOBase.SKU_DESC = Sku.DESCRIPTION;
                        rowWOBase.CUST_PN = Sku.CUST_PARTNO;
                        rowWOBase.CUST_PN_VER = "";
                        rowWOBase.CUSTOMER_NAME = Sku.CUST_SKU_CODE;
                        rowWOBase.ROUTE_ID = rowRoute.ID;
                        rowWOBase.START_STATION = routeDetailList[0].STATION_NAME;
                        rowWOBase.KP_LIST_ID = (keypartIDList != null && keypartIDList.Count > 0) ? keypartIDList[0] : "";
                        rowWOBase.CLOSED_FLAG = "0";
                        rowWOBase.WORKORDER_QTY = Convert.ToDouble(row["GAMNG"]);
                        rowWOBase.INPUT_QTY = 0;
                        rowWOBase.FINISHED_QTY = 0;
                        rowWOBase.SCRAPED_QTY = 0;
                        rowWOBase.STOCK_LOCATION = row["LGORT"].ToString();
                        rowWOBase.PO_NO = "";
                        rowWOBase.CUST_ORDER_NO = row["ABLAD"].ToString();
                        rowWOBase.ROHS = row["ROHS_VALUE"].ToString();
                        rowWOBase.EDIT_EMP = "interface";
                        rowWOBase.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ThrowSqlExeception = true;
                        sql = rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle));
                        //20190212 patty added: create child WO if 2C product
                        if (Sku.SKU_NAME == "X7-2C" || Sku.SKU_NAME == "E1-2C")
                        {
                            SplitWO(Sku, row["AUFNR"].ToString(), row["WERKS"].ToString(), row["REVLV"].ToString(), Sku.DESCRIPTION, Convert.ToDouble(row["GAMNG"]), row["LGORT"].ToString(), row["ROHS_VALUE"].ToString());
                        }
                        //20190401 patty added: update flag
                        string strSQLWO = "Update R_MFPRESETWOHEAD set SAPFLAG='6',RETURNMESG = 'DownloadWO OK!',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') where WO='" + row["AUFNR"].ToString() + "'";
                        this.SFCDB.ExecSQL(strSQLWO);
                        SFCDB.CommitTrain();

                        
                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "ConvertWO", ip + ";" + row["AUFNR"].ToString() + ";ConvertWO fail," + ex.Message.ToString(), sql, "interface");
                        string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='ConvertWO failed: " + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='" + row["AUFNR"].ToString() + "'";
                        SFCDB.ExecSQL(strSQLWOError);
                    }
                }
            }
        }

        protected void ConvertWOText()
        {
            SaveWOText();
            foreach (DataRow dr in dtWOText.Rows)
            {
                try
                {
                    R_WO_DEVIATION Deviation = new R_WO_DEVIATION()
                    {
                        ID = R_WO_TEXT.GetNewID(BU, SFCDB),
                        WORKORDERNO = dr["AUFNR"].ToString(),
                        DEVIATION = dr["LTXA1"].ToString(),
                        EDIT_EMP = "Interface",
                        EDIT_TIME = DateTime.Now
                    };

                    SFCDB.ORM.Insertable<R_WO_DEVIATION>(Deviation).ExecuteCommand();
                    SFCDB.CommitTrain();
                }
                catch (Exception)
                {
                    continue;
                }

            }

        }


        /// <summary>
        /// Split wo to child wo
        /// 2019/02/05 patty added for 2c product
        /// </summary>    
        protected void SplitWO(C_SKU sku, string PWO, string plant, string skuV, string skuDesc, double WOQTY, string stockLocation, string WOROHS)
        {
            C_SKU ChildSku = null;
            rowWOBaseC = null;
            C_SeriesC = null;
            try
            {
                //check child SKU
                ChildSku = C_SKU.GetSku("SM" + sku.SKUNO, SFCDB);

                string childSeries = "";
                if (ChildSku == null)
                {
                    throw new Exception("(PE) Child sku of " + sku.SKUNO + " has not been setup in CloudMES.");
                }

                if (sku.C_SERIES_ID.ToString() != "")
                {
                    C_SeriesC = T_Series.GetDetailById(SFCDB, ChildSku.C_SERIES_ID);
                    if (C_SeriesC == null)
                    {
                        throw new Exception("(PE) The series of " + ChildSku.SKUNO + " has not been setup in CloudMES.");
                    }
                    childSeries = C_Series.SERIES_NAME;
                }
                else
                {
                    childSeries = "2C CHILD";
                }
                //check child route
                rowRouteC = (Row_C_ROUTE)C_ROUTE.GetRouteBySkuno(ChildSku.ID, SFCDB, DB_TYPE_ENUM.Oracle);
                if (rowRouteC == null)
                {
                    throw new Exception("(PE) The route of " + sku.SKUNO + " has not been setup in CloudMES.");
                }
                routeDetailListC = RouteDetail.GetByRouteIdOrderBySEQASC(rowRouteC.ID, SFCDB);
                if (routeDetailListC == null || routeDetailListC.Count == 0)
                {
                    throw new Exception("(PE) Get route detail fail by " + rowRouteC.ID);
                }
                //check child sku kp list
                keypartIDListC = t_c_kp_list.GetListIDBySkuno(ChildSku.SKUNO, SFCDB);
                if (keypartIDListC.Count > 0 && keypartIDListC.Count != 1)
                {
                    throw new Exception("(PE) Skuno :" + ChildSku.SKUNO + " have multiple keypart list.");
                }

                rowWOBaseC = (Row_R_WO_BASE)R_WO_BASE.NewRow();
                rowWOBaseC.ID = R_WO_BASE.GetNewID(BU, SFCDB);
                rowWOBaseC.WORKORDERNO = "SM" + PWO;
                rowWOBaseC.PLANT = plant;
                rowWOBaseC.RELEASE_DATE = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                rowWOBaseC.DOWNLOAD_DATE = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                rowWOBaseC.PRODUCTION_TYPE = "BTO";
                rowWOBaseC.WO_TYPE = "CHILD";
                rowWOBaseC.SKUNO = ChildSku.SKUNO;
                rowWOBaseC.SKU_VER = skuV;
                rowWOBaseC.SKU_SERIES = childSeries;
                rowWOBaseC.SKU_NAME = childSeries;
                rowWOBaseC.SKU_DESC = skuDesc;
                rowWOBaseC.CUST_PN = ChildSku.SKUNO;
                rowWOBaseC.CUST_PN_VER = "";
                rowWOBaseC.CUSTOMER_NAME = ChildSku.CUST_SKU_CODE;
                rowWOBaseC.ROUTE_ID = rowRouteC.ID;
                rowWOBaseC.START_STATION = routeDetailList[0].STATION_NAME;
                rowWOBaseC.KP_LIST_ID = (keypartIDListC != null && keypartIDListC.Count > 0) ? keypartIDListC[0] : "";
                rowWOBaseC.CLOSED_FLAG = "0";
                rowWOBaseC.WORKORDER_QTY = Convert.ToDouble(WOQTY*2); //two SMs for each unit
                rowWOBaseC.INPUT_QTY = 0;
                rowWOBaseC.FINISHED_QTY = 0;
                rowWOBaseC.SCRAPED_QTY = 0;
                rowWOBaseC.STOCK_LOCATION = stockLocation;
                rowWOBaseC.PO_NO = "";
                rowWOBaseC.CUST_ORDER_NO = "";
                rowWOBaseC.ROHS = WOROHS;
                rowWOBaseC.EDIT_EMP = "interface";
                rowWOBaseC.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                SFCDB.ThrowSqlExeception = true;
                sql = rowWOBaseC.GetInsertString(DB_TYPE_ENUM.Oracle);
                SFCDB.ExecSQL(rowWOBaseC.GetInsertString(DB_TYPE_ENUM.Oracle));
                SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.DownLoadWO", "SplitWO", ip + ";" + PWO + ";ConvertWO fail," + ex.Message.ToString(), sql, "interface");
                string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='SplitWO failed: " + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='SM" + PWO + "'";
                SFCDB.ExecSQL(strSQLWOError);
            }
                
            
        }
    }
}
