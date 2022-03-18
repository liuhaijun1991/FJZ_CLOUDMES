using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using MESDataObject.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using static MESDataObject.Constants.PublicConstants;

namespace MESDataObject.Module
{
    public class T_R_SN : DataObjectTable
    {
        public T_R_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN);
            TableName = "r_sn".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckSNExists(string StrSN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Any(t => t.SN == StrSN && t.VALID_FLAG == "1") || DB.ORM.Queryable<R_SN_HIS>().Any(t => t.SN == StrSN && t.VALID_FLAG == MesBool.Yes.ExtValue());
        }

        public int Insert(R_SN R_SN, OleExec DB)
        {
            return DB.ORM.Insertable<R_SN>(R_SN).ExecuteCommand();
        }

        public GET_SeriesCustomer_BySn GetSeriesCtmBySn(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>
                ((a, b, c, d) =>
                    a.SN == SN
                    && a.VALID_FLAG == "1"
                    && a.SKUNO == b.SKUNO
                    && b.C_SERIES_ID == c.ID
                    && c.CUSTOMER_ID == d.ID)
                .Select((a, b, c, d) => new GET_SeriesCustomer_BySn
                {
                    SN = a.SN,
                    SKUNO = b.SKUNO,
                    SERIES_NAME = c.SERIES_NAME,
                    SERIES_DESCRIPTION = c.DESCRIPTION,
                    CUSTOMER_NAME = d.CUSTOMER_NAME,
                    CUSTOMER_DESCRIPTION = d.DESCRIPTION
                }).ToList().FirstOrDefault();
        }



        /// <summary>
        /// ADD BY HGB 2019.06.21
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckExists(string StrSN, OleExec DB)
        {
            return CheckSNExists(StrSN, DB);
        }

        /// <summary>
        /// add by hgb 2019.06.20
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SN LoadData(string StrSN, OleExec DB)
        {
            //return DB.ORM.Queryable<R_SN>().Where(t => t.VALID_FLAG == "1" && (t.SN == StrSN || t.BOXSN == StrSN)).ToList().FirstOrDefault();
            return DB.ORM.Queryable<R_SN>().Where(t => t.VALID_FLAG == "1" && t.SN == StrSN).ToList().FirstOrDefault();
        }

