using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.Interface.SAPRFC;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using System.Data;
using MESPubLab.MESStation;
using MESPubLab.MESInterface;
using Newtonsoft.Json.Linq;
using MESPubLab.SAP_RFC;

namespace MESStation.Interface
{
    public class MrbBackFlush: MesAPIBase
    {
        #region 外部調用時，請給以下賦值 
        public MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0020 zrfc_sfc_nsg_0020;
        public MESPubLab.SAP_RFC.ZCPP_NSBG_0091 zcpp_nsg_0091;
        public OleExec SFCDB = null; 
        public string Plant = "";
        public string DB = "";
        public T_R_SYNC_LOCK synLock;
        public T_R_MRB_GT R_MRB_GT;
        public T_H_MRB_GT H_MRB_GT;
        #endregion

        private bool IsRuning = false;
        private string lockIp = "";
        private string postDate;
        private string sql = "";
        private string logMessage = "";       
        private DataTable MRBTable;
        private List<R_MRB_GT> MRBGTList;
        private Row_R_MRB_GT rowR_MRBGT;
        private Row_H_MRB_GT rowH_MRBGT;

        protected APIInfo _MrbBackFlushGT = new APIInfo()
        {
            FunctionName = "MrbBackFlushGT",
            Description = "_MrbBackFlushGT",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDoWOToMRBBackFlush = new APIInfo()
        {
            FunctionName = "DoWOToMRBBackFlush",
            Description = "MRB SN入工單拋賬",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDoAssyToMrbBackFlush = new APIInfo()
        {
            FunctionName = "DoAssyToMrbBackFlush",
            Description = "MRB 組裝退料拋賬",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        public MrbBackFlush()
        {
            //Apis.Add(_MrbBackFlushGT.FunctionName, _MrbBackFlushGT);
            Apis.Add(FDoWOToMRBBackFlush.FunctionName, FDoWOToMRBBackFlush);
            Apis.Add(FDoAssyToMrbBackFlush.FunctionName, FDoAssyToMrbBackFlush);
        }
        public void MrbBackFlushGT(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DoBackFlushGT(this.IP);
                StationReturn.Status = StationReturnStatusValue.Pass;
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }
        public void DoBackFlushGT(string clientip)
        {
            try
            {
                R_MRB_GT = new T_R_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                H_MRB_GT = new T_H_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                zrfc_sfc_nsg_0020 = new MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0020(this.BU);
                zcpp_nsg_0091 = new MESPubLab.SAP_RFC.ZCPP_NSBG_0091(this.BU);
                IsRuning = synLock.IsLock("HWD_MrbBackFlush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
                if (IsRuning)
                {
                    throw new Exception("HWD MrbBackFlush interface is running on " + lockIp + ",Please try again later");
                }
                synLock.SYNC_Lock(BU, clientip, "HWD_MrbBackFlush", "HWD_MrbBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);                
                DoMrbBackFlush();
                synLock.SYNC_UnLock(BU, clientip, "HWD_MrbBackFlush", "HWD_MrbBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
            }
            catch (Exception ex)
            {
                synLock.SYNC_UnLock(BU, clientip, "HWD_MrbBackFlush", "HWD_MrbBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
                throw new Exception("Start MrbBackFlush Fail" + ex.Message);
            }
        }

        public void DoMrbBackFlush()
        {
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                //月結不給拋賬
                throw new Exception("This time is monthly,can't BackFlush");
            }
            else
            {
                WOToMRBBackFlush();
                AssyToMrbBackFlush();
            }
        }

        private void WOToMRBBackFlush()
        {
            MRBGTList = new List<R_MRB_GT>();
            MRBTable = new DataTable();

            sql = $@" select * from r_mrb_gt where sap_flag = '0' and zcpp_flag = 0  and sap_station_code is not null  ";
            MRBTable = SFCDB.RunSelect(sql).Tables[0];
            if (MRBTable != null && MRBTable.Rows.Count > 0)
            {
                foreach (DataRow row in MRBTable.Rows)
                {
                    rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.NewRow();
                    rowR_MRBGT.loadData(row);
                    MRBGTList.Add(rowR_MRBGT.GetDataObject());
                }
            }

            postDate = InterfacePublicValues.GetPostDate(SFCDB);

            if (MRBGTList != null && MRBGTList.Count > 0)
            {
                foreach (R_MRB_GT r_mrb_gt in MRBGTList)
                {
                    rowR_MRBGT = null;
                    rowH_MRBGT = null;
                    logMessage = "";
                    zrfc_sfc_nsg_0020.SetValue("I_AUFNR", r_mrb_gt.WORKORDERNO);
                    zrfc_sfc_nsg_0020.SetValue("I_BUDAT", postDate);
                    zrfc_sfc_nsg_0020.SetValue("I_FLAG", r_mrb_gt.CONFIRMED_FLAG);
                    zrfc_sfc_nsg_0020.SetValue("I_LGORT_TO", r_mrb_gt.TO_STORAGE);
                    zrfc_sfc_nsg_0020.SetValue("I_LMNGA", r_mrb_gt.TOTAL_QTY.ToString());
                    zrfc_sfc_nsg_0020.SetValue("I_STATION", r_mrb_gt.SAP_STATION_CODE);

                    rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.GetObjByID(r_mrb_gt.ID, SFCDB);
                    try
                    {
                        rowR_MRBGT.SAP_FLAG = "1";
                        zrfc_sfc_nsg_0020.CallRFC();
                    }
                    catch (Exception ex)
                    {
                        Interface.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "WOToMRBBackFlush", r_mrb_gt.WORKORDERNO + ";" + this.IP + ";" + ex.ToString(), "", this.LoginUser.EMP_NO);                        
                        rowR_MRBGT.SAP_FLAG = "2";
                    }
                    SFCDB.ExecSQL(rowR_MRBGT.GetUpdateString(DB_TYPE_ENUM.Oracle));

                    logMessage = " M:" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE")
                               + " M1:" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE1")
                               + " M2" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE2");
                    rowH_MRBGT = (Row_H_MRB_GT)H_MRB_GT.NewRow();
                    rowH_MRBGT.ID = H_MRB_GT.GetNewID(BU, SFCDB);
                    rowH_MRBGT.WORKORDERNO = r_mrb_gt.WORKORDERNO;
                    rowH_MRBGT.SAP_STATION_CODE = r_mrb_gt.SAP_STATION_CODE;
                    rowH_MRBGT.FROM_STORAGE = r_mrb_gt.FROM_STORAGE;
                    rowH_MRBGT.TO_STORAGE = r_mrb_gt.TO_STORAGE;
                    rowH_MRBGT.TOTAL_QTY = r_mrb_gt.TOTAL_QTY;
                    rowH_MRBGT.CONFIRMED_FLAG = r_mrb_gt.CONFIRMED_FLAG;
                    rowH_MRBGT.ZCPP_FLAG = r_mrb_gt.ZCPP_FLAG;
                    rowH_MRBGT.SAP_FLAG = r_mrb_gt.SAP_FLAG;
                    rowH_MRBGT.SKUNO = r_mrb_gt.SKUNO;
                    rowH_MRBGT.SAP_MESSAGE = logMessage;
                    rowH_MRBGT.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowH_MRBGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(rowH_MRBGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                }
            }
        }

        private void AssyToMrbBackFlush()
        {
            MRBGTList = new List<R_MRB_GT>();
            MRBTable = new DataTable();

            sql = $@" select * from r_mrb_gt where sap_flag = 0 and zcpp_flag = 1  and sap_station_code is not null ";
            MRBTable = SFCDB.RunSelect(sql).Tables[0];
            if (MRBTable != null && MRBTable.Rows.Count > 0)
            {
                foreach (DataRow row in MRBTable.Rows)
                {
                    rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.NewRow();
                    rowR_MRBGT.loadData(row);
                    MRBGTList.Add(rowR_MRBGT.GetDataObject());
                }               
            }

            postDate = InterfacePublicValues.GetPostDate(SFCDB);

            if (MRBGTList != null && MRBGTList.Count > 0)
            {
                foreach (R_MRB_GT r_mrb_gt in MRBGTList)
                {
                    rowR_MRBGT = null;
                    rowH_MRBGT = null;
                    logMessage = "";
                    zcpp_nsg_0091.SetValue("I_WERKS", "WDN1");
                    zcpp_nsg_0091.SetValue("I_MATNR", r_mrb_gt.SKUNO);
                    zcpp_nsg_0091.SetValue("I_LMNGA", r_mrb_gt.TOTAL_QTY.ToString());
                    zcpp_nsg_0091.SetValue("I_LGORT", r_mrb_gt.TO_STORAGE);
                    zcpp_nsg_0091.SetValue("I_AUFNR", r_mrb_gt.FROM_STORAGE);
                    zcpp_nsg_0091.SetValue("I_BUDAT", postDate);
                    rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.GetObjByID(r_mrb_gt.ID, SFCDB);
                    try
                    {
                        zcpp_nsg_0091.CallRFC();
                        if (zcpp_nsg_0091.GetValue("O_MESSAGE").IndexOf("Material not exsit,pls check!") >= 0)
                        {
                            rowR_MRBGT.SAP_FLAG = "2";
                        }
                        else
                        {
                            rowR_MRBGT.SAP_FLAG = "1";
                        }
                    }
                    catch (Exception ex)
                    {
                        Interface.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.HWD.MrbBackFlush", "AssyToMrbBackFlush", r_mrb_gt.WORKORDERNO + ";" + this.IP + ";" + ex.ToString(), "", this.LoginUser.EMP_NO);
                        rowR_MRBGT.SAP_FLAG = "1";
                    }
                    SFCDB.ExecSQL(rowR_MRBGT.GetUpdateString(DB_TYPE_ENUM.Oracle));

                    logMessage = "M:" + zcpp_nsg_0091.GetValue("O_MESSAGE");
                    rowH_MRBGT = (Row_H_MRB_GT)H_MRB_GT.NewRow();
                    rowH_MRBGT.ID = H_MRB_GT.GetNewID(BU, SFCDB);
                    rowH_MRBGT.WORKORDERNO = r_mrb_gt.WORKORDERNO;
                    rowH_MRBGT.SAP_STATION_CODE = r_mrb_gt.SAP_STATION_CODE;
                    rowH_MRBGT.FROM_STORAGE = r_mrb_gt.FROM_STORAGE;
                    rowH_MRBGT.TO_STORAGE = r_mrb_gt.TO_STORAGE;
                    rowH_MRBGT.TOTAL_QTY = r_mrb_gt.TOTAL_QTY;
                    rowH_MRBGT.CONFIRMED_FLAG = r_mrb_gt.CONFIRMED_FLAG;
                    rowH_MRBGT.ZCPP_FLAG = r_mrb_gt.ZCPP_FLAG;
                    rowH_MRBGT.SAP_FLAG = r_mrb_gt.SAP_FLAG;
                    rowH_MRBGT.SKUNO = r_mrb_gt.SKUNO;
                    rowH_MRBGT.SAP_MESSAGE = logMessage;
                    rowH_MRBGT.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowH_MRBGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                    SFCDB.ExecSQL(rowH_MRBGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                }
            }
        }

        /// <summary>
        /// MRB SN入工單拋賬
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DoWOToMRBBackFlush(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                R_MRB_GT = new T_R_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                H_MRB_GT = new T_H_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                zrfc_sfc_nsg_0020 = new MESPubLab.SAP_RFC.ZRFC_SFC_NSG_0020(this.BU);
                MRBGTList = new List<R_MRB_GT>();
                MRBTable = new DataTable();

                if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
                {
                    //月結不給拋賬
                    //throw new Exception("This time is monthly,can't BackFlush");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180803152122", new string[] { }));
                }

                IsRuning = synLock.IsLock("HWD_WOToMRBBackFlush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
                if (IsRuning)
                {
                    //throw new Exception("HWD WOToMRBBackFlush interface is running on " + lockIp + ",Please try again later");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180803152222", new string[] { lockIp }));
                }
                synLock.SYNC_Lock(BU, this.IP, "HWD_WOToMRBBackFlush", "HWD_WOToMRBBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
                //避免拋賬還沒有執行完，又有SN掃入MRB入同一個工單，導致拋賬數據不對
                sql = " update r_mrb_gt set sap_flag = '2' where sap_flag = '0' and zcpp_flag = 0 and sap_station_code is not null ";
                SFCDB.ExecSQL(sql);
                SFCDB.CommitTrain();

                sql = $@" select * from r_mrb_gt where sap_flag = '2' and zcpp_flag = 0 and sap_station_code is not null  ";
                MRBTable = SFCDB.RunSelect(sql).Tables[0];
                if (MRBTable != null && MRBTable.Rows.Count > 0)
                {
                    foreach (DataRow row in MRBTable.Rows)
                    {
                        rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.NewRow();
                        rowR_MRBGT.loadData(row);
                        MRBGTList.Add(rowR_MRBGT.GetDataObject());
                    }
                }

                postDate = InterfacePublicValues.GetPostDate(SFCDB);

                if (MRBGTList != null && MRBGTList.Count > 0)
                {
                    foreach (R_MRB_GT r_mrb_gt in MRBGTList)
                    {
                        rowR_MRBGT = null;
                        rowH_MRBGT = null;
                        logMessage = "";
                        zrfc_sfc_nsg_0020.SetValue("I_AUFNR", r_mrb_gt.WORKORDERNO);
                        zrfc_sfc_nsg_0020.SetValue("I_BUDAT", postDate);
                        zrfc_sfc_nsg_0020.SetValue("I_FLAG", r_mrb_gt.CONFIRMED_FLAG);
                        zrfc_sfc_nsg_0020.SetValue("I_LGORT_TO", r_mrb_gt.TO_STORAGE);
                        zrfc_sfc_nsg_0020.SetValue("I_LMNGA", r_mrb_gt.TOTAL_QTY.ToString());
                        zrfc_sfc_nsg_0020.SetValue("I_STATION", r_mrb_gt.SAP_STATION_CODE);

                        rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.GetObjByID(r_mrb_gt.ID, SFCDB);
                        try
                        {
                            //默認拋賬是成功的,因為如果RFC返回的信息如有變動，而拋賬程序沒有及時更新的話，就可能會導致重複拋賬
                            rowR_MRBGT.SAP_FLAG = "1";
                            zrfc_sfc_nsg_0020.CallRFC();
                        }
                        catch (Exception ex)
                        {
                            rowR_MRBGT.SAP_FLAG = "2";
                            Interface.WriteIntoMESLog(SFCDB, BU, "MESStation", "MESStation.Interface.MrbBackFlush", "WOToMRBBackFlush", r_mrb_gt.WORKORDERNO + ";" + this.IP + ";" + ex.ToString(), "", this.LoginUser.EMP_NO);
                            //continue;
                        }
                        // zrfc_sfc_nsg_0020 中包含三個動作101，521(重工工單轉倉)，轉倉，flag，flag1，flag2 一次對應這三個動作
                        // flag，flag1，flag2 這幾個flag 0表示OK，1表示false       
                        logMessage = " M:" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE")
                                   + " M1:" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE1")
                                   + " M2" + zrfc_sfc_nsg_0020.GetValue("O_MESSAGE2");
                        //zrfc_sfc_nsg_0020.GetValue("O_FLAG") 有時候這個返回的是空值
                        //if (zrfc_sfc_nsg_0020.GetValue("O_FLAG") == "0")
                        //{                                                    
                        //    rowR_MRBGT.SAP_MESSAGE = logMessage;
                        //}
                        //else
                        //{
                        //    rowR_MRBGT.SAP_MESSAGE = logMessage;
                        //}
                        rowR_MRBGT.SAP_MESSAGE = logMessage;

                        SFCDB.ExecSQL(rowR_MRBGT.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        
                        rowH_MRBGT = (Row_H_MRB_GT)H_MRB_GT.NewRow();
                        rowH_MRBGT.ID = H_MRB_GT.GetNewID(BU, SFCDB);
                        rowH_MRBGT.WORKORDERNO = r_mrb_gt.WORKORDERNO;
                        rowH_MRBGT.SAP_STATION_CODE = r_mrb_gt.SAP_STATION_CODE;
                        rowH_MRBGT.FROM_STORAGE = r_mrb_gt.FROM_STORAGE;
                        rowH_MRBGT.TO_STORAGE = r_mrb_gt.TO_STORAGE;
                        rowH_MRBGT.TOTAL_QTY = r_mrb_gt.TOTAL_QTY;
                        rowH_MRBGT.CONFIRMED_FLAG = r_mrb_gt.CONFIRMED_FLAG;
                        rowH_MRBGT.ZCPP_FLAG = r_mrb_gt.ZCPP_FLAG;
                        rowH_MRBGT.SAP_FLAG = rowR_MRBGT.SAP_FLAG;
                        rowH_MRBGT.SKUNO = r_mrb_gt.SKUNO;
                        rowH_MRBGT.SAP_MESSAGE = logMessage;
                        rowH_MRBGT.EDIT_EMP = this.LoginUser.EMP_NO;
                        rowH_MRBGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowH_MRBGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }
                synLock.SYNC_UnLock(BU, this.IP, "HWD_WOToMRBBackFlush", "HWD_WOToMRBBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                throw exception;
            }
        }

        /// <summary>
        /// MRB 組裝退料拋賬
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DoAssyToMrbBackFlush(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                R_MRB_GT = new T_R_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                H_MRB_GT = new T_H_MRB_GT(SFCDB, DB_TYPE_ENUM.Oracle);
                synLock = new T_R_SYNC_LOCK(SFCDB, DB_TYPE_ENUM.Oracle);
                zcpp_nsg_0091 = new MESPubLab.SAP_RFC.ZCPP_NSBG_0091(this.BU);
                MRBGTList = new List<R_MRB_GT>();
                MRBTable = new DataTable();

                if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
                {
                    //月結不給拋賬
                    //throw new Exception("This time is monthly,can't BackFlush");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180803152122", new string[] { }));
                }

                IsRuning = synLock.IsLock("HWD_AssyToMrbBackFlush", SFCDB, DB_TYPE_ENUM.Oracle, out lockIp);
                if (IsRuning)
                {
                    //throw new Exception("HWD AssyToMrbBackFlush interface is running on " + lockIp + ",Please try again later");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180803152222", new string[] { lockIp }));
                }
                synLock.SYNC_Lock(BU, this.IP, "HWD_AssyToMrbBackFlush", "HWD_AssyToMrbBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);

                //避免拋賬還沒有執行完，又有SN掃入MRB入同一個工單，導致拋賬數據不對
                sql = " update r_mrb_gt set sap_flag = 2 where sap_flag = 0 and zcpp_flag = 1 and sap_station_code is not null ";
                SFCDB.ExecSQL(sql);
                SFCDB.CommitTrain();

                sql = $@" select * from r_mrb_gt where sap_flag = 2 and zcpp_flag = 1 and sap_station_code is not null ";
                MRBTable = SFCDB.RunSelect(sql).Tables[0];
                if (MRBTable != null && MRBTable.Rows.Count > 0)
                {
                    foreach (DataRow row in MRBTable.Rows)
                    {
                        rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.NewRow();
                        rowR_MRBGT.loadData(row);
                        MRBGTList.Add(rowR_MRBGT.GetDataObject());
                    }
                }
                postDate = InterfacePublicValues.GetPostDate(SFCDB);
                if (MRBGTList != null && MRBGTList.Count > 0)
                {
                    foreach (R_MRB_GT r_mrb_gt in MRBGTList)
                    {
                        rowR_MRBGT = null;
                        rowH_MRBGT = null;
                        logMessage = "";
                        zcpp_nsg_0091.SetValue("I_WERKS", "WDN1");
                        zcpp_nsg_0091.SetValue("I_MATNR", r_mrb_gt.SKUNO);
                        zcpp_nsg_0091.SetValue("I_LMNGA", r_mrb_gt.TOTAL_QTY.ToString());
                        zcpp_nsg_0091.SetValue("I_LGORT", r_mrb_gt.TO_STORAGE);
                        zcpp_nsg_0091.SetValue("I_AUFNR", r_mrb_gt.FROM_STORAGE);
                        zcpp_nsg_0091.SetValue("I_BUDAT", postDate);
                        rowR_MRBGT = (Row_R_MRB_GT)R_MRB_GT.GetObjByID(r_mrb_gt.ID, SFCDB);
                        try
                        {
                            //默認拋賬是成功的,因為如果RFC返回的信息如有變動，而拋賬程序沒有及時更新的話，就可能會導致重複拋賬
                            rowR_MRBGT.SAP_FLAG = "1";
                            zcpp_nsg_0091.CallRFC();
                            if (zcpp_nsg_0091.GetValue("O_FLAG") == "1" && zcpp_nsg_0091.GetValue("O_MESSAGE").IndexOf("Material not exsit,pls check!") >= 0)
                            {
                                rowR_MRBGT.SAP_FLAG = "2";
                            }                           
                        }
                        catch (Exception ex)
                        {
                            Interface.WriteIntoMESLog(SFCDB, BU, "MESStation", "MESStation.Interface.MrbBackFlush", "AssyToMrbBackFlush", r_mrb_gt.WORKORDERNO + ";" + this.IP + ";" + ex.ToString(), "", this.LoginUser.EMP_NO);
                            rowR_MRBGT.SAP_FLAG = "2";
                        }
                        SFCDB.ExecSQL(rowR_MRBGT.GetUpdateString(DB_TYPE_ENUM.Oracle));

                        logMessage = "M:" + zcpp_nsg_0091.GetValue("O_MESSAGE");
                        rowH_MRBGT = (Row_H_MRB_GT)H_MRB_GT.NewRow();
                        rowH_MRBGT.ID = H_MRB_GT.GetNewID(BU, SFCDB);
                        rowH_MRBGT.WORKORDERNO = r_mrb_gt.WORKORDERNO;
                        rowH_MRBGT.SAP_STATION_CODE = r_mrb_gt.SAP_STATION_CODE;
                        rowH_MRBGT.FROM_STORAGE = r_mrb_gt.FROM_STORAGE;
                        rowH_MRBGT.TO_STORAGE = r_mrb_gt.TO_STORAGE;
                        rowH_MRBGT.TOTAL_QTY = r_mrb_gt.TOTAL_QTY;
                        rowH_MRBGT.CONFIRMED_FLAG = r_mrb_gt.CONFIRMED_FLAG;
                        rowH_MRBGT.ZCPP_FLAG = r_mrb_gt.ZCPP_FLAG;
                        rowH_MRBGT.SAP_FLAG = rowR_MRBGT.SAP_FLAG;
                        rowH_MRBGT.SKUNO = r_mrb_gt.SKUNO;
                        rowH_MRBGT.SAP_MESSAGE = logMessage;
                        rowH_MRBGT.EDIT_EMP = this.LoginUser.EMP_NO;
                        rowH_MRBGT.EDIT_TIME = InterfacePublicValues.GetDBDateTime(SFCDB, DB_TYPE_ENUM.Oracle);
                        SFCDB.ExecSQL(rowH_MRBGT.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }
                synLock.SYNC_UnLock(BU, this.IP, "HWD_AssyToMrbBackFlush", "HWD_AssyToMrbBackFlush", this.LoginUser.EMP_NO, SFCDB, DB_TYPE_ENUM.Oracle);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                throw exception;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

       
    }
}
