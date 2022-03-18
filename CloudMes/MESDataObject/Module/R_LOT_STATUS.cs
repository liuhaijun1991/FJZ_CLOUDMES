using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{ 
    public class T_R_LOT_STATUS : DataObjectTable
    {
        public T_R_LOT_STATUS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LOT_STATUS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LOT_STATUS);
            TableName = "R_LOT_STATUS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
                
        public Row_R_LOT_STATUS GetByLotNo(string LotNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_LOT_STATUS row = (Row_R_LOT_STATUS)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Row_R_LOT_STATUS GetByInput(string _InputData, string ColoumName, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS R = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                //Modify by LLF 2018-02-24
                //strsql = $@" select ID from r_lot_status where {ColoumName}='{_InputData.Replace("'", "''")}' and closed_flag='0'";
                strsql = $@" select ID from r_lot_status where {ColoumName}='{_InputData.Replace("'", "''")}' ";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { ColoumName+":" + _InputData });
                    //throw new MESReturnMessage(errMsg);
                    R = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Row_R_LOT_STATUS GetLotBySku(string _InputData, string ColoumName, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS R = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select ID from r_lot_status where {ColoumName}='{_InputData.Replace("'", "''")}' and closed_flag='0'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { ColoumName+":" + _InputData });
                    //throw new MESReturnMessage(errMsg);
                    R = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<OBA_Yield_Data> GetDataByDays(string strTimeFrom, string strTimeTo,string SAMPLE_STATION,string skuFlag,OleExec DB)
        {
            List<OBA_Yield_Data> OBAList = new List<OBA_Yield_Data>();
            string strsql = "";
            string strsql0 = "";
            string strGroupBy = "";
            string strSelect = "NULL AS SKUNO,";
            if (skuFlag == "True")
            {
                strSelect = "SKUNO,";
                strGroupBy = ",SKUNO";
            }

            strsql = $@"SELECT TO_CHAR(EDIT_TIME,'YYYY/MM/DD') AS TIME,{strSelect}COUNT(LOT_QTY) AS TOTALBUILTLOTS,SUM(LOT_QTY) AS TotalLotQTY,
                        SUM(SAMPLE_QTY) AS TOTALSAMPLEQTY,SUM(PASS_QTY) AS TOTALPASSQTY,SUM(FAIL_QTY) AS TOTALFAILQTY
                        FROM R_LOT_STATUS WHERE 
                        EDIT_TIME 
                        BETWEEN 
                        TO_DATE('{strTimeFrom} 00:00:00','YYYY/MM/DD HH24:MI:SS') AND
                        TO_DATE('{strTimeTo} 23:59:59','YYYY/MM/DD HH24:MI:SS')
                        AND SAMPLE_STATION='{SAMPLE_STATION}'
                        GROUP BY TO_CHAR(EDIT_TIME,'YYYY/MM/DD'){strGroupBy}
                        ORDER BY TO_CHAR(EDIT_TIME,'YYYY/MM/DD')";

            strsql0 = $@"SELECT TO_CHAR(EDIT_TIME,'YYYY/MM/DD') AS TIME,{strSelect}COUNT(*) AS TOTALFAILLOTS
                        FROM R_LOT_STATUS
                        WHERE FAIL_QTY>= REJECT_QTY AND FAIL_QTY<>0
                        AND EDIT_TIME BETWEEN
                        TO_DATE('{strTimeFrom} 00:00:00','YYYY/MM/DD HH24:MI:SS') AND
                        TO_DATE('{strTimeTo} 23:59:59','YYYY/MM/DD HH24:MI:SS')
                        AND SAMPLE_STATION='{SAMPLE_STATION}'
                        GROUP BY TO_CHAR(EDIT_TIME, 'YYYY/MM/DD'){strGroupBy}
                        ORDER BY TO_CHAR(EDIT_TIME, 'YYYY/MM/DD')";

            DataTable table = DB.ExecSelect(strsql).Tables[0];
            DataTable table0 = DB.ExecSelect(strsql0).Tables[0];

            foreach (DataRow item in table.Rows)
            {
                OBAList.Add(new OBA_Yield_Data
                     {
                        Time = item["TIME"].ToString(),
                        SKUNO = item["SKUNO"].ToString(),
                        TotalBuiltLots = item["TOTALBUILTLOTS"].ToString(),
                        TotalLotQTY = item["TotalLotQTY"].ToString(),
                        TotalSampleQTY = item["TOTALSAMPLEQTY"].ToString(),
                        TotalPassQTY = item["TOTALPASSQTY"].ToString(),
                        TotalFailQTY = item["TOTALFAILQTY"].ToString(),
                    }
                 );
            }
            
            foreach (var o in OBAList)
            {
                o.TotalFailLots = "0";
                o.TotalPassLots = o.TotalBuiltLots;
                o.LotFailRate = "0.00%";
                o.LotFailRatex100 = 0.00;
                foreach (DataRow item in table0.Rows)
                {
                    if ((o.Time == item["TIME"].ToString() && skuFlag != "True") || (o.Time == item["TIME"].ToString() && o.SKUNO == item["SKUNO"].ToString() && skuFlag == "True"))
                    {
                        double Rate;
                        o.TotalFailLots = item["TOTALFAILLOTS"].ToString();
                        o.TotalPassLots = (Convert.ToInt32(o.TotalBuiltLots) - Convert.ToInt32(o.TotalFailLots)).ToString();
                        Rate = Convert.ToDouble(o.TotalFailLots)==0?0:Math.Round((Convert.ToDouble(o.TotalFailLots) / Convert.ToDouble(o.TotalBuiltLots)), 4);
                        o.LotFailRate = (Rate * 100).ToString() + "%";
                        o.LotFailRatex100 = Rate * 100;
                        break;
                    }
                }
                
            }

            return OBAList;
        }

        public List<OBA_Yield_Data> GetDataByWeeks(string strTimeFrom, string strTimeTo,string Year,string Week,string SAMPLE_STATION,string skuFlag, OleExec DB)
        {
            List<OBA_Yield_Data> OBAList = new List<OBA_Yield_Data>();
            string strsql = "";
            string strsql0 = "";
            string strTimeShow = Year+"-WK"+ Week;
            string strGroupBy = "";
            string strSelect = "NULL AS SKUNO,";
            if (skuFlag == "True") {
                strSelect = "SKUNO,";
                strGroupBy = "GROUP BY SKUNO";
            }
            strsql = $@"SELECT '{strTimeShow}' AS TIME,{strSelect}COUNT(LOT_QTY) AS TOTALBUILTLOTS,SUM(LOT_QTY) AS TotalLotQTY,
                        SUM(SAMPLE_QTY) AS TOTALSAMPLEQTY,SUM(PASS_QTY) AS TOTALPASSQTY,SUM(FAIL_QTY) AS TOTALFAILQTY
                        FROM R_LOT_STATUS WHERE 
                        EDIT_TIME BETWEEN 
                        TO_DATE('{strTimeFrom} 00:00:00','YYYY/MM/DD HH24:MI:SS') AND
                        TO_DATE('{strTimeTo} 23:59:59','YYYY/MM/DD HH24:MI:SS')
                        AND SAMPLE_STATION='{SAMPLE_STATION}'
                        {strGroupBy}";

            strsql0 = $@"SELECT '{strTimeShow}' AS TIME,{strSelect}COUNT(*) AS TOTALFAILLOTS
                        FROM R_LOT_STATUS
                        WHERE FAIL_QTY>= REJECT_QTY AND FAIL_QTY<>0
                        AND EDIT_TIME BETWEEN
                        TO_DATE('{strTimeFrom} 00:00:00','YYYY/MM/DD HH24:MI:SS') AND
                        TO_DATE('{strTimeTo} 23:59:59','YYYY/MM/DD HH24:MI:SS')
                        AND SAMPLE_STATION='{SAMPLE_STATION}'
                        {strGroupBy}";

            DataTable table = DB.ExecSelect(strsql).Tables[0];
            DataTable table0 = DB.ExecSelect(strsql0).Tables[0];

            foreach (DataRow item in table.Rows)
            {
                OBAList.Add(new OBA_Yield_Data
                {  
                    Time = item["TIME"].ToString(),
                    SKUNO = item["SKUNO"].ToString(),
                    TotalBuiltLots = item["TOTALBUILTLOTS"].ToString() != "" ? item["TOTALBUILTLOTS"].ToString() : "0",
                    TotalLotQTY = item["TotalLotQTY"].ToString() != "" ? item["TotalLotQTY"].ToString() : "0",
                    TotalSampleQTY = item["TOTALSAMPLEQTY"].ToString() != "" ? item["TOTALSAMPLEQTY"].ToString() : "0",
                    TotalPassQTY = item["TOTALPASSQTY"].ToString() != "" ? item["TOTALPASSQTY"].ToString() : "0",
                    TotalFailQTY = item["TOTALFAILQTY"].ToString() != "" ? item["TOTALFAILQTY"].ToString() : "0",
                }
                 );
            }
       
            foreach (var o in OBAList)
            {
                o.TotalFailLots = "0";
                o.TotalPassLots = o.TotalBuiltLots;
                o.LotFailRate = "0.00%";
                o.LotFailRatex100 = 0.00;
                foreach (DataRow item in table0.Rows)
                {
                    if ((o.Time == item["TIME"].ToString()&&skuFlag!="True")||(o.Time == item["TIME"].ToString() && o.SKUNO == item["SKUNO"].ToString() && skuFlag == "True"))
                    {
                        double Rate;
                        o.TotalFailLots = item["TOTALFAILLOTS"].ToString();
                        o.TotalPassLots = (Convert.ToInt32(o.TotalBuiltLots) - Convert.ToInt32(o.TotalFailLots)).ToString();
                        Rate = Convert.ToDouble(o.TotalFailLots) == 0 ? 0 : Math.Round((Convert.ToDouble(o.TotalFailLots) / Convert.ToDouble(o.TotalBuiltLots)), 4);
                        o.LotFailRate = Rate==0?"0.00%":(Rate * 100).ToString() + "%";
                        o.LotFailRatex100 = Rate * 100;
                        break;
                    }
                }
            }

            return OBAList;
        }
        public List<OBA_Yield_Data> GetDataByMonth(string strTimeFrom, string strTimeTo, string SAMPLE_STATION, string skuFlag, OleExec DB)
        {
            List<OBA_Yield_Data> OBAList = new List<OBA_Yield_Data>();
            strTimeTo = strTimeTo.Substring(5, 2) == "12" ? (Convert.ToInt32(strTimeTo.Substring(0, 4)) + 1).ToString()+"/"+ "01" : strTimeTo.Substring(0, 4) + "/" + (Convert.ToInt32(strTimeTo.Substring(5, 2)) + 1).ToString();

            string strsql = "";
            string strsql0 = "";
            string strGroupBy = "";
            string strSelect = "NULL AS SKUNO,";
         
            
            if (skuFlag == "True")
            {
                strSelect = "SKUNO,";
                strGroupBy = ",SKUNO";
            }
            strsql = $@"SELECT TO_CHAR(EDIT_TIME,'YYYY/MM') AS TIME,{strSelect}COUNT(LOT_QTY) AS TOTALBUILTLOTS,SUM(LOT_QTY) AS TotalLotQTY,
                        SUM(SAMPLE_QTY) AS TOTALSAMPLEQTY,SUM(PASS_QTY) AS TOTALPASSQTY,SUM(FAIL_QTY) AS TOTALFAILQTY
                        FROM R_LOT_STATUS WHERE 
                        EDIT_TIME BETWEEN 
                        TO_DATE('{strTimeFrom}','YYYY/MM') AND
                        TO_DATE('{strTimeTo}','YYYY/MM')
                        AND SAMPLE_STATION='{SAMPLE_STATION}'
                        GROUP BY TO_CHAR(EDIT_TIME, 'YYYY/MM'){strGroupBy}
                        ORDER BY TO_CHAR(EDIT_TIME, 'YYYY/MM')";

            strsql0 = $@"SELECT TO_CHAR(EDIT_TIME,'YYYY/MM') AS TIME,{strSelect}COUNT(*) AS TOTALFAILLOTS
                        FROM R_LOT_STATUS
                        WHERE FAIL_QTY>= REJECT_QTY AND FAIL_QTY<>0
                        AND EDIT_TIME BETWEEN
                        TO_DATE('{strTimeFrom}','YYYY/MM') AND
                        TO_DATE('{strTimeTo}','YYYY/MM')
                        AND SAMPLE_STATION='{SAMPLE_STATION}'
                        GROUP BY TO_CHAR(EDIT_TIME, 'YYYY/MM'){strGroupBy}
                        ORDER BY TO_CHAR(EDIT_TIME, 'YYYY/MM')";


            DataTable table = DB.ExecSelect(strsql).Tables[0];
            DataTable table0 = DB.ExecSelect(strsql0).Tables[0];

            foreach (DataRow item in table.Rows)
            {
                OBAList.Add(new OBA_Yield_Data
                {
                    Time = item["TIME"].ToString(),
                    SKUNO = item["SKUNO"].ToString(),
                    TotalBuiltLots = item["TOTALBUILTLOTS"].ToString(),
                    TotalLotQTY = item["TotalLotQTY"].ToString(),
                    TotalSampleQTY = item["TOTALSAMPLEQTY"].ToString(),
                    TotalPassQTY = item["TOTALPASSQTY"].ToString(),
                    TotalFailQTY = item["TOTALFAILQTY"].ToString(),
                }
                 );
            }

            foreach (var o in OBAList)
            {
                o.TotalFailLots = "0";
                o.TotalPassLots = o.TotalBuiltLots;
                o.LotFailRate = "0.00%";
                o.LotFailRatex100 = 0.00;
                foreach (DataRow item in table0.Rows)
                {
                    if ((o.Time == item["TIME"].ToString() && skuFlag != "True") || (o.Time == item["TIME"].ToString() && o.SKUNO == item["SKUNO"].ToString() && skuFlag == "True"))
                    {
                        double Rate;
                        o.TotalFailLots = item["TOTALFAILLOTS"].ToString();
                        o.TotalPassLots = (Convert.ToInt32(o.TotalBuiltLots) - Convert.ToInt32(o.TotalFailLots)).ToString();
                        Rate = Convert.ToDouble(o.TotalFailLots) == 0 ? 0 : Math.Round((Convert.ToDouble(o.TotalFailLots) / Convert.ToDouble(o.TotalBuiltLots)), 4);
                        o.LotFailRate = (Rate * 100).ToString() + "%";
                        o.LotFailRatex100 = Rate * 100;
                        break;
                    }
                }

            }
            return OBAList;
        }

        public Row_R_LOT_STATUS GetLotBySkuAnd(string skuno, string stationName, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS R = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select ID from r_lot_status where skuno='{skuno}' and SAMPLE_STATION='{stationName}' and closed_flag='0'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {                   
                    R = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 通過SN 獲取它所在的LOT_NO
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetLotBySNForInLot(string _SN, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_lot_status a where a.closed_flag=1 and exists (select 1 from r_lot_detail b where b.sn = '{_SN.Replace("'", "''")}' and a.id = b.lot_id) order by a.id desc";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //Modify by LLF 2018-02-07
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + _SN });
                    //throw new MESReturnMessage(errMsg);
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 通過SN 獲取它所在的LOT_NO
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetLotBySN(string _SN, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_lot_status a where exists (select 1 from r_lot_detail b where b.sn = '{_SN.Replace("'", "''")}' and a.id = b.lot_id) order by a.id desc";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //Modify by LLF 2018-02-07
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + _SN });
                    //throw new MESReturnMessage(errMsg);
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 通過SN獲取沒有COLSED的LOT信息
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetLotBySNNotCloesd(string sn, OleExec DB)
        {
            string strsql = "";
            string id = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select id from r_lot_status a where a.closed_flag='0' and exists (select 1 from r_lot_detail b 
                                where b.sn = '{sn.Replace("'", "''")}' and a.id = b.lot_id) order by a.id desc";
                id = DB.ExecSelectOneValue(strsql)?.ToString();
                if (id != null)
                {
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(id, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        public Row_R_LOT_STATUS GetSampleLotBySN(string _SN, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_lot_status a where exists (select 1 from r_lot_detail b where b.sn = '{_SN.Replace("'", "''")}' and a.id = b.lot_id and a.closed_flag='1' and a.lot_status_flag='0') order by a.id desc";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        /// <summary>
        /// By packNo 取LotInfo     
        /// </summary>
        /// <param name="packNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_LOT_STATUS> getSampleLotByPackNo(string packNo, OleExec DB)
        {
            List<R_LOT_STATUS> res = new List<R_LOT_STATUS>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string strsql = $@" select b.* from r_lot_pack a,r_lot_status b where a.lotno=b.lot_no and a.packno='{packNo}'  ";
                DataSet ds = DB.ExecSelect(strsql);
                foreach (DataRow VARIABLE in ds.Tables[0].Rows)
                {
                    Row_R_LOT_STATUS row = (Row_R_LOT_STATUS)this.NewRow();
                    row.loadData(VARIABLE);
                    res.Add(row.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return res;
        }

        /// <summary>
        /// FQC Lot 過站
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="LotNo"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="Station"></param>
        /// <param name="Line"></param>
        /// <param name="BU"></param>
        /// <param name="DB"></param>
        /// <param name="FailInfos"></param>
        public void LotPassStation(string SerialNo, string LotNo, string PassOrFail, string EmpNo, string Station, string DeviceName, string Line, string BU, OleExec DB, params string[] FailInfos)
        {
            bool PassedFlag = true;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_LOT_STATUS StatusRow = (Row_R_LOT_STATUS)NewRow();
            T_R_LOT_DETAIL DetailTable = new T_R_LOT_DETAIL(DB, this.DBType);
            Row_R_LOT_DETAIL DetailRow = (Row_R_LOT_DETAIL)DetailTable.NewRow();
            R_LOT_STATUS Status = null;
            R_LOT_DETAIL Detail = null;
            T_R_SN SnTable = new T_R_SN(DB, this.DBType);
            List<string> LotsSN = new List<string>();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                PassedFlag = PassOrFail.ToUpper().Equals("PASS") ? true : false;
                //sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}' AND SAMPLE_STATION='{Station}' AND LINE='{Line}'"; //判斷有沒有 LOT
                sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}' AND SAMPLE_STATION='{Station}'"; //判斷有沒有 LOT
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    StatusRow.loadData(dt.Rows[0]);
                    Status = StatusRow.GetDataObject();
                    sql = $@"SELECT A.* FROM R_LOT_DETAIL A,R_SN B WHERE LOT_ID='{StatusRow.ID}' AND B.SN='{SerialNo}' AND A.SN=B.SN"; //判斷Lot中有沒有這個SN並且沒有被抽檢過
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        DetailRow.loadData(dt.Rows[0]);
                        Detail = DetailRow.GetDataObject();
                        if (Detail.SAMPLING.Equals("1"))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000093", new string[] { SerialNo }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000094", new string[] { SerialNo, LotNo }));
                    }

                    if (Status.CLOSED_FLAG == "1") //Lot 關閉
                    {
                        if (PassedFlag)
                        {
                            //更新 R_LOT_DETAIL STATUS
                            Detail.STATUS = "1"; //1 表示抽檢通過
                            Detail.SAMPLING = "1";//1 表示被抽檢了
                            //更新 R_LOT_STATUS PASS_QTY
                            Status.PASS_QTY++;
                        }
                        else
                        {
                            //更新 R_LOT_DETAIL STATUS ,FAIL_CODE,FAIL_LOCATION,DESCRIPTION

                            Detail.STATUS = "0";//0 表示抽檢失敗
                            Detail.SAMPLING = "1";//1 表示被抽檢了
                            if (FailInfos != null && FailInfos.Length == 3) //記錄失敗原因
                            {
                                Detail.FAIL_CODE = FailInfos[0];
                                Detail.FAIL_LOCATION = FailInfos[1];
                                Detail.DESCRIPTION = FailInfos[2];
                            }


                            //更新 R_LOT_STATUS FAIL_QTY
                            Status.FAIL_QTY++;

                        }
                        if (Status.FAIL_QTY >= Status.REJECT_QTY && Status.FAIL_QTY!=0)
                        {
                            //更新 R_LOT_STATUS 關閉，NG，
                            //Status.CLOSED_FLAG = "1";// 1 表示關閉Lot
                            Status.LOT_STATUS_FLAG = "2";// 2 表示整個 Lot 不良
                            //更新 R_LOT_DETAIL 鎖定LOT 中所有
                            Detail.EDIT_EMP = EmpNo;
                            Detail.EDIT_TIME = GetDBDateTime(DB);
                            DetailRow.ConstructRow(Detail);
                            DB.ExecSQL(DetailRow.GetUpdateString(this.DBType));
                            //該批次鎖定--add by Eden 2018-05-04
                            sql = $@"update r_lot_detail set sampling='4' where lot_id='{Detail.LOT_ID}'";
                            DB.ExecSQL(sql);
                            //DetailTable.LockLotBySn(SerialNo, EmpNo, DB);
                        }
                        else
                        {
                            if (Status.PASS_QTY + Status.FAIL_QTY >= Status.SAMPLE_QTY)
                            {
                                //更新 R_LOT_STATUS 關閉，OK
                                //Status.CLOSED_FLAG = "1";
                                Status.LOT_STATUS_FLAG = "1"; // 1 表示整個 Lot 正常
                                //更新 R_LOT_DETAIL 鎖定FAIL 的，其他的正常過站
                                //sql = $@"SELECT * FROM R_LOT_DETAIL WHERE LOT_ID='{StatusRow.ID}' AND STATUS='0'";

                                //sql = $@"SELECT * FROM R_LOT_DETAIL WHERE LOT_ID='{StatusRow.ID}' AND ((SAMPLING='1' AND STATUS='1') OR (SAMPLING='0'))";
                                //dt = DB.ExecSelect(sql).Tables[0];
                                //if (dt.Rows.Count > 0)
                                //{
                                //    foreach (DataRow dr in dt.Rows)
                                //    {
                                //        LotsSN.Add(dr["SN"].ToString());
                                //    }
                                //    SnTable.LotsPassStation(LotsSN, Line, Station, DeviceName, BU, PassOrFail, EmpNo, DB); // 過站
                                //}

                                var snobjlist = DB.ORM.Queryable<R_SN, R_LOT_DETAIL>((rs, rld) => rs.SN == rld.SN)
                                    .Where((rs, rld) => rs.VALID_FLAG == "1"
                                                        && rld.LOT_ID == StatusRow.ID &&
                                                        ((rld.SAMPLING == "1" && rld.STATUS == "1") ||
                                                         rld.SAMPLING == "0"))
                                    .Select((rs, rld) => rs).ToList();
                                SnTable.LotsPassStation(snobjlist, Line, Station, DeviceName, BU, PassOrFail, EmpNo, DB); // 過站

                                //記錄通過數 ,UPH
                                foreach (string SN in LotsSN)
                                {
                                    SnTable.RecordYieldRate(Detail.WORKORDERNO, 1, SN, "PASS", Line, Station, EmpNo, BU, DB);
                                    SnTable.RecordUPH(Detail.WORKORDERNO, 1, SN, "PASS", Line, Station, EmpNo, BU, DB);
                                }
                            }

                            Detail.EDIT_EMP = EmpNo;
                            Detail.EDIT_TIME = GetDBDateTime(DB);
                            DetailRow.ConstructRow(Detail);
                            DB.ExecSQL(DetailRow.GetUpdateString(this.DBType));

                        }

                        Status.EDIT_EMP = EmpNo;
                        Status.EDIT_TIME = GetDBDateTime(DB);
                        StatusRow.ConstructRow(Status);
                        DB.ExecSQL(StatusRow.GetUpdateString(this.DBType));
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000201", new string[] { LotNo }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000091", new string[] { LotNo }));
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public void InLotPassStation(string NewLotFlag,R_SN SNObj, string LotNo,string LotSatusID, string Station,string EmpNo,string AQL_TYPE, string Line, string BU, OleExec DB)
        {
            T_R_LOT_STATUS Table_R_Lot_Status = new T_R_LOT_STATUS(DB, DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = (Row_R_LOT_STATUS)NewRow();
            T_R_LOT_DETAIL Table_R_Lot_Detail = new T_R_LOT_DETAIL(DB, DBType);
            Row_R_LOT_DETAIL Row_R_Lot_Detail = (Row_R_LOT_DETAIL)Table_R_Lot_Detail.NewRow();

            T_C_AQLTYPE Table_C_AQLTYPE = new T_C_AQLTYPE(DB, DBType);
            try
            {
                string LotID = "";
                if (NewLotFlag == "1")
                {
                    //Modify by LLF 2018-03-19,生成ID，需根據Table生成
                    //LotID= GetNewID(BU, DB);
                    LotID = Table_R_Lot_Status.GetNewID(BU, DB);
                    Row_R_Lot_Status.ID = LotID;
                    Row_R_Lot_Status.LOT_NO = LotNo;
                    Row_R_Lot_Status.SKUNO = SNObj.SKUNO;
                    Row_R_Lot_Status.AQL_TYPE = AQL_TYPE;
                    Row_R_Lot_Status.LOT_QTY = 1;
                    Row_R_Lot_Status.REJECT_QTY = 0;
                    Row_R_Lot_Status.SAMPLE_QTY = 1;
                    Row_R_Lot_Status.PASS_QTY = 0;
                    Row_R_Lot_Status.FAIL_QTY = 0;
                    Row_R_Lot_Status.CLOSED_FLAG = "0";
                    Row_R_Lot_Status.LOT_STATUS_FLAG = "0";
                    Row_R_Lot_Status.LINE = Line;
                    Row_R_Lot_Status.SAMPLE_STATION = Station;
                    Row_R_Lot_Status.EDIT_EMP = EmpNo;
                    Row_R_Lot_Status.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_Lot_Status.GetInsertString(DBType));
                }
                else
                {
                    LotID = LotSatusID;
                    Row_R_Lot_Status = (Row_R_LOT_STATUS)Table_R_Lot_Status.GetObjByID(LotSatusID, DB);
                    int LotQty = (int)Row_R_Lot_Status.LOT_QTY + 1;
                    int SampleQty = Table_C_AQLTYPE.GetSampleQty(AQL_TYPE, LotQty, DB);
                    Row_R_Lot_Status.SAMPLE_QTY = SampleQty;

                    Row_R_Lot_Status.LOT_QTY += 1;
                    DB.ExecSQL(Row_R_Lot_Status.GetUpdateString(DBType));
                }

                //Modify by LLF 2018-03-19,生成ID，需根據Table生成
                //Row_R_Lot_Detail.ID = GetNewID(BU, DB);
                Row_R_Lot_Detail.ID = Table_R_Lot_Detail.GetNewID(BU, DB);
                Row_R_Lot_Detail.LOT_ID = LotID;
                //Row_R_Lot_Detail.LOT_ID = LotID;
                Row_R_Lot_Detail.SN = SNObj.SN;
                Row_R_Lot_Detail.WORKORDERNO = SNObj.WORKORDERNO;
                Row_R_Lot_Detail.CREATE_DATE = GetDBDateTime(DB);
                Row_R_Lot_Detail.SAMPLING = "0";
                Row_R_Lot_Detail.STATUS = "0";
                Row_R_Lot_Detail.FAIL_CODE = "";
                Row_R_Lot_Detail.FAIL_LOCATION = "";
                Row_R_Lot_Detail.DESCRIPTION = "";
                Row_R_Lot_Detail.CARTON_NO = "";
                Row_R_Lot_Detail.PALLET_NO = "";
                Row_R_Lot_Detail.EDIT_EMP = EmpNo;
                Row_R_Lot_Detail.EDIT_TIME = GetDBDateTime(DB);
                DB.ExecSQL(Row_R_Lot_Detail.GetInsertString(DBType));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void OrtInLotPass(string NewLotFlag, R_SN SNObj, int WoQty, int SampleQty, string LotSatusID, string Station, string EmpNo, string Line, string BU, OleExec DB)
        {
            T_R_LOT_STATUS Table_R_Lot_Status = new T_R_LOT_STATUS(DB, DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = (Row_R_LOT_STATUS)NewRow();
            T_R_LOT_DETAIL Table_R_Lot_Detail = new T_R_LOT_DETAIL(DB, DBType);
            Row_R_LOT_DETAIL Row_R_Lot_Detail = (Row_R_LOT_DETAIL)Table_R_Lot_Detail.NewRow();
            try
            {
                string LotID = "";
                if (NewLotFlag == "1")
                {
                    //Modify by LLF 2018-03-19,生成ID，需根據Table生成
                    //LotID= GetNewID(BU, DB);
                    LotID = Table_R_Lot_Status.GetNewID(BU, DB);
                    Row_R_Lot_Status.ID = LotID;
                    Row_R_Lot_Status.LOT_NO = "";
                    Row_R_Lot_Status.SKUNO = SNObj.WORKORDERNO;
                    Row_R_Lot_Status.AQL_TYPE = "";
                    Row_R_Lot_Status.LOT_QTY = WoQty;
                    Row_R_Lot_Status.REJECT_QTY = 0;
                    Row_R_Lot_Status.SAMPLE_QTY = SampleQty;
                    Row_R_Lot_Status.PASS_QTY = 1;
                    Row_R_Lot_Status.FAIL_QTY = 0;
                    Row_R_Lot_Status.CLOSED_FLAG = "0";
                    Row_R_Lot_Status.LOT_STATUS_FLAG = "0";
                    Row_R_Lot_Status.LINE = Line;
                    Row_R_Lot_Status.SAMPLE_STATION = Station;
                    Row_R_Lot_Status.EDIT_EMP = EmpNo;
                    Row_R_Lot_Status.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_Lot_Status.GetInsertString(DBType));
                }
                else
                {
                    LotID = LotSatusID;
                    Row_R_Lot_Status = (Row_R_LOT_STATUS)Table_R_Lot_Status.GetObjByID(LotSatusID, DB);
                    int PASS_QTY = (int)Row_R_Lot_Status.PASS_QTY + 1;

                    Row_R_Lot_Status.PASS_QTY += 1;
                    DB.ExecSQL(Row_R_Lot_Status.GetUpdateString(DBType));
                }

                //Modify by LLF 2018-03-19,生成ID，需根據Table生成
                //Row_R_Lot_Detail.ID = GetNewID(BU, DB);
                Row_R_Lot_Detail.ID = Table_R_Lot_Detail.GetNewID(BU, DB);
                Row_R_Lot_Detail.LOT_ID = LotID;
                //Row_R_Lot_Detail.LOT_ID = LotID;
                Row_R_Lot_Detail.SN = SNObj.SN;
                Row_R_Lot_Detail.WORKORDERNO = SNObj.WORKORDERNO;
                Row_R_Lot_Detail.CREATE_DATE = GetDBDateTime(DB);
                Row_R_Lot_Detail.SAMPLING = "0";
                Row_R_Lot_Detail.STATUS = "0";
                Row_R_Lot_Detail.FAIL_CODE = "";
                Row_R_Lot_Detail.FAIL_LOCATION = "";
                Row_R_Lot_Detail.DESCRIPTION = "";
                Row_R_Lot_Detail.CARTON_NO = "";
                Row_R_Lot_Detail.PALLET_NO = "";
                Row_R_Lot_Detail.EDIT_EMP = EmpNo;
                Row_R_Lot_Detail.EDIT_TIME = GetDBDateTime(DB);
                DB.ExecSQL(Row_R_Lot_Detail.GetInsertString(DBType));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_LOT_STATUS> ListByLotNo(string LotNo, string Line, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_LOT_STATUS> row = new List<R_LOT_STATUS>();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}' AND LINE='{Line}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Row_R_LOT_STATUS RLotStation = (Row_R_LOT_STATUS)NewRow();
                    RLotStation.loadData(dt.Rows[0]);
                    row.Add(RLotStation.GetDataObject());
                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// WZW 關閉Lot號
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateColseLot(string LotNo, string Line, OleExec DB)
        {
            string StrSql = "";
            int res = 0;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                StrSql = $@"UPDATE R_LOT_STATUS SET CLOSED_FLAG='1' WHERE LOT_NO='{LotNo}' AND LINE='{Line}'";
                res = DB.ExecSqlNoReturn(StrSql, null);
                return res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public int ToUpdateLot(string LotNo, string Line, String LOTQTY, String SAMPLEQTY, string Closed, OleExec DB)
        {
            string StrSql = "";
            int res = 0;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                StrSql = $@"UPDATE R_LOT_STATUS SET LOT_QTY='{LOTQTY}',SAMPLE_QTY='{SAMPLEQTY}',CLOSED_FLAG='{Closed}',EDIT_TIME=SYSDATE WHERE LOT_NO='{LotNo}' AND LINE='{Line}'";
                res = DB.ExecSqlNoReturn(StrSql, null);
                return res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// WZW BY KUS 5DXLot
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetBySKU5DXLot(string SKU, string Line, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_LOT_STATUS row = (Row_R_LOT_STATUS)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_LOT_STATUS WHERE SKUNO='{SKU}'AND CLOSED_FLAG='0'AND LINE='{Line}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// WZW BY 5DXLot IP
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_LOT_STATUS> GetBy5DXLotIP(string SKU, string LINE, string IP, OleExec DB)
        {
            string sql = string.Empty;
            R_LOT_STATUS LotStation = new R_LOT_STATUS();
            DataTable dt = new DataTable();
            List<R_LOT_STATUS> row = new List<R_LOT_STATUS>();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_LOT_STATUS A LEFT JOIN R_MESNO B ON A.LOT_NO=B.MES_NO WHERE A.SKUNO='{SKU}'AND A.CLOSED_FLAG='0'AND B.IP='{IP}' and B.LINE='{LINE}' AND A.LINE=B.LINE ";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Row_R_LOT_STATUS RowRLotStation = (Row_R_LOT_STATUS)NewRow();
                    RowRLotStation.loadData(dt.Rows[0]);
                    row.Add(RowRLotStation.GetDataObject());

                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// WZW 生成LOT號
        /// </summary>
        /// <param name="_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void/*int*/ InLot(string BU, string Lot, string SKU, string StationName, string Line, string Emp, OleExec DB)
        {
            //string strsql = "";
            //int Res;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_LOT_STATUS Table_R_Lot_Status = new T_R_LOT_STATUS(DB, DBType);
                    Row_R_LOT_STATUS Row_R_Lot_Status = (Row_R_LOT_STATUS)NewRow();
                    //T_R_LOT_DETAIL Table_R_Lot_Detail = new T_R_LOT_DETAIL(DB, DBType);
                    //Row_R_LOT_DETAIL Row_R_Lot_Detail = (Row_R_LOT_DETAIL)Table_R_Lot_Detail.NewRow();
                    //T_C_AQLTYPE Table_C_AQLTYPE = new T_C_AQLTYPE(DB, DBType);
                    //                strsql = $@"INSERT INTO R_LOT_STATUS (ID, LOT_NO, SKUNO, AQL_TYPE, LOT_QTY, REJECT_QTY, SAMPLE_QTY, PASS_QTY, FAIL_QTY, CLOSED_FLAG, LOT_STATUS_FLAG, SAMPLE_STATION, LINE, EDIT_EMP, EDIT_TIME, AQL_LEVEL) 
                    //VALUES( '{Table_R_Lot_Status.GetNewID(BU, DB)}', '{Lot}', '{SKU}', '0', '0', '0','0', '0', '0', '0', '0', '{StationName}', '{Line}','{Emp}', SYSDATE, '')";
                    //                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                    //                //DB.ExecuteCommand()
                    //                Res = Convert.ToInt32(DB.ExecSQL(strsql));
                    //                return Res;
                    string LotID = "";
                    LotID = Table_R_Lot_Status.GetNewID(BU, DB);
                    Row_R_Lot_Status.ID = LotID;
                    Row_R_Lot_Status.LOT_NO = Lot;
                    Row_R_Lot_Status.SKUNO = SKU;
                    Row_R_Lot_Status.AQL_TYPE = "";
                    Row_R_Lot_Status.LOT_QTY = 0;
                    Row_R_Lot_Status.REJECT_QTY = 0;
                    Row_R_Lot_Status.SAMPLE_QTY = 0;
                    Row_R_Lot_Status.PASS_QTY = 0;
                    Row_R_Lot_Status.FAIL_QTY = 0;
                    Row_R_Lot_Status.CLOSED_FLAG = "0";
                    Row_R_Lot_Status.LOT_STATUS_FLAG = "0";
                    Row_R_Lot_Status.LINE = Line;
                    Row_R_Lot_Status.SAMPLE_STATION = StationName;
                    Row_R_Lot_Status.EDIT_EMP = Emp;
                    Row_R_Lot_Status.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_Lot_Status.GetInsertString(DBType));
                }
                catch (Exception)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { Lot });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public R_LOT_STATUS GetLastFQCRecord(string Skuno, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno).ToList().OrderByDescending(t => t.EDIT_TIME).FirstOrDefault();
        }

        public int GetLast5FQCRecordTIGHTCount(string Skuno, string AQLType, string LotClose, string LotStatus, OleExec DB)//LotStatus:Lot pass 為1,Fail 為 2
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno).ToList().OrderByDescending(t => t.EDIT_TIME).Take(5).ToList().Where(t => t.AQL_TYPE == AQLType && t.CLOSED_FLAG == LotClose && t.LOT_STATUS_FLAG == LotStatus).ToList().Count;
        }

        public R_LOT_STATUS GetOBALastFailRecordBySku(string Skuno, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno && t.CLOSED_FLAG == "2" && t.LOT_STATUS_FLAG == "2").ToList().OrderByDescending(t => t.EDIT_TIME).FirstOrDefault();
        }

        public int GetOBALastFailAfter5RecordBySku(string Skuno, DateTime? Date, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno && t.EDIT_TIME > Date).ToList().Take(5).ToList().Where(t => t.CLOSED_FLAG == "2" && t.LOT_STATUS_FLAG == "1").ToList().Count;
        }

        public int GetOBALastFailBefore5RecordBySku(string Skuno, DateTime? Date, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno && t.EDIT_TIME < Date).ToList().Take(5).ToList().Where(t => t.CLOSED_FLAG == "2" && t.LOT_STATUS_FLAG == "2").ToList().Count;
        }

        public R_LOT_STATUS GetLastOpenLotBySku(string Skuno, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno && t.CLOSED_FLAG == "0").ToList().Take(1).ToList().Where(t => t.CLOSED_FLAG == "0").ToList().OrderByDescending(t => t.LOT_NO).FirstOrDefault();
        }

        /// <summary>
        /// 關閉本線別,本工站,其他料號的lot狀態
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="station"></param>
        /// <param name="lasteditby"></param>
        /// <param name="Line"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateLOTFULL_FLAG(string SKU, string station, string Line, OleExec DB)
        {
            string StrSql = "";
            int res = 0;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                StrSql = $@"UPDATE R_LOT_STATUS set CLOSED_FLAG = 1, EDIT_TIME = SYSDATE where CLOSED_FLAG = 0 and skuno != '{SKU}' and line='{Line}' and sample_station='{station}'";
                res = DB.ExecSqlNoReturn(StrSql, null);
                return res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 獲取一個沒有滿的LOT號(AOI用)
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="ProductLine"></param>
        /// <param name="Station"></param>
        /// <param name="LotSize"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetLotNoNotFull(string Skuno, string ProductLine, string Station, int LotSize, OleExec DB)
        {
            string SqlStr = $@"
                select LOT_NO from R_LOT_STATUS 
                WHERE CLOSED_FLAG=0 
                AND SKUNO='{Skuno}'
                and SAMPLE_STATION='{Station}'
                and LINE='{ProductLine}'
                AND LOT_QTY<'{LotSize}'
                ";
            DataSet result = DB.ExecSelect(SqlStr);
            string res = string.Empty;



            if (result.Tables[0].Rows.Count > 0)
            {
                res = result.Tables[0].Rows[0][0].ToString();
            }
            return res;
        }

        public DataRow GetLotStatusByLotNo(string LotNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            //DataRow row = new DataRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT LOT_NO,SKUNO,LOT_QTY,SAMPLE_QTY,ID FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
                else
                {
                    //沒有LOT號
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20190228102239", new string[] { LotNo });
                    throw new MESReturnMessage(errMsg);
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<string> GetOBANoCloseLotNo(string IP, string StrStation, OleExec DB)
        {
            List<string> ListSN = new List<string>();

            //           string StrSQL = $@"SELECT '' AS LOT_NO FROM DUAL union
            // SELECT distinct a.LOT_NO FROM R_LOT_STATUS A, R_LOT_PACK B,R_PACKING C WHERE A.lot_no = b.LOTNO AND A.LOT_NO = '{IP}'
            //AND B.packno = c.pack_no  union  SELECT A.LOT_NO FROM R_LOT_STATUS A, R_LOT_DETAIL B WHERE A.lot_no = '{IP}' AND A.ID = B.LOT_ID ";

            string StrSQL = $@" SELECT '' AS LOT_NO FROM DUAL union
             SELECT '{IP}' AS LOT_NO FROM DUAL union
             SELECT distinct a.LOT_NO FROM R_LOT_STATUS A, R_LOT_PACK B
              WHERE A.lot_no = b.LOTNO AND a.closed_flag = '0' AND a.LINE='{IP}' AND a.SAMPLE_STATION='{StrStation}' order by LOT_NO desc ";

            DataTable table = DB.ExecSelect(StrSQL).Tables[0];
            if (table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    ListSN.Add(dr["LOT_NO"].ToString());
                }
            }
            return ListSN;
        }

        public R_LOT_STATUS GetLastOpenLotBySkuLine(string Skuno, string IP, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.SKUNO == Skuno && t.LINE == IP && t.CLOSED_FLAG == "0").ToList().Take(1).ToList().Where(t => t.CLOSED_FLAG == "0").ToList().OrderByDescending(t => t.LOT_NO).FirstOrDefault();
        }

        public R_LOT_STATUS GetLast5DXOpenLotByLine(string Line, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS>().Where(t => t.LINE == Line && t.CLOSED_FLAG == "0").ToList().ToList().OrderByDescending(t => t.LOT_NO).FirstOrDefault();
        }

        public R_LOT_STATUS GetNotClosingLot(string skuno, string aql_type, string sample_station, bool is_rework, OleExec DB)
        {
            if (is_rework)
            {
                return DB.ORM.Queryable<R_LOT_STATUS>().Where(r => r.SKUNO == skuno && r.AQL_TYPE == aql_type && r.SAMPLE_STATION == sample_station
                && r.CLOSED_FLAG == "0" && r.LOT_STATUS_FLAG == "0" && r.EDIT_EMP == "REWORK").ToList().FirstOrDefault();
            }
            else
            {
                return DB.ORM.Queryable<R_LOT_STATUS>().Where(r => r.SKUNO == skuno && r.AQL_TYPE == aql_type && r.SAMPLE_STATION == sample_station
                && r.CLOSED_FLAG == "0" && r.LOT_STATUS_FLAG == "0").ToList().FirstOrDefault();
            }
        }

        public R_LOT_STATUS GetLotBySNAndStation(string sn, string station, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((s, d) => s.ID == d.LOT_ID)
                .Where((s, d) => d.SN == sn && s.SAMPLE_STATION == station)
                .OrderBy((s, d) => d.CREATE_DATE, SqlSugar.OrderByType.Desc)
                .Select((s, d) => s).ToList().FirstOrDefault();
        }
        public R_LOT_STATUS GetLotBySNAndWo(string sn, string wo, OleExec DB)
        {
            return DB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((s, d) => s.ID == d.LOT_ID)
                .Where((s, d) => d.SN == sn && d.WORKORDERNO == wo)
                .OrderBy((s, d) => d.CREATE_DATE, SqlSugar.OrderByType.Desc)
                .Select((s, d) => s).ToList().FirstOrDefault();
        }

        public int InsertNewLot(R_LOT_STATUS lotObject, OleExec DB)
        {
            return DB.ORM.Insertable<R_LOT_STATUS>(lotObject).ExecuteCommand();
        }

        public int UpdateLot(R_LOT_STATUS lotObject, OleExec DB)
        {
            return DB.ORM.Updateable<R_LOT_STATUS>(lotObject).Where(r => r.ID == lotObject.ID).ExecuteCommand();
        }
    }
    public class Row_R_LOT_STATUS : DataObjectBase
    {
        public Row_R_LOT_STATUS(DataObjectInfo info) : base(info)
        {

        }
        public R_LOT_STATUS GetDataObject()
        {
            R_LOT_STATUS DataObject = new R_LOT_STATUS();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.LINE = this.LINE;
            DataObject.SAMPLE_STATION = this.SAMPLE_STATION;
            DataObject.LOT_STATUS_FLAG = this.LOT_STATUS_FLAG;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.FAIL_QTY = this.FAIL_QTY;
            DataObject.PASS_QTY = this.PASS_QTY;
            DataObject.SAMPLE_QTY = this.SAMPLE_QTY;
            DataObject.REJECT_QTY = this.REJECT_QTY;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.AQL_TYPE = this.AQL_TYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.ID = this.ID;
            DataObject.AQL_LEVEL = this.AQL_LEVEL;
            return DataObject;
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
        public string SAMPLE_STATION
        {
            get
            {
                return (string)this["SAMPLE_STATION"];
            }
            set
            {
                this["SAMPLE_STATION"] = value;
            }
        }
        public string LOT_STATUS_FLAG
        {
            get
            {
                return (string)this["LOT_STATUS_FLAG"];
            }
            set
            {
                this["LOT_STATUS_FLAG"] = value;
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
        public double? FAIL_QTY
        {
            get
            {
                return (double?)this["FAIL_QTY"];
            }
            set
            {
                this["FAIL_QTY"] = value;
            }
        }
        public double? PASS_QTY
        {
            get
            {
                return (double?)this["PASS_QTY"];
            }
            set
            {
                this["PASS_QTY"] = value;
            }
        }
        public double? SAMPLE_QTY
        {
            get
            {
                return (double?)this["SAMPLE_QTY"];
            }
            set
            {
                this["SAMPLE_QTY"] = value;
            }
        }
        public double? REJECT_QTY
        {
            get
            {
                return (double?)this["REJECT_QTY"];
            }
            set
            {
                this["REJECT_QTY"] = value;
            }
        }
        public double? LOT_QTY
        {
            get
            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
            }
        }
        public string AQL_TYPE
        {
            get
            {
                return (string)this["AQL_TYPE"];
            }
            set
            {
                this["AQL_TYPE"] = value;
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
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
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
        public string AQL_LEVEL
        {
            get
            {
                return (string)this["AQL_LEVEL"];
            }
            set
            {
                this["AQL_LEVEL"] = value;
            }
        }
    }
    public class R_LOT_STATUS
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string LOT_NO{get;set;}
        public string SKUNO{get;set;}
        public string AQL_TYPE{get;set;}
        public double? LOT_QTY{get;set;}
        public double? REJECT_QTY{get;set;}
        public double? SAMPLE_QTY{get;set;}
        public double? PASS_QTY{get;set;}
        public double? FAIL_QTY{get;set;}
        public string CLOSED_FLAG{get;set;}
        public string LOT_STATUS_FLAG{get;set;}
        public string SAMPLE_STATION{get;set;}
        public string LINE{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string AQL_LEVEL { get; set; }
    }
    public class OBA_Yield_Data
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Time { get; set; }
        public string TotalBuiltLots { get; set; }
        public string SKUNO { get; set; }
        public string TotalPassLots { get; set; }
        public string TotalFailLots { get; set; }
        public string TotalLotQTY { get; set; }
        public string LotFailRate { get; set; }
        public double LotFailRatex100 { get; set; }
        public string TotalSampleQTY { get; set; }
        public string TotalPassQTY { get; set; }
        public string TotalFailQTY { get; set; }
        
    }
}