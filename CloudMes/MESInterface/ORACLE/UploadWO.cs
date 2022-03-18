using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MESPubLab;


namespace MESInterface.ORACLE
{
    public class UploadWO : taskBase
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
        public T_R_MFPRESETWOHEAD t_r_mfpresetwohead;
        #endregion


        private ZCPP_NSBG_0279 ZCPP_NSBG_0279;
        private string sql = "";
        private string lockIp = "";

        //private bool woIsExist = false;
        private bool skuIsExist = false;
        //private bool autoDownLoad = true;
        private bool IsRuning = false;



        //Initial SAP and return table groups
        private DataTable dtITAB = new DataTable();
        private DataTable dtWOBOM = new DataTable();


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

            string strWOHead = "select*From R_MFPRESETWOHEAD WHERE  SAPFLAG='4' and (CANCELLED = '0' or CANCELLED is null)";
            // DataTable dtWOHead = this.SFCDB.ExecSelect(strWOHead, null).Tables[0];
            DataTable dtWOHead = this.SFCDB.ExecuteDataSet(strWOHead, CommandType.Text, null).Tables[0];
            // dtITAB.Clear();
            for (int i = 0; i < dtWOHead.Rows.Count; i++)
            {
                try
                {
                    SFCDB.BeginTrain();
                    //20190323 Patty added SKU must be existed in C_SKU
                    skuIsExist = C_SKU.CheckSku(dtWOHead.Rows[i]["SKUNO"].ToString(), SFCDB);
                    if (!skuIsExist)
                    {
                        throw new Exception("(PE) SKU " + dtWOHead.Rows[i]["SKUNO"].ToString() + " has not been setup in CloudMES.");
                    }
                    //TOG2 = PTO: does not required WO details
                    if (dtWOHead.Rows[i]["PRODUCTION_TYPE"].ToString() == "PTO")
                    {                        
                        dtWOBOM.Clear();

                        ZCPP_NSBG_0279.UploadSetValuePTO(this.Plant, dtWOHead.Rows[i]["SKUNO"].ToString(), "TOG2", dtWOHead.Rows[i]["WO"].ToString(), dtWOHead.Rows[i]["WOQTY"].ToString(), dtWOHead.Rows[i]["PO"].ToString() + dtWOHead.Rows[i]["POLINE"].ToString(), DateTime.Now.ToString("yyyyMMdd"));
                    }

                    //TOG1 = ATO: required WO details
                    if (dtWOHead.Rows[i]["PRODUCTION_TYPE"].ToString() == "ATO")
                    {
                        string strWODetail = "select*From R_MFPRESETWODETAIL  where  wo='" + dtWOHead.Rows[i]["WO"].ToString() + "'";
                        DataTable dtWODetail = this.SFCDB.ExecSelect(strWODetail, null).Tables[0];
                        dtWOBOM.Clear();
                        ZCPP_NSBG_0279.UploadSetValueATO(this.Plant, dtWOHead.Rows[i]["Groupid"].ToString(), "TOG1", dtWOHead.Rows[i]["WO"].ToString(), dtWOHead.Rows[i]["WOQTY"].ToString(), dtWOHead.Rows[i]["PO"].ToString() + dtWOHead.Rows[i]["POLINE"].ToString(), DateTime.Now.ToString("yyyyMMdd"), dtWODetail);
                    }


                    //Call RFC
                    ZCPP_NSBG_0279.CallRFC();
                    //get RFC Return  result
                    dtITAB = ZCPP_NSBG_0279.GetTableValue("RETURN");
                    //dtWOBOM = ZCPP_NSBG_0279.GetTableValue("PODETAIL");

                    Output.Tables.Add(dtITAB);
                    //Output.Tables.Add(dtWOBOM);

                    if (dtITAB.Rows.Count > 0)
                    {
                        if (!dtITAB.Rows[0][3].ToString().Contains("saved"))
                        {
                            throw new Exception(dtITAB.Rows[0][3].ToString());
                        }
                        //else
                        //{
                        //    SaveWOBOM(dtWOBOM.Select(), dtWOHead.Rows[i]["WO"].ToString());
                        //}
                    }


                    string strSQLWO = "Update R_MFPRESETWOHEAD set SAPFLAG='5',RETURNMESG = 'UploadWO OK!',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') where WO='" + dtWOHead.Rows[i]["WO"].ToString() + "'";
                    this.SFCDB.ExecSQL(strSQLWO);
                    this.SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {                     
                    //write log
                    SFCDB.RollbackTrain();
                    //WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.ORACLE.UploadWO", "CallRFC", ex.Message.ToString(), "", "interface", dtWOHead.Rows[i]["WO"].ToString(), dtWOHead.Rows[i]["SKUNO"].ToString());
                    t_r_mfpresetwohead.UpdateSAPFLAG(SFCDB, dtWOHead.Rows[i]["WO"].ToString(), ex.Message.ToString());
                    //string strSQLWOError = "Update R_MFPRESETWOHEAD set RETURNMESG='" + ex.Message.ToString() + "',EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') where WO='" + dtWOHead.Rows[i]["WO"].ToString() + "'";
                    //SFCDB.ExecSQL(strSQLWOError);
                    continue;
                }


            } // edn loop

        }

        // <summary>
        /// downlaod wo info into r_wo_item from sap
        /// </summary>
        protected void SaveWOBOM(DataRow[] rowDownloadBOM, string wo)
        {
            //2019/04/03 Patty modified: Clear BOM and re pull again
            if (rowDownloadBOM.Length > 0)
            {
                string strSQLBOM = "Delete R_WO_BOM where WO='" + wo + "'";
                this.SFCDB.ExecSQL(strSQLBOM);
                this.SFCDB.CommitTrain();
            }
            else
            {
                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.ORACLE.UploadWO", "SaveWOBOM",  "(SAP) No WO BOM returned from SAP.", "", "interface", wo, "");
            }
            

            for (int m = 0; m < rowDownloadBOM.Length; m++)
            {
                sql = "";               
                try
                {                   
                    Row_R_WO_BOM rowRWOBOM = (Row_R_WO_BOM)R_WO_BOM.NewRow();
                    rowRWOBOM.ID = R_WO_BOM.GetNewID(BU, SFCDB);
                    rowRWOBOM.AUFNR = rowDownloadBOM[m]["AUFNR"].ToString();
                    rowRWOBOM.POSNR = rowDownloadBOM[m]["POSNR"].ToString();
                    rowRWOBOM.MATNR = rowDownloadBOM[m]["MATNR"].ToString();
                    rowRWOBOM.REVLV = rowDownloadBOM[m]["REVLV"].ToString();
                    rowRWOBOM.MENGE = rowDownloadBOM[m]["BDMNG"].ToString();
                    rowRWOBOM.PMATNR = rowDownloadBOM[m]["PMATNR"].ToString();

                    sql = rowRWOBOM.GetInsertString(DB_TYPE_ENUM.Oracle);
                    this.SFCDB.ExecSQL(sql);
                    this.SFCDB.CommitTrain();

                }
                catch (Exception ex)
                {
                    WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.ORACLE.UploadWO", "SaveWOBOM", rowDownloadBOM[m]["AUFNR"].ToString() + "; Download r_wo_bom  fail," + ex.Message.ToString(), sql, "interface", rowDownloadBOM[m]["AUFNR"].ToString(), "");
                    continue;
                }
            }
        }



    }
}
