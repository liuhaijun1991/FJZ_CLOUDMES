using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_MAIN : DataObjectTable
    {
        public T_R_REPAIR_MAIN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_MAIN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_MAIN);
            TableName = "R_REPAIR_MAIN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_REPAIR_MAIN> GetRepairMainBySN(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_REPAIR_MAIN row_main = null;
            List<R_REPAIR_MAIN> mains = new List<R_REPAIR_MAIN>();
            string sql = $@"select * from {TableName} where sn='{sn.Replace("'", "''")}'";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_R_REPAIR_MAIN)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public List<R_REPAIR_MAIN> GetRepairListSNAndStation(OleExec sfcdb, string sn, string station, string closed)
        {
            return sfcdb.ORM.Queryable<R_REPAIR_MAIN>().Where(r => r.SN == sn && r.FAIL_STATION == station && r.CLOSED_FLAG == closed).ToList();
        }

        public int ReplaceSnRepairFailMain(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_REPAIR_MAIN R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }

        public bool SNIsRepaired(string sn, OleExec sfcdb)
        {
            string strSql = $@"select * from r_repair_main where sn='{sn}' and closed_flag='0'";
            DataTable dt = sfcdb.ExecSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int GetRepairedCount(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"select * from r_repair_main where sn='{sn}' and closed_flag='1'";
                dt = DB.ExecSelect(strSql).Tables[0];
                result = dt.Rows.Count;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }

        public int GetRepairedCountOnSameLocation(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"SELECT * FROM (
                            SELECT M.SN, F.FAIL_LOCATION, COUNT(FAIL_LOCATION) FAILS
                            FROM R_REPAIR_MAIN M, R_REPAIR_FAILCODE F
                            WHERE M.ID = F.REPAIR_MAIN_ID
                            AND FAIL_LOCATION IS NOT NULL
                            AND FAIL_LOCATION <> 'N/A'
                            AND M.SN = '{sn}'
                            GROUP BY M.SN, F.FAIL_LOCATION
                            ORDER BY SN ASC) WHERE FAILS >= 2
                            ORDER BY FAILS DESC";

                dt = DB.ExecSelect(strSql).Tables[0];
                if(dt.Rows.Count != 0)
                {
                    result = Convert.ToInt32(dt.Rows[0][2].ToString());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }

        public int GetRepairedCountByStation(string sn, string station, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"select * from r_repair_main where sn='{sn}' and fail_station = '{station}' and closed_flag='1'";
                dt = DB.ExecSelect(strSql).Tables[0];
                result = dt.Rows.Count;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }

        public int Insert(R_SN SnObject, string Station, string Device, string Emp, DateTime? FailTime, string Bu, OleExec DB)
        {
            R_REPAIR_MAIN repair = new R_REPAIR_MAIN();
            repair.ID = GetNewID(Bu, DB);
            repair.SN = SnObject.SN;
            repair.WORKORDERNO = SnObject.WORKORDERNO;
            repair.SKUNO = SnObject.SKUNO;
            repair.FAIL_LINE = Device;
            repair.FAIL_STATION = Station;
            repair.FAIL_DEVICE = Device;
            repair.FAIL_EMP = Emp;
            repair.FAIL_TIME = FailTime;
            repair.CREATE_TIME = GetDBDateTime(DB);
            repair.CLOSED_FLAG = "0";
            repair.EDIT_TIME = GetDBDateTime(DB);
            repair.EDIT_EMP = Emp;
            return Insert(repair, DB);
        }

        public int Insert(R_REPAIR_MAIN repair, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_MAIN>(repair).ExecuteCommand();
        }

        /// <summary>
        /// wzw
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="SN"></param>
        /// <param name="Repair"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_REPAIR_MAIN> GetListByWoSNRepair(string WO, string SN, string Repair, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.WORKORDERNO == WO && t.SN == SN && t.CLOSED_FLAG == Repair).ToList();
        }
        /// <summary>
        /// WZW 维修保存按钮需要
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void GetMaxSNTO(Dictionary<string, string> DicStation, Dictionary<string, OleExec> DicDB, ref Dictionary<string, string> DicRef)
        {
            string SN = DicStation["SN"].ToString();
            string PCBASN = DicStation["PCBASN"].ToString();
            string Location = DicStation["Location"].ToString();
            string NewTrackNo = DicStation["NewTrackNo"].ToString();
            string TrackNo = DicStation["TrackNo"].ToString();
            string ActionCode = DicStation["ActionCode"].ToString();
            string RootCause = DicStation["RootCause"].ToString();
            string TR_SN = DicStation["TR_SN"].ToString();
            string Component = DicStation["Component"].ToString();
            string MFR_Name = DicStation["MFR_Name"].ToString();
            string VendName = DicStation["VendName"].ToString();
            string Date_Code = DicStation["Date_Code"].ToString();
            string Lot_Code = DicStation["Lot_Code"].ToString();
            string PartDes = DicStation["PartDes"].ToString();
            string ID = DicStation["ID"].ToString();
            string Process = DicStation["Process"].ToString();
            string Description = DicStation["Description"].ToString();
            string SolderNo = DicStation["SolderNo"].ToString();
            string PackType = DicStation["PackType"].ToString();
            string NEW_KP = DicStation["NEW_KP"].ToString();
            string Old_KP = DicStation["Old_KP"].ToString();
            string ReplaceNo = DicStation["ReplaceNo"].ToString();
            string FailSysPtom = DicStation["FailSysPtom"].ToString();
            string LocationChkbox = DicStation["LocationChkbox"];
            string FailCodeID = DicStation["FailCodeID"];
            string BU = DicStation["BU"];
            string EMP = DicStation["EMP"];
            string FailDescription = DicStation["FAILDESCRIPTION"];
            string STATUS = DicStation["STATUS"];
            int StrSolderNo = Convert.ToInt32(DicStation["StrSolder"].ToString());

            OleExec DB = DicDB["SFCDB"];
            OleExec APDB = DicDB["APDB"];

            T_R_SN RSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            R_SN SNINFO = RSN.LoadSN(SN, DB);
            if (SNINFO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SN }));
            }
            T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
            List<R_TESTRECORD> ListRTestRecordSNTestDesc = RTestRecord.GetListBYSNDesc(SN, DB);
            string FailEventPoint = ListRTestRecordSNTestDesc[0].STATION_NAME;
            if (Location == "PS")
            {
                FailEventPoint = "1";
            }
            try
            {
                System.Data.OleDb.OleDbParameter[] paras = new System.Data.OleDb.OleDbParameter[]
                    {
                new System.Data.OleDb.OleDbParameter("MYPSN", PCBASN),
                new System.Data.OleDb.OleDbParameter("MYWO", SNINFO.WORKORDERNO),
                new System.Data.OleDb.OleDbParameter("MYSTATION", FailEventPoint),
                new System.Data.OleDb.OleDbParameter("MYLOCATION", Location),
                new System.Data.OleDb.OleDbParameter("MYTRSN", TrackNo),
                new System.Data.OleDb.OleDbParameter("NEWTRSN", NewTrackNo),
                new System.Data.OleDb.OleDbParameter("REPLACESN", ReplaceNo),
                new System.Data.OleDb.OleDbParameter("NEWKPSN", NEW_KP),
                new System.Data.OleDb.OleDbParameter("OLDKPSN", Old_KP),
                new System.Data.OleDb.OleDbParameter("MYEMP",SNINFO.EDIT_EMP),
                new System.Data.OleDb.OleDbParameter("P_NO",SNINFO.SKUNO ),
                new System.Data.OleDb.OleDbParameter("FAIL_SYMPTOM", FailDescription==null ? "":FailDescription),
                new System.Data.OleDb.OleDbParameter("ID_NO", ID),
                new System.Data.OleDb.OleDbParameter("G_ERROR_CODE", RootCause),
                new System.Data.OleDb.OleDbParameter("G_fail_type", STATUS),
                new System.Data.OleDb.OleDbParameter("RES", System.Data.OleDb.OleDbType.VarChar, 5000),
                    };
                AP_DLL APDLL = new AP_DLL();
                Dictionary<string, string> DicAPMaterial = new Dictionary<string, string>();
                string ResMessage = APDLL.OraclOleDbParameter("MES1.Z_INSERT_KP_REPLACE_UPDATE_NEW", paras, ref DicAPMaterial, APDB);
                if (ResMessage != null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000263", new string[] { }));
                }
                if (DicAPMaterial["RES"].ToString() != "OK")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000263", new string[] { "MES1.Z_INSERT_KP_REPLACE_UPDATE_NEW" }));
                }
            }
            catch (Exception)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105111722", new string[] { "MES1.Z_INSERT_KP_REPLACE_UPDATE_NEW" }));
            }
            if (LocationChkbox.ToUpper() == "ON")
            {
                T_R_REPAIR_OFFLINE RRepairOffline = new T_R_REPAIR_OFFLINE(DB, DB_TYPE_ENUM.Oracle);
                RRepairOffline.AutoRepairSave(DicStation, DicDB, ref DicRef);

            }
            else
            {
                T_R_REPAIR_TRANSFER RRepairTransfer = new T_R_REPAIR_TRANSFER(DB, DB_TYPE_ENUM.Oracle);
                RRepairTransfer.common(DicStation, DicDB, ref DicRef);
            }
        }
        public R_REPAIR_MAIN GetSNBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == SN && t.CLOSED_FLAG == "0").ToList().FirstOrDefault();
        }

        public int UpdateRepairSN(R_REPAIR_MAIN RepairSN, string SN, string CLOSEDFLAG, OleExec DB)
        {
            return DB.ORM.Updateable<R_REPAIR_MAIN>(RepairSN).Where(t => t.SN == SN && t.CLOSED_FLAG == CLOSEDFLAG).ExecuteCommand();
        }
        public List<R_REPAIR_MAIN> GetSNBySNFailStation(string SN, List<string> FailStaion, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == SN && FailStaion.Contains(t.FAIL_STATION) && t.CLOSED_FLAG == "0").ToList();
        }
        public List<R_REPAIR_MAIN> GetSNMainFailCodeCount(string SN, string FailStaion, string Repaired, string FailCode, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE>((p1, p2) => p1.SN == p2.SN && p1.FAIL_TIME == p2.FAIL_TIME).Where((p1, p2) => p1.SN == SN && p1.FAIL_STATION == FailStaion && p1.REPAIRING_FLAG == Repaired && p2.FAIL_CODE == FailCode).OrderBy(t => t.FAIL_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public void NewEFox_sfctransrepair_spCheckIN(string SN, string BU, string EMP, string Line, string RepairName, string DEPARTMENT, string CHECKINTYPE, OleExec DB)
        {
            T_R_MES_LOG TRMESLOG = new T_R_MES_LOG(DB, DB_TYPE_ENUM.Oracle);
            R_MES_LOG RMESLOG = new R_MES_LOG();
            RMESLOG.ID = TRMESLOG.GetNewID(BU, DB);
            RMESLOG.PROGRAM_NAME = "RepairCheck";
            RMESLOG.CLASS_NAME = "MESDataObject.Module.T_R_REPAIR_MAIN";
            RMESLOG.FUNCTION_NAME = "NewEFox_sfctransrepair_spCheckIN";
            RMESLOG.LOG_MESSAGE = "CHECKIN";
            RMESLOG.LOG_SQL = RepairName;
            RMESLOG.EDIT_EMP = EMP;
            RMESLOG.EDIT_TIME = TRMESLOG.GetDBDateTime(DB);
            int TRMESLOGNum = TRMESLOG.InsertkpsnLog(RMESLOG, DB);
            T_R_REPAIR_TRANSFER TRRepairTransfer = new T_R_REPAIR_TRANSFER(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN TRSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            Row_R_SN RowRSN = TRSN.getR_SNbySN(SN, DB);
            if (RowRSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181217135158", new string[] { SN }));
            }
            T_R_REPAIR_MAIN TRRepairMain = new T_R_REPAIR_MAIN(DB, DB_TYPE_ENUM.Oracle);
            List<string> ListStation = new List<string>();
            ListStation.Clear();
            ListStation.Add("VI");
            ListStation.Add("#5DX");
            List<R_REPAIR_MAIN> ListRRepairMainCheckINSN = TRRepairMain.GetSNRepairCheckin(SN, "0", ListStation, DB);
            T_R_TESTRECORD TRTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
            ListStation.Clear();
            ListStation.Add("A");
            ListStation.Add("S");
            List<R_TESTRECORD> ListRTestRecordSNNOTStation = TRTestRecord.GetListBYSNNOStationFListStatusNO(SN, "XRAY", ListStation, DB);
            T_R_SN_STATION_DETAIL TRSNStationDetail = new T_R_SN_STATION_DETAIL(DB, DB_TYPE_ENUM.Oracle);
            List<R_SN_STATION_DETAIL> ListRSNStatinDetailSN = TRSNStationDetail.GetSNRepairTEST(SN, DB);
            List<R_TESTRECORD> ListRTestRecordTestActionSN = TRTestRecord.GetTestAction(SN, DB);
            if (ListRRepairMainCheckINSN.Count > 0 || ListRTestRecordSNNOTStation.Count > 0 || ListRSNStatinDetailSN.Count > 0 || ListRTestRecordTestActionSN.Count > 0)
            {
                List<R_SN> ListRSNSN = TRSN.GetRSNbySN(SN, DB);
                List<R_TESTRECORD> ListRTestRecordSN = TRTestRecord.GetListBYSN(SN, DB);
                if (ListRSNSN.Count <= 0 || ListRSNSN.Count <= 0)
                {
                    //导资料有专门的工具/网站
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181217135158", new string[] { SN }));
                }
                T_R_REPAIR_SELF TRRepairSelf = new T_R_REPAIR_SELF(DB, DB_TYPE_ENUM.Oracle);
                List<R_REPAIR_SELF> ListRRepairSelf = TRRepairSelf.GetSNBySN(SN, DB);
                if (ListRRepairSelf.Count > 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181217135158", new string[] { SN }));
                }
                R_SN RSNRepairSN = TRSN.GetBySNRepairSN(SN, "1", DB);
                if (TRRepairMain.SNIsRepaired(SN, DB) && RSNRepairSN == null)
                {
                    int UpdateSNRepairNum = TRSN.UpdateSNRepair(SN, "1", DB);
                    if (UpdateSNRepairNum < 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { SN }));
                    }
                }
                string SKUNO = "";
                string WO = "";
                R_SN RSNRowSN = TRSN.GetSNRowBySN(SN, DB);
                T_R_SN_KP TRSNKP = new T_R_SN_KP(DB, DB_TYPE_ENUM.Oracle);
                SKUNO = RSNRowSN.SKUNO;
                WO = RSNRowSN.WORKORDERNO;
                if (RSNRowSN == null)
                {
                    List<R_SN_KP> ListRSNKP = TRSNKP.GetBYSNSCANTYPE(SN, 1, DB);
                    if (ListRSNKP != null)
                    {
                        R_SN RSNRowKPSN = TRSN.GetSNRowBySN(ListRSNKP[0].SN, DB);
                        SKUNO = RSNRowKPSN.SKUNO;
                        WO = RSNRowKPSN.WORKORDERNO;
                    }
                }
                T_C_SKU TCSKU = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle);
                T_C_SECOND_USER TCSecondUser = new T_C_SECOND_USER(DB, DB_TYPE_ENUM.Oracle);
                //C_SKU CSKUSKUNO = TCSKU.GetSku(SKUNO, DB);
                //List<C_SECOND_USER> ListCSecondUserEMP = TCSecondUser.GetEmpStationNameSelect(RepairName, "REPAIRCHECK", DB);
                List<C_SECOND_USER> ListCSecondUserName = TCSecondUser.GetEmpStationNameSelect(RepairName, "REPAIRCHECK", DB);
                if (ListCSecondUserName == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190105091528", new string[] { EMP }));
                }
                if (ListCSecondUserName == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190105091528", new string[] { RepairName }));
                }
                if (ListCSecondUserName[0].STATION_ITEM == "REPAIR" || ListCSecondUserName[0].STATION_ITEM == "SE")
                {
                    #region
                    if (ListCSecondUserName[0].STATION_ITEM == "REPAIR")
                    {
                        ListStation.Clear();
                        ListStation.Add("A");
                        ListStation.Add("S");
                        ListStation.Add("F");
                        //List<R_TESTRECORD> ListRTestRecordSNTatus = TRTestRecord.GetSNStatusBYSN(SN, ListStation, DB);//此处有问题
                        List<R_TESTRECORD> ListRTestRecordSNTatus = TRTestRecord.GetRepairNotA(SN, DB);
                        if (ListRTestRecordSNTatus.Count < 0)
                        {
                            //报错未写    郵件通知禁止維修為A的記錄    該SSN測試最后一站狀態不為:F或A,不能CheckIn!或則過站記錄最後一站不是fail        
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190402145524", new string[] { SN }));
                        }
                        List<R_SN_STATION_DETAIL> ListRSNStationDetail = TRSNStationDetail.GetRepairNotA(SN, DB);
                        if (ListRSNStationDetail.Count < 0)
                        {
                            //报错未写    郵件通知禁止維修為A的記錄    該SSN測試最后一站狀態不為:F或A,不能CheckIn!或則過站記錄最後一站不是fail        
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190402145801", new string[] { SN }));
                        }
                        List<R_REPAIR_TRANSFER> ListRRepairTransferSN = TRRepairTransfer.GetSNOrderEnd(SN, DB);
                        string DEPARTMENTS = "";
                        string STATION_ITEMS = ListCSecondUserName[0].STATION_ITEM;
                        //if (ListRRepairTransferSN.Count <= 0)
                        //{
                        //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219101419", new string[] { SN }));
                        //}
                        //else if (ListRRepairTransferSN[0].DEPARTMENT == "")
                        //{
                        //    ListRRepairTransferSN[0].DEPARTMENT = "PD";
                        //}
                        if (ListRRepairTransferSN.Count > 0)
                        {
                            if (ListRRepairTransferSN[0].DEPARTMENT == "REPAIR" || ListRRepairTransferSN[0].DEPARTMENT == "SE"/*ListRRepairTransferSN[0].DEPARTMENT == "REPAIR" || ListRRepairTransferSN[0].DEPARTMENT == "SE"*/)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219101419", new string[] { SN }));
                            }
                            DEPARTMENTS = ListRRepairTransferSN[0].DEPARTMENT;
                            //DEPARTMENTS = "PD";
                            STATION_ITEMS = "PD";
                        }
                        else
                        {
                            //ListRRepairTransferSN[0].DEPARTMENT = ListRRepairTransferSN[0].DEPARTMENT == null ? "PD" : ListRRepairTransferSN[0].DEPARTMENT;
                            //ListRRepairTransferSN[0].DEPARTMENT = "PD";
                            DEPARTMENTS = "PD";
                            STATION_ITEMS = "PD";
                        }
                        if (DEPARTMENTS == "PD" && STATION_ITEMS == "PD"/*ListRRepairTransferSN[0].DEPARTMENT == "PD" && ListCSecondUserEMP[0].STATION_ITEM == "PD"*/)
                        {
                            List<string> ListString = new List<string>();
                            ListString.Add("5DX");
                            List<R_REPAIR_MAIN> ListRRepairMainSNStation = TRRepairMain.GetSNRepairCheckin(SN, "0", ListString, DB);
                            ListString.Clear();
                            ListString.Add("PICT");
                            ListString.Add("AOI2");
                            ListString.Add("AOI4");
                            ListString.Add("VI");
                            ListString.Add("OBA");
                            List<R_REPAIR_MAIN> ListRRepairMainSNFailStation = TRRepairMain.GetSNRepairCheckin(SN, "0", ListString, DB);
                            if (ListRRepairMainSNStation.Count > 0)
                            {
                                T_R_5DX_TESTRECORD TR5DXTestRecord = new T_R_5DX_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
                                List<R_5DX_TESTRECORD> ListR5DXTestRecordSNStatus = TR5DXTestRecord.GetSNStatusBYSN(SN, "F", DB);
                                if (ListR5DXTestRecordSNStatus.Count > 0)
                                {
                                    R_REPAIR_TRANSFER RRepairTransfer = new R_REPAIR_TRANSFER();
                                    RRepairTransfer.ID = TRRepairTransfer.GetNewID(BU, DB);
                                    RRepairTransfer.REPAIR_MAIN_ID = ListRRepairMainSNStation[0].ID;
                                    RRepairTransfer.SN = ListR5DXTestRecordSNStatus[0].SN;
                                    RRepairTransfer.LINE_NAME = Line;
                                    RRepairTransfer.STATION_NAME = ListR5DXTestRecordSNStatus[0].STATION_NAME;
                                    RRepairTransfer.SKUNO = SKUNO;
                                    RRepairTransfer.WORKORDERNO = WO;
                                    RRepairTransfer.IN_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                    RRepairTransfer.IN_SEND_EMP = EMP;
                                    RRepairTransfer.IN_RECEIVE_EMP = RepairName;
                                    RRepairTransfer.OUT_TIME = null;
                                    RRepairTransfer.OUT_SEND_EMP = "";
                                    RRepairTransfer.OUT_RECEIVE_EMP = "";
                                    RRepairTransfer.DESCRIPTION = "";
                                    RRepairTransfer.CLOSED_FLAG = "0";
                                    RRepairTransfer.CREATE_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                    RRepairTransfer.EDIT_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                    RRepairTransfer.EDIT_EMP = EMP;
                                    RRepairTransfer.DEPARTMENT = DEPARTMENT;
                                    int InsertRepairCheckinNum = TRRepairTransfer.InsertRepairCheckin(RRepairTransfer, DB);
                                    if (InsertRepairCheckinNum <= 0)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                                    }
                                }
                                else
                                {
                                    T_H_5DX_TESTRECORD TH5DXTestRecord = new T_H_5DX_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
                                    List<H_5DX_TESTRECORD> ListH5DXTestRecord = TH5DXTestRecord.GetSNStatusBYSN(SN, "F", DB);
                                    if (ListH5DXTestRecord.Count <= 0)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190418145908", new string[] { SN }));
                                    }
                                    R_REPAIR_TRANSFER RRepairTransfer = new R_REPAIR_TRANSFER();
                                    RRepairTransfer.ID = TRRepairTransfer.GetNewID(BU, DB);
                                    RRepairTransfer.REPAIR_MAIN_ID = ListRRepairMainSNStation[0].ID;
                                    RRepairTransfer.SN = ListH5DXTestRecord[0].SN;
                                    RRepairTransfer.LINE_NAME = Line;
                                    RRepairTransfer.STATION_NAME = ListH5DXTestRecord[0].STATION_NAME;
                                    RRepairTransfer.SKUNO = SKUNO;
                                    RRepairTransfer.WORKORDERNO = WO;
                                    RRepairTransfer.IN_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                    RRepairTransfer.IN_SEND_EMP = EMP;
                                    RRepairTransfer.IN_RECEIVE_EMP = RepairName;
                                    RRepairTransfer.OUT_TIME = null;
                                    RRepairTransfer.OUT_SEND_EMP = "";
                                    RRepairTransfer.OUT_RECEIVE_EMP = "";
                                    RRepairTransfer.DESCRIPTION = "";
                                    RRepairTransfer.CLOSED_FLAG = "0";
                                    RRepairTransfer.CREATE_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                    RRepairTransfer.EDIT_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                    RRepairTransfer.EDIT_EMP = EMP;
                                    RRepairTransfer.DEPARTMENT = DEPARTMENT;
                                    int InsertRepairCheckinNum = TRRepairTransfer.InsertRepairCheckin(RRepairTransfer, DB);
                                    if (InsertRepairCheckinNum <= 0)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                                    }
                                }
                            }
                            else if (ListRRepairMainSNFailStation.Count > 0)
                            {
                                List<R_SN_STATION_DETAIL> ListRSNStationDetailSNValidFlag = TRSNStationDetail.GetSNBYSNValid(SN, "1", DB);
                                if (ListRSNStationDetailSNValidFlag.Count <= 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190418145459", new string[] { SN }));
                                }
                                R_REPAIR_TRANSFER RRepairTransfer = new R_REPAIR_TRANSFER();
                                RRepairTransfer.ID = TRRepairTransfer.GetNewID(BU, DB);
                                RRepairTransfer.REPAIR_MAIN_ID = ListRRepairMainSNFailStation[0].ID;
                                RRepairTransfer.SN = ListRSNStationDetailSNValidFlag[0].SN;
                                RRepairTransfer.LINE_NAME = Line;
                                RRepairTransfer.STATION_NAME = ListRSNStationDetailSNValidFlag[0].STATION_NAME;
                                RRepairTransfer.SKUNO = SKUNO;
                                RRepairTransfer.WORKORDERNO = WO;
                                RRepairTransfer.IN_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                RRepairTransfer.IN_SEND_EMP = EMP;
                                RRepairTransfer.IN_RECEIVE_EMP = RepairName;
                                RRepairTransfer.OUT_TIME = null;
                                RRepairTransfer.OUT_SEND_EMP = "";
                                RRepairTransfer.OUT_RECEIVE_EMP = "";
                                RRepairTransfer.DESCRIPTION = "";
                                RRepairTransfer.CLOSED_FLAG = "0";
                                RRepairTransfer.CREATE_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                RRepairTransfer.EDIT_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                RRepairTransfer.EDIT_EMP = EMP;
                                RRepairTransfer.DEPARTMENT = DEPARTMENT;
                                int InsertRepairCheckinNum = TRRepairTransfer.InsertRepairCheckin(RRepairTransfer, DB);
                                if (InsertRepairCheckinNum <= 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                                }
                            }
                            else
                            {
                                List<R_TESTRECORD> ListRTestRecordStartSN = TRTestRecord.GetSNStatusBYSN(SN, ListString, DB);
                                if (ListRTestRecordStartSN.Count <= 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190418150004", new string[] { SN }));
                                }
                                R_REPAIR_MAIN R_REPAIR_MAIN = TRRepairMain.GetSNBySN(SN, DB);
                                R_REPAIR_TRANSFER RRepairTransfer = new R_REPAIR_TRANSFER();
                                RRepairTransfer.ID = TRRepairTransfer.GetNewID(BU, DB);
                                RRepairTransfer.REPAIR_MAIN_ID = R_REPAIR_MAIN.ID;
                                RRepairTransfer.SN = ListRTestRecordStartSN[0].SN;
                                RRepairTransfer.LINE_NAME = Line;
                                RRepairTransfer.STATION_NAME = ListRTestRecordStartSN[0].STATION_NAME;
                                RRepairTransfer.SKUNO = SKUNO;
                                RRepairTransfer.WORKORDERNO = WO;
                                RRepairTransfer.IN_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                RRepairTransfer.IN_SEND_EMP = EMP;
                                RRepairTransfer.IN_RECEIVE_EMP = RepairName;
                                RRepairTransfer.OUT_TIME = null;
                                RRepairTransfer.OUT_SEND_EMP = "";
                                RRepairTransfer.OUT_RECEIVE_EMP = "";
                                RRepairTransfer.DESCRIPTION = "";
                                RRepairTransfer.CLOSED_FLAG = "0";
                                RRepairTransfer.CREATE_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                RRepairTransfer.EDIT_TIME = TRRepairTransfer.GetDBDateTime(DB);
                                RRepairTransfer.EDIT_EMP = EMP;
                                RRepairTransfer.DEPARTMENT = DEPARTMENT;
                                int InsertRepairCheckinNum = TRRepairTransfer.InsertRepairCheckin(RRepairTransfer, DB);
                                if (InsertRepairCheckinNum <= 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                                }
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190103135533", new string[] { ListRRepairTransferSN[0].IN_RECEIVE_EMP }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190103140441", new string[] { RepairName }));
                    }
                    #endregion
                }
            }
            else if (CHECKINTYPE == "自動分板" && RowRSN == null)
            {
                R_REPAIR_TRANSFER RRepairTransfer = new R_REPAIR_TRANSFER();
                RRepairTransfer.ID = TRRepairTransfer.GetNewID(BU, DB);
                RRepairTransfer.REPAIR_MAIN_ID = ListRRepairMainCheckINSN[0].ID;
                RRepairTransfer.SN = SN;
                RRepairTransfer.LINE_NAME = Line;
                RRepairTransfer.STATION_NAME = "CHECKIN";
                RRepairTransfer.SKUNO = RowRSN.SKUNO;
                RRepairTransfer.WORKORDERNO = RowRSN.WORKORDERNO;
                RRepairTransfer.IN_TIME = TRRepairTransfer.GetDBDateTime(DB);
                RRepairTransfer.IN_SEND_EMP = EMP;
                RRepairTransfer.IN_RECEIVE_EMP = RepairName;
                RRepairTransfer.OUT_TIME = null;
                RRepairTransfer.OUT_SEND_EMP = "";
                RRepairTransfer.OUT_RECEIVE_EMP = "";
                RRepairTransfer.DESCRIPTION = "";
                RRepairTransfer.CLOSED_FLAG = "0";
                RRepairTransfer.CREATE_TIME = TRRepairTransfer.GetDBDateTime(DB);
                RRepairTransfer.EDIT_TIME = TRRepairTransfer.GetDBDateTime(DB);
                RRepairTransfer.EDIT_EMP = EMP;
                RRepairTransfer.DEPARTMENT = DEPARTMENT;
                int InsertRepairCheckinNum = TRRepairTransfer.InsertRepairCheckin(RRepairTransfer, DB);
                if (InsertRepairCheckinNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190103140614", new string[] { SN }));
            }
        }
        public List<R_REPAIR_MAIN> GetSNRepairCheckin(string SN, string Repaired, List<string> FAIL_STATION, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == SN && t.CLOSED_FLAG == Repaired && FAIL_STATION.Contains(t.FAIL_STATION)).OrderBy(t => t.CREATE_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_REPAIR_MAIN> GetBYSNOUT(string RelSn, OleExec DB)
        {
            string strSql = $@" SELECT * FROM R_REPAIR_MAIN A WHERE A.SN='{RelSn}' AND A.CLOSED_FLAG='0'
    AND EDIT_TIME >= (SELECT MAX(EDIT_TIME) FROM R_REPAIR_TRANSFER  WHERE SN = '{RelSn}') ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_MAIN> listSn = new List<R_REPAIR_MAIN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_MAIN ret = (Row_R_REPAIR_MAIN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_MAIN> GetBYSNRepair(string RelSn, OleExec DB)
        {
            string strSql = $@" SELECT * FROM R_REPAIR_MAIN WHERE SN = '{RelSn}' AND CLOSED_FLAG = '1' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_MAIN> listSn = new List<R_REPAIR_MAIN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_MAIN ret = (Row_R_REPAIR_MAIN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public int InsertRepairMain(object InsertSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                result = DB.ExecSqlNoReturn(InsertSql.ToString(), null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }
    }
    public class Row_R_REPAIR_MAIN : DataObjectBase
    {
        public Row_R_REPAIR_MAIN(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_MAIN GetDataObject()
        {
            R_REPAIR_MAIN DataObject = new R_REPAIR_MAIN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FAIL_LINE = this.FAIL_LINE;
            DataObject.FAIL_STATION = this.FAIL_STATION;
            DataObject.FAIL_DEVICE = this.FAIL_DEVICE;
            DataObject.FAIL_EMP = this.FAIL_EMP;
            DataObject.FAIL_TIME = this.FAIL_TIME;
            DataObject.DISTRIBUTION_EMP = this.DISTRIBUTION_EMP;
            DataObject.DISTRI_TIME = this.DISTRIBUTION_TIME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.REPAIRING_FLAG = this.REPAIRING_FLAG;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }

        public string REPAIRING_FLAG
        {
            get
            {
                return (string)this["REPAIRING_FLAG"];
            }
            set
            {
                this["REPAIRING_FLAG"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string FAIL_LINE
        {
            get
            {
                return (string)this["FAIL_LINE"];
            }
            set
            {
                this["FAIL_LINE"] = value;
            }
        }
        public string FAIL_STATION
        {
            get
            {
                return (string)this["FAIL_STATION"];
            }
            set
            {
                this["FAIL_STATION"] = value;
            }
        }
        public string FAIL_DEVICE
        {
            get
            {
                return (string)this["FAIL_DEVICE"];
            }
            set
            {
                this["FAIL_DEVICE"] = value;
            }
        }
        public string FAIL_EMP
        {
            get
            {
                return (string)this["FAIL_EMP"];
            }
            set
            {
                this["FAIL_EMP"] = value;
            }
        }
        public DateTime? FAIL_TIME
        {
            get
            {
                return (DateTime?)this["FAIL_TIME"];
            }
            set
            {
                this["FAIL_TIME"] = value;
            }
        }
        public string DISTRIBUTION_EMP
        {
            get
            {
                return (string)this["DISTRIBUTION_EMP"];
            }
            set
            {
                this["DISTRIBUTION_EMP"] = value;
            }
        }
        public DateTime? DISTRIBUTION_TIME
        {
            get
            {
                return (DateTime?)this["DISTRIBUTION_TIME"];
            }
            set
            {
                this["DISTRIBUTION_TIME"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class R_REPAIR_MAIN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SN { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string FAIL_LINE { get; set; }
        public string FAIL_STATION { get; set; }
        public string FAIL_DEVICE { get; set; }
        public string FAIL_EMP { get; set; }
        public DateTime? FAIL_TIME { get; set; }
        public string DISTRIBUTION_EMP { get; set; }
        [SugarColumn(ColumnName = "DISTRIBUTION_TIME")]
        public DateTime? DISTRI_TIME { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string CLOSED_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string REPAIRING_FLAG { get; set; }
    }
    public class R_REPAIR_MAIN_EX
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string MAIN_ID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }
}