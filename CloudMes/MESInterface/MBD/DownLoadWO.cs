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

namespace MESInterface.MBD
{
    public class DownLoadWO : taskBase
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
        private R_WO_TYPE rowWOType;
        private Row_R_WO_BASE rowWOBase;
        private List<C_ROUTE_DETAIL> routeDetailList;
        private List<C_KEYPART> keypartList;
        private List<string> keypartIDList;
        private List<C_ROUTE_DETAIL> linkStationList;
        private C_SERIES C_Series;

        public override void init()
        {
            try
            {
                //從Ini檔中獲取BU/Plant/DB/Cust/Count/ConvertWo/DownloadWO
                BU = ConfigGet("BU");
                Plant = ConfigGet("PLANT");
                DB = ConfigGet("DB");//與App.config中配置的匹配
                CUST = ConfigGet("CUST");
                COUNT = ConfigGet("COUNT");
                //可以依工單前綴來確定哪種類型的工單需要自動轉,哪種需手動轉
                arrayConvertWO = ConfigGet("CONVERTWO").Split(',');
                _downloadWO = ConfigGet("DOWNLOADWO");
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                //創建DownloadWO_UI,并附值給輸入對象Output.UI
                Output.UI = new DownLoadWO_UI(this);
                //依據Ini檔配置的DB從ConnectionManager中獲取數據庫連接
                SFCDB = new OleExec(DB, false);
                //程序運行狀態表            
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                //工單欄位對照表
                C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(SFCDB, DB_TYPE_ENUM.Oracle);
                //從SAP Download下來后分別存儲在以下三張表
                R_WO_HEADER = new T_R_WO_HEADER(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_ITEM = new T_R_WO_ITEM(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_TEXT = new T_R_WO_TEXT(SFCDB, DB_TYPE_ENUM.Oracle);
                //獲取料號/Route
                C_SKU = new T_C_SKU(SFCDB, DB_TYPE_ENUM.Oracle);
                RouteDetail = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                C_ROUTE = new T_C_ROUTE(SFCDB, DB_TYPE_ENUM.Oracle);
                T_Series = new T_C_SERIES(SFCDB, DB_TYPE_ENUM.Oracle);
                //工單類型及前綴信息
                WOType = new T_R_WO_TYPE(SFCDB, DB_TYPE_ENUM.Oracle);
                Keypart = new T_C_KEYPART(SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_BASE = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                t_c_kp_list = new T_C_KP_LIST(SFCDB, DB_TYPE_ENUM.Oracle);
                //創建RFC類:建立SAP連接,建立RFC輸入輸出參數
                ZRFC_SFC_NSG_0001B = new ZRFC_SFC_NSG_0001B(BU);
            }
            catch (Exception e)
            {
                throw new Exception("Init DownLoadWO Fail" + e.Message);
            }
            //取得RFC的輸出參數Table對象,并附值給Output.Tables
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("ITAB"));
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("WO_HEADER"));
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("WO_ITEM"));
            Output.Tables.Add(ZRFC_SFC_NSG_0001B.GetTableValue("WO_TEXT"));
        }

