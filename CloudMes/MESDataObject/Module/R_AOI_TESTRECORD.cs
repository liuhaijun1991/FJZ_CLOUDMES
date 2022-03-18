using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using MESDataObject.Module;

namespace MESDataObject.Module
{
    public class T_R_AOI_TESTRECORD : DataObjectTable
    {
        public T_R_AOI_TESTRECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_AOI_TESTRECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_AOI_TESTRECORD);
            TableName = "R_AOI_TESTRECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 保存讀取到的AOI測試數據,FUNCTION
        /// </summary>
        /// <param name="FileInfo"></param>
        /// <param name="FilesSn">1:保存FileSN數據;0保存LinkSN數據</param>
        /// <param name="DB"></param>
        /// <param name="BU"></param>
        /// <returns></returns>
        public void SaveAOITestData(AOIFileInfo FileInfo,int IsFilesSN,string emp_no, OleExec DB,string BU)
        {
            
            int SNCount;
        
            if (IsFilesSN == 1)
            {
                #region 存儲FilesSN
                SNCount = FileInfo.FilesSNs.Count;
                for (int i = 0; i < SNCount; i++)
                {
                    string SaveDataSql = string.Empty;
                    string ID = string.Empty;
                    int k = IsDataExist(FileInfo.FileSN, FileInfo.FilesSNs[i].SN, FileInfo.TestDate, DB);
                    //存在記錄k=0,更新當前記錄;不存在記錄k>0,插入當前記錄
                    #region 存在記錄,更新當前記錄;不存在記錄,插入當前記錄
                    if (k > 0)
                    {
                        SaveDataSql = $@"update R_AOI_TESTRECORD set  DATA1=nvl(:data1,1),DATA2=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss'),EDIT_EMP=:editemp
                                                where SN=:filesn and DETAIL_SN=:detailsn and TEST_TIME=:testtime and TEST_NAME=:testname and STATION=:station and STATUS=:status
                                       ";
                    }
                    else
                    {
                        ID = MesDbBase.GetNewID(DB.ORM, BU, "R_AOI_TESTRECORD");
                        SaveDataSql = $@"insert into R_AOI_TESTRECORD(ID,SN,DETAIL_SN,TEST_TIME,TEST_NAME,LINE,STATION,STATUS,MODEL_FLAG,FAIL_COUNT,FAIL_DETAIL,DATA1,DATA2,EDIT_EMP,EDIT_TIME)
                                                values(:id,:filesn,:detailsn,:testtime,:testname,:line,:station,:status,:modelflag,:failcount,:faildetail,:data1,to_char(sysdate,'yyyy-mm-dd hh24:mi:ss'),:editemp,sysdate)
                                       ";
                    }
                    #endregion
                    #region 參數
                    OleDbParameter[] paramet = new OleDbParameter[]
                    {
                            new OleDbParameter(":id",ID),
                            new OleDbParameter(":filesn", FileInfo.FileSN),
                            new OleDbParameter(":detailsn", FileInfo.FilesSNs[i].SN),
                            new OleDbParameter(":testtime", FileInfo.TestDate),
                            new OleDbParameter(":testname", FileInfo.TestName),
                            new OleDbParameter(":line", FileInfo.ProductLine),
                            new OleDbParameter(":station", FileInfo.Station),
                            new OleDbParameter(":status", FileInfo.FilesSNs[i].Status ? "P":"F"),
                            new OleDbParameter(":modelflag", FileInfo.LinkType),
                            new OleDbParameter(":failcount", FileInfo.FilesSNs[i].FailCount),
                            new OleDbParameter(":faildetail", FileInfo.FilesSNs[i].FailDetail),
                            new OleDbParameter(":data1", FileInfo.FilesSNs[i].Status ? "1":"2"),
                            //new OleDbParameter(":data2", FileInfo.FileSN),
                            new OleDbParameter(":editemp",emp_no)
                        //new OleDbParameter(":edittime", FileInfo.FileSN),
                    };
                    #endregion
                    
                    DB.ExecuteNonQuery(SaveDataSql, CommandType.Text, paramet);
                    DB.CommitTrain();
                    DB.BeginTrain();
                   
                }
                #endregion
            }
            else
            {
                #region 存儲LinkSN
                SNCount = FileInfo.LinkSNs.Count;
                for (int i = 0; i < SNCount; i++)
                {
                    //ID :id
                    string SaveDataSql = string.Empty;
                    string ID = string.Empty;
                    #region 存在記錄,更新;不存在記錄則插入
                    int k = IsDataExist(FileInfo.FileSN, FileInfo.LinkSNs[i].SN, FileInfo.TestDate, DB);
                    if (k > 0)
                    {
                        SaveDataSql = $@"update R_AOI_TESTRECORD set  DATA1=nvl(:data1,1),DATA2=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss'),EDIT_EMP=:editemp
                                                where SN=:filesn and DETAIL_SN=:detailsn and TEST_TIME=:testtime and TEST_NAME=:testname and STATION=:station and STATUS=:status
                                        ";
                    }
                    else
                    {
                        ID = MesDbBase.GetNewID(DB.ORM, BU, "R_AOI_TESTRECORD");
                        SaveDataSql = $@"insert into R_AOI_TESTRECORD(ID,SN,DETAIL_SN,TEST_TIME,TEST_NAME,LINE,STATION,STATUS,MODEL_FLAG,FAIL_COUNT,FAIL_DETAIL,DATA1,DATA2,EDIT_EMP,EDIT_TIME)
                                                values(:id,:filesn,:detailsn,:testtime,:testname,:line,:station,:status,:modelflag,:failcount,:faildetail,:data1,to_char(sysdate,'yyyy-mm-dd hh24:mi:ss'),:editemp,sysdate)
                                        ";
                    }
                    #endregion
                    #region
                    OleDbParameter[] paramet = new OleDbParameter[]
                    {
                            new OleDbParameter(":id",ID),
                            new OleDbParameter(":filesn", FileInfo.FileSN),
                            new OleDbParameter(":detailsn", FileInfo.LinkSNs[i].SN),
                            new OleDbParameter(":testtime", FileInfo.TestDate),
                            new OleDbParameter(":testname", FileInfo.TestName),
                            new OleDbParameter(":line", FileInfo.ProductLine),
                            new OleDbParameter(":station", FileInfo.Station),
                            new OleDbParameter(":status", FileInfo.LinkSNs[i].Status ? "P":"F"),
                            new OleDbParameter(":modelflag", FileInfo.LinkType),
                            new OleDbParameter(":failcount", FileInfo.LinkSNs[i].FailCount),
                            new OleDbParameter(":faildetail", FileInfo.LinkSNs[i].FailDetail),
                            new OleDbParameter(":data1", FileInfo.LinkSNs[i].Status ? "1":"2"),
                            //new OleDbParameter(":data2", FileInfo.FileSN),
                            new OleDbParameter(":editemp", emp_no)
                    //new OleDbParameter(":edittime", FileInfo.FileSN),
                    };
                    #endregion
                    //DB.BeginTrain();
                    DB.ExecuteNonQuery(SaveDataSql, CommandType.Text, paramet);
                    DB.CommitTrain();
                    DB.BeginTrain();
                    

                }
                #endregion
            }
          
        }
                