        public DataTable GetSNByWo(string Workorder, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == Workorder && t.VALID_FLAG == "1").ToDataTable();
        }

        public List<object> GetPieChartTestData(OleExec DB)
        {
            List<object> objects = new List<object>();
            List<object> SNs = DB.ORM.Queryable<R_SN>().GroupBy(t => t.SKUNO).Select<object>("SKUNO,COUNT(ID) COUNT").ToList();
            foreach (object sn in SNs)
            {
                objects.Add(new List<object> { ((dynamic)sn).SKUNO, ((dynamic)sn).COUNT });
            }
            return objects;
        }

        public List<object> GetLChartTestData(OleExec DB)
        {
            List<object> result = new List<object>();
            List<object> objs = DB.ORM.Queryable<R_UPH_DETAIL>().Where(t => t.WORK_DATE.ToString() == "2018/7/24" && t.WORKORDERNO == "002328000318" && t.STATION_NAME == "FT2")
                .Select<object>("WORK_TIME,TOTAL_FRESH_PASS_QTY").ToList();
            foreach (object obj in objs)
            {
                result.Add(new List<object> { ((dynamic)obj).WORK_TIME, ((dynamic)obj).TOTAL_FRESH_PASS_QTY });
            }
            return result;
        }

        public R_SN LoadSN(string StrSN, OleExec DB)
        {
            //R_SN R_Sn = null;
            //Row_R_SN Row_R_Sn = (Row_R_SN)NewRow();
            //DataTable Dt = new DataTable();
            //string StrSql = $@"select * from r_sn where sn='{StrSN}' and valid_flag='1'";
            //Dt = DB.ExecSelect(StrSql).Tables[0];
            //if (Dt.Rows.Count > 0)
            //{
            //    Row_R_Sn.loadData(Dt.Rows[0]);
            //    R_Sn=Row_R_Sn.GetDataObject();
            //}

            //return R_Sn;
            return DB.ORM.Queryable<R_SN>().Where(t => t.SN == StrSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        public R_SN CheckSn(string StrSN, OleExec DB)
        {
            //R_SN R_Sn = null;
            //Row_R_SN Row_R_Sn = (Row_R_SN)NewRow();
            //DataTable Dt = new DataTable();
            //string StrSql = $@"select * from r_sn where sn='{StrSN}' and valid_flag='1'";
            //Dt = DB.ExecSelect(StrSql).Tables[0];
            //if (Dt.Rows.Count > 0)
            //{
            //    Row_R_Sn.loadData(Dt.Rows[0]);
            //    R_Sn = Row_R_Sn.GetDataObject();
            //}

            //return R_Sn;
            return DB.ORM.Queryable<R_SN>().Where(t => t.SN == StrSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        public List<R_SN> GetFinishedVanillaSN(string CMODSN, OleExec DB)
        {
            //string strSql = $@"select * from R_SN where SN in(select SN from R_SN_KP where value=:value and SCANTYPE = 'SN') and COMPLETED_FLAG = '1'";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":value", CMODSN) };
            //DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            //List<R_SN> listSn = new List<R_SN>();
            //if (res.Rows.Count > 0)
            //{
            //    foreach (DataRow item in res.Rows)
            //    {
            //        Row_R_SN ret = (Row_R_SN)NewRow();
            //        ret.loadData(item);
            //        listSn.Add(ret.GetDataObject());
            //    }
            //}
            //return listSn;
            return DB.ORM.Queryable<R_SN>()
                .Where(t => t.COMPLETED_FLAG == "1")
                .Where(t => SqlFunc.Subqueryable<R_SN_KP>().Where(s => s.VALUE == CMODSN && s.SCANTYPE == "SN" && s.SN == t.SN).Any())
                .ToList();
        }

        public List<R_SN> GetSnListByListSN(string lstSN, OleExec sfcdb)
        {
            string sql = $@"select * from R_SN where SN IN ({lstSN}) AND VALID_FLAG = 1";//and a.CLOSED_FLAG !='1' and c.SHIPPED_FLAG!='1'
            List<R_SN> snList = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(sfcdb, this.DBType);
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Row_R_SN rowSN = (Row_R_SN)t_r_sn.NewRow();
                    rowSN.loadData(row);
                    snList.Add(rowSN.GetDataObject());
                }
            }
            return snList;
        }

        public R_SN CheckSnStatus(string StrSN, OleExec DB)
        {
            //R_SN R_Sn = null;
            //Row_R_SN Row_R_Sn = (Row_R_SN)NewRow();
            //DataTable Dt = new DataTable();
            //string StrSql = $@"select*From r_sn a, c_sku_detail b where a.sn='{StrSN}' and a.skuno=b.value and b.category='Rotation' and b.gategory_name='ControlKp' and a.completed_flag=1 and repair_failed=0";
            //Dt = DB.ExecSelect(StrSql).Tables[0];
            //if (Dt.Rows.Count > 0)
            //{
            //    Row_R_Sn.loadData(Dt.Rows[0]);
            //    R_Sn = Row_R_Sn.GetDataObject();
            //}

            //return R_Sn;
            //return DB.ORM.Queryable<R_SN,C_SKU_DETAIL>().Where(t => t.SN == StrSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();

            return DB.ORM.Queryable<R_SN, C_SKU_DETAIL>((rsn, csd) => rsn.SKUNO == csd.VALUE).Where((rsn, csd) => rsn.SN == StrSN && rsn.VALID_FLAG == "1" && rsn.COMPLETED_FLAG == "1"
               && rsn.REPAIR_FAILED_FLAG == "0" && csd.CATEGORY == "Rotation" && csd.CATEGORY_NAME == "ControlKp")
               .Select((rsn) => new R_SN { ID = SqlSugar.SqlFunc.GetSelfAndAutoFill(rsn.ID) }).ToList().FirstOrDefault();
        }


        public List<R_SN> GETSN(string _WO, OleExec DB)
        {

            string strSql = string.Empty;
            DataTable dt = new DataTable();
            List<R_SN> SNInfolsit = new List<R_SN>();
            try
            {
                if (DBType == DB_TYPE_ENUM.Oracle)
                {
                    strSql = $@" select * from r_sn where workorderno='{_WO.Replace("'", "''")}'";
                    dt = DB.ExecSelect(strSql).Tables[0];
                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            SNInfolsit.Add(new R_SN
                            {
                                ID = item["ID"].ToString(),
                                SN = item["SN"].ToString(),
                                SKUNO = item["SKUNO"].ToString(),
                                WORKORDERNO = item["WORKORDERNO"].ToString(),
                                PLANT = item["PLANT"].ToString(),
                                ROUTE_ID = item["ROUTE_ID"].ToString(),
                                STARTED_FLAG = item["STARTED_FLAG"].ToString(),
                                // START_TIME = Convert.ToDateTime(item["START_TIME"]),
                                START_TIME = item["START_TIME"].ToString() == "" ? null : ((DateTime?)item["START_TIME"]),
                                PACKED_FLAG = item["PACKED_FLAG"].ToString(),
                                PACKDATE = item["PACKDATE"].ToString() == "" ? null : ((DateTime?)item["PACKDATE"]),
                                COMPLETED_FLAG = item["COMPLETED_FLAG"].ToString(),
                                COMPLETED_TIME = item["COMPLETED_TIME"].ToString() == "" ? null : ((DateTime?)item["COMPLETED_TIME"]),
                                SHIPPED_FLAG = item["SHIPPED_FLAG"].ToString(),
                                SHIPDATE = item["SHIPDATE"].ToString() == "" ? null : ((DateTime?)item["SHIPDATE"]),
                                REPAIR_FAILED_FLAG = item["REPAIR_FAILED_FLAG"].ToString(),
                                CURRENT_STATION = item["CURRENT_STATION"].ToString(),
                                NEXT_STATION = item["NEXT_STATION"].ToString(),
                                KP_LIST_ID = item["KP_LIST_ID"].ToString(),
                                PO_NO = item["PO_NO"].ToString(),
                                CUST_ORDER_NO = item["CUST_ORDER_NO"].ToString(),
                                CUST_PN = item["CUST_PN"].ToString(),
                                BOXSN = item["BOXSN"].ToString(),
                                SCRAPED_FLAG = item["SCRAPED_FLAG"].ToString(),
                                SCRAPED_TIME = item["SCRAPED_TIME"].ToString() == "" ? null : ((DateTime?)item["SCRAPED_TIME"]),
                                PRODUCT_STATUS = item["PRODUCT_STATUS"].ToString(),
                                REWORK_COUNT = Convert.ToDouble(item["REWORK_COUNT"].ToString() == "" ? "0" : item["REWORK_COUNT"].ToString()),
                                VALID_FLAG = item["VALID_FLAG"].ToString(),
                                STOCK_STATUS = item["STOCK_STATUS"].ToString(),
                                STOCK_IN_TIME = item["STOCK_IN_TIME"].ToString() == "" ? null : ((DateTime?)item["STOCK_IN_TIME"]),
                                EDIT_EMP = item["EDIT_EMP"].ToString(),
                                EDIT_TIME = item["EDIT_TIME"].ToString() == "" ? null : ((DateTime?)item["EDIT_TIME"]),

                            });
                        }
                    }
                    return SNInfolsit;
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
            catch (Exception)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        #region add by champion

        /// <summary>
        /// 獲取數據庫時間
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public DateTime GetDBDateTime(OleExec DB)
        //{
        //    string strSql = "select sysdate from dual";
        //    if (this.DBType == DB_TYPE_ENUM.Oracle)
        //    {
        //        strSql = "select sysdate from dual";
        //    }
        //    else if (this.DBType == DB_TYPE_ENUM.SqlServer)
        //    {
        //        strSql = "select get_date() ";
        //    }
        //    else
        //    {
        //        throw new Exception(this.DBType.ToString() + " not Work");
        //    }
        //    return (DateTime)DB.ExecSelectOneValue(strSql);

        //}
        public Row_R_SN GetWoBySn(string _SN, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select workorderno from r_sn where sn='{_SN}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + _SN });
                    throw new MESReturnMessage(errMsg);
                }
                Row_R_SN R = (Row_R_SN)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 批量插入 R_SN 中，針對SN投入和Panel 投入，如果Panel 投入的話，SN 實例的 SN 屬性是沒有值的，因此以 ID 代替
        /// 返回插入的 ID
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> AddToRSn(List<R_SN> SNs, string Line, string StationName, string DeviceName, string Bu, OleExec DB)
        {
            string sql = string.Empty;
            string result = string.Empty;
            T_R_SN Table_R_SN = new T_R_SN(DB, DBType); //add by LLF 2018-03-19
            Row_R_SN row = null;
            List<string> SNIds = new List<string>();
            bool ModifyFlag = false;
            DateTime DateTime = GetDBDateTime(DB);
            string NextStation = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                NextStation = GetNextStation(SNs.ElementAt(0).ROUTE_ID, SNs.ElementAt(0).CURRENT_STATION, DB);
                foreach (R_SN SN in SNs)
                {
                    if (SN.ID != null && SN.SN.Equals(SN.ID))
                    {
                        ModifyFlag = true;
                    }
                    //Modify by LLF 2018-03-19,獲取ID,需根據表名生成
                    //SN.ID = GetNewID(Bu, DB); 
                    SN.ID = Table_R_SN.GetNewID(Bu, DB);
                    SN.START_TIME = DateTime;
                    SN.EDIT_TIME = DateTime;
                    SN.NEXT_STATION = NextStation;
                    if (string.IsNullOrEmpty(SN.SN) || ModifyFlag)
                    {
                        SN.SN = SN.ID;
                    }
                    row = (Row_R_SN)this.ConstructRow(SN);
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    RecordPassStationDetail(SN, Line, StationName, DeviceName, Bu, DB);
                    //RecordPassStationDetail(SN.SN, Line, StationName, DeviceName, Bu, DB);
                    SNIds.Add(SN.ID);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SNIds;
        }

        /// <summary>
        /// 記錄良率
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="SerialNo"></param>
        /// <param name="Status"></param>
        /// <param name="Day"></param>
        /// <param name="Time"></param>
        /// <param name="Line"></param>
        /// <param name="Station"></param>
        /// <param name="EmpNo"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string RecordYieldRate(string WorkOrder, double LinkQty, string SerialNo, string Status,
                                        string Line, string Station, string EmpNo, string Bu, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            bool Reworked = false;
            bool Passed = false;
            T_R_YIELD_RATE_DETAIL YieldRateTable = null;
            Row_R_YIELD_RATE_DETAIL YieldRateRow = null;
            T_R_SN TRSN = new T_R_SN(DB, this.DBType);
            T_R_WO_BASE WoTable = null;
            Row_R_WO_BASE WoRow = null;
            DateTime DateTime = GetDBDateTime(DB);
            string Day = DateTime.ToString("yyyy-MM-dd");
            string Time = DateTime.ToString("HH");

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (Bu.Substring(0, 3) == "MBD")
                {
                    Reworked = WorkTimes(WorkOrder, SerialNo, Station, DB) > 1 ? true : false;
                }
                else if (Bu.Substring(0, 3) == "BPD")
                {
                    try
                    {
                        Reworked = TRSN.GetSN(SerialNo, DB).PRODUCT_STATUS.Equals("FRESH") ? false : true;
                    }
                    catch (Exception)
                    {
                        return result;
                    }

                }
                else
                {
                    Reworked = WorkTimes(WorkOrder, SerialNo, Station, DB) > 0 ? true : false;
                }
                //Reworked = WorkTimes(SerialNo, Station, DB) > 1 ? true : false;
                Passed = Status.ToUpper().Equals("PASS");
                YieldRateTable = new T_R_YIELD_RATE_DETAIL(DB, this.DBType);
                YieldRateRow = (Row_R_YIELD_RATE_DETAIL)YieldRateTable.NewRow();
                sql = $@"SELECT * FROM R_YIELD_RATE_DETAIL WHERE WORK_DATE=to_date( '{Day}','yyyy-mm-dd')  AND WORK_TIME='{Time}' AND PRODUCTION_LINE='{Line}'
                            AND STATION_NAME='{Station}' AND WORKORDERNO='{WorkOrder}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    YieldRateRow.loadData(dt.Rows[0]);
                    if (Reworked)
                    {
                        YieldRateRow.TOTAL_REWORK_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_REWORK_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_REWORK_FAIL_QTY += LinkQty;
                        }
                    }
                    else
                    {
                        YieldRateRow.TOTAL_FRESH_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_FRESH_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_FRESH_FAIL_QTY += LinkQty;
                        }
                    }
                    YieldRateRow.EDIT_EMP = EmpNo;
                    YieldRateRow.EDIT_TIME = DateTime;
                    sql = YieldRateRow.GetUpdateString(this.DBType);
                }
                else
                {
                    WoTable = new T_R_WO_BASE(DB, this.DBType);
                    WoRow = WoTable.GetWo(WorkOrder, DB);
                    YieldRateRow.ID = YieldRateTable.GetNewID(Bu, DB);
                    YieldRateRow.WORK_DATE = Convert.ToDateTime(Day);
                    YieldRateRow.WORK_TIME = Time;
                    YieldRateRow.PRODUCTION_LINE = Line;
                    YieldRateRow.CLASS_NAME = GetWorkClass(DB);
                    YieldRateRow.STATION_NAME = Station;
                    YieldRateRow.WORKORDERNO = WorkOrder;
                    YieldRateRow.SKUNO = WoRow.SKUNO;
                    YieldRateRow.SKU_NAME = WoRow.SKU_NAME;
                    YieldRateRow.SKU_SERIES = WoRow.SKU_SERIES;
                    YieldRateRow.EDIT_EMP = EmpNo;
                    YieldRateRow.EDIT_TIME = DateTime;
                    YieldRateRow.TOTAL_FRESH_BUILD_QTY = YieldRateRow.TOTAL_FRESH_FAIL_QTY = YieldRateRow.TOTAL_FRESH_PASS_QTY = 0;
                    YieldRateRow.TOTAL_REWORK_BUILD_QTY = YieldRateRow.TOTAL_REWORK_FAIL_QTY = YieldRateRow.TOTAL_REWORK_PASS_QTY = 0;

                    if (Reworked)
                    {
                        YieldRateRow.TOTAL_REWORK_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_REWORK_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_REWORK_FAIL_QTY = LinkQty;
                        }
                    }
                    else
                    {
                        YieldRateRow.TOTAL_FRESH_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_FRESH_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_FRESH_FAIL_QTY = LinkQty;
                        }
                    }
                    sql = YieldRateRow.GetInsertString(this.DBType);

                }

                result = DB.ExecSQL(sql);
                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        /// <summary>
        /// 記錄 UPH
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="UnitType"></param>
        /// <param name="SerialNo"></param>
        /// <param name="Status"></param>
        /// <param name="Day"></param>
        /// <param name="Time"></param>
        /// <param name="Line"></param>
        /// <param name="Station"></param>
        /// <param name="EmpNo"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string RecordUPH(string WorkOrder, double LinkQty, string SerialNo, string Status,
                                        string Line, string Station, string EmpNo, string Bu, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            bool Reworked = false;
            bool Passed = false;
            T_R_UPH_DETAIL UPHTable = null;
            Row_R_UPH_DETAIL UPHRow = null;
            T_R_SN TRSN = new T_R_SN(DB, this.DBType);
            T_R_WO_BASE WoTable = null;
            Row_R_WO_BASE WoRow = null;
            DateTime DateTime = GetDBDateTime(DB);
            string Day = DateTime.ToString("yyyy-MM-dd");
            string Time = DateTime.ToString("HH");

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (Bu.Substring(0, 3) == "MBD")
                {
                    Reworked = WorkTimes(WorkOrder, SerialNo, Station, DB) > 1 ? true : false;
                }
                else if (Bu.Substring(0, 3) == "BPD")
                {
                    try
                    {
                        Reworked = TRSN.GetSN(SerialNo, DB).PRODUCT_STATUS.Equals("FRESH") ? false : true;
                    }
                    catch (Exception)
                    {
                        return result;
                    }

                }
                else
                {
                    Reworked = WorkTimes(WorkOrder, SerialNo, Station, DB) > 0 ? true : false;
                }
                //Reworked = WorkTimes(SerialNo, Station, DB) > 1 ? true : false;
                Passed = Status.ToUpper().Equals("PASS");
                UPHTable = new T_R_UPH_DETAIL(DB, this.DBType);
                UPHRow = (Row_R_UPH_DETAIL)UPHTable.NewRow();
                sql = $@"SELECT * FROM R_UPH_DETAIL WHERE WORK_DATE=to_date( '{Day}','yyyy-mm-dd') AND WORK_TIME='{Time}' AND PRODUCTION_LINE='{Line}'
                            AND STATION_NAME='{Station}' AND WORKORDERNO='{WorkOrder}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    UPHRow.loadData(dt.Rows[0]);
                    if (Reworked)
                    {
                        UPHRow.TOTAL_REWORK_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_REWORK_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_REWORK_FAIL_QTY += LinkQty;
                        }
                    }
                    else
                    {
                        UPHRow.TOTAL_FRESH_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_FRESH_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_FRESH_FAIL_QTY += LinkQty;
                        }
                    }
                    UPHRow.EDIT_EMP = EmpNo;
                    UPHRow.EDIT_TIME = DateTime;
                    sql = UPHRow.GetUpdateString(this.DBType);
                }
                else
                {
                    WoTable = new T_R_WO_BASE(DB, this.DBType);
                    WoRow = WoTable.GetWo(WorkOrder, DB);
                    UPHRow.ID = UPHTable.GetNewID(Bu, DB);
                    UPHRow.WORK_DATE = Convert.ToDateTime(Day);
                    UPHRow.WORK_TIME = Time;
                    UPHRow.PRODUCTION_LINE = Line;
                    UPHRow.CLASS_NAME = GetWorkClass(DB);
                    UPHRow.STATION_NAME = Station;
                    UPHRow.WORKORDERNO = WorkOrder;
                    UPHRow.SKUNO = WoRow.SKUNO;
                    UPHRow.SKU_NAME = WoRow.SKU_NAME;
                    UPHRow.SKU_SERIES = WoRow.SKU_SERIES;
                    UPHRow.EDIT_EMP = EmpNo;
                    UPHRow.EDIT_TIME = DateTime;
                    UPHRow.TOTAL_FRESH_BUILD_QTY = UPHRow.TOTAL_FRESH_FAIL_QTY = UPHRow.TOTAL_FRESH_PASS_QTY = 0;
                    UPHRow.TOTAL_REWORK_BUILD_QTY = UPHRow.TOTAL_REWORK_FAIL_QTY = UPHRow.TOTAL_REWORK_PASS_QTY = 0;

                    if (Reworked)
                    {
                        UPHRow.TOTAL_REWORK_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_REWORK_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_REWORK_FAIL_QTY = LinkQty;
                        }
                    }
                    else
                    {
                        UPHRow.TOTAL_FRESH_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_FRESH_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_FRESH_FAIL_QTY = LinkQty;
                        }
                    }
                    sql = UPHRow.GetInsertString(this.DBType);

                }

                result = DB.ExecSQL(sql);
                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 判斷該 SN 過了多少次這個工站
        /// </summary>
        /// <param name="wo">工单，为了解决R_SN_STATION_DETAIL查询慢而增加的参数</param>
        /// <param name="SerialNo"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int WorkTimes(string wo, string SerialNo, string Station, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            int TotalReworkTimes = 0;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE WORKORDERNO='{wo}' AND SN='{SerialNo}' AND CURRENT_STATION='{Station}'";
                dt = DB.ExecSelect(sql).Tables[0];
                //由於可能剛剛有執行插入過站記錄的操作，所以上面的查詢必然有值，但是卻不算是重工，所以這裡加入一個判斷，如果上面查出來的結果裡面 EDIT_TIME 超過 30 秒
                //之前的記錄都認為是之前插入的過站記錄，而在 30 秒之內的都認為是本次的操作，不算是 Rework
                if (dt.Rows.Count > 0 && DateTime.Compare(DateTime.Parse(dt.Rows[0]["EDIT_TIME"].ToString()).AddSeconds(30), GetDBDateTime(DB)) < 0)
                {
                    TotalReworkTimes = dt.Rows.Count;
                }
                return TotalReworkTimes;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 獲取下一站
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="CurrentStation"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetNextStation(string RouteId, string CurrentStation, OleExec DB)
        {
            T_C_ROUTE_DETAIL RouteDetailTable = null;
            List<C_ROUTE_DETAIL> RouteDetails = null;
            Dictionary<string, object> Routes = null;
            string NextStation = string.Empty;
            int Counter = 0;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                RouteDetailTable = new T_C_ROUTE_DETAIL(DB, this.DBType);

                if (CurrentStation.Equals("JOBFINISH"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000090", new string[] { CurrentStation }));
                }

                RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(RouteId, DB);
                Routes = RouteDetailTable.GetNextStations(RouteId, CurrentStation, DB);
                Counter = ((List<string>)Routes["NextStations"]).Count + ((List<string>)Routes["DirectLinks"]).Count;
                if (Counter == 0)
                {
                    //黃楊盛 2018年4月24日14:54:17 沒有跳站的情況下更新為使用最後一個的station_type
                    //NextStation = "NA_SHIP";
                    NextStation = RouteDetails[RouteDetails.Count - 1].STATION_TYPE;
                }
                else if (((List<string>)Routes["NextStations"]).Count > 0)
                {
                    NextStation = ((List<string>)Routes["NextStations"]).ElementAt(0).ToString();
                }
                else if (((List<string>)Routes["DirectLinks"]).Count > 0)
                {
                    NextStation = ((List<string>)Routes["DirectLinks"]).ElementAt(0).ToString();
                }
                //else if (Counter == 1 && ((List<string>)Routes["NextStations"]).Count > 0)
                //{
                //    NextStation = ((List<string>)Routes["NextStations"]).ElementAt(0).ToString();
                //}
                //else if (Counter == 1 && ((List<string>)Routes["DirectLinks"]).Count > 0)
                //{
                //    NextStation = ((List<string>)Routes["DirectLinks"]).ElementAt(0).ToString();
                //}
                //else
                //{
                //    NextStation = "NA";
                //}
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return NextStation;
        }

        /// <summary>
        /// 獲取流程中的最後一站
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetLastStation(string RouteId, OleExec DB)
        {
            string LastStation = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND STATION_TYPE='JOBFINISHED'";
                sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND instr(STATION_TYPE,'JOBFINISH')>0";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    LastStation = dt.Rows[0]["STATION_NAME"].ToString();
                }
                return LastStation;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<string> GetLastStationList(string RouteId, OleExec DB)
        {
            List<string> LastStation = new List<string>();
            string sql = string.Empty;
            DataTable dt = new DataTable();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND STATION_TYPE='JOBFINISHED'";
                sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND instr(STATION_TYPE,'JOBFINISH')>0";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        LastStation.Add(dt.Rows[i]["STATION_NAME"].ToString());
                    }
                    return LastStation;
                }
                else
                {
                    if (RouteId.StartsWith("BPD"))
                    {
                        return LastStation;
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180629092630", new string[] { RouteId }));
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 更新 SN 狀態，用於 SN 過站
        /// </summary>
        /// <param name="SnObject"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void ChangeSnStatus(ref R_SN SnObject, string PassOrFail, string EmpNo, OleExec DB, Tuple<string, string, string> cachNextStation)
        {
            bool ReworkFlag = false;
            bool RepairFlag = false;
            bool PackedFlag = false;
            bool FinishedFlag = false;
            bool ShippedFlag = false;
            bool StockinFlag = false;
            //int MaxReworkCount = 1; // 保存設定的最大重工次數
            int HadReworkedTimes = 0;
            string OriginalNextStation = string.Empty,
                routeId = SnObject.ROUTE_ID,
                NextStation = string.Empty;
            T_R_WO_BASE WoBase = new T_R_WO_BASE(DB, this.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(DB, this.DBType);

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SnObject != null)
                {
                    OriginalNextStation = SnObject.NEXT_STATION.ToUpper();
                    //MaxReworkCount 還需要獲取維修的最大次數
                    //RepairFlag = PassOrFail.ToUpper().Equals("PASS") ? false : MaxReworkCount - SnObject.REWORK_COUNT == 1;
                    RepairFlag = PassOrFail.ToUpper().Equals("PASS") ? false : true;
                    HadReworkedTimes = WorkTimes(SnObject.WORKORDERNO, SnObject.SN, SnObject.NEXT_STATION, DB);
                    ReworkFlag = HadReworkedTimes > 0 ? true : false;
                    PackedFlag = OriginalNextStation.Contains("PACK")
                            || OriginalNextStation.Contains("CTN") || OriginalNextStation.Contains("CARTON")
                            || OriginalNextStation.Contains("PALT") || OriginalNextStation.Contains("PALLET") ? true : false;
                    //FinishedFlag = OriginalNextStation.Equals(GetLastStation(SnObject.ROUTE_ID, DB)) ? true : false;

                    //路由中有多個JOBFINISH工站
                    List<string> jobfinishList = GetLastStationList(routeId, DB);
                    if (jobfinishList.Count > 0)
                    {
                        FinishedFlag = jobfinishList.Find(j => j == OriginalNextStation) != null ? true : false;
                    }

                    ShippedFlag = DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(x =>
                        x.ROUTE_ID == routeId && x.STATION_NAME == OriginalNextStation &&
                        x.STATION_TYPE == "SHIPFINISH").Any();

                    StockinFlag = DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(x =>
                        x.ROUTE_ID == routeId && x.STATION_NAME == OriginalNextStation &&
                        x.STATION_TYPE == "JOBSTOCKIN").Any();

                    //這個應該是必須要判斷的
                    if (ReworkFlag)
                    {
                        SnObject.REWORK_COUNT = HadReworkedTimes++;
                        SnObject.PRODUCT_STATUS = "REWORK";
                    }
                    else
                    {
                        SnObject.PRODUCT_STATUS = "FRESH";
                        //Rito 20191209,添加上面的一行,否則產品狀態將一直為Rework   
                    }

                    if (RepairFlag)
                    {
                        //NextStation = "Repair_" + SnObject.NEXT_STATION;
                        SnObject.REPAIR_FAILED_FLAG = "1";
                        //SnObject.NEXT_STATION = NextStation;
                    }
                    else
                    {
                        SnObject.REPAIR_FAILED_FLAG = "0";
                        /*Rito 20191209,添加上面的一行如果原本SN所在的RepairFlag是1呢?     
                          SH BPD遇到一種狀況
                          測試三分鐘之內從測試數據庫中讀到一個SN,第一次Fail,我們的TestRecord有不良的記錄,R_SN也置為Repair狀態了
                          但馬上又Pass了,同樣TestRecord有Pass的記錄,R_SN仍為Repair狀態,產線不能維修,後續工站也做不了,
                          因為RepairMain也沒有數據
                         */
                        if (cachNextStation.Item1 == SnObject.ROUTE_ID &&
                            cachNextStation.Item2 == SnObject.NEXT_STATION)
                            NextStation = cachNextStation.Item3;
                        else
                            NextStation = GetNextStation(SnObject.ROUTE_ID, SnObject.NEXT_STATION, DB);

                        if (OriginalNextStation == "STOCKIN" || OriginalNextStation == "B29M")
                        {
                            //NextStation = GetNextStation(SnObject.ROUTE_ID, SnObject.NEXT_STATION, DB);
                            SnObject.COMPLETED_FLAG = "1";
                            SnObject.COMPLETED_TIME = GetDBDateTime(DB);
                            SnObject.CURRENT_STATION = OriginalNextStation;
                            SnObject.NEXT_STATION = NextStation;
                            WoBase.UpdateFinishQty(SnObject.WORKORDERNO, 1, DB);
                        }
                        else if (FinishedFlag)
                        {
                            //NextStation = GetNextStation(SnObject.ROUTE_ID, SnObject.NEXT_STATION, DB);
                            SnObject.COMPLETED_FLAG = "1";
                            SnObject.COMPLETED_TIME = GetDBDateTime(DB);
                            SnObject.CURRENT_STATION = OriginalNextStation;
                            SnObject.NEXT_STATION = NextStation.Equals("CBS2") ? "CBS2" : (NextStation.Equals("SHIPOUT") ? "SHIPOUT" : "JOBFINISH");
                            WoBase.UpdateFinishQty(SnObject.WORKORDERNO, 1, DB);
                        }
                        else if (ShippedFlag)
                        {
                            SnObject.SHIPPED_FLAG = "1";
                            SnObject.SHIPDATE = GetDBDateTime(DB);
                            SnObject.CURRENT_STATION = OriginalNextStation;
                            SnObject.NEXT_STATION = "SHIPFINISH";
                        }
                        //else if (StockinFlag)
                        //{
                        //    SnObject.COMPLETED_FLAG = "1";
                        //    SnObject.COMPLETED_TIME = GetDBDateTime(DB);
                        //    SnObject.CURRENT_STATION = OriginalNextStation;
                        //    SnObject.NEXT_STATION = "S101";
                        //    SnObject.STOCK_STATUS = "1";
                        //    SnObject.STOCK_IN_TIME = GetDBDateTime(DB);
                        //    WoBase.UpdateFinishQty(SnObject.WORKORDERNO, 1, DB);
                        //}
                        else
                        {
                            //NextStation = GetNextStation(SnObject.ROUTE_ID, SnObject.NEXT_STATION, DB);
                            SnObject.CURRENT_STATION = OriginalNextStation;
                            SnObject.NEXT_STATION = NextStation;

                            if (PackedFlag)
                            {
                                SnObject.PACKED_FLAG = "1";
                                SnObject.PACKDATE = GetDBDateTime(DB);
                            }

                            if (StockinFlag)
                            {
                                SnObject.STOCK_STATUS = "1";
                                SnObject.STOCK_IN_TIME = GetDBDateTime(DB);
                            }
                            //if (ShippedFlag)
                            //{
                            //    SnObject.SHIPPED_FLAG = "1";
                            //    SnObject.SHIPDATE = GetDBDateTime(DB);
                            //}
                        }
                    }
                    SnObject.EDIT_TIME = GetDBDateTime(DB);
                    SnObject.EDIT_EMP = EmpNo;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// 更新 SN退站 狀態，用於 SN 過站
        /// </summary>
        /// <param name="SnObject"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void ChangeReturnSnStatus(ref R_SN SnObject, string PassOrFail, string EmpNo, OleExec DB)
        {
            string OriginalNextStation = string.Empty,
                routeId = SnObject.ROUTE_ID,
                NextStation = string.Empty;
            T_R_WO_BASE WoBase = new T_R_WO_BASE(DB, this.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(DB, this.DBType);

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SnObject != null)
                {
                    OriginalNextStation = SnObject.CURRENT_STATION.ToUpper();
                    NextStation = SnObject.NEXT_STATION.ToUpper();
                    SnObject.CURRENT_STATION = OriginalNextStation;
                    SnObject.NEXT_STATION = NextStation;

                    SnObject.EDIT_TIME = GetDBDateTime(DB);
                    SnObject.EDIT_EMP = EmpNo;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// SN 過站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="DB"></param>
        public void PassStation(string SerialNo, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            //string sql = string.Empty;
            //Row_R_SN SnRow = (Row_R_SN)NewRow();
            //T_R_SN_STATION_DETAIL SnStationDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            //DataTable dt = new DataTable();
            List<string> SNs = new List<string>();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                SNs.Add(SerialNo);
                LotsPassStation(SNs, Line, StationName, DeviceName, Bu, PassOrFail, EmpNo, DB);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }
        /// <summary>
        /// 退站的SN過站
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void ReturnPassStation(string SerialNo, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            Row_R_SN SnRow = (Row_R_SN)NewRow();
            T_R_SN_STATION_DETAIL SnStationDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            DataTable dt = new DataTable();
            List<string> SNs = new List<string>();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                SNs.Add(SerialNo);
                LotsRetrunStation(SNs, Line, StationName, DeviceName, Bu, PassOrFail, EmpNo, DB);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        /// <summary>
        /// 多個 SN 批量過站，需要處於相同狀態
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void LotsPassStation(List<string> SNs, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            T_R_SN T_RS = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            R_SN TemplateSNObject = null;
            Row_R_SN row = (Row_R_SN)NewRow();
            T_R_NORMAL_BONEPILE nBON = new T_R_NORMAL_BONEPILE(DB, DB_TYPE_ENUM.Oracle);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SNs.Count > 0)
                {
                    var snobj = T_RS.LoadSN(SNs.FirstOrDefault(), DB);
                    var CachNextStation = Tuple.Create(snobj.ROUTE_ID, snobj.NEXT_STATION,
                        GetNextStation(snobj.ROUTE_ID, snobj.NEXT_STATION, DB));
                    foreach (string SN in SNs)
                    {
                        //sql = $@"SELECT * FROM R_SN WHERE SN='{SN}' AND VALID_FLAG='1'"; //表示當前有效的 SN
                        TemplateSNObject = T_RS.LoadSN(SN, DB);
                        //dt = DB.ExecSelect(sql).Tables[0];
                        if (TemplateSNObject != null)
                        {
                            if (!TemplateSNObject.NEXT_STATION.Equals(StationName))
                            {
                                throw new MESReturnMessage($@"The Next Station Of '{SN}' Is Not Equal To {StationName}");
                            }
                            //row.loadData(dt.Rows[0]);
                            //TemplateSNObject = row.GetDataObject();
                            ChangeSnStatus(ref TemplateSNObject, PassOrFail, EmpNo, DB, CachNextStation);
                            //row.ConstructRow(TemplateSNObject);
                            //sql = row.GetUpdateString(this.DBType);
                            //DB.ExecSQL(sql);
                            if (Bu == "BPD" && StationName != "VIR" && StationName != "B29M" && !(StationName.EndsWith("LOADING")) && TemplateSNObject.CURRENT_STATION != StationName && TemplateSNObject.NEXT_STATION != StationName)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190211165713", new string[] { StationName, SN, TemplateSNObject.CURRENT_STATION }, DB));
                            }
                            DB.ORM.Updateable<R_SN>(TemplateSNObject).Where(t => t.ID == TemplateSNObject.ID).ExecuteCommand();
                            //過站自動關閉normal BONEPILE 20210707 chc
                            if (nBON.QueryableBONEPILE(DB, TemplateSNObject.SN) != null)
                            {
                                nBON.updataBONEPILE(DB, TemplateSNObject.SN, StationName, EmpNo, GetDBDateTime(DB));
                            }
                            RecordPassStationDetail(TemplateSNObject, Line, StationName, DeviceName, Bu, DB);

                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SN }));
                        }

                    }

                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// SN 退站更新記錄
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void LotsRetrunStation(List<string> SNs, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            string returnUrl = "";
            DataTable dt = new DataTable();
            R_SN TemplateSNObject = null;
            Row_R_SN row = (Row_R_SN)NewRow();
            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(DB, DB_TYPE_ENUM.Oracle);
            R_SN_LOG snLog = new R_SN_LOG();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SNs.Count > 0)
                {
                    foreach (string SN in SNs)
                    {
                        sql = $@"SELECT * FROM R_SN WHERE SN='{SN}' AND VALID_FLAG='1'"; //表示當前有效的 SN
                        dt = DB.ExecSelect(sql).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            row.loadData(dt.Rows[0]);
                            TemplateSNObject = row.GetDataObject();
                            returnUrl = TemplateSNObject.NEXT_STATION + "-->" + DeviceName;
                            TemplateSNObject.CURRENT_STATION = StationName;
                            TemplateSNObject.NEXT_STATION = DeviceName;

                            ChangeReturnSnStatus(ref TemplateSNObject, PassOrFail, EmpNo, DB);
                            row.ConstructRow(TemplateSNObject);
                            sql = row.GetUpdateString(this.DBType);
                            DB.ExecSQL(sql);
                            //RecordPassStationDetail(row.SN, Line, StationName, StationName, Bu, DB);
                            RecordPassStationDetail(TemplateSNObject, Line, "RETURN", returnUrl, Bu, DB);

                            snLog.ID = t_r_sn_log.GetNewID(Bu, DB);
                            snLog.SN = TemplateSNObject.SN;
                            snLog.SNID = TemplateSNObject.ID;
                            snLog.LOGTYPE = "RETURN";
                            snLog.DATA1 = returnUrl;
                            snLog.FLAG = "1";
                            snLog.CREATETIME = t_r_sn_log.GetDBDateTime(DB);
                            snLog.CREATEBY = EmpNo;
                            t_r_sn_log.Save(DB, snLog);

                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SN }));
                        }

                    }

                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        /// <summary>
        /// 多個 SN 批量過站，需要處於相同狀態
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void LotsPassStation(List<R_SN> SNs, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            //R_SN TemplateSNObject = null;
            Row_R_SN row = (Row_R_SN)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SNs.Count > 0)
                {
                    var CachNextStation = Tuple.Create(SNs.FirstOrDefault().ROUTE_ID, SNs.FirstOrDefault().NEXT_STATION,
                        GetNextStation(SNs.FirstOrDefault().ROUTE_ID, SNs.FirstOrDefault().NEXT_STATION, DB));
                    var sns = new List<R_SN>();
                    foreach (R_SN snobj in SNs)
                    {
                        R_SN TemplateSNObject = snobj;
                        if (TemplateSNObject.VALID_FLAG == "1")
                        {
                            ChangeSnStatus(ref TemplateSNObject, PassOrFail, EmpNo, DB, CachNextStation);                            
                            DB.ORM.Updateable(TemplateSNObject).ExecuteCommand();
                            RecordPassStationDetail(TemplateSNObject, Line, StationName, DeviceName, Bu, DB);
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { snobj.SN }));
                        }
                    }

                    #region 暫時還原回一個一個過站
                    //for (int i = 0; i < SNs.Count; i++)
                    //{
                    //    R_SN TemplateSNObject = SNs[i];
                    //    if (TemplateSNObject.VALID_FLAG == "1")
                    //    {
                    //        ChangeSnStatus(ref TemplateSNObject, PassOrFail, EmpNo, DB, CachNextStation);
                    //        sns.Add(TemplateSNObject);
                    //    }
                    //    else
                    //    {
                    //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SNs[i].SN }));
                    //    }
                    //}                    
                    //var n = DB.ORM.Updateable(sns).ExecuteCommand();
                    //if (n != sns.Count)
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                    //}
                    //n = RecordPassStationDetail(sns, Line, StationName, DeviceName, Bu, DB);
                    //if (n != sns.Count)
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231"));
                    //}
                    #endregion
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// Panel 過站
        /// </summary>
        /// <param name="PanelSN"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void PanelPassStation(string PanelSN, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            List<string> PanelSNs = new List<string>();
            DataTable dt = new DataTable();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT A.* FROM R_SN A JOIN R_PANEL_SN B ON A.ID=B.SN WHERE B.PANEL='{PanelSN}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PanelSNs.Add(dr["SN"].ToString());
                    }
                    LotsPassStation(PanelSNs, Line, StationName, DeviceName, Bu, PassOrFail, EmpNo, DB);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000089", new string[] { PanelSN }));
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public void PalletShipOutRecord(string PalletNo, string createByUserName, string line, string bu, string stationName, R_DN_STATUS rDnStatus, OleExec dbOleExec)
        {
            //var rSnList = dbOleExec.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_PACKING>((rs, rp1, rsp, rp2) =>
            //        rs.ID == rsp.SN_ID && rp1.ID == rp2.PARENT_PACK_ID &&
            //        rp2.ID == rsp.PACK_ID && rp1.PACK_NO == PalletNo)
            //    .Select((rs, rp1, rsp, rp2) => rs).ToList().Distinct().ToList();
            T_R_PACKING TRP = new T_R_PACKING(dbOleExec, DB_TYPE_ENUM.Oracle);
            var rSnList = new List<R_SN>();
            TRP.GetSnListByPackNo(PalletNo, ref rSnList, dbOleExec);
            DoPalletShipOutRecord(PalletNo, rSnList, createByUserName, line, bu, stationName, rDnStatus, dbOleExec);
        }
        public void DoPalletShipOutRecord(string PalletNo,List<R_SN> rSnList, string createByUserName, string line, string bu, string stationName, R_DN_STATUS rDnStatus, OleExec dbOleExec)
        {
            foreach (var rSn in rSnList)
            {
                dbOleExec.ORM.Insertable<R_SHIP_DETAIL>(new R_SHIP_DETAIL()
                {
                    ID = rSn.ID,
                    SN = rSn.SN,
                    SKUNO = rSn.SKUNO,
                    DN_NO = rDnStatus.DN_NO,
                    DN_LINE = rDnStatus.DN_LINE,
                    SHIPDATE = System.DateTime.Now,
                    CREATEBY = createByUserName,
                    SHIPORDERID = rDnStatus.ID
                }).ExecuteCommand();
            }

            var rShipDetails = dbOleExec.ORM.Queryable<R_SHIP_DETAIL>()
                .Where(x => x.DN_NO == rDnStatus.DN_NO && x.DN_LINE == rDnStatus.DN_LINE).ToList();
            if (rShipDetails.Count > rDnStatus.QTY)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801091520", new string[] { PalletNo, rSnList.Count().ToString(), rDnStatus.QTY.ToString() }));
            else if (rShipDetails.Count == rDnStatus.QTY)
            {
                rDnStatus.DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_CQA.ExtValue();
                rDnStatus.EDITTIME = System.DateTime.Now;
                dbOleExec.ORM.Updateable(rDnStatus).WhereColumns(x => new { x.DN_NO, x.DN_LINE }).ExecuteCommand();
            }
            LotsPassStation(rSnList, line, stationName, stationName, bu, "PASS", createByUserName, dbOleExec);
        }

        /// <summary>
        /// 分板過站
        /// 先確定 Panel 中是否還有未替換的 SN
        /// 接著從 R_SN 中找當前第一筆並且替換掉 R_SN 中的 SN，然後調用 SN 過站
        /// </summary>
        /// <param name="PanelSN"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="SerialNo"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        /// <param name="APDB"></param>
        /// <param name="PANELObj"></param>
        public void SplitsPassStation(string PanelSN, string Line, string StationName, string DeviceName, string Bu, string SerialNo, string PassOrFail, string EmpNo, OleExec DB, OleExec APDB, R_PANEL_SN PANELObj)
        {
            string sql = string.Empty;
            AP_DLL APObj = new AP_DLL();
            string APVirtualSn = "";

            DataTable dt = new DataTable();
            Row_R_SN SnRow = (Row_R_SN)NewRow();
            string PanelId = string.Empty;
            string GetSnID = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_PANEL_SN WHERE PANEL='{PanelSN}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //從 Panel 中找到還沒有被分配實際 SN 的記錄
                    //  sql = $@"SELECT A.ID FROM R_PANEL_SN A,R_SN B WHERE A.PANEL='{PanelSN}' AND A.SN=B.SN AND B.SN=B.ID  ";
                    sql = $@"SELECT A.* FROM R_PANEL_SN A,R_SN B WHERE A.PANEL='{PanelSN}' AND A.SN=B.SN AND B.SN=B.ID ORDER BY A.SEQ_NO ";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        PanelId = dt.Rows[0]["ID"].ToString();
                        GetSnID = dt.Rows[0]["SN"].ToString();
                        //   sql = $@"SELECT * FROM R_SN WHERE SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelSN}') AND ID=SN";
                        sql = $@"SELECT * FROM R_SN WHERE ID ='{GetSnID}'";
                        dt = DB.ExecSelect(sql).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            try
                            {
                                SnRow.loadData(dt.Rows[0]);
                                SnRow.SN = SerialNo;
                                SnRow.EDIT_TIME = GetDBDateTime(DB);
                                SnRow.EDIT_EMP = EmpNo;
                                sql = SnRow.GetUpdateString(this.DBType);
                                DB.ExecSQL(sql);

                                // Update R_PANEL_SN,add by LLF 2018-02-06
                                T_R_PANEL_SN RPanelSN = new T_R_PANEL_SN(DB, this.DBType);
                                Row_R_PANEL_SN Row_Panel = (Row_R_PANEL_SN)RPanelSN.NewRow();
                                Row_Panel = (Row_R_PANEL_SN)RPanelSN.GetObjByID(PanelId, DB);
                                Row_Panel.SN = SerialNo;
                                Row_Panel.EDIT_EMP = EmpNo;
                                Row_Panel.EDIT_TIME = GetDBDateTime(DB);
                                DB.ExecSQL(Row_Panel.GetUpdateString(this.DBType));

                                //Update r_sn_kp add by fgg 2018.05.23
                                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, this.DBType);
                                t_r_sn_kp.UpdateSNBySnId(SnRow.ID, SerialNo, EmpNo, DB);

                                //Update AP 
                                //OleExecPool APDBPool = Station.DBS["APDB"];
                                int number;
                                Int32.TryParse(PANELObj.SEQ_NO.ToString(), out number);
                                if (number >= 10)
                                {
                                    APVirtualSn = PANELObj.PANEL + PANELObj.SEQ_NO.ToString();
                                }
                                else
                                {

                                    APVirtualSn = PANELObj.PANEL + "0" + PANELObj.SEQ_NO.ToString();
                                }

                                string result = APObj.APUpdatePanlSN(APVirtualSn, SerialNo, APDB);

                                if (!result.Equals("OK"))
                                {
                                    throw new MESReturnMessage(result);
                                }

                                //update R_SN_STATION_DETAIL 
                                T_R_SN_STATION_DETAIL RSnStationDetail = new T_R_SN_STATION_DETAIL(DB, this.DBType);
                                RSnStationDetail.UpdateRSnStationDetailBySNID(SerialNo, dt.Rows[0]["ID"].ToString(), DB);

                                PassStation(SerialNo, Line, StationName, DeviceName, Bu, PassOrFail, EmpNo, DB); //調用 SN 過站

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000088", new string[] { PanelSN }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000149", new string[] { PanelSN }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000038", new string[] { PanelSN }));
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 補 Allpart 資料
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Wo"></param>
        /// <param name="StationName"></param>
        /// <param name="ProductionLine"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AddAPRecords(string SerialNo, string Wo, string StationName, string ProductionLine, OleExec DB)
        {
            //Modify by LLF 2017-01-27 
            string result = string.Empty;
            //R_SN SnObject = null;
            //T_R_PANEL_SN table = null;
            //R_PANEL_SN PanelObject = null;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] parameters = new OleDbParameter[5];
                parameters[0] = new OleDbParameter("VAR_PANELNO", SerialNo);
                parameters[0].OleDbType = OleDbType.VarChar;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = new OleDbParameter("VAR_WORKORDERNO", Wo);
                parameters[1].OleDbType = OleDbType.VarChar;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = new OleDbParameter("VAR_PRODUCTIONLINE", ProductionLine);
                parameters[2].OleDbType = OleDbType.VarChar;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = new OleDbParameter("VAR_NEXTEVENT", StationName);
                parameters[3].OleDbType = OleDbType.VarChar;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = new OleDbParameter("VAR_MESSAGE", OleDbType.VarChar);
                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Size = 200;

                result = DB.ExecProcedureNoReturn("MES1.CMC_INSERTDATA_SP", parameters);

                return result;
                //SnObject = GetDetailBySN(SerialNo, DB);
                //if (SnObject != null)
                //{
                //table = new T_R_PANEL_SN(DB, this.DBType);
                //PanelObject = table.GetPanelBySn(SerialNo, DB);
                // if (PanelObject != null)
                //{
                //OleDbParameter[] parameters = new OleDbParameter[5];
                //parameters[0] = new OleDbParameter("VAR_PANELNO", PanelObject.PANEL);
                //parameters[0].OleDbType = OleDbType.VarChar;
                //parameters[0].Direction = ParameterDirection.Input;

                //parameters[1] = new OleDbParameter("VAR_WORKORDERNO", SnObject.WORKORDERNO);
                //parameters[1].OleDbType = OleDbType.VarChar;
                //parameters[1].Direction = ParameterDirection.Input;

                //parameters[2] = new OleDbParameter("VAR_PRODUCTIONLINE", ProductionLine);
                //parameters[2].OleDbType = OleDbType.VarChar;
                //parameters[2].Direction = ParameterDirection.Input;

                //parameters[3] = new OleDbParameter("VAR_NEXTEVENT", SnObject.NEXT_STATION);
                //parameters[3].OleDbType = OleDbType.VarChar;
                //parameters[3].Direction = ParameterDirection.Input;

                //parameters[4] = new OleDbParameter("VAR_MESSAGE", OleDbType.VarChar);
                //parameters[4].Direction = ParameterDirection.Output;

                //result = DB.ExecProcedureNoReturn("MES1.CMC_INSERTDATA_SP", parameters);

                //return result;
                //}
                //else
                //{
                //   throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000049", new string[] { SerialNo }));
                //}
            }
            //    else
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SerialNo }));
            //    }
            //}
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        //Add by LLF 2018-01-26 begin
        public string GetAPTrCode(string WorkOrderNo, string TrSN, string Process, string EmpNo, string MacAddress, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string TRCode = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetTRCode = new OleDbParameter[7];

                ParasForGetTRCode[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetTRCode[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetTRCode[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                ParasForGetTRCode[2].Direction = ParameterDirection.Input;

                ParasForGetTRCode[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                ParasForGetTRCode[3].Direction = ParameterDirection.Input;

                ParasForGetTRCode[4] = new OleDbParameter("G_PROCESS", Process);
                ParasForGetTRCode[4].Direction = ParameterDirection.Input;

                ParasForGetTRCode[5] = new OleDbParameter("V_TRCODE", OleDbType.VarChar);
                ParasForGetTRCode[5].Direction = ParameterDirection.Output;
                ParasForGetTRCode[5].Size = 2000;

                ParasForGetTRCode[6] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetTRCode[6].Direction = ParameterDirection.Output;
                ParasForGetTRCode[6].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_TRCODE", ParasForGetTRCode);
                TRCode = GetTRCodeDic["V_TRCODE"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return TRCode;
        }

        //ADD BY KING
        public string GetAPTrCodes(string WorkOrderNo, string TrSN, string Process, string EmpNo, string MacAddress, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string TRCode = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetTRCode = new OleDbParameter[7];

                ParasForGetTRCode[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetTRCode[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetTRCode[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                ParasForGetTRCode[2].Direction = ParameterDirection.Input;

                ParasForGetTRCode[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                ParasForGetTRCode[3].Direction = ParameterDirection.Input;

                ParasForGetTRCode[4] = new OleDbParameter("G_PROCESS", Process);
                ParasForGetTRCode[4].Direction = ParameterDirection.Input;

                ParasForGetTRCode[5] = new OleDbParameter("V_TRCODE", OleDbType.VarChar);
                ParasForGetTRCode[5].Direction = ParameterDirection.Output;
                ParasForGetTRCode[5].Size = 2000;

                ParasForGetTRCode[6] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetTRCode[6].Direction = ParameterDirection.Output;
                ParasForGetTRCode[6].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_TRCODE_CPE2", ParasForGetTRCode);
                TRCode = GetTRCodeDic["V_TRCODE"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return TRCode;
        }
        //Add by LLF 2018-01-26 End

        /// <summary>
        ///  補 SMTLOADING 資料
        /// </summary>
        /// <param name="WorkOrderNo"></param>
        /// <param name="PanelSn"></param>
        /// <param name="TrSN"></param>
        /// <param name="Process"></param>
        /// <param name="LinkQty"></param>
        /// <param name="EmpNo"></param>
        /// <param name="MacAddress"></param>
        /// <param name="TrCode"></param>
        /// <param name="flag"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AddSMTLoadingRecords(string WorkOrderNo, string PanelSn, string TrSN, string Process, double LinkQty, string EmpNo, string MacAddress, string TrCode, string flag, OleExec DB)
        {
            string result = string.Empty;
            //Dictionary<string, object> GetTRCodeDic = null;
            Dictionary<string, object> InsertSnLinkDic = null;
            //string TRCode = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //OleDbParameter[] ParasForGetTRCode = new OleDbParameter[7];
                OleDbParameter[] ParasForInsert = new OleDbParameter[11];

                //ParasForGetTRCode[0] = new OleDbParameter("G_TRSN", TrSN);
                //ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[1] = new OleDbParameter("G_WO", WorkOrderNo);
                //ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                //ParasForGetTRCode[2].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                //ParasForGetTRCode[3].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[4] = new OleDbParameter("G_PROCESS", Process);
                //ParasForGetTRCode[4].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[5] = new OleDbParameter("V_TRCODE", OleDbType.VarChar);
                //ParasForGetTRCode[5].Direction = ParameterDirection.Output;
                //ParasForGetTRCode[5].Size = 2000;

                //ParasForGetTRCode[6] = new OleDbParameter("RES", OleDbType.VarChar);
                //ParasForGetTRCode[6].Direction = ParameterDirection.Output;
                //ParasForGetTRCode[6].Size = 2000;

                //GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_TRCODE", ParasForGetTRCode);
                //TRCode = GetTRCodeDic["V_TRCODE"].ToString();

                ParasForInsert[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForInsert[0].Direction = ParameterDirection.Input;

                ParasForInsert[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForInsert[1].Direction = ParameterDirection.Input;

                ParasForInsert[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                ParasForInsert[2].Direction = ParameterDirection.Input;

                ParasForInsert[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                ParasForInsert[3].Direction = ParameterDirection.Input;

                ParasForInsert[4] = new OleDbParameter("G_TRCODE", TrCode);
                ParasForInsert[4].Direction = ParameterDirection.Input;

                ParasForInsert[5] = new OleDbParameter("G_PANELNO", PanelSn);
                ParasForInsert[5].Direction = ParameterDirection.Input;

                ParasForInsert[6] = new OleDbParameter("G_LINK_QTY", LinkQty);
                ParasForInsert[6].OleDbType = OleDbType.Numeric;
                ParasForInsert[6].Direction = ParameterDirection.Input;

                ParasForInsert[7] = new OleDbParameter("G_PROCESS", Process);
                ParasForInsert[7].Direction = ParameterDirection.Input;

                ParasForInsert[8] = new OleDbParameter("G_FLAG", flag);// sp中默認值為N即不帶PANELNO;T則表示要有PANELNO
                ParasForInsert[8].Direction = ParameterDirection.Input;

                ParasForInsert[9] = new OleDbParameter("V_EXT_QTY", OleDbType.VarChar);
                ParasForInsert[9].Direction = ParameterDirection.Output;
                ParasForInsert[9].Size = 2000;

                ParasForInsert[10] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForInsert[10].Direction = ParameterDirection.Output;
                ParasForInsert[10].Size = 2000;

                InsertSnLinkDic = DB.ExecProcedureReturnDic("MES1.Z_INSERT_SN_LINK", ParasForInsert);
                result = InsertSnLinkDic["RES"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// For NSDI AP（LH,FJZ）
        /// </summary>
        /// <param name="WorkOrderNo"></param>
        /// <param name="PanelSn"></param>
        /// <param name="MacAddress"></param>
        /// <param name="DB"></param>
        /// <param name="Ext_QTY"></param>
        /// <returns></returns>
        public string AddSMTLoadingRecordsNSDIAP(string WorkOrderNo, string PanelSn, string MacAddress, OleExec DB, ref string Ext_QTY)
        {
            string result = string.Empty;
            Dictionary<string, object> InsertSnLinkDic = null;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForInsert = new OleDbParameter[5];

                ParasForInsert[0] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForInsert[0].Direction = ParameterDirection.Input;

                ParasForInsert[1] = new OleDbParameter("G_MAC_ADDRESS", MacAddress);
                ParasForInsert[1].Direction = ParameterDirection.Input;

                ParasForInsert[2] = new OleDbParameter("G_PSN", PanelSn);
                ParasForInsert[2].Direction = ParameterDirection.Input;

                ParasForInsert[3] = new OleDbParameter("G_EXT_QTY", OleDbType.VarChar);
                ParasForInsert[3].Direction = ParameterDirection.Output;
                ParasForInsert[3].Size = 2000;

                ParasForInsert[4] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForInsert[4].Direction = ParameterDirection.Output;
                ParasForInsert[4].Size = 2000;

                InsertSnLinkDic = DB.ExecProcedureReturnDic("MES1.Z_INSERT_SN_LINK", ParasForInsert);
                result = InsertSnLinkDic["RES"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }


        public string AddSMTLoadingRecords(string WorkOrderNo, List<string> StrSns, string PanelSn, string TrSN, string Process, double LinkQty, string EmpNo, string MacAddress, string TrCode, OleExec DB)
        {
            string result = string.Empty;
            //Dictionary<string, object> GetTRCodeDic = null;
            Dictionary<string, object> InsertSnLinkDic = null;


            DB.BeginTrain();
            string sql = $@"DELETE FROM MES4.R_TEMP WHERE FLAG='{PanelSn}'";
            DB.ExecuteNonQuery(sql, CommandType.Text, null);
            foreach (string StrSn in StrSns)
            {
                sql = $@"INSERT INTO MES4.R_TEMP(SN,FLAG) VALUES('{StrSn}','{PanelSn}')";
                DB.ExecuteNonQuery(sql, CommandType.Text, null);

            }
            DB.CommitTrain();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {

                OleDbParameter[] ParasForInsert = new OleDbParameter[11];

                ParasForInsert[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForInsert[0].Direction = ParameterDirection.Input;

                ParasForInsert[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForInsert[1].Direction = ParameterDirection.Input;

                ParasForInsert[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                ParasForInsert[2].Direction = ParameterDirection.Input;

                ParasForInsert[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                ParasForInsert[3].Direction = ParameterDirection.Input;

                ParasForInsert[4] = new OleDbParameter("G_TRCODE", TrCode);
                ParasForInsert[4].Direction = ParameterDirection.Input;

                ParasForInsert[5] = new OleDbParameter("G_P_SN", "");
                ParasForInsert[5].Direction = ParameterDirection.Input;

                ParasForInsert[6] = new OleDbParameter("G_PANELNO", PanelSn);
                ParasForInsert[6].Direction = ParameterDirection.Input;

                ParasForInsert[7] = new OleDbParameter("G_LINK_QTY", LinkQty);
                ParasForInsert[7].OleDbType = OleDbType.Numeric;
                ParasForInsert[7].Direction = ParameterDirection.Input;

                ParasForInsert[8] = new OleDbParameter("G_PROCESS", Process);
                ParasForInsert[8].Direction = ParameterDirection.Input;

                ParasForInsert[9] = new OleDbParameter("V_EXT_QTY", OleDbType.VarChar);
                ParasForInsert[9].Direction = ParameterDirection.Output;
                ParasForInsert[9].Size = 2000;

                ParasForInsert[10] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForInsert[10].Direction = ParameterDirection.Output;
                ParasForInsert[10].Size = 2000;

                InsertSnLinkDic = DB.ExecProcedureReturnDic("MES1.CLOUDMES_SMTLOADING", ParasForInsert);
                result = InsertSnLinkDic["RES"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DB"></param>
        public string RecordPassStationDetail(string SerialNo, string Line, string StationName, string DeviceName, string Bu, OleExec DB, string Pass = "0")
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_SN SnRow = (Row_R_SN)NewRow();
            T_R_SN_STATION_DETAIL SnDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            Row_R_SN_STATION_DETAIL SnDetailRow = (Row_R_SN_STATION_DETAIL)SnDetailTable.NewRow();
            string result = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_SN WHERE SN='{SerialNo}' AND VALID_FLAG=1";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        SnRow.loadData(dt.Rows[0]);
                        sql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE SN='{SerialNo}' AND STATION_NAME='{StationName}'";
                        dt = DB.ExecSelect(sql).Tables[0];
                        SnDetailRow.ConstructRow((R_SN)SnRow.GetDataObject());
                        SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                        SnDetailRow.R_SN_ID = SnRow.ID;
                        SnDetailRow.LINE = Line;
                        SnDetailRow.REPAIR_FAILED_FLAG = Pass;
                        SnDetailRow.CLASS_NAME = GetWorkClass(DB);
                        SnDetailRow.DEVICE_NAME = DeviceName;
                        SnDetailRow.STATION_NAME = StationName;
                        SnDetailRow.EDIT_TIME = GetDBDateTime(DB);
                        //SnDetailRow.PRODUCT_STATUS = WorkTimes(SerialNo, StationName, DB) > 0 ? "REWORK" : "FRESH";
                        sql = SnDetailRow.GetInsertString(this.DBType);
                        result = DB.ExecSQL(sql);
                        if (Int32.Parse(result) == 0)
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }));
                    }
                    catch (Exception ex)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }) + ex.Message);
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SerialNo }));
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// 寫過站記錄(優化後)
        /// </summary>
        /// <param name="snobj"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        [Obsolete("请不要使用这个方法，此方法将被弃用", false)]
        public string RecordPassStationDetail(R_SN snobj, string Line, string StationName, string DeviceName, string Bu, OleExec DB, string Pass = "0")
        {
            string result = string.Empty;
            if (snobj.VALID_FLAG == "1")
            {
                T_R_SN_STATION_DETAIL SnDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
                Row_R_SN_STATION_DETAIL SnDetailRow = (Row_R_SN_STATION_DETAIL)SnDetailTable.NewRow();
                SnDetailRow.ConstructRow(snobj);
                SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                SnDetailRow.R_SN_ID = snobj.ID;
                SnDetailRow.LINE = Line;
                SnDetailRow.REPAIR_FAILED_FLAG = Pass;
                SnDetailRow.CLASS_NAME = GetWorkClass(DB);
                SnDetailRow.DEVICE_NAME = DeviceName;
                SnDetailRow.STATION_NAME = StationName;
                SnDetailRow.EDIT_TIME = GetDBDateTime(DB);
                //SnDetailRow.PRODUCT_STATUS = WorkTimes(SerialNo, StationName, DB) > 0 ? "REWORK" : "FRESH";
                result = DB.ExecSQL(SnDetailRow.GetInsertString(this.DBType));
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { snobj.SN }));
            }

            return result;
        }

        /// <summary>
        /// 批量过站记录(批量过站优化)
        /// </summary>
        /// <param name="snobj"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        public int RecordPassStationDetail(List<R_SN> snobj, string Line, string StationName, string DeviceName, string Bu, OleExec DB, string Pass = "0")
        {
            int n = 0;
            var sndetail = new List<R_SN_STATION_DETAIL>();
            for (int i = 0; i < snobj.Count; i++)
            {
                if (snobj[i].VALID_FLAG == "1")
                {
                    R_SN_STATION_DETAIL SnDetailRow = DataHelper.Mapper<R_SN_STATION_DETAIL, R_SN>(snobj[i]);
                    SnDetailRow.ID = MesDbBase.GetNewID<R_SN_STATION_DETAIL>(DB.ORM, Bu);
                    SnDetailRow.R_SN_ID = snobj[i].ID;
                    SnDetailRow.LINE = Line;
                    SnDetailRow.REPAIR_FAILED_FLAG = Pass;
                    SnDetailRow.CLASS_NAME = GetWorkClass(DB);
                    SnDetailRow.DEVICE_NAME = DeviceName;
                    SnDetailRow.STATION_NAME = StationName;
                    SnDetailRow.EDIT_TIME = GetDBDateTime(DB);

                    sndetail.Add(SnDetailRow);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { snobj[i].SN }));
                }
            }
            n = DB.ORM.Insertable(sndetail).ExecuteCommand();

            return n;
        }


        #endregion

        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DB"></param>
        /// 
        public string PassStationExtDetail(string SerialNo, string Description, string StationName, string Location, string EMP_NO, string Bu, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            //Row_R_SN SnRow = (Row_R_SN)NewRow();
            T_R_MES_EXT SnDetailTable = new T_R_MES_EXT(DB, this.DBType);
            Row_R_MES_EXT SnDetailRow = (Row_R_MES_EXT)SnDetailTable.NewRow();
            string result = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {

                try
                {
                    //SnRow.loadData(dt.Rows[0]);
                    //SnDetailRow.ConstructRow((R_SN)SnRow.GetDataObject());
                    SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                    SnDetailRow.DATA1 = SerialNo;
                    SnDetailRow.DATA2 = Location;
                    SnDetailRow.DATA3 = Description;
                    SnDetailRow.DATA4 = StationName;
                    SnDetailRow.EDITEMP = EMP_NO;
                    SnDetailRow.EDITTIME = GetDBDateTime(DB);

                    sql = SnDetailRow.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    if (Int32.Parse(result) == 0)
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }));
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }) + ex.Message);
                }


            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        public string LinkPassStationDetail(R_SN SNObj, string WO, string Skuno, string RouteID, string Line, string StationName, string DeviceName, string EmpNO, string Bu, OleExec DB)
        {
            string sql = "";
            T_R_SN_STATION_DETAIL SnDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            Row_R_SN_STATION_DETAIL SnDetailRow = (Row_R_SN_STATION_DETAIL)SnDetailTable.NewRow();
            string result = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                SnDetailRow.R_SN_ID = SNObj.ID;
                SnDetailRow.SN = SNObj.SN;
                SnDetailRow.SHIPPED_FLAG = SNObj.SHIPPED_FLAG;
                SnDetailRow.SHIPDATE = SNObj.SHIPDATE;
                SnDetailRow.COMPLETED_FLAG = SNObj.COMPLETED_FLAG;
                SnDetailRow.COMPLETED_TIME = SNObj.COMPLETED_TIME;
                SnDetailRow.STARTED_FLAG = "1";
                SnDetailRow.START_TIME = SNObj.START_TIME;
                SnDetailRow.PLANT = SNObj.PLANT;
                SnDetailRow.WORKORDERNO = WO;
                SnDetailRow.SKUNO = Skuno;
                SnDetailRow.ROUTE_ID = RouteID;
                SnDetailRow.VALID_FLAG = "1";
                SnDetailRow.CURRENT_STATION = SNObj.CURRENT_STATION;
                SnDetailRow.NEXT_STATION = SNObj.NEXT_STATION;
                SnDetailRow.LINE = Line;
                SnDetailRow.REPAIR_FAILED_FLAG = "0";
                SnDetailRow.CLASS_NAME = GetWorkClass(DB);
                SnDetailRow.DEVICE_NAME = DeviceName;
                SnDetailRow.STATION_NAME = StationName;
                SnDetailRow.EDIT_EMP = EmpNO;
                SnDetailRow.EDIT_TIME = GetDBDateTime(DB);
                sql = SnDetailRow.GetInsertString(this.DBType);
                result = DB.ExecSQL(sql);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public R_SN GetSN(string sn, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        public R_SN GetSNByBoxSN(string sn, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(t => (t.SN == sn || t.BOXSN == sn) && t.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        public int Update(R_SN SnObject, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN>(SnObject).Where(t => t.ID == SnObject.ID).ExecuteCommand();
        }

        public R_SN GetDetailBySN(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //sql = $@"select * from r_sn where (sn='{sn.Replace("'", "''")}' or boxsn='{sn.Replace("'", "''")}')  and valid_flag='1'";//只取有效的sn
                sql = $@"select * from r_sn where (sn='{sn.Replace("'", "''")}')  and valid_flag='1'";//只取有效的sn
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    sql = $@"select * from r_sn where (sn='{sn.Replace("'", "''")}')  and valid_flag='2' order by start_time desc ";//取最晚的一笔转投的
                    dt = db.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count >= 1)
                    {
                        row_rsn = (Row_R_SN)this.NewRow();
                        row_rsn.loadData(dt.Rows[0]);
                    }
                    else
                    {
                        string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { sn });
                        throw new MESReturnMessage(errMsg);
                    }

                    
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }

        public R_SN GetDetailByID(string ID, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //sql = $@"select * from r_sn where (sn='{sn.Replace("'", "''")}' or boxsn='{sn.Replace("'", "''")}')  and valid_flag='1'";//只取有效的sn
                sql = $@"select * from r_sn where id='{ID.Replace("'", "''")}' ";
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { ID });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }

        ///
        public R_SN GetDetailByPanelSN(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from r_sn a,r_panel_sn b where b.panel='{sn.Replace("'", "''")}' and a.WORKORDERNO=b.WORKORDERNO and a.sn=b.sn and valid_flag='1'";//只取有效的sn
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count >= 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + sn });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }

        /// <summary>
        /// 判断SN是否已经使用，已经使用返回TRUE，没有使用返回FALSE
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool IsUsed(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            sql = $@"select * from r_sn where sn=:sn  and valid_flag='1'";//只取有效的sn
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":sn", sn);
            dt = db.ExecuteDataTable(sql, CommandType.Text, paramet);
            if (dt.Rows.Count > 0)            
                return true;

            sql = $@"select * from r_sn_his where sn=:sn  and valid_flag='1'";//只取有效的sn
            dt = db.ExecuteDataTable(sql, CommandType.Text, paramet);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断StartSN&EndSN區間是否已经使用，已经使用返回TRUE，没有使用返回FALSE
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool SNRangeIsUsed(string strStartSN, string strEndSN, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            sql = $@"select * from r_sn where sn between :strStartSN and :strEndSN and valid_flag='1'";//只取有效的sn
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":strStartSN", strStartSN);
            paramet[1] = new OleDbParameter(":strEndSN", strEndSN);
            dt = db.ExecuteDataTable(sql, CommandType.Text, paramet);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //get list<r_sn> by wo .
        public List<R_SN> GetRSNbyWo(string wo, OleExec DB)
        {
            string strSql = $@"select * from r_sn where workorderno=:wo";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":wo", wo) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            //else
            //{
            //    return null;
            //}
            return listSn;
        }


        /// <summary>
        /// 取到 SN VALID_FLAG=0 時間最晚的無效記錄
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<R_SN> GetINVaildSN(string sn, string MarkSnid, OleExec DB)
        {

            string strSql = $@" select* from r_sn a where a.sn=:sn  and a.valid_flag = '0' and a.edit_time in (select max(b.edit_time) from r_sn b where b.sn = a.sn and b.id<>:MarkSnid)";

            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":sn", sn);
            paramet[1] = new OleDbParameter(":MarkSnid", MarkSnid);
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            return listSn;
        }

        /// <summary>
        /// 传入一个panelSn 返回对应R_SN表里SN的集合
        /// </summary>
        /// <param name="PanelSn"></param>
        /// <param name="DB"></param>
        /// <returns>List<R_SN></returns>
        public List<R_SN> GetRSNbyPsn(string PanelSn, OleExec DB)
        {
            //Modify BY LLF 2018-01-27
            //string strSql = $@"SELECT * FROM R_SN WHERE ID IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelSn.Replace("'", "''")}')";
            //string strSql = $@"SELECT * FROM R_SN WHERE SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelSn.Replace("'", "''")}')";
            string strSql = $@"SELECT * FROM R_SN A WHERE EXISTS (SELECT * FROM R_PANEL_SN B WHERE A.SN=B.SN AND A.WORKORDERNO=B.WORKORDERNO AND PANEL='{PanelSn.Replace("'", "''")}')";//修復BUG－一個PANEL裡有SN已經掃LINK，其他沒過BIP的產品會導致ROUTE改變，增加WORKORDER作為條件2018-04-07 11:00 by LJD
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":PanelSn", PanelSn) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }

        //Add by LLF 2018-01-27
        public List<R_SN> GetRSNbySN(string SN, OleExec DB)
        {
            string strSql = $@"SELECT * FROM R_SN WHERE SN ='{SN.Replace("'", "''")}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }

        /// <summary>
        /// SN入MRB時，更新SN當前站為MRB，下一站為REWORK，completed_flag=1,completed_time=now
        /// </summary>
        /// <param name="snid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int SN_Mrb_Pass_action(string snid, string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set current_station='MRB',next_station='REWORK',completed_flag='1',completed_time=sysdate,edit_emp=:userno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":userno", userno),
                new OleDbParameter(":snid", snid) };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }
        /// <summary>
        /// SN入MRB時，更新SN當前站為MRB，下一站為REWORK，不更新completed_flag=1,completed_time=now
        /// </summary>
        /// <param name="snid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int SN_Mrb_Pass_actionNotUpdateCompleted(string snid, string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set current_station='MRB',next_station='REWORK',edit_emp=:userno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":userno", userno),
                new OleDbParameter(":snid", snid) };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }
        public int SNCBS_Mrb_Pass_actionNotUpdateCompleted(string snid, string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set PACKED_FLAG =0,current_station='MRB',next_station='REWORK',edit_emp=:userno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":userno", userno),
                new OleDbParameter(":snid", snid) };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }
        public int updateValid_Flag(string snid, string validFlag, string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set valid_flag=:validflag,edit_emp=:empno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":validflag", validFlag),
                new OleDbParameter(":empno", userno),
                new OleDbParameter(":snid", snid)
            };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }

        //當確定為跳站時(程序傳進來的工站名稱等於流程中定義的當前CurrentStation名稱的跳站名稱),
        //更新r_sn的CurrentStation and NextStation,以便不變更ChangeSnStatus的邏輯
        //CurrentStation為SN當前的NextStation,NextStation為程序傳進來的工站名稱
        public int TiaoZhanUpdateCurrentNextStation(string snid, string CurrentStation, string NextStation, OleExec DB)
        {
            string strSql = $@" update r_sn set current_station=:CurrentStation,next_station=:NextStation where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":CurrentStation", CurrentStation),
                new OleDbParameter(":NextStation", NextStation),
                new OleDbParameter(":snid", snid) };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }
        public R_SN GetById(string snid, OleExec DB)
        {
            string strSql = $@" select * from r_sn  where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":snid", snid)
            };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_R_SN ret = (Row_R_SN)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public int AddNewSN(R_SN NewSn, OleExec DB)
        {
            Row_R_SN rowSN = (Row_R_SN)this.ConstructRow(NewSn);
            string strSql = rowSN.GetInsertString(this.DBType);
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, null);
            return res;
        }

        public Row_R_SN getR_SNbySN(string SN, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_MENU where SN='{SN}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_SN ret = (Row_R_SN)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<R_SN> GetObaSnListByLotNo(string lotNo, OleExec DB)
        {
            List<R_SN> res = new List<R_SN>();
            string strSql = $@" SELECT E.* FROM R_LOT_PACK A,R_PACKING B,R_PACKING C,R_SN_PACKING D,R_SN E
                                WHERE A.LOTNO='{lotNo}' AND A.PACKNO=B.PACK_NO AND B.ID=C.PARENT_PACK_ID AND C.ID=D.PACK_ID AND D.SN_ID=E.ID  AND E.VALID_FLAG=1 ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_SN r = (Row_R_SN)NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        public List<R_SN> GetSnListByPack(string packNo, OleExec DB)
        {
            List<R_SN> res = new List<R_SN>();
            string strSql = $@"  SELECT E.* FROM R_PACKING B,R_PACKING C,R_SN_PACKING D,R_SN E
                                WHERE B.ID=C.PARENT_PACK_ID AND C.ID=D.PACK_ID AND D.SN_ID=E.ID  AND E.VALID_FLAG=1 AND B.PACK_NO='{packNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_SN r = (Row_R_SN)NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        //Add by LLF 2018-02-19 begin
        public string GetAPPTHTrCode(string WorkOrderNo, string Station, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string TRCode = string.Empty;
            string Message = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetPTHTRCode = new OleDbParameter[5];

                ParasForGetPTHTRCode[0] = new OleDbParameter("g_type", "");
                ParasForGetPTHTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetPTHTRCode[1] = new OleDbParameter("g_wo", WorkOrderNo);
                ParasForGetPTHTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetPTHTRCode[2] = new OleDbParameter("g_station", Station);
                ParasForGetPTHTRCode[2].Direction = ParameterDirection.Input;

                ParasForGetPTHTRCode[3] = new OleDbParameter("g_tr_code", OleDbType.VarChar);
                ParasForGetPTHTRCode[3].Direction = ParameterDirection.Output;
                ParasForGetPTHTRCode[3].Size = 2000;

                ParasForGetPTHTRCode[4] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetPTHTRCode[4].Direction = ParameterDirection.Output;
                ParasForGetPTHTRCode[4].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_EXISTS_TRCODE_PTH", ParasForGetPTHTRCode);
                Message = GetTRCodeDic["RES"].ToString();

                if (Message == "OK")
                {
                    TRCode = GetTRCodeDic["g_tr_code"].ToString();
                }
                else
                {
                    throw new MESReturnMessage(Message);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return TRCode;
        }
        //Add by LLF 2018-02-19 End

        //add by LLF 2018-02-22 begin
        public R_SN GetDetailByPanelAndSN(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from r_sn where sn='{sn.Replace("'", "''")}' and valid_flag='1'";//只取有效的sn
                sql = sql + $@"union select a.* from r_sn a, r_panel_sn b where b.panel = '{sn.Replace("'", "''")}' and a.sn=b.sn and valid_flag='1'";//只取有效的sn
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + sn });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }
        ///add by LLF 2018-02-22 end

        public string GetSkuNotSE(string pack_no, OleExec db)
        {
            string skuno;
            DataTable dt = new DataTable();
            string sql = $@"SELECT * FROM R_SN WHERE ID IN (
            SELECT SN_ID FROM R_SN_PACKING WHERE PACK_ID IN(
            SELECT ID FROM R_PACKING WHERE PARENT_PACK_ID IN (
            SELECT ID FROM R_PACKING WHERE PACK_NO ='{pack_no}')))";
            dt = db.ExecSelect(sql).Tables[0];
            if (dt != null && dt.Rows.Count != 0)
            {
                return skuno = dt.Rows[0]["skuno"].ToString();
            }
            else
            {
                return skuno = "";
            }
        }

        public string UpdateSNKeyparStatus(string SnID, string Emp_NO, string Valid, OleExec db)
        {
            string result = "";
            string sql = "";
            DataTable Dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_SN Rows = (Row_R_SN)NewRow();
                sql = $@"select * from r_sn where ID='{SnID}' and valid_flag='1'";//只取有效的sn
                Dt = db.ExecSelect(sql).Tables[0];
                if (Dt.Rows.Count == 1)
                {
                    Rows = (Row_R_SN)this.NewRow();
                    Rows.loadData(Dt.Rows[0]);
                }
                Rows.ID = SnID;
                Rows.VALID_FLAG = Valid;
                Rows.SHIPPED_FLAG = "1";
                Rows.SHIPDATE = GetDBDateTime(db);
                Rows.EDIT_EMP = Emp_NO;
                Rows.EDIT_TIME = GetDBDateTime(db);
                result = db.ExecSQL(Rows.GetUpdateString(DBType, SnID));

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public string InsertLinkSN(string Sn, string WO, string Skuno, string RouteID, string Kp_List_ID, string Station, string NextStation, string Emp_NO, string Bu, OleExec db, string Plant)
        {
            string result = "";
            T_R_SN Table_R_SN = new T_R_SN(db, DBType);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_SN RowsInsert = (Row_R_SN)NewRow();
                RowsInsert.ID = Table_R_SN.GetNewID(Bu, db);
                RowsInsert.SN = Sn;
                RowsInsert.PLANT = Plant;
                RowsInsert.STARTED_FLAG = "1";
                RowsInsert.START_TIME = GetDBDateTime(db);
                RowsInsert.WORKORDERNO = WO;
                RowsInsert.SKUNO = Skuno;
                RowsInsert.ROUTE_ID = RouteID;
                RowsInsert.SHIPPED_FLAG = "0";
                RowsInsert.COMPLETED_FLAG = "0";
                RowsInsert.REPAIR_FAILED_FLAG = "0";
                RowsInsert.STOCK_STATUS = "0";
                RowsInsert.VALID_FLAG = "1";
                RowsInsert.KP_LIST_ID = Kp_List_ID;
                RowsInsert.CURRENT_STATION = Station;
                RowsInsert.NEXT_STATION = NextStation;
                RowsInsert.EDIT_EMP = Emp_NO;
                RowsInsert.EDIT_TIME = GetDBDateTime(db);
                result = db.ExecSQL(RowsInsert.GetInsertString(DBType));
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        public string UpdateLinkKPID(string oldID, string sn, string newID, OleExec db)
        {
            string result = "";
            string sql = "";
            DataTable dt = new DataTable();
            T_R_SN Table_R_SN = new T_R_SN(db, DBType);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from r_sn_kp where r_sn_id ='{oldID}' and sn = '{sn}'";
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    sql = $@"update r_sn_kp set r_sn_id = '{newID}' where  r_sn_id ='{oldID}' and sn = '{sn}'";
                    result = db.ExecSQL(sql);
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        public int ReplaceSn(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public int UpdateSNRMAStaus(string SnID, string Skuno, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN R SET R.workorderno = 'RMA',R.CURRENT_STATION = 'RMA',SKUNO = '{Skuno}' WHERE R.ID='{SnID}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// Oba工站ByLotNo取批次里所有SN
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_SN> GetSnByLotNoWithOba(string lotNo, OleExec DB)
        {
            //string strSql = $@"  select e.* from r_sn_packing a ,r_lot_pack b,r_packing c,r_packing d,r_sn e where a.pack_id=c.id and b.lotno='{lotNo}' and c.parent_pack_id=d.id  and d.pack_no=b.packno and a.sn_id=e.id ";
            //DataSet ds = DB.ExecSelect(strSql);
            //List<R_SN> res = new List<R_SN>();
            //foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            //{
            //    Row_R_SN r = (Row_R_SN)this.NewRow();
            //    r.loadData(VARIABLE);
            //    res.Add(r.GetDataObject());
            //}
            //return res;
            return DB.ORM.Queryable<R_SN_PACKING, R_LOT_PACK, R_PACKING, R_PACKING, R_SN>((a, b, c, d, e) => a.PACK_ID == c.ID && c.PARENT_PACK_ID == d.ID && d.PACK_NO == b.PACKNO && a.SN_ID == e.ID)
                .Where((a, b, c, d, e) => b.LOTNO == lotNo).Select((a, b, c, d, e) => e).ToList();
        }

        public string InsertRMASN(string Sn, string WO, string Skuno, string RouteID, string Kp_List_ID, string Station, string NextStation, string Emp_NO, string Bu, OleExec db, string Plant, string ProductStatus)
        {
            string result = "";
            T_R_SN Table_R_SN = new T_R_SN(db, DBType);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_SN RowsInsert = (Row_R_SN)NewRow();
                RowsInsert.ID = Table_R_SN.GetNewID(Bu, db);
                RowsInsert.SN = Sn;
                RowsInsert.PLANT = Plant;
                RowsInsert.STARTED_FLAG = "1";
                RowsInsert.START_TIME = GetDBDateTime(db);
                RowsInsert.PACKED_FLAG = "0";
                RowsInsert.WORKORDERNO = WO;
                RowsInsert.SKUNO = Skuno;
                RowsInsert.ROUTE_ID = RouteID;
                RowsInsert.SHIPPED_FLAG = "0";
                RowsInsert.COMPLETED_FLAG = "1";
                RowsInsert.COMPLETED_TIME = GetDBDateTime(db);
                RowsInsert.STOCK_STATUS = "0";
                RowsInsert.VALID_FLAG = "1";
                RowsInsert.PRODUCT_STATUS = ProductStatus;
                RowsInsert.KP_LIST_ID = Kp_List_ID;
                RowsInsert.CURRENT_STATION = Station;
                RowsInsert.NEXT_STATION = NextStation;
                RowsInsert.EDIT_EMP = Emp_NO;
                RowsInsert.EDIT_TIME = GetDBDateTime(db);
                RowsInsert.REWORK_COUNT = 0;
                RowsInsert.REPAIR_FAILED_FLAG = "0";
                RowsInsert.CUST_PN = Skuno;
                RowsInsert.SCRAPED_FLAG = "0";
                result = db.ExecSQL(RowsInsert.GetInsertString(DBType));
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public string GetPanelWaitReplaceSn(string panel, OleExec sfcdb)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_PANEL_SN WHERE PANEL='{panel}'";
            dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //從 Panel 中找到還沒有被分配實際 SN 的記錄
                sql = $@"SELECT A.* FROM R_PANEL_SN A,R_SN B WHERE A.PANEL='{panel}' AND A.SN=B.SN AND B.SN=B.ID ORDER BY A.SEQ_NO ";
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["SN"].ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000149", new string[] { panel }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000038", new string[] { panel }));
            }
        }

        public void GetSNsByPackNo(string PackNo, ref List<R_SN> SNs, OleExec DB)
        {
            //遞歸查詢及其緩慢,更新為直接查詢--add by Eden 
            R_PACKING P = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
            if (P.PARENT_PACK_ID != null && P.PARENT_PACK_ID != "")
                SNs = DB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((rp1, rp2, rn, rsp) =>
                        rp1.PACK_NO == rp2.PARENT_PACK_ID
                        && rp1.ID == rsp.PACK_ID && rsp.SN_ID == rn.ID).Where((rp1, rp2, rn, rsp) =>
                        rn.VALID_FLAG == "1" && rp2.PACK_NO == PackNo)
                    .Select((rp1, rp2, rn, rsp) => rn).ToList();
            else
                SNs = DB.ORM.Queryable<R_PACKING, R_SN, R_SN_PACKING>((rp, rn, rsp) =>
                        rp.ID == rsp.PACK_ID && rsp.SN_ID == rn.ID).Where((rp, rn, rsp) =>
                        rn.VALID_FLAG == "1" && rp.PARENT_PACK_ID == P.ID)
                    .Select((rp, rn, rsp) => rn).ToList();
            //if (SNs == null)
            //{
            //    SNs = new List<R_SN>();
            //}
            //R_PACKING P = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
            //if (P != null)
            //{
            //    List<R_PACKING> Packings = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == P.ID).ToList();
            //    foreach (R_PACKING Packing in Packings)
            //    {
            //        GetSNsByPackNo(Packing.PACK_NO,ref SNs, DB);
            //    }

            //    List<R_SN> SNList = DB.ORM.Queryable<R_SN, R_SN_PACKING>((s, sp) => s.ID == sp.SN_ID).Where((s, sp) => sp.PACK_ID == P.ID && s.VALID_FLAG=="1").Select((s, sp) => s).ToList();
            //    SNs.AddRange(SNList);
            //}
        }

        public void GetSNByCarton(string PackNo, ref List<R_SN> SNs, OleExec DB)
        {
            R_PACKING P = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
            SNs = DB.ORM.Queryable<R_SN, R_SN_PACKING>((rn, rsp) => rsp.SN_ID == rn.ID)
                .Where((rn, rsp) => rsp.PACK_ID == P.ID && rn.VALID_FLAG == "1")
                .Select((rn, rsp) => rn).ToList();
        }
        public void UpdateShippingFlag(List<R_SN> snList, string flag, string user, OleExec DB)
        {
            var idList = snList.Select(t => t.ID).ToList();
            var sns = DB.ORM.Queryable<R_SN>().Where(t => idList.Contains(t.ID)).ToList();
            if (sns.Count > 0)
            {
                for (int i = 0; i < sns.Count; i++)
                {
                    DateTime dt = GetDBDateTime(DB);

                    sns[i].SHIPPED_FLAG = flag;
                    sns[i].SHIPDATE = null;
                    sns[i].EDIT_TIME = dt;
                    sns[i].EDIT_EMP = user;
                }
                DB.ORM.Updateable(sns).ExecuteCommand();
            }
        }

        /// <summary>
        /// 讀取 R_SN 中指定工單已經在指定站位之後的 SN 數量
        /// </summary>
        /// <param name="Wo"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetWoPassStationQty(string Wo, string Station, OleExec DB)
        {
            List<C_ROUTE_DETAIL> RouteDetails = DB.ORM.Queryable<R_WO_BASE, C_ROUTE_DETAIL>((rwb, crd) => rwb.ROUTE_ID == crd.ROUTE_ID)
                .Where((rwb, crd) => rwb.WORKORDERNO == Wo).Select((rwb, crd) => crd).ToList();
            C_ROUTE_DETAIL CurrentDetail = RouteDetails.Find(t => t.STATION_NAME == Station);
            if (CurrentDetail != null)
            {
                List<string> RestStations = RouteDetails.FindAll(t => t.SEQ_NO >= CurrentDetail.SEQ_NO).Select(t => t.STATION_NAME).ToList();
                return DB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == Wo && RestStations.Contains(t.CURRENT_STATION)).ToList().Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 檢查SN格式 Simon 2019/2/14 
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns>格式正確："OK" ；錯誤："NG" </returns>
        public string CheckSNFormat(string sn, OleExec DB)
        {
            string sql = $@"select SFC.check_char('{sn}') from dual";
            return DB.ExecSelectOneValue(sql).ToString();
        }

        public List<R_SN> LoadSNWO(string SN, OleExec DB)
        {
            //List<R_SN> Seq = new List<R_SN>();
            //string sql = string.Empty;
            //DataTable dt = new DataTable("C_SEQNO");
            //Row_R_SN SeqRow = (Row_R_SN)NewRow();

            //if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            //{
            //    sql = $@"select * from r_sn where sn='{SN}' and valid_flag='1'";
            //    //  string StrSql = $@"select * from r_sn where sn='{StrSN}' and valid_flag='1'";
            //    dt = DB.ExecSelect(sql, null).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        SeqRow.loadData(dr);
            //        Seq.Add(SeqRow.GetDataObject());
            //    }
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}

            //return Seq;
            return DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").ToList();
        }

        /// <summary>
        /// add by zc
        /// </summary>
        public void SnRmaCheck(string SN, string WO, string BU, string EMP_NO, OleExec dbOleExec)
        {
            //string SqlSnId = $"SELECT ID FROM R_RMA WHERE SN='{SN}' AND ROWNUM=1 ";

            string SqlSnSkuno = $"SELECT ID,SKUNO FROM R_SN WHERE SN='{SN}' AND ROWNUM=1 ";
            string SnId = dbOleExec.ExecSelect(SqlSnSkuno, null).Tables[0].Rows[0]["ID"].ToString();
            string Skuno = dbOleExec.ExecSelect(SqlSnSkuno, null).Tables[0].Rows[0]["SKUNO"].ToString();
            string Sql1 = $@"SELECT * FROM R_RMA WHERE SN='{SN.Trim().ToUpper()}' OR SN=SFC.RIGHT('{SN.Trim().ToUpper()}',11) ";
            DataTable dt1 = dbOleExec.ExecSelect(Sql1).Tables[0];
            if (dt1.Rows.Count > 0)
            {
                string Sql2 = $"SELECT SN,UNLOCKED_FLAG FROM R_RMA WHERE SFC.RIGHT(SN,11)='{SN.Trim().ToUpper()}'";
                DataTable dt2 = dbOleExec.ExecSelect(Sql2).Tables[0];
                string MrbSn = dt2.Rows[0]["SN"].ToString();
                string UnlockFlag = dt2.Rows[0]["UNLOCKED_FLAG"].ToString();
                if (SN == MrbSn.Substring(0, 11) && UnlockFlag == "0")//應該從右邊截取MrbSn然後查看是否與SN相等
                {
                    string Sql5 = $@"SELECT * FROM R_RMA WHERE  SFC.RIGHT(SN,11)='{SN.Trim().ToUpper()}'";
                    DataTable dt3 = dbOleExec.ExecSelect(Sql5).Tables[0];
                    if (dt3.Rows.Count > 0)//sn從右往左截取11位是否在rma表中
                    {
                        if (MrbSn == SN.Trim().ToUpper())
                        {
                            string Sql3 = $"UPDATE  R_RMA SET SN= 'RM1'||'-'||'{SN.Trim().ToUpper()}' where SN='{SN.Trim().ToUpper()}'";
                            dbOleExec.ExecuteNonQuery(Sql3, CommandType.Text, null);
                        }
                        else
                        {
                            string Sql3 = $"UPDATE R_RMA SET SN='RM'|| TO_CHAR(SUBSTR('{SN.Trim().ToUpper()}',3,1)+1) ||'-'||SFC.RIGHT(SN,11)WHERE  SUBSTR(SN,1,2)='RM' and SFC.RIGHT(SN,11)='{SN.Trim().ToUpper()}'";
                            dbOleExec.ExecuteNonQuery(Sql3, CommandType.Text, null);
                        }
                        T_R_RMA TRMA = new T_R_RMA(dbOleExec, DB_TYPE_ENUM.Oracle);
                        Row_R_RMA Row = (Row_R_RMA)TRMA.NewRow();
                        Row.ID = TRMA.GetNewID(BU, dbOleExec);
                        Row.R_SN_ID = SnId;
                        Row.SN = SN;
                        Row.WO = WO;
                        Row.SKUNO = Skuno;
                        Row.BU = BU;
                        Row.SCAN_EMP = EMP_NO;
                        Row.SCAN_TIME = System.DateTime.Now;
                        Row.LOCKED_FLAG = "1";
                        Row.UNLOCKED_FLAG = "1";
                        Row.VALID_FLAG = "0";
                        Row.EDIT_EMP = EMP_NO;
                        Row.EDIT_TIME = System.DateTime.Now;
                        try
                        {
                            string Sql4 = Row.GetInsertString(DB_TYPE_ENUM.Oracle);
                            string Result = dbOleExec.ExecSQL(Sql4);
                            if (Result == "")
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190329142938", new string[] { SN }));
                            }
                        }
                        catch
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190329142938", new string[] { SN }));
                        }
                    }
                    else//sn從右往左截取11位不在rma表中
                    {
                        string Sql6 = $"UPDATE R_RMA SET SN='RM'|| TO_CHAR(SUBSTR('{SN.Trim().ToUpper()}',3,1)+1) ||'-'||SFC.RIGHT(SN,11)WHERE  SUBSTR(SN,1,2)='RM' and SFC.RIGHT(SN,11)='{SN.Trim().ToUpper()}'";
                        dbOleExec.ExecuteNonQuery(Sql6, CommandType.Text, null);
                        string Sql7 = $"UPDATE  R_RMA SET SN= 'RM1'||'-'||'{SN.Trim().ToUpper()}' where SN='{SN.Trim().ToUpper()}'";
                        dbOleExec.ExecuteNonQuery(Sql7, CommandType.Text, null);
                        T_R_RMA TRMA = new T_R_RMA(dbOleExec, DB_TYPE_ENUM.Oracle);
                        Row_R_RMA Row = (Row_R_RMA)TRMA.NewRow();
                        Row.ID = TRMA.GetNewID(BU, dbOleExec);
                        Row.R_SN_ID = SnId;
                        Row.SN = SN;
                        Row.WO = WO;
                        Row.SKUNO = Skuno;
                        Row.BU = BU;
                        Row.SCAN_EMP = EMP_NO;
                        Row.SCAN_TIME = System.DateTime.Now;
                        Row.LOCKED_FLAG = "1";
                        Row.UNLOCKED_FLAG = "1";
                        Row.VALID_FLAG = "0";
                        Row.EDIT_EMP = EMP_NO;
                        Row.EDIT_TIME = System.DateTime.Now;
                        try
                        {
                            string Sql8 = Row.GetInsertString(DB_TYPE_ENUM.Oracle);
                            string Result = dbOleExec.ExecSQL(Sql8);
                            if (Result == "")
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190329142938", new string[] { SN }));
                            }
                        }
                        catch
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190329142938", new string[] { SN }));
                        }
                    }
                }
            }
            else
            {
                T_R_RMA TRMA = new T_R_RMA(dbOleExec, DB_TYPE_ENUM.Oracle);
                Row_R_RMA Row = (Row_R_RMA)TRMA.NewRow();
                Row.ID = TRMA.GetNewID(BU, dbOleExec);
                Row.R_SN_ID = SnId;
                Row.SN = SN;
                Row.WO = WO;
                Row.SKUNO = Skuno;
                Row.BU = BU;
                Row.SCAN_EMP = EMP_NO;
                Row.SCAN_TIME = System.DateTime.Now;
                Row.LOCKED_FLAG = "1";
                Row.UNLOCKED_FLAG = "1";
                Row.VALID_FLAG = "0";
                Row.EDIT_EMP = EMP_NO;
                Row.EDIT_TIME = System.DateTime.Now;
                try
                {
                    string Sql8 = Row.GetInsertString(DB_TYPE_ENUM.Oracle);
                    string Result = dbOleExec.ExecSQL(Sql8);
                    if (Result == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SN }));
                    }
                }
                catch
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SN }));
                }
            }
            #region 有參考作用的代碼，已注釋
            //var Data=dbOleExec.ORM.Queryable<R_RMA>().Where(r =>r.SN==SN||r.SN==WO).Select(r=> new { r.SKUNO});
            //string Skuno = Data.ToString();
            //Data=dbOleExec.ORM.Queryable<>
            //var rSnList = dbOleExec.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_PACKING>((rs, rp1, rsp, rp2) =>
            //        rs.ID == rsp.SN_ID && rp1.ID == rp2.PARENT_PACK_ID &&
            //        rp2.ID == rsp.PACK_ID && rp1.PACK_NO == PalletNo)
            //    .Select((rs, rp1, rsp, rp2) => rs).ToList().Distinct().ToList();
            //foreach (var rSn in rSnList)
            //{
            //    dbOleExec.ORM.Insertable<R_SHIP_DETAIL>(new R_SHIP_DETAIL()
            //    {
            //        ID = rSn.ID,
            //        SN = rSn.SN,
            //        SKUNO = rSn.SKUNO,
            //        DN_NO = rDnStatus.DN_NO,
            //        DN_LINE = rDnStatus.DN_LINE,
            //        SHIPDATE = System.DateTime.Now,
            //        CREATEBY = createByUserName
            //    }).ExecuteCommand();
            //}
            //    sql = SnDetailRow.GetInsertString(this.DBType);
            //    result = DB.ExecSQL(sql);
            //    if (Int32.Parse(result) == 0)
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }));
            //}
            #endregion 有參考作用的代碼，已注釋

        }

        public int SN_Mrb_Pass_action(string snid, string StationName, string userno, OleExec DB)
        {
            //string strSql = $@" update r_sn set current_station='{StationName}',completed_flag='1',completed_time=sysdate,edit_emp=:userno,edit_time=sysdate where id=:snid";
            //OleDbParameter[] paramet = new OleDbParameter[] {
            //    new OleDbParameter(":userno", userno),
            //    new OleDbParameter(":snid", snid) };
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
            return DB.ORM.Updateable<R_SN>()
                .UpdateColumns(t => new R_SN() { CURRENT_STATION = StationName, COMPLETED_FLAG = "1", COMPLETED_TIME = GetDBDateTime(DB), EDIT_EMP = userno, EDIT_TIME = GetDBDateTime(DB) })
                .Where(t => t.ID == snid)
                .ExecuteCommand();
        }

        public int SN_Mrb_Pass_actionNotUpdateCompleted(string snid, string StationName, string userno, OleExec DB)
        {
            //string strSql = $@" update r_sn set current_station='{StationName}',edit_emp=:userno,edit_time=sysdate where id=:snid";
            //OleDbParameter[] paramet = new OleDbParameter[] {
            //    new OleDbParameter(":userno", userno),
            //    new OleDbParameter(":snid", snid) };
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
            //return DB.ORM.Updateable<R_SN>()
            //    .UpdateColumns(t => new R_SN() { CURRENT_STATION = StationName, EDIT_EMP = userno, EDIT_TIME = GetDBDateTime(DB) })
            //    .Where(t => t.ID == snid)
            //    .ExecuteCommand();

            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.ID == snid).First();
            sn.CURRENT_STATION = StationName;
            sn.EDIT_TIME = GetDBDateTime(DB);
            sn.EDIT_EMP = userno;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }

        /// <summary>
        /// update SN Scrap
        /// </summary>
        /// <param name="snid"></param>
        /// <param name="StationName"></param>
        /// <param name="userno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int SN_Mrb_ScrapUpdate(string snid, string StationName, string userno, OleExec DB)
        {
            //string strSql = $@" update r_sn set current_station='{StationName}',scraped_flag='1',scraped_time=sysdate,edit_emp=:userno,edit_time=sysdate where id=:snid";
            //OleDbParameter[] paramet = new OleDbParameter[] {
            //    new OleDbParameter(":userno", userno),
            //    new OleDbParameter(":snid", snid) };
            //int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return res;
            //return DB.ORM.Updateable<R_SN>()
            //    .UpdateColumns(t => new R_SN() { CURRENT_STATION = StationName, SCRAPED_FLAG = "1", SCRAPED_TIME = GetDBDateTime(DB), EDIT_TIME = GetDBDateTime(DB), EDIT_EMP = userno })
            //    .Where(t => t.ID == snid).ExecuteCommand();

            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.ID == snid).First();
            sn.CURRENT_STATION = StationName;
            sn.SCRAPED_FLAG = "1";
            sn.SCRAPED_TIME = GetDBDateTime(DB);
            sn.EDIT_TIME = GetDBDateTime(DB);
            sn.EDIT_EMP = userno;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }

        public int UpdatePackloadingSN(string Sn, string snId, string EmpNo, OleExec DB)
        {
            //DataTable dt = new DataTable();
            //Row_R_SN SnRow = (Row_R_SN)NewRow();
            //string sql = $@"SELECT * FROM R_SN WHERE ID ='{snId}' AND VALID_FLAG = '1' ";
            //dt = DB.ExecSelect(sql).Tables[0];
            //if (dt.Rows.Count == 1)
            //{
            //    try
            //    {
            //        SnRow.loadData(dt.Rows[0]);
            //        SnRow.SN = sn;
            //        SnRow.EDIT_TIME = GetDBDateTime(DB);
            //        SnRow.EDIT_EMP = EmpNo;
            //        SnRow.VALID_FLAG = "0";
            //        sql = SnRow.GetUpdateString(this.DBType);
            //        int res = DB.ExecuteNonQuery(sql, CommandType.Text, null);
            //        return res;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
            //else
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180807164259", new string[] { sn }));
            //}
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN() { SN = sn, EDIT_TIME = GetDBDateTime(DB), EDIT_EMP = EmpNo, VALID_FLAG = "0" })
            //    .Where(t => t.ID == snId && t.VALID_FLAG == "1").ExecuteCommand();
            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.ID == snId).First();
            sn.SN = Sn;
            sn.EDIT_TIME = GetDBDateTime(DB);
            sn.EDIT_EMP = EmpNo;
            sn.VALID_FLAG = "0";
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }

        public List<R_SN> GetCartonListBySN(string Carton, OleExec DB)
        {
            //            List<R_SN> res = new List<R_SN>();
            //            string strSql = $@"SELECT * FROM R_SN WHERE ID IN (
            //SELECT SN_ID FROM R_SN_PACKING WHERE PACK_ID = (
            //SELECT ID FROM R_PACKING WHERE PACK_NO='{Carton}'))";
            //            DataSet ds = DB.ExecSelect(strSql);
            //            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            //            {
            //                Row_R_SN r = (Row_R_SN)NewRow();
            //                r.loadData(VARIABLE);
            //                res.Add(r.GetDataObject());
            //            }
            //            return res;
            return DB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rs, rsp, rp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                .Where((rs, rsp, rp) => rp.PACK_NO == Carton)
                .Select((rs, rsp, rp) => rs).ToList();
        }

        /// <summary>
        /// WZW 2018/08/23 修改R_SN Valid_flag字段
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="Valid"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public int UpdateSNValid(string Sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //int result = 0;
            //if (this.DBType == DB_TYPE_ENUM.Oracle)
            //{
            //    string strSql = $@"UPDATE R_SN SET VALID_FLAG='0' WHERE SN='{Sn}'AND VALID_FLAG='1'";
            //    result = DB.ExecSqlNoReturn(strSql, null);
            //}
            //else
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            //}
            //return result;
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN() { VALID_FLAG = "0" }).Where(t => t.SN == Sn && t.VALID_FLAG == "1").ExecuteCommand();

            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.SN == Sn && t.VALID_FLAG == "1").First();
            sn.VALID_FLAG = "0";
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }

        public List<string> AddToMultiRSn(List<R_SN> SNs, string Line, string StationName, string DeviceName, string Bu, OleExec DB)
        {
            string sql = string.Empty;
            string result = string.Empty;
            T_R_SN Table_R_SN = new T_R_SN(DB, DBType); //add by LLF 2018-03-19
            Row_R_SN row = null;
            List<string> SNIds = new List<string>();
            //bool ModifyFlag = false;
            DateTime DateTime = GetDBDateTime(DB);
            string NextStation = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                NextStation = GetNextStation(SNs.ElementAt(0).ROUTE_ID, SNs.ElementAt(0).CURRENT_STATION, DB);
                foreach (R_SN SN in SNs)
                {
                    if (SN.ID != null && SN.SN.Equals(SN.ID))
                    {
                        //ModifyFlag = true;
                    }
                    //Modify by LLF 2018-03-19,獲取ID,需根據表名生成
                    //SN.ID = GetNewID(Bu, DB); 
                    SN.ID = Table_R_SN.GetNewID(Bu, DB);
                    SN.START_TIME = DateTime;
                    SN.EDIT_TIME = DateTime;
                    SN.NEXT_STATION = NextStation;
                    row = (Row_R_SN)this.ConstructRow(SN);
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    //RecordPassStationDetail(SN.SN, Line, StationName, DeviceName, Bu, DB);
                    RecordPassStationDetail(SN, Line, StationName, DeviceName, Bu, DB);
                    SNIds.Add(SN.ID);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SNIds;
        }
        /// <summary>
        /// 获取SN条码里的周数与目前系统时间的周数的差值
        /// 2020.04.22 Champion 截取字符串沒必要訪問數據庫，資源浪費
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string getSNWeekDiff(string SN, OleExec DB)
        {

            string strSql = $@" select ((to_char(sysdate,'YYYY') - (substr('{SN}',4,2)+1996)) * 53 + to_char(sysdate,'WW') - substr('{SN}',6,2)) from dual ";
            string res = DB.ExecSqlReturn(strSql);
            return res;
        }
        /// <summary>
        /// 获取SN条码里的周数
        /// 2020.04.22 Champion 截取字符串沒必要訪問數據庫，資源浪費
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string getSNWeek(string SN, OleExec DB)
        {

            string strSql = $@" select substr('{SN}',6,2) from dual ";
            string res = DB.ExecSqlReturn(strSql);
            return res;
        }
        /// <summary>
        /// WZW 檢查SN是否在當前工站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="StationName"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool GetSNByExistence(string SN, string StationName, OleExec DB)
        {
            //string StrSQL = $@"SELECT * FROM R_SN WHERE SN={SN} AND NEXT_STATION='{StationName}'";
            //bool SNExistence = false;
            //int Num = DB.ExecSqlNoReturn(StrSQL, null);
            //if (Num > 0)
            //{
            //    SNExistence = true;
            //}
            //return SNExistence;
            return DB.ORM.Queryable<R_SN>().Any(t => t.SN == SN && t.NEXT_STATION == StationName);
        }

        public R_SN GetKPSNByKPSNSKUNO(string KPSN, OleExec DB)
        {
            //return DB.ORM.Queryable<R_SN, R_SN_KP>((p1, p2) => p1.SN == p2.VALUE).Where((p1, p2) => p2.SN == KPSN).ToList().FirstOrDefault();
            return DB.ORM.Queryable<R_SN, R_SN_KP>((p1, p2) => p1.SN == p2.SN && p2.VALUE == KPSN && p1.VALID_FLAG == "1" && p2.VALID_FLAG == 1).Select((p1, p2) => p1).ToList().FirstOrDefault();
        }
        public R_SN GetKPSNBySNSKUNO(string KPSN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN, R_SN_KP>((p1, p2) => p1.SN == p2.SN).Where((p1, p2) => p2.SN == KPSN).Select((p1, p2) => p1).ToList().FirstOrDefault();
        }
        public R_SN GetSNIDBySN(string SNID, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(p1 => p1.ID == SNID).ToList().FirstOrDefault();
        }
        public R_SN GetRowById(string ID, OleExec db)
        {
            return db.ORM.Queryable<R_SN>().Where(t => t.ID == ID).ToList().FirstOrDefault();
        }
        public R_SN GetSNRowBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(p1 => p1.SN == SN && p1.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        public int UpdateR_SN(Row_R_SN SNInFo, string SN, string VALID_FLAG, OleExec db)
        {
            return db.ORM.Updateable<R_SN>(SNInFo).Where(t => t.SN == SN && t.VALID_FLAG == VALID_FLAG).ExecuteCommand();
        }
        public int UpdateRepairSNInFo(R_SN UPdateRepairInFo, string SN, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN>(UPdateRepairInFo).Where(t => t.SN == SN).ExecuteCommand();
        }
        public List<R_SN> GetSNORB9CSNBySN(string SN, OleExec DB)
        {
            string orsn = "B9C" + SN.Substring(SN.Length - 8, 8);
            //return DB.ORM.Queryable<R_SN>().Where(p1 => (p1.SN == SN || p1.SN == ORSN) && p1.VALID_FLAG=="1").ToList();
            return DB.ORM.Queryable<R_SN>().Where(p1 => (p1.SN == SN || p1.SN == orsn) && p1.VALID_FLAG == "1").ToList();
        }
        public R_SN GetSNByKPSN(string SN, OleExec DB)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            return DB.ORM.Queryable<R_SN, R_SN_KP>((p1, p2) => p1.SN == p2.VALUE).Where((p1, p2) => (p2.VALUE == SN || p2.VALUE == ORSN)).Select((p1, p2) => p1).ToList().FirstOrDefault();
        }
        public R_SN GetSNBySN(string SN, OleExec DB)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            return DB.ORM.Queryable<R_SN>().Where(t => (t.SN == SN || t.SN == ORSN)).ToList().FirstOrDefault();
        }
        public R_SN GetSNLocatinBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(p1 => p1.SN == SN && p1.VALID_FLAG == "1" && p1.COMPLETED_FLAG == "1").ToList().FirstOrDefault();
        }
        public List<R_SN> GetBySNControlRun(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(p1 => p1.SN == SN && p1.VALID_FLAG == "1" && (p1.COMPLETED_FLAG == "1" || p1.PACKED_FLAG == "1" || p1.REPAIR_FAILED_FLAG == "1" || p1.STOCK_STATUS == "1" || p1.CURRENT_STATION.Length == 0)).ToList();
        }
        public int UpdateSNShipping(string SN, string shipped, OleExec DB)
        {
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => t.SHIPPED_FLAG == shipped).Where(t => t.SN == SN).ExecuteCommand();
            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").First();
            sn.SHIPPED_FLAG = shipped;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }
        public int UpdateSNStationComStock(string SN, string Current, string Next, string Packed, string Com, string Stock, OleExec DB)
        {
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => t.CURRENT_STATION == Current && t.NEXT_STATION == Next && t.PACKED_FLAG == Packed && t.COMPLETED_FLAG == Com && t.STOCK_STATUS == Stock).Where(t => t.SN == SN).ExecuteCommand();
            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").First();
            sn.CURRENT_STATION = Current;
            sn.NEXT_STATION = Next;
            sn.PACKED_FLAG = Packed;
            sn.COMPLETED_FLAG = Com;
            sn.STOCK_STATUS = Stock;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }
        public int UpdateSN(string SN, string TempSN, OleExec DB)
        {
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => t.SN == TempSN).Where(t => t.SN == SN).ExecuteCommand();
            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").First();
            sn.SN = TempSN;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }
        public void UpdateNextStation(string SN, string StationName, OleExec DB)
        {
            R_SN RSN = LoadSN(SN, DB);
            RSN.NEXT_STATION = StationName;
            //var n = DB.ORM.Updateable(RSN).ExecuteCommand();
            var n1 = DB.ORM.Updateable(RSN).WhereColumns(x => new { x.SN, x.VALID_FLAG }).ExecuteCommand();
        }
        public R_SN GetSNWOBySN(string WO, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(p1 => p1.WORKORDERNO == WO && p1.VALID_FLAG == "1" && p1.SHIPPED_FLAG == "0").ToList().FirstOrDefault();
        }
        public List<R_SN> GetSFCCount(string SKUNO, OleExec DB)
        {
            //string strSql = "SELECT * from R_SN a where a.skuno =:SKUNO";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SKUNO", SKUNO) };
            //List<R_SN> result = new List<R_SN>();
            //DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            //if (res.Rows.Count > 0)
            //{
            //    for (int i = 0; i < res.Rows.Count; i++)
            //    {
            //        Row_R_SN ret = (Row_R_SN)NewRow();
            //        ret.loadData(res.Rows[i]);
            //        result.Add(ret.GetDataObject());
            //    }
            //    return result;
            //}
            //else
            //{
            //    return null;
            //}
            return DB.ORM.Queryable<R_SN>().Where(t => t.SKUNO == SKUNO && t.VALID_FLAG != "0").ToList();

        }
        public int UpdateSNRepair(string SN, string RepairFailedFlag, OleExec DB)
        {
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => t.REPAIR_FAILED_FLAG == RepairFailedFlag).Where(t => t.SN == SN).ExecuteCommand();
            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").First();
            sn.REPAIR_FAILED_FLAG = RepairFailedFlag;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }
        public R_SN GetBySNRepairSN(string SN, string REPAIR_FAILED_FLAG, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN>().Where(p1 => p1.SN == SN && p1.REPAIR_FAILED_FLAG == REPAIR_FAILED_FLAG && p1.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
        //public List<R_SN> GetSFCCount(string SKUNO, OleExec DB)
        //{
        //    return DB.ORM.Queryable<R_SN>().Where(p1 => p1.SKUNO== SKUNO&& p1.VALID_FLAG == "1" && (p1.COMPLETED_FLAG == "1" || p1.PACKED_FLAG == "1" || p1.REPAIR_FAILED_FLAG == "1" || p1.STOCK_STATUS == "1" || p1.CURRENT_STATION.Length == 0)).ToList();
        //}

        /// <returns></returns>
        public int ReplaceSnValid(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //int result = 0;
            //string strSql = string.Empty;

            //if (this.DBType == DB_TYPE_ENUM.Oracle)
            //{
            //    strSql = $@"UPDATE R_SN R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}' and valid_flag=1";
            //    result = DB.ExecSqlNoReturn(strSql, null);
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}
            //return result;
            //return DB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN() { SN = NewSn }).Where(t => t.SN == OldSn && t.VALID_FLAG == "1").ExecuteCommand();
            var sn = DB.ORM.Queryable<R_SN>().Where(t => t.SN == OldSn && t.VALID_FLAG == "1").First();
            sn.SN = NewSn;
            return DB.ORM.Updateable(sn).ExecuteCommand();
        }
        public List<R_SN> GetSNIsItBound(string SN, OleExec DB)
        {
            //List<R_SN> res = new List<R_SN>();
            //string strSql = $@"SELECT * FROM R_SN WHERE SN='{SN}' AND (PACKED_FLAG='1' OR COMPLETED_FLAG='1' OR STOCK_STATUS='1' OR SHIPPED_FLAG='1')";
            //DataSet ds = DB.ExecSelect(strSql);
            //foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            //{
            //    Row_R_SN r = (Row_R_SN)NewRow();
            //    r.loadData(VARIABLE);
            //    res.Add(r.GetDataObject());
            //}
            //return res;
            return DB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && (t.PACKED_FLAG == "1" || t.COMPLETED_FLAG == "1" || t.STOCK_STATUS == "1" || t.SHIPPED_FLAG == "1")).ToList();
        }

        public bool HasSN(string SN, OleExec db)
        {
            //bool res = false;
            //string strsql = $@"select * from r_sn where  SN='{SN}' and 
            //                stock_status=1 and shipped_flag=1";
            //DataTable Dt = db.ExecSelect(strsql).Tables[0];
            //if (Dt.Rows.Count > 0)
            //{
            //    res = true;
            //}
            //return res;
            return db.ORM.Queryable<R_SN>().Any(t => t.SN == SN && t.STOCK_STATUS == "1" && t.SHIPPED_FLAG == "1");
        }

        public int UpdateSNCurrentNextStation(string SN, string CurrentStation, string NextStation, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN { CURRENT_STATION = CurrentStation, NEXT_STATION = NextStation }).Where(t => t.SN == SN && t.VALID_FLAG == "1").ExecuteCommand();
        }
        public int UpdateSNValidFlag(string SN, string ValidFlag, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN>().UpdateColumns(t => t.VALID_FLAG == ValidFlag).Where(t => t.SN == SN).ExecuteCommand();
        }

        /// <summary>
        /// 棧板費領出貨記錄(與DN無關)
        /// </summary>
        public void PalletExpenseShipOutRecord(string ExpenseNo, string PalletNo, string EmpNo, string Line, string Bu, string stationName, OleExec SFCDB)
        {
            T_R_PACKING TRP = new T_R_PACKING(SFCDB, DB_TYPE_ENUM.Oracle);
            var rSnList = new List<R_SN>();
            TRP.GetSnListByPackNo(PalletNo, ref rSnList, SFCDB);
            foreach (var rSn in rSnList)
            {
                SFCDB.ORM.Insertable(new R_SHIP_DETAIL()
                {
                    ID = rSn.ID,
                    SN = rSn.SN,
                    SKUNO = rSn.SKUNO,
                    DN_NO = ExpenseNo,
                    DN_LINE = "",
                    SHIPDATE = DateTime.Now,
                    CREATEBY = EmpNo,
                    SHIPORDERID = ""
                }).ExecuteCommand();
            }

            LotsPassStation(rSnList, Line, stationName, stationName, Bu, "PASS", EmpNo, SFCDB);
        }

        /// <summary>
        /// VNDCN Keep Link Relation To Rework
        /// </summary>
        /// <returns></returns>
        public bool CheckReworkUpdateShipFlagControl(string sn, OleExec sfcdb)
        {
            string sql = $@"
            select a.value series, a.extval csn, b.value sn
              from r_function_control    a,
                   r_function_control_ex b,
                   c_series              c,
                   c_sku                 d,
                   r_sn                  e,
                   r_sn_kp               f
             where a.id = b.detail_id
               and upper(a.value) = upper(c.series_name)
               and c.id = d.c_series_id
               and d.skuno = e.skuno
               and e.sn = f.value
               and b.value = f.sn
               and a.functionname = 'REWORK_UPDATE_SHIPFLAG'
               and a.functiontype = 'NOSYSTEM'
               and a.controlflag = 'Y'
               and e.sn = '{sn}'
               and e.valid_flag = 1
               and f.valid_flag = 1";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            bool controlRs = dt.Rows.Count > 0;

            return controlRs;
        }

        public List<R_SN> GetListSNByParentPackId(string parentPackId, OleExec DB)
        {
            List<R_SN> packingList = new List<R_SN>();
            Row_R_SN rowPacking;
            string strSql = $@"select * from r_packing a,r_sn_packing b,r_sn c where a.parent_pack_id='{parentPackId}' and a.id = b.pack_id and b.sn_id = c.id ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                rowPacking = (Row_R_SN)this.NewRow();
                rowPacking.loadData(row);
                packingList.Add(rowPacking.GetDataObject());
            }
            return packingList;
        }
    }
    public class Row_R_SN : DataObjectBase
    {
        public Row_R_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_SN GetDataObject()
        {
            R_SN DataObject = new R_SN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.STARTED_FLAG = this.STARTED_FLAG;
            DataObject.START_TIME = this.START_TIME;
            DataObject.PACKED_FLAG = this.PACKED_FLAG;
            DataObject.PACKDATE = this.PACKDATE;
            DataObject.COMPLETED_FLAG = this.COMPLETED_FLAG;
            DataObject.COMPLETED_TIME = this.COMPLETED_TIME;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.REPAIR_FAILED_FLAG = this.REPAIR_FAILED_FLAG;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.KP_LIST_ID = this.KP_LIST_ID;
            DataObject.PO_NO = this.PO_NO;
            DataObject.CUST_ORDER_NO = this.CUST_ORDER_NO;
            DataObject.CUST_PN = this.CUST_PN;
            DataObject.BOXSN = this.BOXSN;
            DataObject.SCRAPED_FLAG = this.SCRAPED_FLAG;
            DataObject.SCRAPED_TIME = this.SCRAPED_TIME;
            DataObject.PRODUCT_STATUS = this.PRODUCT_STATUS;
            DataObject.REWORK_COUNT = this.REWORK_COUNT;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.STOCK_STATUS = this.STOCK_STATUS;
            DataObject.STOCK_IN_TIME = this.STOCK_IN_TIME;
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
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
            }
        }
        public string STARTED_FLAG
        {
            get
            {
                return (string)this["STARTED_FLAG"];
            }
            set
            {
                this["STARTED_FLAG"] = value;
            }
        }
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string PACKED_FLAG
        {
            get
            {
                return (string)this["PACKED_FLAG"];
            }
            set
            {
                this["PACKED_FLAG"] = value;
            }
        }
        public DateTime? PACKDATE
        {
            get
            {
                return (DateTime?)this["PACKDATE"];
            }
            set
            {
                this["PACKDATE"] = value;
            }
        }
        public string COMPLETED_FLAG
        {
            get
            {
                return (string)this["COMPLETED_FLAG"];
            }
            set
            {
                this["COMPLETED_FLAG"] = value;
            }
        }
        public DateTime? COMPLETED_TIME
        {
            get
            {
                return (DateTime?)this["COMPLETED_TIME"];
            }
            set
            {
                this["COMPLETED_TIME"] = value;
            }
        }
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string REPAIR_FAILED_FLAG
        {
            get
            {
                return (string)this["REPAIR_FAILED_FLAG"];
            }
            set
            {
                this["REPAIR_FAILED_FLAG"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
            }
        }
        public string KP_LIST_ID
        {
            get
            {
                return (string)this["KP_LIST_ID"];
            }
            set
            {
                this["KP_LIST_ID"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string CUST_ORDER_NO
        {
            get
            {
                return (string)this["CUST_ORDER_NO"];
            }
            set
            {
                this["CUST_ORDER_NO"] = value;
            }
        }
        public string CUST_PN
        {
            get
            {
                return (string)this["CUST_PN"];
            }
            set
            {
                this["CUST_PN"] = value;
            }
        }
        public string BOXSN
        {
            get
            {
                return (string)this["BOXSN"];
            }
            set
            {
                this["BOXSN"] = value;
            }
        }
        public string SCRAPED_FLAG
        {
            get
            {
                return (string)this["SCRAPED_FLAG"];
            }
            set
            {
                this["SCRAPED_FLAG"] = value;
            }
        }
        public DateTime? SCRAPED_TIME
        {
            get
            {
                return (DateTime?)this["SCRAPED_TIME"];
            }
            set
            {
                this["SCRAPED_TIME"] = value;
            }
        }
        public string PRODUCT_STATUS
        {
            get
            {
                return (string)this["PRODUCT_STATUS"];
            }
            set
            {
                this["PRODUCT_STATUS"] = value;
            }
        }
        public double? REWORK_COUNT
        {
            get
            {
                return (double?)this["REWORK_COUNT"];
            }
            set
            {
                this["REWORK_COUNT"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string STOCK_STATUS
        {
            get
            {
                return (string)this["STOCK_STATUS"];
            }
            set
            {
                this["STOCK_STATUS"] = value;
            }
        }

        public DateTime? STOCK_IN_TIME
        {
            get
            {
                return (DateTime?)this["STOCK_IN_TIME"];
            }
            set
            {
                this["STOCK_IN_TIME"] = value;
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
    public class R_SN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string PLANT { get; set; }
        public string ROUTE_ID { get; set; }
        public string STARTED_FLAG { get; set; }
        public DateTime? START_TIME { get; set; }
        public string PACKED_FLAG { get; set; }
        public DateTime? PACKDATE { get; set; }
        public string COMPLETED_FLAG { get; set; }
        public DateTime? COMPLETED_TIME { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string REPAIR_FAILED_FLAG { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string KP_LIST_ID { get; set; }
        public string PO_NO { get; set; }
        public string CUST_ORDER_NO { get; set; }
        public string CUST_PN { get; set; }
        public string BOXSN { get; set; }
        public string SCRAPED_FLAG { get; set; }
        public DateTime? SCRAPED_TIME { get; set; }
        public string PRODUCT_STATUS { get; set; }
        public double? REWORK_COUNT { get; set; }
        public string VALID_FLAG { get; set; }
        public string STOCK_STATUS { get; set; }
        public DateTime? STOCK_IN_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }

        public override string ToString()
        {
            return SN;
        }
    }

    //public class GetSNByWorkorderNo
    //{
    //    public string ID;
    //    public string SN;
    //    public string SKUNO;
    //    public string WORKORDERNO;
    //    public string PLANT;
    //    public string ROUTE_ID;
    //    public string STARTED_FLAG;
    //    public string START_TIME;
    //    public string PACKED_FLAG;
    //    public string PACKDATE;
    //    public string COMPLETED_FLAG;
    //    public string COMPLETED_TIME;
    //    public string SHIPPED_FLAG;
    //    public string SHIPDATE;
    //    public string REPAIR_FAILED_FLAG;
    //    public string CURRENT_STATION;
    //    public string NEXT_STATION;
    //    public string KP_LIST_ID;
    //    public string PO_NO;
    //    public string CUST_ORDER_NO;
    //    public string CUST_PN;
    //    public string BOXSN;
    //    public string SCRAPED_FLAG;
    //    public string SCRAPED_TIME;
    //    public string PRODUCT_STATUS;
    //    public string REWORK_COUNT;
    //    public string VALID_FLAG;
    //    public string EDIT_EMP;
    //    public DateTime? EDIT_TIME;
    //}


    public class T_R_YIELD_RATE_DETAIL : DataObjectTable
    {
        public T_R_YIELD_RATE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_YIELD_RATE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_YIELD_RATE_DETAIL);
            TableName = "R_YIELD_RATE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_YIELD_RATE_DETAIL : DataObjectBase
    {
        public Row_R_YIELD_RATE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_YIELD_RATE_DETAIL GetDataObject()
        {
            R_YIELD_RATE_DETAIL DataObject = new R_YIELD_RATE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.WORK_DATE = this.WORK_DATE;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.PRODUCTION_LINE = this.PRODUCTION_LINE;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.TOTAL_FRESH_BUILD_QTY = this.TOTAL_FRESH_BUILD_QTY;
            DataObject.TOTAL_FRESH_PASS_QTY = this.TOTAL_FRESH_PASS_QTY;
            DataObject.TOTAL_FRESH_FAIL_QTY = this.TOTAL_FRESH_FAIL_QTY;
            DataObject.TOTAL_REWORK_BUILD_QTY = this.TOTAL_REWORK_BUILD_QTY;
            DataObject.TOTAL_REWORK_PASS_QTY = this.TOTAL_REWORK_PASS_QTY;
            DataObject.TOTAL_REWORK_FAIL_QTY = this.TOTAL_REWORK_FAIL_QTY;
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
        public DateTime WORK_DATE
        {
            get
            {
                return (DateTime)this["WORK_DATE"];
            }
            set
            {
                this["WORK_DATE"] = value;
            }
        }
        public string WORK_TIME
        {
            get
            {
                return (string)this["WORK_TIME"];
            }
            set
            {
                this["WORK_TIME"] = value;
            }
        }
        public string PRODUCTION_LINE
        {
            get
            {
                return (string)this["PRODUCTION_LINE"];
            }
            set
            {
                this["PRODUCTION_LINE"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
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
        public string SKU_NAME
        {
            get
            {
                return (string)this["SKU_NAME"];
            }
            set
            {
                this["SKU_NAME"] = value;
            }
        }
        public string SKU_SERIES
        {
            get
            {
                return (string)this["SKU_SERIES"];
            }
            set
            {
                this["SKU_SERIES"] = value;
            }
        }
        public double? TOTAL_FRESH_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_PASS_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_FAIL_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_PASS_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_FAIL_QTY"] = value;
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
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_YIELD_RATE_DETAIL
    {
        public string ID { get; set; }
        public DateTime WORK_DATE { get; set; }
        public string WORK_TIME { get; set; }
        public string PRODUCTION_LINE { get; set; }
        public string CLASS_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_SERIES { get; set; }
        public double? TOTAL_FRESH_BUILD_QTY { get; set; }
        public double? TOTAL_FRESH_PASS_QTY { get; set; }
        public double? TOTAL_FRESH_FAIL_QTY { get; set; }
        public double? TOTAL_REWORK_BUILD_QTY { get; set; }
        public double? TOTAL_REWORK_PASS_QTY { get; set; }
        public double? TOTAL_REWORK_FAIL_QTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime EDIT_TIME { get; set; }
    }

    public class T_R_UPH_DETAIL : DataObjectTable
    {
        public T_R_UPH_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_UPH_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_UPH_DETAIL);
            TableName = "R_UPH_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_UPH_DETAIL : DataObjectBase
    {
        public Row_R_UPH_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_UPH_DETAIL GetDataObject()
        {
            R_UPH_DETAIL DataObject = new R_UPH_DETAIL();
            DataObject.ID = this.ID;
            DataObject.WORK_DATE = this.WORK_DATE;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.PRODUCTION_LINE = this.PRODUCTION_LINE;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.TOTAL_FRESH_BUILD_QTY = this.TOTAL_FRESH_BUILD_QTY;
            DataObject.TOTAL_FRESH_PASS_QTY = this.TOTAL_FRESH_PASS_QTY;
            DataObject.TOTAL_FRESH_FAIL_QTY = this.TOTAL_FRESH_FAIL_QTY;
            DataObject.TOTAL_REWORK_BUILD_QTY = this.TOTAL_REWORK_BUILD_QTY;
            DataObject.TOTAL_REWORK_PASS_QTY = this.TOTAL_REWORK_PASS_QTY;
            DataObject.TOTAL_REWORK_FAIL_QTY = this.TOTAL_REWORK_FAIL_QTY;
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
        public DateTime WORK_DATE
        {
            get
            {
                return (DateTime)this["WORK_DATE"];
            }
            set
            {
                this["WORK_DATE"] = value;
            }
        }
        public string WORK_TIME
        {
            get
            {
                return (string)this["WORK_TIME"];
            }
            set
            {
                this["WORK_TIME"] = value;
            }
        }
        public string PRODUCTION_LINE
        {
            get
            {
                return (string)this["PRODUCTION_LINE"];
            }
            set
            {
                this["PRODUCTION_LINE"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
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
        public string SKU_NAME
        {
            get
            {
                return (string)this["SKU_NAME"];
            }
            set
            {
                this["SKU_NAME"] = value;
            }
        }
        public string SKU_SERIES
        {
            get
            {
                return (string)this["SKU_SERIES"];
            }
            set
            {
                this["SKU_SERIES"] = value;
            }
        }
        public double? TOTAL_FRESH_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_PASS_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_FAIL_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_PASS_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_FAIL_QTY"] = value;
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
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_UPH_DETAIL
    {
        public string ID { get; set; }
        public DateTime WORK_DATE { get; set; }
        public string WORK_TIME { get; set; }
        public string PRODUCTION_LINE { get; set; }
        public string CLASS_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_SERIES { get; set; }
        public double? TOTAL_FRESH_BUILD_QTY { get; set; }
        public double? TOTAL_FRESH_PASS_QTY { get; set; }
        public double? TOTAL_FRESH_FAIL_QTY { get; set; }
        public double? TOTAL_REWORK_BUILD_QTY { get; set; }
        public double? TOTAL_REWORK_PASS_QTY { get; set; }
        public double? TOTAL_REWORK_FAIL_QTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime EDIT_TIME { get; set; }
    }

    public class T_R_REPRINT_RECORD : DataObjectTable
    {
        public T_R_REPRINT_RECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPRINT_RECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_UPH_DETAIL);
            TableName = "R_REPRINT_RECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int AddReprintRecord(string Skuno, string Sn, string StationName, string EditEmp, OleExec DB)
        {
            R_REPRINT_RECORD record = new R_REPRINT_RECORD();
            record.SKUNO = Skuno;
            record.SN = Sn;
            record.STATION_NAME = StationName;
            record.EDIT_EMP = EditEmp;
            record.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable<R_REPRINT_RECORD>(record).ExecuteCommand();
        }
    }

    public class R_REPRINT_RECORD
    {
        public string SKUNO { get; set; }
        public string SN { get; set; }
        public string STATION_NAME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }

    public class GET_SeriesCustomer_BySn
    {
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string SERIES_NAME { get; set; }
        public string SERIES_DESCRIPTION { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_DESCRIPTION { get; set; }
    }

    public enum ENUM_R_SN
    {
        /// <summary>
        /// 记录无效
        /// </summary>
        [EnumValue("0")]
        VALID_FLAG_FALSE,
        /// <summary>
        /// 记录有效
        /// </summary>
        [EnumValue("1")]
        VALID_FLAG_TRUE
    }
}
