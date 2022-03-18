using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESInterface.ORACLE
{
    public class PreUploadWO:taskBase
    {
        #region invoked by other program and intialize all input parameters     
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
        public T_R_WO_BOM R_WO_BOM;
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
        
       //20190323 Patty changed RFC  
        //private ZCPP_NSBG_0005NE ZCPP_NSBG_0005NE;        
        private ZCPP_NSBG_0279 ZCPP_NSBG_0279;
        private string sql = "";
        private string lockIp = "";

        private bool woIsExist = false;
        //private bool skuIsExist = false;
        //private bool autoDownLoad = true;
        private bool IsRuning = false;



        //Initial SAP and return table groups
        private DataTable dtITAB = new DataTable();
        //private DataTable dtWOHeader = new DataTable();
        //private DataTable dtWOItem = new DataTable();
        //private DataTable dtWOText = new DataTable();
        //private DataTable dtConvertWO = new DataTable();
        private DataTable dtWOBOM = new DataTable();
        //private DataRow[] rowWOHeader;
        // private DataRow[] rowWOItem;

        //private Row_C_SKU rowSku;
        //private Row_C_ROUTE rowRoute;
        //private R_WO_TYPE rowWOType;
        //private Row_R_WO_BASE rowWOBase;
        //private List<C_ROUTE_DETAIL> routeDetailList;
        //private List<C_KEYPART> keypartList;
        //private List<string> keypartIDList;
        //private List<C_ROUTE_DETAIL> linkStationList;
        //private C_SERIES C_Series;
        public T_R_MFPRESETWOHEAD t_r_mfpresetwohead;

        public override void init()
        {
            try
            {
                //Get BU/Plant/DB/Cust/Count/ConvertWo/DownloadWO via Ini
                BU = ConfigGet("BU");
                Plant = ConfigGet("PLANT");
                DB = ConfigGet("DB");// Match setting from App.config
                CUST = ConfigGet("CUST");
                COUNT = ConfigGet("COUNT");
                //Follow up W/O prefix to determine which w/o need to get auto converted or manual way
                //arrayConvertWO = ConfigGet("CONVERTWO").Split(',');
                //_downloadWO = ConfigGet("DOWNLOADWO");
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
               
                //Get DB connection from ConnectionManager
                SFCDB = new OleExec(DB, false);
                //program running status            
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                //W/O fields mapping table
                C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(SFCDB, DB_TYPE_ENUM.Oracle);

                //save data into belwo table from SAP
                R_WO_BOM = new T_R_WO_BOM(SFCDB,DB_TYPE_ENUM.Oracle);

                //GET SKU Route
                C_SKU = new T_C_SKU(SFCDB, DB_TYPE_ENUM.Oracle);
                //WO Typre and prefix
                WOType = new T_R_WO_TYPE(SFCDB, DB_TYPE_ENUM.Oracle);

                //create RFC class and setup SAP connection & Input & output
                // ZCPP_NSBG_0005NE = new ZCPP_NSBG_0005NE(BU);
                ZCPP_NSBG_0279 = new ZCPP_NSBG_0279(BU);
            }
            catch (Exception e)
            {
                throw new Exception("Init DownLoadWO Fail" + e.Message);
            }
            //Get return message and assign Output.Tables
            Output.Tables.Add(ZCPP_NSBG_0279.GetTableValue("RETURN"));
            //20190323 Patty: new RFC result table is PODETAIL, BOM_LIST is Cisco pre upload RFC.
            //Output.Tables.Add(ZCPP_NSBG_0279.GetTableValue("BOM_LIST"));
            Output.Tables.Add(ZCPP_NSBG_0279.GetTableValue("PODETAIL"));
        }       
       

        public override void Start()
        {
            //check if current task is running or not :SELECT * From R_SYNC_LOCK where Lock_Name='{Section}'
            IsRuning = synLock.IsLock(Section, SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
            if (IsRuning)
            {
                throw new Exception(Section + " interface is running on " + lockIp + ",Please try again later");
            }
            try
            {
                //TEST();
                //log start transaction
                synLock.SYNC_Lock(BU, ip, Section, Section, "interface", SFCDB, DB_TYPE_ENUM.Oracle);
                //SFCDB.CommitTrain();
                CallRFC();
            }
            catch (Exception ex)
            {
                //SFCDB.RollbackTrain();
                throw new Exception("Start DownLoadWO Fail" + ex.Message);
            }
            finally
            {
                synLock.SYNC_UnLock(BU, ip, Section, Section, "interface", SFCDB, DB_TYPE_ENUM.Oracle);
            }
            //SFCDB.CommitTrain();

        }
        /// <summary>
        /// please assign value for public attributes
        /// </summary>
        private void CallRFC()
        {
            t_r_mfpresetwohead = new T_R_MFPRESETWOHEAD(SFCDB, DB_TYPE_ENUM.Oracle);

            string strWOHead = "select*From R_MFPRESETWOHEAD WHERE  SAPFLAG='2' and (CANCELLED = '0' or CANCELLED is null)";
                // DataTable dtWOHead = this.SFCDB.ExecSelect(strWOHead, null).Tables[0];
                DataTable dtWOHead = this.SFCDB.ExecuteDataSet(strWOHead, CommandType.Text, null).Tables[0];
                for (int i = 0; i < dtWOHead.Rows.Count; i++)
                {
                    try {
                        SFCDB.BeginTrain();
                    
                    //TOG2 = PTO: does not required WO details
                    if (dtWOHead.Rows[i]["PRODUCTION_TYPE"].ToString() == "PTO")
                    {
                        dtITAB.Clear();
                        dtWOBOM.Clear();

                        ZCPP_NSBG_0279.PreUploadSetValuePTO(this.Plant, dtWOHead.Rows[i]["SKUNO"].ToString(), "TOG2", dtWOHead.Rows[i]["WO"].ToString(), dtWOHead.Rows[i]["WOQTY"].ToString(), dtWOHead.Rows[i]["PO"].ToString() + dtWOHead.Rows[i]["POLINE"].ToString(), DateTime.Now.ToString("yyyyMMdd"));
                    }

                    //TOG1 = ATO: required WO details
                    if (dtWOHead.Rows[i]["PRODUCTION_TYPE"].ToString() == "ATO")
                        {
                            string strWODetail = "select*From R_MFPRESETWODETAIL  where  wo='" + dtWOHead.Rows[i]["WO"].ToString() + "'";
                            DataTable dtWODetail = this.SFCDB.ExecSelect(strWODetail, null).Tables[0];

                            dtITAB.Clear();
                            dtWOBOM.Clear();

                            ZCPP_NSBG_0279.PreUploadSetValueATO(this.Plant, dtWOHead.Rows[i]["Groupid"].ToString(), "TOG1", dtWOHead.Rows[i]["WO"].ToString(), dtWOHead.Rows[i]["WOQTY"].ToString(), dtWOHead.Rows[i]["PO"].ToString() + dtWOHead.Rows[i]["POLINE"].ToString(), DateTime.Now.ToString("yyyyMMdd"), dtWODetail);
                        }


                        //Call RFC
                        ZCPP_NSBG_0279.CallRFC();
                        //get RFC Return  result
                        dtITAB = ZCPP_NSBG_0279.GetTableValue("RETURN");
                        //dtWOHeader = ZCPP_NSBG_0005NE.GetTableValue("PODETAIL");
                        dtWOBOM = ZCPP_NSBG_0279.GetTableValue("PODETAIL");

                        Output.Tables.Add(dtITAB);
                        Output.Tables.Add(dtWOBOM);
                        if (dtITAB.Rows.Count > 0)
                        {
                            throw new Exception(dtITAB.Rows[0][3].ToString());
                            
                        }
                        SaveWOBOM(dtWOBOM.Select());

                        string strSQLWO = "Update R_MFPRESETWOHEAD set SAPFLAG='3',RETURNMESG = 'Pre-UploadWO OK!',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') where WO='" + dtWOHead.Rows[i]["WO"].ToString() + "'";
                        this.SFCDB.ExecSQL(strSQLWO);
                        this.SFCDB.CommitTrain();


                    }
                    catch (Exception ex)
                    {
                        //write log
                        SFCDB.RollbackTrain();
                        // WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.ORACLE.PreUploadWO", "CallRFC",  ex.Message.ToString(), "", "interface", dtWOHead.Rows[i]["WO"].ToString(), dtWOHead.Rows[i]["SKUNO"].ToString());
                        t_r_mfpresetwohead.UpdateSAPFLAG(SFCDB, dtWOHead.Rows[i]["WO"].ToString(), ex.Message.ToString());
                        //string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='"+ex.Message.ToString()+"',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('"+  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='" + dtWOHead.Rows[i]["WO"].ToString() + "'";
                          //  SFCDB.ExecSQL(strSQLWOError);
                        continue;
                    }


            }
            
            

        }



        /// <summary>
        /// downlaod wo info into r_wo_item from sap
        /// </summary>
        protected void SaveWOBOM(DataRow[] rowDownloadBOM)
        {
            //20190701 patty added: if data exists, copy to MV then delete
            woIsExist = false;
            woIsExist = R_WO_BOM.CheckWOExist(rowDownloadBOM[0]["AUFNR"].ToString(), SFCDB);
            if (woIsExist)
            {
                //move current BOM to MV and delete.
                T_R_WO_BOM t_r_wo_bom = new T_R_WO_BOM(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_WO_BOM> r_wo_bom_list = t_r_wo_bom.GetWOBOM(rowDownloadBOM[0]["AUFNR"].ToString(), SFCDB);
                foreach (var bom in r_wo_bom_list)
                {
                    SFCDB.ORM.Insertable<R_WO_BOM_MV>(bom).ExecuteCommand();
                }
               
                SFCDB.ORM.Deleteable<R_WO_BOM>().Where(t => t.AUFNR == rowDownloadBOM[0]["AUFNR"].ToString()).ExecuteCommand();                
            }

            for (int m = 0; m < rowDownloadBOM.Length; m++)
            {
                sql = "";
               
                try
                {
                    //20190323 Patty modified columns name, old RFC and new RFC return table not the same.
                    //woIsExist = R_WO_BOM.CheckExist(rowDownloadBOM[m]["AUFNR"].ToString(), rowDownloadBOM[m]["IDNRK"].ToString(),  SFCDB);
                    //20190614 patty modified: allow duplications
                   
                    
                    Row_R_WO_BOM rowRWOBOM = (Row_R_WO_BOM)R_WO_BOM.NewRow();
                    rowRWOBOM.ID = R_WO_BOM.GetNewID(BU, SFCDB);
                    rowRWOBOM.AUFNR = rowDownloadBOM[m]["AUFNR"].ToString();
                    rowRWOBOM.POSNR = rowDownloadBOM[m]["POSNR"].ToString();
                    rowRWOBOM.MATNR = rowDownloadBOM[m]["MATNR"].ToString();
                    rowRWOBOM.REVLV = rowDownloadBOM[m]["REVLV"].ToString();
                    //rowRWOBOM.IDNRK = rowDownloadBOM[m]["IDNRK"].ToString();      //no existed from new RFC
                    rowRWOBOM.MENGE = rowDownloadBOM[m]["BDMNG"].ToString();
                    //rowRWOBOM.PIDREV = rowDownloadBOM[m]["PIDREV"].ToString();    //no existed from new RFC
                    //rowRWOBOM.PINDEX = rowDownloadBOM[m]["PINDEX"].ToString();    //no existed from new RFC          
                    //rowRWOBOM.STUFE = rowDownloadBOM[m]["STUFE"].ToString();      //no existed from new RFC
                    //rowRWOBOM.OPMATNR = rowDownloadBOM[m]["OPMATNR"].ToString();  //no existed from new RFC
                    //rowRWOBOM.SEQNO = rowDownloadBOM[m]["SEQNO"].ToString();      //no existed from new RFC
                    //rowRWOBOM.CMATNR = rowDownloadBOM[m]["CMATNR"].ToString();    //no existed from new RFC
                    // rowRWOBOM.CMENGE = rowDownloadBOM[m]["CMENGE"].ToString();    //no existed from new RFC
                    //rowRWOBOM.PIDNRK = rowDownloadBOM[m]["PIDNRK"].ToString();     //no existed from new RFC
                    rowRWOBOM.PMATNR = rowDownloadBOM[m]["PMATNR"].ToString();
                    // rowRWOBOM.FINDEX = rowDownloadBOM[m]["FINDEX"].ToString();    //no existed from new RFC
                    //rowRWOBOM.POTX1 = rowDownloadBOM[m]["POTX1"].ToString();       //no existed from new RFC
                    //rowRWOBOM.POTX2 = rowDownloadBOM[m]["POTX2"].ToString();       //no existed from new RFC

                    sql = rowRWOBOM.GetInsertString(DB_TYPE_ENUM.Oracle);
                    this.SFCDB.ExecSQL(sql);
                    //  SFCDB.ExecSQL(sql);
                    this.SFCDB.CommitTrain();
                   
                }
                catch (Exception ex)
                {
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.ORACLE.PreUploadWO", "SaveWOBOM",  rowDownloadBOM[m]["AUFNR"].ToString() + "; Save r_wo_bom  fail," + ex.Message.ToString(), sql, "interface", rowDownloadBOM[m]["AUFNR"].ToString(),"");
                    t_r_mfpresetwohead.UpdateSAPFLAG(SFCDB, rowDownloadBOM[m]["AUFNR"].ToString(), ex.Message.ToString());
                    //string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='Save r_wo_bom failed: " + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" +  DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS')  where WO='" + rowDownloadBOM[m]["AUFNR"].ToString() + "'";
                    //SFCDB.ExecSQL(strSQLWOError);
                    continue;
                }
            }
        }
        /// <summary>
        /// downlaod wo info into r_wo_text from sap
        /// </summary>
    }
}