        /// <summary>
        /// 判斷R_AOI_TESTRECORD是否存在數據,FUNCTION
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DetailSn"></param>
        /// <param name="TestDate"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public  int IsDataExist(string SN, string DetailSn, string TestDate, OleExec DB)
        {
            //string SaveDataSql = string.Empty;
            int result;
            string IsExistSql = $@"select count(*) from R_AOI_TESTRECORD where SN=:SN and DETAIL_SN=:DetailSn and TEST_TIME=:TestDate";
            OleDbParameter[] paramet = new OleDbParameter[]
               {
                    new OleDbParameter(":SN", SN),
                    new OleDbParameter(":DetailSn", DetailSn),
                    new OleDbParameter(":TestDate", TestDate)
               };

            string res = DB.ExecuteScalar(IsExistSql, CommandType.Text, paramet);

            result = int.Parse(res);

            return result;
        }

        /// <summary>
        /// 獲取沒有DOM資料的DETAIL_SN,Function
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<DataRow> GetNoDomSN(string SN, string Station, OleExec DB)
        {   
            string QuerySql = $@"select DETAIL_SN from R_AOI_TESTRECORD where SN=:sn
                   and STATION=:Station
                   and DETAIL_SN not in (select SN from R_TESTRECORD where STATION_NAME = 'PASTE' AND STATUS = 'P' AND SN = R_AOI_TESTRECORD.DETAIL_SN AND ROWNUM< 2)";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":sn", SN),
                    new OleDbParameter(":Station", Station),
              };

