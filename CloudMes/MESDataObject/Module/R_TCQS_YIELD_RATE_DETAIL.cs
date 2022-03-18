using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TCQS_YIELD_RATE_DETAIL : DataObjectTable
    {
        public T_R_TCQS_YIELD_RATE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TCQS_YIELD_RATE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TCQS_YIELD_RATE_DETAIL);
            TableName = "R_TCQS_YIELD_RATE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 獲取當前數據庫時間所屬的班別
        /// </summary>
        /// <param name="DateTime"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetWorkClass(string DT, OleExec DB)
        {
            string TimeFormat = "HH24:MI:SS";
            DataTable dt = new DataTable();
            string sql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM C_WORK_CLASS WHERE TO_DATE('{DT}','{TimeFormat}')
                            BETWEEN TO_DATE(START_TIME,'{TimeFormat}') AND TO_DATE(END_TIME,'{TimeFormat}')";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["NAME"] != null)
                    {
                        return dt.Rows[0]["NAME"].ToString();
                    }
                    else
                    {
                        throw new Exception("班別的名字不能為空");
                    }
                }
                else
                {
                    throw new Exception("沒有配置班別");
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        //通過TCQS邏輯，判斷本次測試是否合法，并記錄良率統計數據到RTCQS對象中
        public R_TCQS_YIELD_RATE_DETAIL CheckTCQSTest(string SerialNo, string SkuNo, string WorkorderNo, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, DateTime DT, OleExec DB)
        {
            DataTable dataTable = new DataTable();
            string sql = string.Empty;
            string RepassModel = "", RetestModel = "", Device_Name = "";
            int PassLimit = 1, RetestLimit = 1;//所有未設定的機種，默認以該參數統計TCQS
            int PassCount = 0, RetestCount = 0;//臨時表記錄的Pass/Retest測試次數
            int PassCountFlag = 0;     //標識是否有過站記錄，在Total統計時以確定Fresh/Rework狀態
            int RetestCountFlag = 0,   //標識是否有測試記錄,在TCQS統計時以確定Fresh/Rework狀態
                  RepairFlag = 0;      //標識是否做過站處理
            T_R_WO_BASE WoTable = null;
            Row_R_WO_BASE WoRow = null;
            WoTable = new T_R_WO_BASE(DB, this.DBType);
            WoRow = WoTable.GetWo(WorkorderNo, DB);
            //初使化R_TCQS_YIELD_RATE_DETAIL
            R_TCQS_YIELD_RATE_DETAIL RTYRD = new R_TCQS_YIELD_RATE_DETAIL()
            {
                WORK_DATE = DT.ToString("yyyy-MM-dd"),
                WORK_TIME = DT.ToString("HH"),
                PRODUCTION_LINE = Line,
                CLASS_NAME = GetWorkClass(DT.ToString("HH:mm:ss"), DB),
                STATION_NAME = StationName,
                DEVICE_NAME = DeviceName,
                WORKORDERNO = WorkorderNo,
                SKUNO = SkuNo,
                SKU_NAME = WoRow.SKU_NAME,
                SKU_SERIES = WoRow.SKU_SERIES,
                TOTAL_REWORK_BUILD_QTY = 0,
                TOTAL_REWORK_PASS_QTY = 0,
                TOTAL_REWORK_FAIL_QTY = 0,
                TOTAL_FRESH_BUILD_QTY = 0,
                TOTAL_FRESH_PASS_QTY = 0,
                TOTAL_FRESH_FAIL_QTY = 0,
                TCQS_REWORK_BUILD_QTY = 0,
                TCQS_REWORK_PASS_QTY = 0,
                TCQS_REWORK_FAIL_QTY = 0,
                TCQS_FRESH_BUILD_QTY = 0,
                TCQS_FRESH_PASS_QTY = 0,
                TCQS_FRESH_FAIL_QTY = 0,
                EDIT_EMP = EmpNo,
                EDIT_TIME = DT
            };


            sql = $@"Select PASS_LIMIT,RETEST_LIMIT,REPASS_MODEL,RETEST_MODEL from C_MODEL_ATE_SET_T 
                   Where MODEL_NAME='{SkuNo}' and GROUP_NAME='{StationName}' ";
            dataTable = DB.ExecSelect(sql).Tables[0];
            //如果有設定則取設定值，否則取默認值
            if (dataTable.Rows.Count > 0)
            {
                PassLimit = Int32.Parse(dataTable.Rows[0]["PASS_LIMIT"].ToString());
                RetestLimit = Int32.Parse(dataTable.Rows[0]["RETEST_LIMIT"].ToString());
                RepassModel = dataTable.Rows[0]["REPASS_MODEL"].ToString();
                RetestModel = dataTable.Rows[0]["RETEST_MODEL"].ToString();
            }
            //獲取臨時表的測試記錄中Pass次數/Fail次數/測試機臺名稱
            sql = $@"select PASS_COUNT,RETEST_COUNT,DEVICE_NAME from  R_TMP_ATEDATA_T where station_name = '{StationName}' and sn = '{SerialNo}'";
            dataTable = null;
            dataTable = DB.ExecSelect(sql).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                PassCount = Int32.Parse(dataTable.Rows[0]["PASS_COUNT"].ToString());
                RetestCount = Int32.Parse(dataTable.Rows[0]["RETEST_COUNT"].ToString());
                Device_Name = dataTable.Rows[0]["DEVICE_NAME"].ToString();

            }
            //獲取產品狀態Fresh or Rework
            sql = $@"SELECT RESULT_FLAG FROM R_SN_DETAIL WHERE SN = '{SerialNo}' AND STATION_NAME = '{StationName}'";
            dataTable = null;
            dataTable = DB.ExecSelect(sql).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                //有記錄，代表有測試過，因此TCQS為Rework
                RetestCountFlag = 1;
                foreach (DataRow dr in dataTable.Rows)
                {
                    //有過站記錄,因此總良率為Rework
                    if (dr["RESULT_FLAG"].ToString() == "0" || dr["RESULT_FLAG"].ToString() == "1")
                    {
                        PassCountFlag = 1;
                        break;
                    }
                }
            }

            //統計良率統計數據
            if (PassOrFail == "PASS")//測試結果為PASS
            {
                //如果有Fail記錄
                if (RetestCount > 0)
                {
                    //管控測試PASS次數>1
                    if (PassLimit > 1)
                    {
                        //如果有測試PASS的記錄,則需要判斷是否需切換工站,是否滿足測試次數
                        if (PassCount > 0)
                        {
                            //必須切換測試站
                            if (RepassModel == "0")
                            {
                                if (Device_Name == DeviceName)
                                {
                                    //必須換機臺測試
                                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185619");
                                    throw new MESReturnMessage(errMsg);
                                }
                            }
                            else if (RepassModel == "1")  //不允許換機臺測試
                            {
                                if (Device_Name != DeviceName)
                                {
                                    //不允許換機臺測試,必須使用Device_Name機臺測試
                                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185620", new string[] { Device_Name });
                                    throw new MESReturnMessage(errMsg);
                                }
                            }
                            //判斷是否滿足PASS測試次數
                            if (PassCount != PassLimit - 1)//不滿足測試次數
                            {
                                RepairFlag = 1;   //不過站,不清臨時表,過站記錄標識為非過站
                            }
                            RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                            RTYRD.TCQS_REWORK_PASS_QTY = 1;

                        }
                        else  //如果沒有測試PASS的記錄,則直接增加測試PASS次數
                        {
                            RepairFlag = 1;   //不過站,不清臨時表,過站記錄標識為非過站
                            RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                            RTYRD.TCQS_REWORK_PASS_QTY = 1;
                        }
                    }
                    else  //管控測試PASS次數=1
                    {
                        RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                        RTYRD.TCQS_REWORK_PASS_QTY = 1;
                    }
                }
                else  //如果臨時表沒有Fail記錄,則需要判斷是否有測試記錄，以確定是Fresh還是Rework
                {
                    //如果有測試記錄，則為Rework
                    if (RetestCountFlag == 1)
                    {
                        RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                        RTYRD.TCQS_REWORK_PASS_QTY = 1;
                    }
                    else//否則為Fresh
                    {
                        RTYRD.TCQS_FRESH_BUILD_QTY = 1;
                        RTYRD.TCQS_FRESH_PASS_QTY = 1;
                    }
                }
                //如果做過站處理,則還需要統計總良率
                if (RepairFlag == 0)
                {
                    if (PassCountFlag == 1)//有過站記錄
                    {
                        RTYRD.TOTAL_REWORK_BUILD_QTY = 1;
                        RTYRD.TOTAL_REWORK_PASS_QTY = 1;
                    }
                    else//沒有過站記錄
                    {
                        RTYRD.TOTAL_FRESH_BUILD_QTY = 1;
                        RTYRD.TOTAL_FRESH_PASS_QTY = 1;
                    }
                }
            }
            else//測試結果為Fail
            {
                //管控測試重測次數>1
                if (RetestLimit > 1)
                {
                    //有Fail記錄
                    if (RetestCount > 0)
                    {

                        //必須切換測試站
                        if (RepassModel == "0")
                        {
                            if (Device_Name == DeviceName)
                            {
                                //必須換機臺測試
                                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185619");
                                throw new MESReturnMessage(errMsg);
                            }
                        }
                        else if (RepassModel == "1")  //不允許換機臺測試
                        {
                            if (Device_Name != DeviceName)
                            {
                                //不允許換機臺測試,必須使用Device_Name機臺測試
                                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185620", new string[] { Device_Name });
                                throw new MESReturnMessage(errMsg);
                            }
                        }
                        //判斷是否滿足重測次數
                        if (RetestCount != RetestLimit - 1)//不滿足測試次數
                        {
                            RepairFlag = 1;   //不過站,不清臨時表,過站記錄標識為非過站
                        }
                        RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                        RTYRD.TCQS_REWORK_FAIL_QTY = 1;

                    }
                    else  //允許重測次數>1，且臨時表沒有Fail記錄
                    {
                        RepairFlag = 1;   //不過站,不清臨時表,過站記錄標識為非過站
                        //有測試記錄
                        if (RetestCountFlag == 1)
                        {
                            RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                            RTYRD.TCQS_REWORK_FAIL_QTY = 1;
                        }
                        else//沒有測試記錄
                        {
                            RTYRD.TCQS_FRESH_BUILD_QTY = 1;
                            RTYRD.TCQS_FRESH_FAIL_QTY = 1;
                        }
                    }
                }
                else  //管控測試重測次數=1,則需要判斷是否有測試記錄，以確定是Fresh還是Rework
                {
                    //如果有測試記錄，則為Rework
                    if (RetestCountFlag == 1)
                    {
                        RTYRD.TCQS_REWORK_BUILD_QTY = 1;
                        RTYRD.TCQS_REWORK_FAIL_QTY = 1;
                    }
                    else//否則為Fresh
                    {
                        RTYRD.TCQS_FRESH_BUILD_QTY = 1;
                        RTYRD.TCQS_FRESH_FAIL_QTY = 1;
                    }
                }
                //如果做過站處理,則還需要統計總良率
                if (RepairFlag == 0)
                {
                    if (PassCountFlag == 1)//有過站記錄
                    {
                        RTYRD.TOTAL_REWORK_BUILD_QTY = 1;
                        RTYRD.TOTAL_REWORK_FAIL_QTY = 1;
                    }
                    else//沒有過站記錄
                    {
                        RTYRD.TOTAL_FRESH_BUILD_QTY = 1;
                        RTYRD.TOTAL_FRESH_FAIL_QTY = 1;
                    }
                }
            }
            return RTYRD;
        }

        /// <summary>
        /// 記錄TCQS良率&R_TMP_ATEDATA_T&R_SN_Detail過站記錄
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
        public string RecordTCQSYieldRate(R_TCQS_YIELD_RATE_DETAIL RTYRD, string SerialNo, string Bu, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            T_R_TCQS_YIELD_RATE_DETAIL TRTYRD = null;
            Row_R_TCQS_YIELD_RATE_DETAIL RRTYRD = null;
            T_R_TMP_ATEDATA_T TRTAT = null;
            Row_R_TMP_ATEDATA_T RRTAT = null;
            T_R_SN_DETAIL TRSD = null;
            Row_R_SN_DETAIL RRSD = null;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //更新R_TCQS_YIELD_RATE_DETAIL
                TRTYRD = new T_R_TCQS_YIELD_RATE_DETAIL(DB, this.DBType);
                RRTYRD = (Row_R_TCQS_YIELD_RATE_DETAIL)TRTYRD.NewRow();
                sql = $@"Select * From R_TCQS_YIELD_RATE_DETAIL 
                       Where WORK_DATE='{RTYRD.WORK_DATE}' and Work_Time='{RTYRD.WORK_TIME}' and 
                             PRODUCTION_LINE='{RTYRD.PRODUCTION_LINE}' and CLASS_NAME='{RTYRD.CLASS_NAME}' and 
                             STATION_NAME='{RTYRD.STATION_NAME}' and 
                             DEVICE_NAME='{RTYRD.DEVICE_NAME}' and WORKORDERNO='{RTYRD.WORKORDERNO}' ";
                dt = DB.ExecSelect(sql).Tables[0];
                //如果記錄已經存在
                if (dt.Rows.Count > 0)
                {
                    RRTYRD.loadData(dt.Rows[0]);
                    RRTYRD.TOTAL_FRESH_BUILD_QTY += RTYRD.TOTAL_FRESH_BUILD_QTY;
                    RRTYRD.TOTAL_FRESH_PASS_QTY += RTYRD.TOTAL_FRESH_PASS_QTY;
                    RRTYRD.TOTAL_FRESH_FAIL_QTY += RTYRD.TOTAL_FRESH_FAIL_QTY;
                    RRTYRD.TOTAL_REWORK_BUILD_QTY += RTYRD.TOTAL_REWORK_BUILD_QTY;
                    RRTYRD.TOTAL_REWORK_PASS_QTY += RTYRD.TOTAL_REWORK_PASS_QTY;
                    RRTYRD.TOTAL_REWORK_FAIL_QTY += RTYRD.TOTAL_REWORK_FAIL_QTY;
                    RRTYRD.TCQS_FRESH_BUILD_QTY += RTYRD.TCQS_FRESH_BUILD_QTY;
                    RRTYRD.TCQS_FRESH_PASS_QTY += RTYRD.TCQS_FRESH_PASS_QTY;
                    RRTYRD.TCQS_FRESH_FAIL_QTY += RTYRD.TCQS_FRESH_FAIL_QTY;
                    RRTYRD.TCQS_REWORK_BUILD_QTY += RTYRD.TCQS_REWORK_BUILD_QTY;
                    RRTYRD.TCQS_REWORK_PASS_QTY += RTYRD.TCQS_REWORK_PASS_QTY;
                    RRTYRD.TCQS_REWORK_FAIL_QTY += RTYRD.TCQS_REWORK_FAIL_QTY;
                    RRTYRD.EDIT_EMP = RTYRD.EDIT_EMP;
                    RRTYRD.EDIT_TIME = RTYRD.EDIT_TIME;
                    sql = RRTYRD.GetUpdateString(this.DBType);
                }
                else//記錄不存在,則Insert
                {
                    RRTYRD.ID = TRTYRD.GetNewID(Bu, DB);
                    RRTYRD.WORK_DATE = RTYRD.WORK_DATE;
                    RRTYRD.WORK_TIME = RTYRD.WORK_TIME;
                    RRTYRD.PRODUCTION_LINE = RTYRD.PRODUCTION_LINE;
                    RRTYRD.CLASS_NAME = RTYRD.CLASS_NAME;
                    RRTYRD.STATION_NAME = RTYRD.STATION_NAME;
                    RRTYRD.DEVICE_NAME = RTYRD.DEVICE_NAME;
                    RRTYRD.WORKORDERNO = RTYRD.WORKORDERNO;
                    RRTYRD.SKUNO = RTYRD.SKUNO;
                    RRTYRD.SKU_NAME = RTYRD.SKU_NAME;
                    RRTYRD.SKU_SERIES = RTYRD.SKU_SERIES;
                    RRTYRD.TOTAL_FRESH_BUILD_QTY = RTYRD.TOTAL_FRESH_BUILD_QTY;
                    RRTYRD.TOTAL_FRESH_PASS_QTY = RTYRD.TOTAL_FRESH_PASS_QTY;
                    RRTYRD.TOTAL_FRESH_FAIL_QTY = RTYRD.TOTAL_FRESH_FAIL_QTY;
                    RRTYRD.TOTAL_REWORK_BUILD_QTY = RTYRD.TOTAL_REWORK_BUILD_QTY;
                    RRTYRD.TOTAL_REWORK_PASS_QTY = RTYRD.TOTAL_REWORK_PASS_QTY;
                    RRTYRD.TOTAL_REWORK_FAIL_QTY = RTYRD.TOTAL_REWORK_FAIL_QTY;
                    RRTYRD.TCQS_FRESH_BUILD_QTY = RTYRD.TCQS_FRESH_BUILD_QTY;
                    RRTYRD.TCQS_FRESH_PASS_QTY = RTYRD.TCQS_FRESH_PASS_QTY;
                    RRTYRD.TCQS_FRESH_FAIL_QTY = RTYRD.TCQS_FRESH_FAIL_QTY;
                    RRTYRD.TCQS_REWORK_BUILD_QTY = RTYRD.TCQS_REWORK_BUILD_QTY;
                    RRTYRD.TCQS_REWORK_PASS_QTY = RTYRD.TCQS_REWORK_PASS_QTY;
                    RRTYRD.TCQS_REWORK_FAIL_QTY = RTYRD.TCQS_REWORK_FAIL_QTY;
                    RRTYRD.EDIT_EMP = RTYRD.EDIT_EMP;
                    RRTYRD.EDIT_TIME = RTYRD.EDIT_TIME;
                    sql = RRTYRD.GetInsertString(this.DBType);
                }
                try
                {
                    result = DB.ExecSQL(sql);
                    //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                    if (Int32.Parse(result) < -1)
                    {
                        string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000215", new string[] { "R_TCQS_YIELD_RATE_DETAIL" });
                        throw new MESReturnMessage(errMsg + ":" + result);
                    }
                }
                catch (Exception ee)//執行SQL異常
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000215", new string[] { "R_TCQS_YIELD_RATE_DETAIL" });
                    throw new MESReturnMessage(errMsg + ":" + ee.Message);
                }
                //更新R_TMP_ATEDATA_T
                if (RTYRD.TOTAL_FRESH_BUILD_QTY + RTYRD.TOTAL_REWORK_BUILD_QTY > 0)//如果是過站處理,則刪除臨時表記錄
                {
                    sql = $@"Delete From R_TMP_ATEDATA_T Where SN='{SerialNo}' and Station_Name='{RTYRD.STATION_NAME}' ";
                }
                else
                {
                    TRTAT = new T_R_TMP_ATEDATA_T(DB, this.DBType);
                    RRTAT = (Row_R_TMP_ATEDATA_T)TRTAT.NewRow();
                    sql = $@"SELECT * FROM r_tmp_atedata_t
                           WHERE Station_Name = '{RTYRD.STATION_NAME}' AND SN = '{SerialNo}' ";
                    dt = DB.ExecSelect(sql).Tables[0];
                    //如果記錄已經存在,則Update
                    if (dt.Rows.Count > 0)
                    {
                        RRTAT.loadData(dt.Rows[0]);
                        RRTAT.DEVICE_NAME = RTYRD.DEVICE_NAME;
                        RRTAT.PASS_COUNT = RRTAT.PASS_COUNT + RTYRD.TCQS_FRESH_PASS_QTY + RTYRD.TCQS_REWORK_PASS_QTY;
                        RRTAT.RETEST_COUNT = RRTAT.RETEST_COUNT + RTYRD.TCQS_FRESH_FAIL_QTY + RTYRD.TCQS_REWORK_FAIL_QTY;
                        sql = RRTAT.GetUpdateString(this.DBType);
                    }
                    else//記錄不存在則Insert
                    {
                        RRTAT.ID = TRTAT.GetNewID(Bu, DB);
                        RRTAT.WORKORDERNO = RTYRD.WORKORDERNO;
                        RRTAT.SKUNO = RTYRD.SKUNO;
                        RRTAT.SN = SerialNo;
                        RRTAT.PRODUCTION_LINE = RTYRD.PRODUCTION_LINE;
                        RRTAT.SECTION_NAME = "1";
                        RRTAT.STATION_NAME = RTYRD.STATION_NAME;
                        RRTAT.DEVICE_NAME = RTYRD.DEVICE_NAME;
                        RRTAT.PASS_COUNT = RTYRD.TCQS_FRESH_PASS_QTY + RTYRD.TCQS_REWORK_PASS_QTY;
                        RRTAT.RETEST_COUNT = RTYRD.TCQS_FRESH_FAIL_QTY + RTYRD.TCQS_REWORK_FAIL_QTY;
                        RRTAT.EDIT_EMP = RTYRD.EDIT_EMP;
                        RRTAT.EDIT_TIME = RTYRD.EDIT_TIME;
                        sql = RRTAT.GetInsertString(this.DBType);
                    }
                }
                try
                {
                    result = DB.ExecSQL(sql);
                    //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                    if (Int32.Parse(result) < -1)
                    {
                        string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000215", new string[] { "R_TMP_ATEDATA_T" });
                        throw new MESReturnMessage(errMsg + ":" + result);
                    }
                }
                catch (Exception)//執行SQL異常
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000215", new string[] { "R_TMP_ATEDATA_T" });
                    throw new MESReturnMessage(errMsg + ":" + result);
                }
                //新增一筆R_SN_DETAIL
                TRSD = new T_R_SN_DETAIL(DB, this.DBType);
                RRSD = (Row_R_SN_DETAIL)TRSD.NewRow();
                RRSD.ID = TRSD.GetNewID(Bu, DB);
                RRSD.SN = SerialNo;
                RRSD.SKUNO = RTYRD.SKUNO;
                RRSD.WORKORDERNO = RTYRD.WORKORDERNO;
                RRSD.LINE = RTYRD.PRODUCTION_LINE;
                RRSD.STATION_NAME = RTYRD.STATION_NAME;
                RRSD.DEVICE_NAME = RTYRD.DEVICE_NAME;
                RRSD.CLASS_NAME = RTYRD.CLASS_NAME;
                //如果以PASS過站,則Flag=1
                if (RTYRD.TOTAL_FRESH_PASS_QTY + RTYRD.TOTAL_REWORK_PASS_QTY > 0)
                {
                    RRSD.RESULT_FLAG = "1";
                }
                else if (RTYRD.TOTAL_FRESH_FAIL_QTY + RTYRD.TOTAL_REWORK_FAIL_QTY > 0)//如果以Fail進維修,則Flag=0
                {
                    RRSD.RESULT_FLAG = "0";

                }
                else if (RTYRD.TCQS_FRESH_PASS_QTY + RTYRD.TCQS_REWORK_PASS_QTY > 0)//如果PASS但不過站,則Flag=3
                {
                    RRSD.RESULT_FLAG = "3";
                }
                else//如果FAIL但不過站,則Flag=2
                {
                    RRSD.RESULT_FLAG = "2";
                }
                RRSD.EDIT_EMP = RTYRD.EDIT_EMP;
                RRSD.EDIT_TIME = RTYRD.EDIT_TIME;
                sql = RRSD.GetInsertString(this.DBType);
                try
                {
                    result = DB.ExecSQL(sql);
                    //如果數據更新成功，則返回數據更新的記錄數,如果為Begn...END返回為-1,可被 Int32.Parse 方法轉換成 int
                    if (Int32.Parse(result) < -1)
                    {
                        string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000215", new string[] { "R_SN_DETAIL" });
                        throw new MESReturnMessage(errMsg + ":" + result);
                    }
                }
                catch (Exception ee)//執行SQL異常
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000215", new string[] { "R_SN_DETAIL" });
                    throw new MESReturnMessage(errMsg + ":" + ee.Message);
                }
                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        /// <summary>
        /// SN掃入維修ByFailCode
        /// </summary>
        public string SNFailByFailCode(R_TCQS_YIELD_RATE_DETAIL RTYRD, C_ERROR_CODE FailCodeObject, string SerialNo, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string result = "";
            //ErrorCode不能為空
            if (FailCodeObject == null || FailCodeObject.ERROR_CODE == null || FailCodeObject.ERROR_CODE == "")
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "ERROR_CODE" }));
            }

            //更新R_SN.Repair_Fail_Flag=1
            T_R_SN TRSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            R_SN RSN = TRSN.GetDetailBySN(SerialNo, DB);
            Row_R_SN RowRSN = (Row_R_SN)TRSN.NewRow();
            RowRSN = (Row_R_SN)TRSN.GetObjByID(RSN.ID, DB);
            RowRSN.REPAIR_FAILED_FLAG = "1";
            RowRSN.EDIT_EMP = EmpNo;
            RowRSN.EDIT_TIME = RTYRD.EDIT_TIME;
            result = DB.ExecSQL(RowRSN.GetUpdateString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(result) > 0))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
            }

            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
            T_R_SN_STATION_DETAIL TRSNSationDetail = new T_R_SN_STATION_DETAIL(DB, DB_TYPE_ENUM.Oracle);
            result = TRSNSationDetail.AddDetailToRSnStationDetail(TRSNSationDetail.GetNewID(Bu, DB),
                RowRSN.GetDataObject(), RTYRD.PRODUCTION_LINE, RTYRD.STATION_NAME, RTYRD.DEVICE_NAME, DB);
            if (!(Convert.ToInt32(result) > 0))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "STATION_DETAIL" }));
            }

            //新增一筆到R_REPAIR_MAIN
            string RepairMainID = "";
            T_R_REPAIR_MAIN TRRepairMain = new T_R_REPAIR_MAIN(DB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_MAIN RowRRepairMain = (Row_R_REPAIR_MAIN)TRRepairMain.NewRow();
            RepairMainID = TRRepairMain.GetNewID(Bu, DB);
            RowRRepairMain.ID = RepairMainID;
            RowRRepairMain.SN = RSN.SN;
            RowRRepairMain.WORKORDERNO = RSN.WORKORDERNO;
            RowRRepairMain.SKUNO = RSN.SKUNO;
            RowRRepairMain.FAIL_LINE = RTYRD.PRODUCTION_LINE;
            RowRRepairMain.FAIL_STATION = RTYRD.STATION_NAME;
            RowRRepairMain.FAIL_DEVICE = RTYRD.DEVICE_NAME;
            RowRRepairMain.FAIL_EMP = EmpNo;
            RowRRepairMain.FAIL_TIME = RTYRD.EDIT_TIME;
            RowRRepairMain.CREATE_TIME = RTYRD.EDIT_TIME;
            RowRRepairMain.EDIT_EMP = EmpNo;
            RowRRepairMain.EDIT_TIME = RTYRD.EDIT_TIME;
            RowRRepairMain.CLOSED_FLAG = "0";
            result = DB.ExecSQL(RowRRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(result) > 0))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "REPAIR_MAIN" }));
            }

            //新增一筆到R_REPAIR_FAILCODE
            T_R_REPAIR_FAILCODE TRRepairFailCode = new T_R_REPAIR_FAILCODE(DB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_FAILCODE RowRRepairFailCode = (Row_R_REPAIR_FAILCODE)TRRepairFailCode.NewRow();
            RowRRepairFailCode.ID = TRRepairFailCode.GetNewID(Bu, DB);
            RowRRepairFailCode.REPAIR_MAIN_ID = RepairMainID;
            RowRRepairFailCode.SN = RSN.SN;
            RowRRepairFailCode.FAIL_CODE = FailCodeObject.ERROR_CODE;
            RowRRepairFailCode.FAIL_EMP = EmpNo;
            RowRRepairFailCode.FAIL_TIME = RTYRD.EDIT_TIME;
            RowRRepairFailCode.FAIL_CATEGORY = FailCodeObject.ERROR_CATEGORY;
            RowRRepairFailCode.FAIL_LOCATION = "";
            RowRRepairFailCode.FAIL_PROCESS = "";
            RowRRepairFailCode.DESCRIPTION = FailCodeObject.ENGLISH_DESC.ToString();
            RowRRepairFailCode.REPAIR_FLAG = "0";
            RowRRepairFailCode.CREATE_TIME = RTYRD.EDIT_TIME;
            RowRRepairFailCode.EDIT_EMP = EmpNo;
            RowRRepairFailCode.EDIT_TIME = RTYRD.EDIT_TIME; ;
            result = DB.ExecSQL(RowRRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(result) > 0))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "REPAIR_FAILCODE" }));
            }
            return RepairMainID;
        }


    }
    public class Row_R_TCQS_YIELD_RATE_DETAIL : DataObjectBase
    {
        public Row_R_TCQS_YIELD_RATE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_TCQS_YIELD_RATE_DETAIL GetDataObject()
        {
            R_TCQS_YIELD_RATE_DETAIL DataObject = new R_TCQS_YIELD_RATE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.WORK_DATE = this.WORK_DATE;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.PRODUCTION_LINE = this.PRODUCTION_LINE;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.DEVICE_NAME = this.DEVICE_NAME;
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
            DataObject.TCQS_FRESH_BUILD_QTY = this.TCQS_FRESH_BUILD_QTY;
            DataObject.TCQS_FRESH_PASS_QTY = this.TCQS_FRESH_PASS_QTY;
            DataObject.TCQS_FRESH_FAIL_QTY = this.TCQS_FRESH_FAIL_QTY;
            DataObject.TCQS_REWORK_BUILD_QTY = this.TCQS_REWORK_BUILD_QTY;
            DataObject.TCQS_REWORK_PASS_QTY = this.TCQS_REWORK_PASS_QTY;
            DataObject.TCQS_REWORK_FAIL_QTY = this.TCQS_REWORK_FAIL_QTY;
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
        public string WORK_DATE
        {
            get
            {
                return (string)this["WORK_DATE"];
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
        public string DEVICE_NAME
        {
            get
            {
                return (string)this["DEVICE_NAME"];
            }
            set
            {
                this["DEVICE_NAME"] = value;
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
        public double? TCQS_FRESH_BUILD_QTY
        {
            get
            {
                return (double?)this["TCQS_FRESH_BUILD_QTY"];
            }
            set
            {
                this["TCQS_FRESH_BUILD_QTY"] = value;
            }
        }
        public double? TCQS_FRESH_PASS_QTY
        {
            get
            {
                return (double?)this["TCQS_FRESH_PASS_QTY"];
            }
            set
            {
                this["TCQS_FRESH_PASS_QTY"] = value;
            }
        }
        public double? TCQS_FRESH_FAIL_QTY
        {
            get
            {
                return (double?)this["TCQS_FRESH_FAIL_QTY"];
            }
            set
            {
                this["TCQS_FRESH_FAIL_QTY"] = value;
            }
        }
        public double? TCQS_REWORK_BUILD_QTY
        {
            get
            {
                return (double?)this["TCQS_REWORK_BUILD_QTY"];
            }
            set
            {
                this["TCQS_REWORK_BUILD_QTY"] = value;
            }
        }
        public double? TCQS_REWORK_PASS_QTY
        {
            get
            {
                return (double?)this["TCQS_REWORK_PASS_QTY"];
            }
            set
            {
                this["TCQS_REWORK_PASS_QTY"] = value;
            }
        }
        public double? TCQS_REWORK_FAIL_QTY
        {
            get
            {
                return (double?)this["TCQS_REWORK_FAIL_QTY"];
            }
            set
            {
                this["TCQS_REWORK_FAIL_QTY"] = value;
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
    public class R_TCQS_YIELD_RATE_DETAIL
    {
        public string ID;
        public string WORK_DATE;
        public string WORK_TIME;
        public string PRODUCTION_LINE;
        public string CLASS_NAME;
        public string STATION_NAME;
        public string DEVICE_NAME;
        public string WORKORDERNO;
        public string SKUNO;
        public string SKU_NAME;
        public string SKU_SERIES;
        public double? TOTAL_FRESH_BUILD_QTY;
        public double? TOTAL_FRESH_PASS_QTY;
        public double? TOTAL_FRESH_FAIL_QTY;
        public double? TOTAL_REWORK_BUILD_QTY;
        public double? TOTAL_REWORK_PASS_QTY;
        public double? TOTAL_REWORK_FAIL_QTY;
        public double? TCQS_FRESH_BUILD_QTY;
        public double? TCQS_FRESH_PASS_QTY;
        public double? TCQS_FRESH_FAIL_QTY;
        public double? TCQS_REWORK_BUILD_QTY;
        public double? TCQS_REWORK_PASS_QTY;
        public double? TCQS_REWORK_FAIL_QTY;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }

    public class T_R_TMP_ATEDATA_T : DataObjectTable
    {
        public T_R_TMP_ATEDATA_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TMP_ATEDATA_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TMP_ATEDATA_T);
            TableName = "R_TMP_ATEDATA_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TMP_ATEDATA_T : DataObjectBase
    {
        public Row_R_TMP_ATEDATA_T(DataObjectInfo info) : base(info)
        {

        }
        public R_TMP_ATEDATA_T GetDataObject()
        {
            R_TMP_ATEDATA_T DataObject = new R_TMP_ATEDATA_T();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SN = this.SN;
            DataObject.PRODUCTION_LINE = this.PRODUCTION_LINE;
            DataObject.SECTION_NAME = this.SECTION_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.DEVICE_NAME = this.DEVICE_NAME;
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.ATE_STATION_NO = this.ATE_STATION_NO;
            DataObject.TEST_FLAG = this.TEST_FLAG;
            DataObject.PASS_COUNT = this.PASS_COUNT;
            DataObject.RETEST_COUNT = this.RETEST_COUNT;
            DataObject.TEST_VALUE = this.TEST_VALUE;
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
        public string SECTION_NAME
        {
            get
            {
                return (string)this["SECTION_NAME"];
            }
            set
            {
                this["SECTION_NAME"] = value;
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
        public string DEVICE_NAME
        {
            get
            {
                return (string)this["DEVICE_NAME"];
            }
            set
            {
                this["DEVICE_NAME"] = value;
            }
        }
        public string ERROR_CODE
        {
            get
            {
                return (string)this["ERROR_CODE"];
            }
            set
            {
                this["ERROR_CODE"] = value;
            }
        }
        public string ATE_STATION_NO
        {
            get
            {
                return (string)this["ATE_STATION_NO"];
            }
            set
            {
                this["ATE_STATION_NO"] = value;
            }
        }
        public double? TEST_FLAG
        {
            get
            {
                return (double?)this["TEST_FLAG"];
            }
            set
            {
                this["TEST_FLAG"] = value;
            }
        }
        public double? PASS_COUNT
        {
            get
            {
                return (double?)this["PASS_COUNT"];
            }
            set
            {
                this["PASS_COUNT"] = value;
            }
        }
        public double? RETEST_COUNT
        {
            get
            {
                return (double?)this["RETEST_COUNT"];
            }
            set
            {
                this["RETEST_COUNT"] = value;
            }
        }
        public double? TEST_VALUE
        {
            get
            {
                return (double?)this["TEST_VALUE"];
            }
            set
            {
                this["TEST_VALUE"] = value;
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
    public class R_TMP_ATEDATA_T
    {
        public string ID;
        public string WORKORDERNO;
        public string SKUNO;
        public string SN;
        public string PRODUCTION_LINE;
        public string SECTION_NAME;
        public string STATION_NAME;
        public string DEVICE_NAME;
        public string ERROR_CODE;
        public string ATE_STATION_NO;
        public double? TEST_FLAG;
        public double? PASS_COUNT;
        public double? RETEST_COUNT;
        public double? TEST_VALUE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}
