using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Configuration;
using MESDataObject.Module;
using System.Data;
using System.Data.SqlClient;
using MESDataObject;

namespace MESInterface.BPD
{
    public class RecordTestTable : taskBase
    {
        SqlSugarClient client = null;
        private StringBuilder ConnectionString = new StringBuilder();
        private T_R_TEST_RECORD Recorder = null;
        private T_R_REPAIR_MAIN RepairMain = null;
        private T_R_SN RSN = null;
        //private T_C_ROUTE_DETAIL TCRD = null;
        private OleExec SFCDB = null;
        private string DBHost;
        private string DBPort;
        private string DBName;
        private string DBUser;
        private string DBPass;
        private string DBLocalName;
        private string BU;
        private static Dictionary<string, List<C_ROUTE_DETAIL>> RouteDetailCache = new Dictionary<string, List<C_ROUTE_DETAIL>>();

        public override void Start()
        {
            DoSomething(SFCDB, BU, "BPD_RecordTestDB", "BPD_RecordTestDB", "Record Test DB Fail", DoRecordTest);
        }

        public string LineMapping(string DeviceName)
        {
            switch (DeviceName.ToUpper())
            {
                case "3DX_01":
                    return "A13XRAY3DX01";
                case "3DX_02":
                    return "A13XRAY3DX02";
                case "RXI_01":
                    return "A13XRAYRXI01";
                case "RXI_02":
                    return "A13XRAYRXI02";
                case "FXSICT1":
                    return "A13ICT";
                case "S1_01":
                    return "A13ICTS101";
                case "S1_02":
                    return "A13ICTS102";
                default:
                    return DeviceName;
            }
        }
        public void DoRecordTest()
        {
            //DataTable dt = null;
            int count = 0;
            int success = 0;
            ////List<string> Sns = SFCDB.ORM.Queryable<R_SN, C_STATION,R_WO_BASE>((rs, cs,rwb) => rs.NEXT_STATION == cs.STATION_NAME && rs.WORKORDERNO==rwb.WORKORDERNO)
            ////                .Where((rs, cs,rwb) => cs.TYPE == "TEST" && rwb.CLOSED_FLAG=="0").Select((rs, cs,rwb) => rs.SN).ToList();

            //List<object> Sns = SFCDB.ORM.SqlQueryable<object>(@"select sn
            //                                                  from r_sn
            //                                                 where workorderno in
            //                                                       (select workorderno from r_wo_base where closed_flag = '0')
            //                                                   and not exists
            //                                                 (select sn from r_test_record where messtation = 'DOM' and state='PASS')
            //                    ").ToList();
            ////foreach (object o in Sns_DOM)
            ////{
            ////    Sns.Add(o.ToString());
            ////}
            //StringBuilder sb = new StringBuilder();
            //foreach (object Sn in Sns)
            //{
            //    sb.Append("'").Append(Sn.ToString()).Append("',");
            //}


            //dt = client.SqlQueryable<object>($@"select top 200 *
            //                                      from (select top 200 machine,
            //                                                   sernum,
            //                                                   CONVERT(varchar(20), rectime, 120) rectime,
            //                                                   area,
            //                                                   passfail,
            //                                                   test,
            //                                                   username
            //                                              from TEDB_52.TestDB.dbo.tbl_dbBPD
            //                                             where senttosfc = 0
            //                                             order by scantime desc) a").ToDataTable();
            List<TestRecord> TRs = client.Queryable<TestRecord>().Where(t => t.SentToSFC == "0").OrderBy(t => t.ScanTime, OrderByType.Desc).Take(1000).ToList();
            foreach (TestRecord TR in TRs)
            {
                count++;
                
                try
                {
                    R_TEST_RECORD record = new R_TEST_RECORD();
                    //string SerialNo = dr["SERNUM"].ToString().Trim();
                    string SerialNo = TR.SerNum;
                    //string Station = dr["AREA"].ToString().Trim();
                    string Station = TR.Area=="ROBAT"?"ICT_S1":TR.Area;
                    //string PassFail = dr["PASSFAIL"].ToString();
                    string PassFail = TR.PassFail;

                    R_SN SnObject = RSN.GetSN(SerialNo, SFCDB);
                    //如果不是正在生產的 SN，測試記錄SentToSFC欄位直接更新為1，不記錄到MES數據庫中
                    if (SnObject == null)
                    {
                        client.Updateable<TestRecord>().UpdateColumns(t => t.SentToSFC == "1").Where(t => t.SerNum == TR.SerNum && t.Area == TR.Area && t.RecTime == TR.RecTime).ExecuteCommand();
                    }
                    else
                    {
                        //如果跳站呢?SnObject.NEXT_STATION肯定不等於Station
                        #region 如果SN當前的NextStation不等於測試資料庫傳進的工站,並且當前工站有跳站內容才會處理
                        //主要就是如果有跳站,更新r_sn並重新加載SnObject
                        if (!Station.Equals("PASTE") && !SnObject.NEXT_STATION.Equals(Station))
                        {
                            string VarRouteID = SnObject.ROUTE_ID;
                            //先獲取SN當前工站,再根據當前工站名稱
                            string VarCurrentStation = SnObject.CURRENT_STATION;
                            T_C_ROUTE_DETAIL RouteDetailT = new T_C_ROUTE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                            List<C_ROUTE_DETAIL> RouteDetailByDirectLinkID = null;
                            RouteDetailByDirectLinkID = RouteDetailT.GetRouteDetailByDirectLinkID(VarRouteID, VarCurrentStation, SFCDB);
                            if (RouteDetailByDirectLinkID != null)
                            {
                                C_ROUTE_DETAIL CStation = RouteDetailByDirectLinkID.Find(t => t.STATION_NAME == Station);
                                if (CStation != null)//肯定不空,所以不用else
                                {
                                    string RouteStationName = CStation.STATION_NAME;
                                    if (RouteStationName.Equals(Station))
                                    {
                                        //目的是判斷到是跳站,更新r_sn的CurrentStation and NextStation,以便不變更updatestatus的邏輯
                                        int result = RSN.TiaoZhanUpdateCurrentNextStation(SnObject.ID, SnObject.NEXT_STATION, RouteStationName, SFCDB);
                                        if (result <= 0)
                                        {
                                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SN, "UPDATE" }));
                                        }
                                        else
                                        {
                                            SnObject = RSN.GetSN(SerialNo, SFCDB);//重置SnObject內容
                                        }
                                    }
                                }
                            }
                        }
                        #endregion 跳站邏輯結束
                        //如果不是 DOM 資料而且產品當前站位與測試站位不相符，不會將測試記錄到數據庫中
                        if (!Station.Equals("PASTE") && !SnObject.NEXT_STATION.Equals(Station))
                        {
                            List<C_ROUTE_DETAIL> RouteDetails = null;
                            if (RouteDetailCache.ContainsKey(SnObject.ROUTE_ID))
                            {
                                RouteDetails = RouteDetailCache[SnObject.ROUTE_ID];
                            }
                            else
                            {
                                RouteDetails = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == SnObject.ROUTE_ID).ToList();
                                RouteDetailCache.Add(SnObject.ROUTE_ID, RouteDetails);
                            }
                            if (RouteDetails.Count > 0)
                            {
                                C_ROUTE_DETAIL NextDetail = RouteDetails.Find(t => t.STATION_NAME == SnObject.NEXT_STATION);
                                C_ROUTE_DETAIL TestDetail = RouteDetails.Find(t => t.STATION_NAME == Station);
                                if (TestDetail == null || SnObject.NEXT_STATION == "SHIPFINISH" || (NextDetail != null && TestDetail != null && NextDetail.SEQ_NO > TestDetail.SEQ_NO))
                                {
                                    client.Updateable<TestRecord>().UpdateColumns(t => t.SentToSFC == "1").Where(t => t.SerNum == TR.SerNum && t.Area == TR.Area && t.RecTime == TR.RecTime).ExecuteCommand();
                                }
                            }
                            else
                            {
                                client.Updateable<TestRecord>().UpdateColumns(t => t.SentToSFC == "1").Where(t => t.SerNum == TR.SerNum && t.Area == TR.Area && t.RecTime == TR.RecTime).ExecuteCommand();
                            }
                            continue;
                        }
                    }