            List<DataRow> datarowlist = new List<DataRow>();
            DataTable res = DB.ExecuteDataTable(QuerySql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        /// <summary>
        /// 標記沒有DOM資料的SN,FUNCTION
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public  int MarkNoDom(string sn, string station, OleExec DB)
        {
            int result;
            string MarkNoDomSql = $@"
                   update R_AOI_TESTRECORD set DATA1='NODOM' , DATA2=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss')
                    WHERE SN=:sn and STATION=:station 
                    and DETAIL_SN not in (select SN from R_TESTRECORD where STATION_NAME='PASTE' AND STATUS='P' AND SN=R_AOI_TESTRECORD.DETAIL_SN AND ROWNUM<2)";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":sn", sn),
                    new OleDbParameter(":station", station),
              };
            result = DB.ExecuteNonQuery(MarkNoDomSql, CommandType.Text, paramet);

            return result;
        }

        /// <summary>
        /// 獲取需要MARK的SN,FUNCTION
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="ProductLine"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public  DataTable GetSNNeedMarkAP(string SN, string Station, string ProductLine, OleExec DB)
        {
            string GetSNSql = $@"
                select SN,DETAIL_SN,STATION,TEST_TIME 
                    from R_AOI_TESTRECORD 
                    where SN=:sn
                    and STATION=:station 
                    and DATA1 !='0'
                    union
                    select SN,DETAIL_SN,STATION,TEST_TIME 
                    from 
                    (
                        select SN,DETAIL_SN,STATION,TEST_TIME  from 
                            (
                            select SN,DETAIL_SN,STATION,TEST_TIME 
                            from R_AOI_TESTRECORD
                            where STATION=:station 
                            and DATA1 in ('AllPartFail', '1', '2', 'DOMOK') 
                            and EDIT_TIME>sysdate-1
                            and LINE=:productline
                            and DATA2 < to_char(sysdate+INTERVAL '-3' MINUTE,'yyyy-mm-dd hh24:mi:ss')               
                            order by DATA2)
                        where rownum<6
                    )
                    ";
            OleDbParameter[] paramet1 = new OleDbParameter[]
             {
                    new OleDbParameter(":sn",SN),
                    new OleDbParameter(":station",Station),
                    new OleDbParameter(":productline",ProductLine)
              };
            DataTable SnDt = DB.ExecuteDataTable(GetSNSql, CommandType.Text, paramet1);
            return SnDt;
        }

        /// <summary>
        /// 針對AP資料進行標記,FUNCTION
        /// </summary>
        /// <param name="resstr"></param>
        /// <param name="detailsn"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public  int MarkAPInfo(string resstr, string detailsn, string station, OleExec DB)
        {
            int ResultSum;

            string MarkAPsql = $@"
                    update  R_AOI_TESTRECORD set DATA1=:resstr,DATA2=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss')
                    where DETAIL_SN=:detailsn and STATION=:station and DATA1 !='0' 
            ";
            OleDbParameter[] paramet = new OleDbParameter[]
                {
                    new OleDbParameter(":resstr",resstr),
                    new OleDbParameter(":detailsn",detailsn),
                    new OleDbParameter(":station",station)
                 };
            ResultSum = DB.ExecuteNonQuery(MarkAPsql, CommandType.Text, paramet);

            #region 提交
            DB.CommitTrain();
            DB.BeginTrain();
            #endregion



            return ResultSum;
        }