        /// <summary>
        /// 补bip allpart data
        /// </summary>
        public void TEST()
        {
            OleExec APDB = new OleExec("HWD_APDB", false);
            string strsql = @"select A.SN,B.PANEL from r_sn_station_detail A, R_PANEL_SN B where A.workorderno = '002510037542' and A.current_station = 'BIP'
                            AND A.SN = B.SN";
            DataSet ds = SFCDB.ExecSelect(strsql);
            try
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        strsql = $@"SELECT p_sn FROM (
                            SELECT DISTINCT(p_sn) FROM MES4.R_TR_PRODUCT_DETAIL WHERE WO = '002510037542' AND P_SN LIKE '{ds.Tables[0].Rows[i]["PANEL"]}%') WHERE ROWNUM =1";
                        string panel_sn = APDB.ExecSelectOneValue(strsql)?.ToString();
                        if (panel_sn == null)
                            continue;
                        strsql = $@"UPDATE MES4.R_TR_PRODUCT_DETAIL SET P_SN='{ds.Tables[0].Rows[i]["SN"]}' WHERE p_SN='{panel_sn}'";
                        APDB.ExecSQL(strsql);
                    }
                    catch (Exception E)
                    {
                        throw E;
                    }
                    finally { }
                }
            }
            catch { }
            finally
            {
                APDB.CloseMe();
            }

        }

        public override void Start()
        {
            //判斷當前任務是否正在被執行:SELECT * From R_SYNC_LOCK where Lock_Name='{Section}'
            IsRuning = synLock.IsLock(Section, SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception(Section + " interface is running on " + lockIp + ",Please try again later");
            }
            try
            {
                //TEST();
                //寫一筆正在執行的記錄
                synLock.SYNC_Lock(BU, ip, Section, Section, "interface", SFCDB, DB_TYPE_ENUM.Oracle);
                SFCDB.CommitTrain();
                CallRFC();
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                throw new Exception("Start DownLoadWO Fail" + ex.Message);
            }
            synLock.SYNC_UnLock(BU, ip, Section, Section, "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            SFCDB.CommitTrain();

        }
        /// <summary>
        /// 外部調用時請賦值public屬性
        /// </summary>
        private void CallRFC()
        {
            try
            {
                //將參數附值給RFCMDT
                ZRFC_SFC_NSG_0001B.SetValue("PLANT", this.Plant);
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
                    ZRFC_SFC_NSG_0001B.SetValue("SCHEDULED_DATE",DateTime.Now.ToString("yyyy-MM-dd"));
                    ZRFC_SFC_NSG_0001B.SetValue("RLDATE", DateTime.Now.ToString("yyyy-MM-dd"));
                }
                //調用RFC
                ZRFC_SFC_NSG_0001B.CallRFC();
                //獲取RFC返回的結果
                dtITAB = ZRFC_SFC_NSG_0001B.GetTableValue("ITAB");
                dtWOHeader = ZRFC_SFC_NSG_0001B.GetTableValue("WO_HEADER");
                dtWOItem = ZRFC_SFC_NSG_0001B.GetTableValue("WO_ITEM");
                dtWOText = ZRFC_SFC_NSG_0001B.GetTableValue("WO_TEXT");
                //？？？？此處Tables是否已經存在
                //異常返回表
                Output.Tables.Add(dtITAB);
                Output.Tables.Add(dtWOHeader);
                Output.Tables.Add(dtWOItem);
                Output.Tables.Add(dtWOText);
                if (dtITAB.Rows.Count > 0)
                {
                    throw new Exception(dtITAB.Rows[0][1].ToString());
                }
                //寫入R_WO_Header&R_WO_Item表
                SaveWOHeader();
                //SaveWOItem();
                //SaveWOText();
                ConvertWO();
                downLoadDate = "";
            }
            catch (Exception ex)
            {
                //write log
                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.MBD.DownLoadWO", "CallRFC", ip + ";" + ex.Message.ToString(), "", "interface");
            }

        }
        /// <summary>
        /// downlaod wo info into r_wo_header from sap
        /// </summary>
        protected void SaveWOHeader()
        {
            //如果ini檔DOWNLOADWO=指定了工單,則按以下執行
            if (_downloadWO != "")
            {
                sql = "";
                woIsExist = false;
                rowWOHeader = null;
                autoDownLoad = false;
                try
                {
                    //從dtWOHeader表中查詢指定的工單
                    rowWOHeader = dtWOHeader.Select("AUFNR='" + _downloadWO + "'");
                    if (rowWOHeader.Length == 0)
                    {
                        throw new Exception("WO " + _downloadWO + " do not exist on sap");
                    }
                    //判斷料號是否存在
                    skuIsExist = C_SKU.CheckSku(rowWOHeader[0]["MATNR"].ToString(), SFCDB);
                    if (!skuIsExist)
                    {
                        throw new Exception(" sku " + rowWOHeader[0]["MATNR"].ToString() + " not exist");
                    }
                    //判斷工單是否已經存在,如果存在則先刪除(因為autoDownLoad=false)
                    woIsExist = R_WO_HEADER.CheckWoHeadByWo(rowWOHeader[0]["AUFNR"].ToString(), autoDownLoad, SFCDB);
                    if (woIsExist)
                    {
                        return;
                    }
                    //寫入R_WO_Header表中       
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
                    //從dtWOItem表中查詢指定的工單信息,并寫入R_WO_Item表中
                    rowWOItem = null;
                    rowWOItem = dtWOItem.Select("AUFNR='" + rowWOHeader[0]["AUFNR"].ToString() + "'");
                    SaveWOItem(rowWOItem);
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.MBD.DownLoadWO", "SaveWOHeader", ip + ";" + _downloadWO + "; Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                }
            }
            //如果ini檔DOWNLOADWO=未指定工單,則按以下執行
            else
            {
                for (int i = 0; i < dtWOHeader.Rows.Count; i++)
                {
                    sql = "";
                    woIsExist = false;
                    try
                    {
                        //判斷料號是否存在
                        skuIsExist = C_SKU.CheckSku(dtWOHeader.Rows[i]["MATNR"].ToString(), SFCDB);
                        if (!skuIsExist)
                        {
                            throw new Exception(" sku " + dtWOHeader.Rows[i]["MATNR"].ToString() + " not exist");
                        }
                        //判斷工單是否已經存在,如果存在則繼續執行下一個WO(因為autoDownLoad=true)
                        woIsExist = R_WO_HEADER.CheckWoHeadByWo(dtWOHeader.Rows[i]["AUFNR"].ToString(), autoDownLoad, SFCDB);
                        if (woIsExist)
                        {
                            continue;
                        }
                        //寫入R_WO_Header表中             
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
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.MBD.DownLoadWO", "SaveWOHeader", ip + ";" + dtWOHeader.Rows[i]["AUFNR"].ToString() + ";Download r_wo_header fail," + ex.Message.ToString(), sql, "interface");
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// 從dtWOItem表中查詢指定的工單信息,并寫入R_WO_Item表中
        /// </summary>
        protected void SaveWOItem(DataRow[] rowDownloadItem)
        {
            for (int m = 0; m < rowDownloadItem.Length; m++)
            {
                sql = "";
                woIsExist = false;
                try
                {
                    //判斷工單Item是否已經存在,如果存在則先刪除(如果autoDownLoad=false)
                    woIsExist = R_WO_ITEM.CheckWoItemByWo(rowDownloadItem[m]["AUFNR"].ToString(), rowDownloadItem[m]["MATNR"].ToString(), autoDownLoad, SFCDB);
                    if (woIsExist)
                    {
                        continue;
                    }
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
                    SFCDB.ExecSQL(sql);
                    SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.MBD.DownLoadWO", "SaveWOItem", ip + ";" + rowDownloadItem[m]["AUFNR"].ToString() + "; Download r_wo_item  fail," + ex.Message.ToString(), sql, "interface");
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
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.MBD.DownLoadWO", "SaveWOText", ip + ";" + dtWOText.Rows[m]["AUFNR"].ToString() + ";Down load r_wo_text fail," + ex.Message.ToString(), "", "interface");
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
                //如果指定了工單Download,且工單不存在于R_WO_BASE表中,則返回該工單R_WO_Header記錄
                dtConvertWO = R_WO_HEADER.GetConvertWoTableByWO(SFCDB, DB_TYPE_ENUM.Oracle, _downloadWO);
            }
            else
            {
                //否則抓取所有不存在于R_WO_BASE表中工單,并結合CONVERTWO=00251的設定返回工單R_WO_Header記錄
                //MBD 正常工單類型,且ModelType不包含094的才轉
                dtConvertWO = R_WO_HEADER.GetConvertWoTableForMBD(SFCDB, DB_TYPE_ENUM.Oracle, arrayConvertWO);
            }
            if (dtConvertWO.Rows.Count > 0)
            {
                //循環所有待轉工單
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
                    try
                    {
                        //判斷料號是否存在
                        Sku = C_SKU.GetSku(row["MATNR"].ToString(), SFCDB);
                        if (Sku == null)
                        {
                            throw new Exception(" sku " + row["MATNR"].ToString() + " not exist");
                        }
                        if (Sku.C_SERIES_ID == null)
                        {
                            throw new Exception(" the series of " + row["MATNR"].ToString() + " can not be null ");
                        }
                        //從C_Series表中獲取系列信息
                        C_Series = T_Series.GetDetailById(SFCDB, Sku.C_SERIES_ID);
                        if (C_Series == null)
                        {
                            throw new Exception(" the series of " + row["MATNR"].ToString() + " not exist");
                        }
                        //從R_WO_TYPE表中獲取工單類型:這張表需要事先設定
                        rowWOType = WOType.GetWOTypeByWO(SFCDB, row["AUART"].ToString());
                        if (rowWOType == null)
                        {
                            throw new Exception("get wo type fail");
                        }
                        //從R_Sku_Route&C_Route表中獲取默認路由
                        rowRoute = (Row_C_ROUTE)C_ROUTE.GetDefaultRouteBySkuno(Sku.ID, SFCDB, DB_TYPE_ENUM.Oracle);
                        if (rowRoute == null)
                        {
                            throw new Exception(" the route of " + row["MATNR"].ToString() + " not exist");
                        }
                        //從C_Route_Detail表中獲取路由  
                        routeDetailList = RouteDetail.GetByRouteIdOrderBySEQASC(rowRoute.ID, SFCDB);
                        if (routeDetailList == null || routeDetailList.Count == 0)
                        {
                            throw new Exception("get route detail fail by " + rowRoute.ID);
                        }
                        //???KeyParts如何轉
                        keypartIDList = t_c_kp_list.GetListIDBySkuno(Sku.SKUNO, SFCDB);
                        if (keypartIDList.Count > 0 && keypartIDList.Count != 1)
                        {
                            throw new Exception("skuno:" + row["MATNR"].ToString() + " have more keypart id");
                        }
                        rowWOBase = (Row_R_WO_BASE)R_WO_BASE.NewRow();
                        rowWOBase.ID = R_WO_BASE.GetNewID(BU, SFCDB);
                        rowWOBase.WORKORDERNO = row["AUFNR"].ToString();
                        rowWOBase.PLANT = row["WERKS"].ToString();
                        rowWOBase.RELEASE_DATE = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        rowWOBase.DOWNLOAD_DATE = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        rowWOBase.PRODUCTION_TYPE = "BTO";//原有邏輯寫死                       
                        rowWOBase.WO_TYPE = rowWOType.WORKORDER_TYPE;
                        rowWOBase.SKUNO = row["MATNR"].ToString();
                        rowWOBase.SKU_VER = row["REVLV"].ToString();
                        rowWOBase.SKU_SERIES = C_Series.SERIES_NAME;
                        rowWOBase.SKU_NAME = Sku.SKU_NAME;
                        rowWOBase.SKU_DESC = Sku.DESCRIPTION;
                        rowWOBase.CUST_PN = Sku.CUST_PARTNO;
                        rowWOBase.CUST_PN_VER = "";
                        rowWOBase.CUSTOMER_NAME = Sku.CUST_SKU_CODE;
                        rowWOBase.ROUTE_ID = rowRoute.ID;
                        rowWOBase.START_STATION = routeDetailList[0].STATION_NAME;
                        //??????
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
                        SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.MBD.DownLoadWO", "ConvertWO", ip + ";" + row["AUFNR"].ToString() + ";ConvertWO fail," + ex.Message.ToString(), sql, "interface");
                    }
                }
            }
        }
    }

}