                    //bool alreadyExist = SFCDB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == SerialNo && t.MESSTATION == Station && t.STARTTIME == Convert.ToDateTime(dr["RECTIME"].ToString()));
                    bool alreadyExist = SFCDB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == SerialNo && t.MESSTATION == Station && t.STARTTIME == TR.RecTime);
                    if (alreadyExist)
                    {
                        continue;
                    }
                    client.Updateable<TestRecord>().UpdateColumns(t => t.SentToSFC == "1").Where(t => t.SerNum == TR.SerNum && t.Area == TR.Area && t.RecTime == TR.RecTime).ExecuteCommand();

                    //寫記錄到 R_TEST_RECORD 表中
                    record.ID = Recorder.GetNewID(BU, SFCDB);
                    record.R_SN_ID = RSN.GetSN(SerialNo, SFCDB).ID;
                    record.SN = SerialNo;
                    PassFail = PassFail.StartsWith("P") ? "PASS" : "FAIL";
                    record.STATE = PassFail;
                    record.TEGROUP = TR.Area.ToUpper();
                    record.TESTATION = Station;
                    record.MESSTATION = Station.Equals("PASTE") ? "DOM" : Station;
                    //record.DEVICE = dr["MACHINE"].ToString();
                    record.DEVICE = LineMapping(TR.Machine);
                    /*  20191125 Request By SY.Lin And IE 彥晶
                    由於Cisco客戶要求:在RoBTA S1上測試的機種必須上拋到客戶的CMRC上; 
                    所以經過TE將資料抓回來後;Machine欄位是fxsict1,Area欄位是ROBAT,
                    SY和彥晶確認將這符合這兩個條件的數據歸類到A13ICTS101                   
                    但Machine欄位是被定義為Line,Area欄位是被定義為工站明稱的,
                   為了滿足他們的實際需求,黨看到TR.Area=="ROBAT" and  TR.Machine=="Machine欄位是fxsict1"  */
                    string VarLine = TR.Machine.ToUpper();
                    string VarStation = TR.Area.ToUpper();
                    if (VarLine == "FXSICT1" && VarStation == "ROBAT")
                    {
                        record.DEVICE = "A13ICTS101";
                    }
                    //record.STARTTIME = Convert.ToDateTime(dr["RECTIME"].ToString());
                    record.STARTTIME = TR.RecTime;
                    record.ENDTIME = record.STARTTIME;
                    record.EDIT_TIME = Recorder.GetDBDateTime(SFCDB);
                    record.DETAILTABLE = "from TEDB 10.117.36.52";
                    //record.EDIT_EMP = dr["USERNAME"].ToString();
                    record.EDIT_EMP = TR.UserName;
                    //record.TESTINFO = TR.RecTime.ToString();
                    Recorder.InsertTestRecord(record, SFCDB);

                    //TR.SentToSFC = "1";
                    

                    //更新測試資料庫的 TENUM3 為 86 表示這筆資料處理過
                    //client.Ado.BeginTran();
                    //List<SugarParameter> parameters = new List<SugarParameter>();
                    //parameters.Add(new SugarParameter("SERIALNO", SerialNo));
                    //parameters.Add(new SugarParameter("AREA", Station));
                    //parameters.Add(new SugarParameter("RECTIME", dr["RECTIME"].ToString()));
                    //client.Ado.ExecuteCommand(@"update tbl_dbBPD
                    //                           set senttosfc = 1
                    //                         where sernum = @SERIALNO
                    //                           and area = @AREA
                    //                           and convert(varchar(20), rectime, 120) = @RECTIME", parameters);
                    //client.Ado.ExecuteCommand($@"update tbl_dbBPD
                    //                           set senttosfc = 1
                    //                         where sernum = '{SerialNo}'
                    //                           and area = '{Station}'");
                    //and convert(varchar(20), rectime, 120) = '{dr["RECTIME"].ToString()}'
                    //client.Ado.CommitTran();

                    //client.Ado.BeginTran(IsolationLevel.ReadCommitted);

                    if (!Station.Equals("PASTE"))
                    {
                        if (PassFail.Equals("PASS"))
                        {
                            //寫過站
                           

                            T_R_REPAIR_MAIN t_repair = new T_R_REPAIR_MAIN(SFCDB, DB_TYPE_ENUM.Oracle);
                            R_REPAIR_MAIN repair = t_repair.GetSNBySN(SerialNo,SFCDB);
                            if (repair != null)
                            {
                                repair.CLOSED_FLAG = "1";
                                int RowRRepairMainNum = t_repair.UpdateRepairSN(repair, SerialNo, "0", SFCDB);
                                if (RowRRepairMainNum < 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { SerialNo }));
                                }
                            }
                            RSN.PassStation(SerialNo, record.DEVICE, record.MESSTATION, record.DEVICE, BU, "PASS", record.EDIT_EMP, SFCDB);
                            success++;
                        }
                        else
                        {
                            RSN.PassStation(SerialNo, record.DEVICE, record.MESSTATION, record.DEVICE, BU, "FAIL", record.EDIT_EMP, SFCDB);
                            //RSN.RecordPassStationDetail(SerialNo, record.DEVICE, record.MESSTATION, record.DEVICE, BU, SFCDB, "1");
                        }

                        //寫UPH，良率
                        RSN.RecordUPH(SnObject.WORKORDERNO, 1, SerialNo, PassFail, record.DEVICE, record.MESSTATION, record.EDIT_EMP, BU, SFCDB);
                        RSN.RecordYieldRate(SnObject.WORKORDERNO, 1, SerialNo, record.STATE, record.DEVICE, record.MESSTATION, record.EDIT_EMP, BU, SFCDB);
                        
                        //else
                        //{
                        //    //寫不良
                        //    SnObject.REPAIR_FAILED_FLAG = "1";
                        //    RSN.Update(SnObject, SFCDB);
                        //    //插入到 R_REPAIR_MAIN 表
                        //    RepairMain.Insert(SnObject, Station, record.DEVICE, record.EDIT_EMP, record.STARTTIME, BU, SFCDB);

                        //}
                    }

                    
                }
                catch (Exception)
                {
                    SFCDB.RollbackTrain();
                    continue;
                }

            }
            MESPubLab.WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.BPD.RecordTestTable", "DoRecordTest", $@"deal with {count} DB records and finally success get {success} records.", "", "MESInterface");
        }

        public override void init()
        {
            DBHost = ConfigGet("TESTDB_HOST");
            DBPort = ConfigGet("TESTDB_PORT");
            DBName = ConfigGet("TESTDB_NAME");
            DBUser = ConfigGet("TESTDB_USER");
            DBPass = CipherTool.Decode(ConfigGet("TESTDB_PASS"));
            DBLocalName = ConfigGet("DB");
            BU = ConfigGet("BU");
            ConnectionString.Append("Data Source=").Append(DBHost).Append(",").Append(DBPort).Append(";")
                .Append("Initial Catalog=").Append(DBName).Append(";")
                .Append("User ID=").Append(DBUser).Append(";")
                .Append("Password=").Append(DBPass).Append(";");
            SFCDB = new OleExec(DBLocalName, true);
            client = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString.ToString(),
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute
            });
            MappingTableList MTL = new MappingTableList();
            MTL.Add("TestRecord", "TestDB.dbo.tbl_dbBPD");
            client.MappingTables = MTL;
            Recorder = new T_R_TEST_RECORD(SFCDB, DB_TYPE_ENUM.Oracle);
            RepairMain = new T_R_REPAIR_MAIN(SFCDB, DB_TYPE_ENUM.Oracle);
            RSN = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
            //設定界面
            Output.UI = new RecordTestTable_UI(this);
            base.init();
        }


    }

    class TestRecord
    {
        public string Machine { get; set; }
        public DateTime RecTime { get; set; }
        public string SerNum { get; set; }
        public string UUTType { get; set; }
        public string Area { get; set; }
        public string PassFail { get; set; }
        public string Test { get; set; }
        public string Cell { get; set; }
        public string LabelNum { get; set; }
        public string LineId { get; set; }
        public string ParentSerNum { get; set; }
        public string PartNum { get; set; }
        public string PartNum2 { get; set; }
        public string SqlTimeStamp { get; set; }
        public string Temperature { get; set; }
        public string UserName { get; set; }
        public string MacId { get; set; }
        public string ScanTime { get; set; }
        public string Status { get; set; }
        public string Repaired { get; set; }
        public string SentToSFC { get; set; }
    }
}