        /// <summary>
        /// 更新DOM狀態,標記為DomOK,FUNCTION
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="ProductLine"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public  int UpdateDomOK(string Station, string ProductLine, OleExec DB)
        {
            int result;

            #region 按條件,獲取更新DOMOK的SN

            string UpdateSql = $@"
                update R_AOI_TESTRECORD set DATA1='DOMOK',DATA2=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss')
                    where STATION=:station 
                    and DATA1 in ('NODOM')
                    and EDIT_TIME >sysdate-1
                    and LINE=:productline
                    and DATA2 < to_char(sysdate+INTERVAL '-5' MINUTE,'yyyy-mm-dd hh24:mi:ss')
                    and (select count(*) from R_TESTRECORD where STATION_NAME='PASTE' and SN=R_AOI_TESTRECORD.DETAIL_SN )>0
            ";
            OleDbParameter[] paramet = new OleDbParameter[]
            {
                    //new OleDbParameter(":sn",SN),
                    new OleDbParameter(":station",Station),
                    new OleDbParameter(":productline",ProductLine)
             };
            result = DB.ExecuteNonQuery(UpdateSql, CommandType.Text, paramet);

            #endregion

            #region 提交
            DB.CommitTrain();
            DB.BeginTrain();
            #endregion

            return result;
        }

        /// <summary>
        /// 獲取需要過站的SN,等於原SP中的遊標取值,FUNCTION
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<DataRow> GetDataBeforePass(string SN, string Station, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();

            string QuerySql = $@"
                    select DETAIL_SN,FAIL_DETAIL,TEST_NAME,CASE WHEN STATUS='P' THEN 'PASS' ELSE 'FAIL' END AS STATUS,
                            LINE,STATION,TEST_TIME from R_AOI_TESTRECORD WHERE SN=:sn and DATA1 in ('AllPartOK')
                            and STATION=:station
            ";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":sn",SN),
                    new OleDbParameter(":station", Station)
              };

            DataTable res = DB.ExecuteDataTable(QuerySql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }

