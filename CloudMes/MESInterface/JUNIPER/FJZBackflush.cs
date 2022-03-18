using MESDataObject.Module;
using MESDBHelper;
using MESPubLab;
using MESPubLab.MESInterface;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.JUNIPER
{
     public class FJZBackflush :taskBase
    {
        private string _BU;
        private DataTable _SAPWaitForBackFlushWo;
        ZRFC_SFC_NSG_0023T SAPINFRFC;
        List<MESInterface.HWD.BackFlush.R_BACKFLUSH_CHECK> bkList;
        T_R_SYNC_LOCK TR_SYNC_LOCK;
        string IP = "";
        string DBName = "";
        string Plant = "";
        OleExec sfcdb;
        string REWORK = "";
        bool isRun = false;

        public delegate void AddDataGridDelegate(string dgvname, DataTable dt);
        public delegate void SetCtrlEnableDelegate(string clname, bool b);
        public delegate void ProcessRateDelegate(string BarName, string LabName, int AllCount, int CerCount);
        public ProcessRateDelegate processRateDelegate;
        public AddDataGridDelegate addDataGridDelegate;
        public SetCtrlEnableDelegate setCtrlEnableDelegate;

        public override void init()
        {
            try
            {
                _BU = ConfigGet("BU");
                DBName = ConfigGet("DB");
                Plant = System.Configuration.ConfigurationManager.AppSettings[_BU + "_SAP_Plant"];
                REWORK = ConfigGet("REWORKWO");
                Output.UI = new frmBackFlush(this);
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                IP = temp[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public override void Start()
        {
            try
            {
                GetSAPWaitForBackFlushWo();
                ToBackFlushCheck();
                CallRfcBackFlush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string BU
        {
            get { return _BU; }
        }
        public DataTable SAPWaitForBackFlushWo
        {
            get { return _SAPWaitForBackFlushWo; }
        }
        public void GetSAPWaitForBackFlushWo()
        {
            if (isRun == false)
            {
                try
                {
                    SAPINFRFC = new ZRFC_SFC_NSG_0023T(BU);
                    SAPINFRFC.SetRFCValue(Plant);
                    SAPINFRFC.CallRFC();
                    _SAPWaitForBackFlushWo = SAPINFRFC.RETTABLE;
                    if (this.addDataGridDelegate != null)
                    {
                        this.addDataGridDelegate("dgSAPDATA", _SAPWaitForBackFlushWo);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void ToBackFlushCheck()
        {
            if (!isRun)
            {
                sfcdb = new OleExec(DBName, false);

                if (InterfacePublicValues.IsMonthly(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle))
                {
                    //月結不給拋賬
                    throw new Exception("This time is monthly,can't BackFlush");
                }

                if (_SAPWaitForBackFlushWo != null && _SAPWaitForBackFlushWo.Rows.Count > 0)
                {
                    T_R_BACKFLUSH_CHECK TR_BACKFLUSH_CHECK = new T_R_BACKFLUSH_CHECK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    DataSet BackFlushCheckTable = TR_BACKFLUSH_CHECK.GetTableConstruct(sfcdb);
                    WOBase WO;
                    T_R_BACKFLUSH_HISTORY TR_BACKFLUSH_HISTORY = new T_R_BACKFLUSH_HISTORY(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_R_MRB TR_MRB = new T_R_MRB(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_R_SN t_r_sn = new T_R_SN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    foreach (DataRow dr in _SAPWaitForBackFlushWo.Rows)
                    {
                        //if (dr["AUFNR"].ToString().Trim() != "009400000060")
                        //{
                        //    continue;
                        //}
                        //沒有在MES系統中生產,那就不用拋賬
                        //if (t_r_sn.GetSNByWo(dr["AUFNR"].ToString().Trim(), sfcdb).Rows.Count <= 0)
                        //{
                        //    continue;
                        //}
                        //使用查詢過站記錄的方式來判斷是否在 MES 系統中有生產
                        if (!sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Any(t => t.WORKORDERNO == dr["AUFNR"].ToString().Trim()))
                        {
                            continue;
                        }
                        DataRow bkItem = BackFlushCheckTable.Tables[0].NewRow();
                        bkItem["WORKORDERNO"] = dr["AUFNR"];
                        bkItem["SKUNO"] = dr["MATNR"];
                        bkItem["SAP_STATION"] = dr["VORNR"];
                        bkItem["WORKORDER_QTY"] = Convert.ToInt64(Math.Ceiling(Convert.ToDecimal(dr["MGVRG"].ToString())));
                        bkItem["BACKFLUSH_QTY"] = Convert.ToInt64(Math.Ceiling(Convert.ToDecimal(dr["LMNGA"].ToString())));
                        try
                        {
                            WO = new WOBase(dr["AUFNR"].ToString().Trim(), sfcdb);
                            string SFCStation = WO.SAPMapping.GetSFCStation(bkItem["SAP_STATION"].ToString());
                            if (dr["AUFNR"].ToString().IndexOf("00257") == 0 || dr["AUFNR"].ToString().IndexOf("00261") == 0)
                            {
                                //SFCStation = WO.Route.GetLastStation();
                                //VNDCN 的002618是正常工單但是HWD的00261是重工工單，這裡會混淆 Edit By ZHB 20201009
                                if (!BU.Contains("DCN"))
                                    SFCStation = WO.Route.GetLastStation();
                            }
                            bkItem["SFC_STATION"] = SFCStation;
                            string LastSapStation = WO.SAPMapping.GetLastSAPStationCode();

                            if (dr["VORNR"].ToString() != LastSapStation)
                            {
                                if (dr["AUFNR"].ToString().IndexOf("00257") == 0 || dr["AUFNR"].ToString().IndexOf("00261") == 0)
                                {
                                    //處理HWD重工工單
                                    //bkItem["SFC_QTY"] = WO.GetStationPassCount(SFCStation, false, sfcdb);
                                    //VNDCN 的002618是正常工單但是HWD的00261是重工工單，這裡會混淆 Edit By ZHB 20201009
                                    if (!BU.Contains("DCN"))
                                        //處理HWD重工工單
                                        bkItem["SFC_QTY"] = WO.GetStationPassCount(SFCStation, false, sfcdb);
                                    else
                                        bkItem["SFC_QTY"] = WO.GetStationPassCount(SFCStation, true, sfcdb);
                                }
                                else
                                {
                                    bkItem["SFC_QTY"] = WO.GetStationPassCount(SFCStation, true, sfcdb);
                                }
                            }
                            else
                            {
                                bkItem["SFC_QTY"] = WO.GetStationPassCount(SFCStation, false, sfcdb);
                            }


                            //計算歷史拋帳數據                          
                            DataSet tmpData = TR_BACKFLUSH_HISTORY.GetBackFlushData(WO.WORKORDERNO, bkItem["SAP_STATION"].ToString(), sfcdb);

                            bkItem["LAST_SFC_QTY"] = tmpData.Tables[0].Rows[0]["C"];

                            //計算MRB的數量
                            tmpData = TR_MRB.GetSNCountByWO(WO.WORKORDERNO, sfcdb);
                            bkItem["MRB_QTY"] = tmpData.Tables[0].Rows[0]["C"];

                            int SFC_QTY = Int32.Parse(bkItem["SFC_QTY"].ToString());
                            int MRB_QTY = Int32.Parse(bkItem["MRB_QTY"].ToString());
                            float SAP_QTY = float.Parse(bkItem["BACKFLUSH_QTY"].ToString());
                            int LAST_SFC_QTY = Int32.Parse(bkItem["LAST_SFC_QTY"].ToString());

                            int DIFF_QTY = SFC_QTY - LAST_SFC_QTY;
                            bkItem["DIFF_QTY"] = DIFF_QTY;
                            bkItem["DIFF_QTY1"] = DIFF_QTY;
                            bkItem["DIFF_QTY2"] = SFC_QTY + MRB_QTY - SAP_QTY;

                            //處理異常數據
                            if (Int32.Parse(bkItem["DIFF_QTY"].ToString()) < 0)
                            {
                                WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface",
                            "MESInterface.HWD.BackFlush.BackFlushHelp",
                            "ToBackFlushCheck", dr["AUFNR"].ToString() + "," + dr["VORNR"].ToString() + ",DATA Err DIFF_QTY < 0", "", "System");
                                bkItem["DIFF_QTY"] = 0;
                            }


                            if (Int32.Parse(bkItem["DIFF_QTY"].ToString())
                                + float.Parse(bkItem["BACKFLUSH_QTY"].ToString())
                                > float.Parse(bkItem["WORKORDER_QTY"].ToString()))
                            {
                                bkItem["DIFF_QTY"] = 0;
                                WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface",
                              "MESInterface.HWD.BackFlush.BackFlushHelp",
                              "ToBackFlushCheck", dr["AUFNR"].ToString() + "," + dr["VORNR"].ToString() + ",DATA Err PassQTY > WORKORDER_QTY", "", "System");
                            }
                            BackFlushCheckTable.Tables[0].Rows.Add(bkItem);
                        }
                        catch (Exception ee)
                        {
                            WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface",
                                "MESInterface.HWD.BackFlush.BackFlushHelp",
                                "ToBackFlushCheck", dr["AUFNR"].ToString() + ":" + ee.Message, "", "System");
                            continue;
                        }
                    }
                    bkList = MESInterface.HWD.BackFlush.R_BACKFLUSH_CHECK.DataAdapert(BackFlushCheckTable, true);
                    if (addDataGridDelegate != null)
                    {
                        addDataGridDelegate("dgBACKFLUSH", BackFlushCheckTable.Tables[0]);
                    }
                    if (setCtrlEnableDelegate != null)
                    {
                        setCtrlEnableDelegate("btnCallRFC", true);
                    }
                }
            }
        }
        public void CallRfcBackFlush()
        {
            if (!isRun)
            {
                sfcdb = new OleExec(DBName, false);
                //判斷是否有其他執行
                TR_SYNC_LOCK = new T_R_SYNC_LOCK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                string lockIP = "";
                string sapMessage = "";
                bool isLock = TR_SYNC_LOCK.IsLock("BACKFLUSH", sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle, out lockIP);
                if (isLock)
                {
                    throw new Exception("BackFlush Is Running,Please Wait!");
                }
                else
                {
                    TR_SYNC_LOCK.SYNC_Lock(BU, IP, "HWD", "BACKFLUSH", "Interface", sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    isRun = true;
                }

                if (InterfacePublicValues.IsMonthly(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle))
                {
                    //月結不給拋賬
                    throw new Exception("This time is monthly,can't BackFlush");
                }


                if (setCtrlEnableDelegate != null)
                {
                    setCtrlEnableDelegate("btnCallRFC", false);
                    setCtrlEnableDelegate("btnStart", false);
                }
                try
                {
                    //ZRFC_SFC_NSG_0009 RFC = new ZRFC_SFC_NSG_0009(_BU);

                    ZRFC_SFC_NSGT_0002 RFC = new ZRFC_SFC_NSGT_0002(_BU);
                    //ZRFC_SFC_NSG_0022 RW_RFC = new ZRFC_SFC_NSG_0022(_BU);
                    ZRFC_SFC_NSGT_0002 RW_RFC = new ZRFC_SFC_NSGT_0002(_BU);
                    string[] ReWorks = REWORK.Split(new char[] { ',' });
                    int curCount = 0;
                    T_R_BACKFLUSH_HISTORY TR_BACKFLUSH_HISTORY = new T_R_BACKFLUSH_HISTORY(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    string ConfigPostDate = InterfacePublicValues.GetPostDate(sfcdb);
                    foreach (MESInterface.HWD.BackFlush.R_BACKFLUSH_CHECK item in bkList)
                    {
                        try
                        {
                            curCount++;
                            if (processRateDelegate != null)
                            {
                                processRateDelegate("BackFlushProgressBar", "labBackFlushProgress", bkList.Count, curCount);
                            }
                            if (item.DIFF_QTY > 0)
                            {
                                bool isRework = false;
                                for (int i = 0; i < ReWorks.Length; i++)
                                {
                                    if (string.IsNullOrEmpty(ReWorks[i]))
                                    {
                                        continue;
                                    }
                                    if (item.WORKORDERNO.IndexOf(ReWorks[i]) == 0)
                                    {
                                        isRework = true;
                                        break;
                                    }
                                }
                                var CofText = "CONFT" + DateTime.Now.ToString("yyyyMMddhhmmssfff");//ADD Confirmation Text
                                if (isRework)
                                {

                                    RW_RFC.SetValue(item.WORKORDERNO, item.SAP_STATION, item.DIFF_QTY.ToString(), ConfigPostDate, CofText);
                                    //RW_RFC.SetValue(item.WORKORDERNO, ConfigPostDate, item.DIFF_QTY.ToString(), item.SAP_STATION, CofText);
                                    RW_RFC.CallRFC();
                                    MESDataObject.Module.R_BACKFLUSH_HISTORY his = new MESDataObject.Module.R_BACKFLUSH_HISTORY();
                                    his.WORKORDERNO = item.WORKORDERNO;
                                    his.WORKORDER_QTY = (double?)item.WORKORDER_QTY;
                                    his.SFC_QTY = (double?)item.SFC_QTY;
                                    his.SFC_STATION = item.SFC_STATION;
                                    his.SKUNO = item.SKUNO;
                                    his.SAP_STATION = item.SAP_STATION;
                                    his.LAST_SFC_QTY = (double?)item.LAST_SFC_QTY;
                                    his.DIFF_QTY = (double?)item.DIFF_QTY;
                                    his.DIFF_QTY1 = (double?)item.DIFF_QTY1;
                                    his.BACK_DATE = GetDBTime(sfcdb);
                                    his.TIMES = 0;
                                    his.DIFF_QTY2 = (double?)item.DIFF_QTY2;
                                    his.MRB_QTY = (double?)item.MRB_QTY;
                                    his.TOSAP = CofText;
                                    string resu = RW_RFC.GetValue("O_FLAG");
                                    if (resu == "0")
                                    {
                                        his.RESULT = "Y";
                                        resu = RW_RFC.GetValue("O_FLAG1");
                                        if (resu == "1")
                                        {
                                            WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface",
                                                "MESInterface.HWD.BackFlush.BackFlushHelp",
                                                "CallRfcBackFlush", his.WORKORDERNO + ","
                                                + his.SAP_STATION + ",521 fail:" +
                                                RW_RFC.GetValue("O_MESSAGE1"), "", "System");
                                        }
                                    }
                                    else
                                    {
                                        his.RESULT = "N";
                                    }
                                    TR_BACKFLUSH_HISTORY.Add(his, sfcdb);
                                }
                                else
                                {
                                    RFC.SetValue(item.WORKORDERNO, item.SAP_STATION, item.DIFF_QTY.ToString(), ConfigPostDate, CofText);
                                    RFC.CallRFC();
                                    MESDataObject.Module.R_BACKFLUSH_HISTORY his = new MESDataObject.Module.R_BACKFLUSH_HISTORY();
                                    his.WORKORDERNO = item.WORKORDERNO;
                                    his.WORKORDER_QTY = (double?)item.WORKORDER_QTY;
                                    his.SFC_QTY = (double?)item.SFC_QTY;
                                    his.SFC_STATION = item.SFC_STATION;
                                    his.SKUNO = item.SKUNO;
                                    his.SAP_STATION = item.SAP_STATION;
                                    his.LAST_SFC_QTY = (double?)item.LAST_SFC_QTY;
                                    his.DIFF_QTY = (double?)item.DIFF_QTY;
                                    his.DIFF_QTY1 = (double?)item.DIFF_QTY1;
                                    his.BACK_DATE = GetDBTime(sfcdb);
                                    his.TIMES = 0;
                                    his.DIFF_QTY2 = (double?)item.DIFF_QTY2;
                                    his.MRB_QTY = (double?)item.MRB_QTY;
                                    his.TOSAP = CofText;
                                    string resu = RFC.GetValue("O_FLAG");
                                    if (resu == "0")
                                    {
                                        his.RESULT = "Y";
                                    }
                                    else
                                    {
                                        his.RESULT = "N";
                                        sapMessage = RFC.GetValue("O_MESSAGE");
                                        WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface", "MESInterface.HWD.BackFlush.BackFlushHelp", "CallRfcBackFlush",
                                            item.WORKORDERNO + "," + item.SAP_STATION + ":" + sapMessage, "", "System");
                                    }
                                    TR_BACKFLUSH_HISTORY.Add(his, sfcdb);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface",
                                      "MESInterface.HWD.BackFlush.BackFlushHelp",
                                      "CallRfcBackFlush", item.WORKORDERNO + "," + item.SAP_STATION + ":" + ex.Message, "", "System");
                            continue;
                        }
                    }
                    isRun = false;
                    TR_SYNC_LOCK.SYNC_UnLock(BU, IP, "HWD", "BACKFLUSH", "Interface", sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    if (setCtrlEnableDelegate != null)
                    {
                        setCtrlEnableDelegate("btnStart", true);
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.WriteIntoMESLog(sfcdb, _BU, "Interface",
                                     "MESInterface.HWD.BackFlush.BackFlushHelp",
                                     "CallRfcBackFlush", ex.Message, "", "System");
                    isRun = false;
                    TR_SYNC_LOCK.SYNC_UnLock(BU, IP, "HWD", "BACKFLUSH", "Interface", sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    if (setCtrlEnableDelegate != null)
                    {
                        setCtrlEnableDelegate("btnStart", true);
                    }
                }
            }
        }
        public DateTime GetDBTime(OleExec sfcdb)
        {

            DateTime DBTime = DateTime.Now;
            string sql = "select  sysdate from dual";
            DataTable table = sfcdb.ExecuteDataTable(sql, CommandType.Text);
            DBTime = (DateTime)table.Rows[0][0];
            return DBTime;
        }

    }
}