            return datarowlist;
        }

        /// <summary>
        /// 獲取第一次過站Fail的SN,等於原SP中的遊標取值,FUNCTION
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<DataRow> GetFirstFailDataBeforePass(string ProductLine, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();

            string QuerySql = $@"
                select * from 
                    (select DETAIL_SN,FAIL_DETAIL,TEST_NAME,CASE WHEN STATUS='P' THEN 'PASS' ELSE 'FAIL' END AS STATUS,
                            LINE,STATION,TEST_TIME from R_AOI_TESTRECORD 
                            WHERE DATA1 in ('AllPartOK')
                            AND LINE=:productline 
                            AND DETAIL_SN in 
                                        ( SELECT SN FROM R_SN 
                                             WHERE SN=R_AOI_TESTRECORD.DETAIL_SN 
                                                AND NEXT_STATION=R_AOI_TESTRECORD.STATION 
                                                AND  REPAIR_FAILED_FLAG=0)
                            AND EDIT_TIME>SYSDATE-1
                    order by DATA2)
                 where rownum<6
            ";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":productline",ProductLine),
                    
              };

            DataTable res = DB.ExecuteDataTable(QuerySql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }

            return datarowlist;
        }

        /// <summary>
        /// 更新R_AOI_TESTRECORD表中的狀態,DATA1=0,表示過站.FUNCTION
        /// </summary>
        /// <param name="DetailSN"></param>
        /// <param name="Station"></param>
        /// <param name="TestTime"></param>
        /// <param name="DB"></param>
        public void AOISNPass(string DetailSN, string Station, string TestTime, OleExec DB)
        {
            string UpdateSql = $@"
                    update R_AOI_TESTRECORD set DATA1='0',DATA2=to_char(sysdate,'yyyy-mm-dd hh24:mi:ss')
                    where DETAIL_SN=:detailsn
                    and STATION=:station
                    and TEST_TIME=:testtime";
            OleDbParameter[] paramet = new OleDbParameter[]
             {
                    //new OleDbParameter(":sn",SN),
                    new OleDbParameter(":detailsn",DetailSN),
                    new OleDbParameter(":station",Station),
                    new OleDbParameter(":testtime",TestTime) 
              };
            DB.ExecuteNonQuery(UpdateSql, CommandType.Text, paramet);
            
        }

        /// <summary>
        /// 檢查AOI表中的SN的DOM資料,Function
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        public string AOICheckDom(string SN,string Skuno,OleExec DB)
        {
            string result = String.Empty;
            T_C_CONTROL cControlTable = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            T_R_TESTRECORD tTestrecordTable = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);


            if (cControlTable.GetControlByNameAndValue("NOCHECKDOM", Skuno, DB) == null && cControlTable.GetControlByNameAndValue("NOCHECKDOM", SN, DB) == null)
            {
                if (tTestrecordTable.GetFirstRecordBySNAndStation("PASTE", SN, DB) == null)
                {
                    //沒有DOM資料,請確認是否有正常掃描
                    result = "MSGCODE20190219183836";
                    return result;
                }

                string CheckDomPN = $@"     select count(*) from  R_TESTRECORD where SN=:sn and CUSTPARTNO in (
                      SELECT SKUNO73 FROM C_K_MAPPING WHERE SKUNO800=:Skuno 
                      UNION
                      SELECT SKUNO800 FROM C_K_MAPPING WHERE SKUNO73=:Skuno
                      UNION
                      SELECT :Skuno FROM DUAL
                    )       ";
                OleDbParameter[] paramet = new OleDbParameter[]
                {
                    new OleDbParameter(":sn",SN),
                    new OleDbParameter(":Skuno",Skuno)
                };

                if (int.Parse(DB.ExecuteScalar(CheckDomPN, CommandType.Text, paramet)) < 1)
                {
                    //DOM料號與工單料號不一致,請確認DOM料號或PE K_Mapping設置是否正確	
                    result = "MSGCODE20190219183930";
                    return result;
                }

                return result;

            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// AOI自動過站時是否需要抽測5dx,及後續一系列操作
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="SN"></param>
        /// <param name="Skuno"></param>
        /// <param name="ProductLine"></param>
        /// <param name="Station"></param>
        /// <param name="EMP"></param>
        /// <param name="dbTime"></param>
        /// <param name="DB"></param>
        /// <param name="WorkOrderNo"></param>
        /// <returns></returns>
        public string AOICheck5DX(string BU,string SN,string Skuno,string ProductLine,string StationName,string EMP,DateTime dbTime,OleExec DB,string WorkOrderNo,string IP)
        {
            #region 檢測是否需要測試5DX
            //dr,是否需要測試5dx
            DataRow dr= NeedCheck5DX(Skuno, StationName, DB);
            //dr0 SKUNO, 
            //dr1 RATE,
            //dr2 CODE_NAME,
            //dr3 STATION_NAME  ,
            //dr4 QTY
            if (dr != null)
            {
                if (Is5DXInRoute(SN, DB) == 1)
                {   
                    //5DX是否在路由中
                    //報錯,該機種路由中存在5DX工站同時配置5DX抽測
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190226081251"));
                }
                else
                {
                    //關閉同一線體存在不同skuno的未關閉的LOT
                    T_R_LOT_STATUS rLotStatusTable = new T_R_LOT_STATUS(DB, DB_TYPE_ENUM.Oracle);
                    rLotStatusTable.UpdateLOTFULL_FLAG(Skuno, StationName, ProductLine, DB);
                    #region 獲取LOT號
                    //獲取LotSize,
                    int LotSize = 100 / int.Parse(dr[1].ToString()) < 10 ? 10 : 100 / int.Parse(dr[1].ToString());
                    //string profilename = "LOT-PCBA " + Skuno + "-" + ProductLine + "-" + Station;
                    //獲取沒有關閉的LotNO,如果沒有,就生成一個
                    string LotNo = rLotStatusTable.GetLotNoNotFull(Skuno, ProductLine, StationName, LotSize, DB).ToUpper().Trim();
                    if (LotNo.Equals(""))
                    {
                        string SeqName = "LH_NSDI_5DXLot";
                        //LotNo = rLotStatusTable.GetNewLotNo(Skuno,ProductLine, StationName, DB).ToUpper().Trim();
                        //LotNo插入數據庫
                        // rLotStatusTable.InLot(BU, LotNo, Skuno, StationName, ProductLine, EMP, DB);

                        T_C_SEQNO CSEQNO = new T_C_SEQNO(DB, DB_TYPE_ENUM.Oracle);
                        LotNo = CSEQNO.GetLotno(SeqName, DB);
                        rLotStatusTable.InLot(BU, LotNo, Skuno, StationName, ProductLine, EMP, DB);
                        T_R_MESNO RMESNO = new T_R_MESNO(DB, DB_TYPE_ENUM.Oracle);
                        RMESNO.InRMESNO(SeqName,BU, LotNo, IP, StationName, ProductLine, EMP, DB);
                    }

                    #endregion

                    #region 檢查是否抽測5dx
                    int yield = int.Parse(dr[1].ToString());
                    DataRow res = rLotStatusTable.GetLotStatusByLotNo(LotNo,DB);
                    int TotalQty = int.Parse(res[2].ToString());
                    int SampleQty = int.Parse(res[3].ToString());
                    string LotID = res[4].ToString();


                    if ((yield == 5 && TotalQty % 20 == 0) || (yield == 100) || (yield != 5 && TotalQty % 10 < int.Parse(yield.ToString().Substring(0, 1))))
                    {
                        TotalQty += 1;
                        SampleQty += 1;
                        T_R_LOT_DETAIL rLotDetail = new T_R_LOT_DETAIL(DB, DB_TYPE_ENUM.Oracle);
                        int SeqNo = rLotDetail.GetSeqNoByLot(LotID, DB);
                        SeqNo += 1;

                        rLotDetail.InsertLotNo(BU, LotID, SN, WorkOrderNo, 1.ToString(), ProductLine, DB, SeqNo);
                        
                        rLotStatusTable.ToUpdateLot(LotNo, ProductLine, TotalQty.ToString(), SampleQty.ToString(), 0.ToString(), DB);
                        T_R_SN rSNTabel = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
                        rSNTabel.UpdateNextStation(SN, "5DX", DB);
                        return SN;
                    }
                    else
                    {
                        TotalQty += 1;
                        rLotStatusTable.ToUpdateLot(LotNo, ProductLine, TotalQty.ToString(), SampleQty.ToString(), 0.ToString(), DB);
                        return null;
                    }





                    #endregion
                }

            }
            return null;
            #endregion
        }


        /// <summary>
        /// 在C_LOT_RATE表中查詢是否配置了抽測5dx
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataRow NeedCheck5DX(string Skuno, string Station,OleExec DB)
        {
            string Sqlstr = $@"select SKUNO,RATE,CODE_NAME,STATION_NAME,QTY from C_LOT_RATE where SKUNO=:skuno and STATION_NAME=:station and SAMPLE_STATION is NULL and VALID_FLAG='1'";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":skuno",Skuno),
                    new OleDbParameter(":station", Station)
              };
            DataTable res = DB.ExecuteDataTable(Sqlstr, CommandType.Text, paramet);

            if (res.Rows.Count > 0)
            {
                return res.Rows[0];
            }
            else
            {
                return null;
            }

            
        }

        /// <summary>
        /// 檢測路由中是否配置了5DX工站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int Is5DXInRoute(string SN,OleExec DB)
        {
            int result;
            string SqlStr = $@"
                select count(*) from C_ROUTE_DETAIL
                    where ROUTE_ID in
                                ( 
                                    select ROUTE_ID from R_SN where SN=:sn 
                                 )
                    and STATION_NAME='5DX'
                ";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":sn",SN)
              };

            result = int.Parse(DB.ExecuteScalar(SqlStr,CommandType.Text,paramet));
            return result;
        }

        public string AOICheck2DX(string BU, string SN, string Skuno, string ProductLine, string StationName, string EMP, DateTime dbTime, OleExec DB, string WorkOrderNo,string IP)
        {
            //檢查是否配置抽測2DX
            DataRow dr = NeedCheck2DX(Skuno, StationName, DB);
            //檢查SN是否已經抽中測試2DX
            DataRow IsIn2DX = IsCheck2DX(SN,WorkOrderNo, StationName, DB);
            T_R_2DX_LOT_STATUS LotStatus2dx = new T_R_2DX_LOT_STATUS(DB, DB_TYPE_ENUM.Oracle);
            if (dr != null)
            {
                #region lot過站??
                //關閉其他LOT
                //Modify by LLF 2019-04-12,增加上傳工站
                //LotStatus2dx.UpdateLotPassFlag(ProductLine,DB);
                LotStatus2dx.UpdateLotPassFlag(ProductLine,"SMT_2DX", DB);
                #endregion

                #region AOI4,檢測前一lot是否fail,是否超時未測試
                if (StationName.Equals("AOI4"))
                {
                    //前一LOT存在2dx Fail 的SN ,立即停線
                    T_R_2DX TR2DX = new T_R_2DX(DB, DB_TYPE_ENUM.Oracle);
                    List<R_2DX> R2DX = TR2DX.IsFailSN(StationName, ProductLine, DB);

                    if (R2DX.Count > 0)
                    {
                        if (R2DX[0].SN != null || R2DX[0].SN != "")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180921160355", new string[] { R2DX[0].SN }));
                        }
                    }
                    //該產品前一LOT超過半小時未測試，請先測試2DX再進行生產！
                    //T_R_2DX_LOT_STATUS R2DXLOTSTATUS = new T_R_2DX_LOT_STATUS(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    R_2DX_LOT_STATUS R2DXLOT = LotStatus2dx.GetByStationLine(StationName, ProductLine, DB);
                    if (R2DXLOT != null/*R2DXLOTSTATUS.GetLotTimeOut(Station.StationName, Station.Line, Station.SFCDB)|| */)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180921165059", new string[] { R2DXLOT.LOT_NO }));
                    }
                }
                #endregion
            }

            //SN需要抽測2DX,且沒有在已抽測中
            #region
            if (dr != null && IsIn2DX == null)
            {

                //同一線體存在不同機種的未關閉的LOT先關閉
                int Num = LotStatus2dx.UpdateLOTFULL_FLAG(Skuno, StationName, EMP, ProductLine,DB);
                if (Num < 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { Skuno }));
                }
                //獲取LOT碼
                int LotSize = int.Parse(dr[4].ToString());
                string LotNo = LotStatus2dx.Get2DXLotNoNotFull(Skuno,ProductLine, StationName, LotSize,DB);
                if (LotNo.Equals(""))
                {
                    //LotNo = LotStatus2dx.GetNew2DXLotNo(Skuno, ProductLine, StationName, DB).ToUpper().Trim();
                   // LotStatus2dx.InLot(BU, LotNo, Skuno, LotSize, StationName, ProductLine, EMP, DB);
                   
                    T_C_SEQNO CSEQNO = new T_C_SEQNO(DB, DB_TYPE_ENUM.Oracle);
                    string SeqName = "LH_NSDI_2DXLot";
                    C_SEQNO SEQNO = CSEQNO.GetSeqnoObj(SeqName, DB);
                    LotNo = CSEQNO.GetLotno(SeqName, DB);
                    //R2DXLOTSTATUS.InLot(Station.BU, Lot, SKUSession.Value.ToString(), Convert.ToDouble(SEQNO.SEQ_NO), Station.StationName, Station.Line, Station.LoginUser.EMP_NO, Station.SFCDB);
                    LotStatus2dx.InLot(BU, LotNo, Skuno, LotSize, StationName, ProductLine, EMP, DB);
                    T_R_MESNO RMESNO = new T_R_MESNO(DB, DB_TYPE_ENUM.Oracle);
                    RMESNO.InRMESNO(SeqName, BU, LotNo, IP, StationName, ProductLine,EMP, DB);




                }

                R_2DX_LOT_STATUS r2DXLotStatus = LotStatus2dx.Get2DXLotStatusByLotno(LotNo, DB);
                // T_R_2DX_LOT_DETAIL R2DXLOTDETAIL = new T_R_2DX_LOT_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_2DX_LOT_DETAIL LotDetail2dx = new T_R_2DX_LOT_DETAIL(DB, DB_TYPE_ENUM.Oracle);
                double SeqNo = LotDetail2dx.GetSeqNoByLot(r2DXLotStatus.ID.ToUpper(), DB);
                SeqNo += 1;
                //插入lotdetail表
                LotDetail2dx.InLot(BU, r2DXLotStatus.ID,LotNo,SN,WorkOrderNo,Skuno,StationName,"0",SeqNo,"0",EMP,DB);
                //更新lot狀態
              
                int LotQty = int.Parse(r2DXLotStatus.LOT_QTY.ToString());
                LotQty += 1;
                LotStatus2dx.UpdateLOT(LotNo,LotQty.ToString(),EMP,DB);


                string res =string.Empty;
                if (LotQty == 1)
                {
                    res = SN + "該SN需要測試2DX";
                }

                return LotNo + "|" + LotSize + "|" + SN + "|" + res;

            }
            #endregion


            return null;

        }

        /// <summary>
        /// 檢測是否抽測2dx
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataRow NeedCheck2DX(string Skuno, string Station, OleExec DB)
        {
            string Sqlstr = $@"select SKUNO,RATE,CODE_NAME,STATION_NAME,QTY from C_LOT_RATE where SKUNO=:skuno and STATION_NAME=:station and SAMPLE_STATION='2DX' and VALID_FLAG='1'";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":skuno",Skuno),
                    new OleDbParameter(":station", Station)
              };
            DataTable res = DB.ExecuteDataTable(Sqlstr, CommandType.Text, paramet);

            if (res.Rows.Count > 0)
            {
                return res.Rows[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 是否已經被抽中測試2DX
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="WorkOrderNo"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataRow IsCheck2DX(string SN, string WorkOrderNo, string Station, OleExec DB)
        {
            string Sqlstr = $@"select 1 from R_2DX_LOT_DETAIL WHERE WO=:workorderno and SN=:sn and STATION=:station";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":sn",SN),
                    new OleDbParameter(":workorderno",WorkOrderNo),
                    new OleDbParameter(":station", Station)
              };
            DataTable res = DB.ExecuteDataTable(Sqlstr, CommandType.Text, paramet);

            if (res.Rows.Count > 0)
            {
                return res.Rows[0];
            }
            else
            {
                return null;
            }
        }



    }
    public class Row_R_AOI_TESTRECORD : DataObjectBase
    {
        public Row_R_AOI_TESTRECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_AOI_TESTRECORD GetDataObject()
        {
            R_AOI_TESTRECORD DataObject = new R_AOI_TESTRECORD();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.DETAIL_SN = this.DETAIL_SN;
            DataObject.TEST_TIME = this.TEST_TIME;
            DataObject.TEST_NAME = this.TEST_NAME;
            DataObject.LINE = this.LINE;
            DataObject.STATION = this.STATION;
            DataObject.STATUS = this.STATUS;
            DataObject.MODEL_FLAG = this.MODEL_FLAG;
            DataObject.FAIL_COUNT = this.FAIL_COUNT;
            DataObject.FAIL_DETAIL = this.FAIL_DETAIL;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string DETAIL_SN
        {
            get
            {
                return (string)this["DETAIL_SN"];
            }
            set
            {
                this["DETAIL_SN"] = value;
            }
        }
        public string TEST_TIME
        {
            get
            {
                return (string)this["TEST_TIME"];
            }
            set
            {
                this["TEST_TIME"] = value;
            }
        }
        public string TEST_NAME
        {
            get
            {
                return (string)this["TEST_NAME"];
            }
            set
            {
                this["TEST_NAME"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string MODEL_FLAG
        {
            get
            {
                return (string)this["MODEL_FLAG"];
            }
            set
            {
                this["MODEL_FLAG"] = value;
            }
        }
        public double? FAIL_COUNT
        {
            get
            {
                return (double?)this["FAIL_COUNT"];
            }
            set
            {
                this["FAIL_COUNT"] = value;
            }
        }
        public string FAIL_DETAIL
        {
            get
            {
                return (string)this["FAIL_DETAIL"];
            }
            set
            {
                this["FAIL_DETAIL"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
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
    }
    public class R_AOI_TESTRECORD
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string DETAIL_SN { get; set; }
        public string TEST_TIME { get; set; }
        public string TEST_NAME { get; set; }
        public string LINE { get; set; }
        public string STATION { get; set; }
        public string STATUS { get; set; }
        public string MODEL_FLAG { get; set; }
        public double? FAIL_COUNT { get; set; }
        public string FAIL_DETAIL { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }

  
    



}