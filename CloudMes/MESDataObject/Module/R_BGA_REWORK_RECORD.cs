using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_BGA_REWORK_RECORD : DataObjectTable
    {
        public T_R_BGA_REWORK_RECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BGA_REWORK_RECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BGA_REWORK_RECORD);
            TableName = "R_BGA_REWORK_RECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 选择入维修Checkin
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public string GetCheckin(string SN, string LOCATION, string DEPARTMENT, string DESCRIPTION, string CHECK_IN_EMP, string B29SN, DateTime DATE, int COUNT, DateTime DATANOW, string SKUNO, string CURRENT_STATION, int TIMES, OleExec DB, string BU)
        {
            string strsql = string.Empty;
            string code = string.Empty;
            //OleDbParameter[] parameters;
            //int result;

            //判断SN序列号是否存在
            if (!IsSNExists(SN, B29SN, DB))
            {
                code = "MSGCODE20190407220224";
                return code;
            }

            //判断SN序列號是否存在維修報表27項中
            if (!IsInRepair(SN,DB))
            {
                code = "MSGCODE20190407220328";
                // code = "MSGCODE20190320150323";
                //throw new MESReturnMessage(code);
                return code;
            }
            //判断SN序列號进入BGA维修区后是否CHECK OUT 出来
            if (IsCheckInBGA(SN, DB))
            {
                code = "MSGCODE20190407220441";
                // code = "MSGCODE20190320151303";
                //throw new MESReturnMessage(code);
                return code;
            }

            //判断SN序列號进入BGA维修区后是否维修超过3次
            //CHECK IN  FLAG=3  
            if (IsCheckInMoreThan3(SN, DB))
            {
                code = "MSGCODE20190407220554";
                // code = "MSGCODE20190320152517";
                //throw new MESReturnMessage(code);
                return code;
            }


            //判断SN序列號同一位置進入BGA的次数，维修2次需報廢！
            if (IsSameLocationMoreThan2(SN, LOCATION, DB))
            {
                code = "MSGCODE20190407221210";
                // code = "MSGCODE20190320153329";
                //throw new MESReturnMessage(code);
                return code;
            }
       

            string ID = MesDbBase.GetNewID(DB.ORM, BU, " R_BGA_REWORK_RECORD");
            string DateNowStr = DATANOW.ToString("yyyy-MM-dd HH:mm:ss");

            strsql = $@" INSERT INTO R_BGA_REWORK_RECORD(ID,SN,SKUNO,CURRENT_STATION,LOCATION,DESCRIPTION,CHECK_IN_FLAG,DEPARTMENT,CHECK_IN_EMP,CHECK_IN_TIME)
            VALUES('{ID}','{SN}','{SKUNO}','{CURRENT_STATION}','{LOCATION}','{DESCRIPTION}','{COUNT + 1}','{DEPARTMENT}','{CHECK_IN_EMP}',to_date('{DateNowStr}','yyyy-mm-dd hh24:mi:ss')) ";
            DB.RunSelect(strsql);


            if (TIMES >= 240000)
            {
                code = "MSGCODE20190407221427";
                //已成功掃進BGA重工區,當前板子沒有AOI4過站記錄和烘烤記錄，需要烘烤！
                // throw new MESReturnMessage(code);
            }
            else if (TIMES >= 72 && TIMES < 240000)
            {
                code = "MSGCODE20190407221845";
                //已掃進BGA重工區,當前時間超過板子過AOI4或者離上次進入烘烤時間超過72小時，需要烘烤！
                //throw new MESReturnMessage(code);
            }
            else if (TIMES < 72)
            {
                code = "MSGCODE20190407215840";
                //code = "此序列號已成功掃進BGA重工區!!";
                //throw new MESReturnMessage(code);
            }


            if (code == "")
            {
                code = "MSGCODE20190407215840";
            }
            return code;
        }

        /// <summary>
        /// 选择入维修CheckOut
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public string GetCheckOut(string SN, string TRACK_NO, string RE_STATION, string CHECK_OUT_EMP, string SKUNO, string CURRENT_STATION, string LOCATION, string DESCRIPTION, OleExec DB,string BU)
        {
            string strsql = string.Empty;
            string code = string.Empty;
            //OleDbParameter[] parameters;
            //int result;
            //判断SN序列号是否进入BGA重工区且没有CHECK OUT 出去

            if (!IsCheckInBGA(SN, DB))
            {
                code = "MSGCODE20190407223629";
                //code = "此序列號未掃進BGA重工區!";
                return code;
            }

            //判断SN序列號當前是否入 HK_IN 且没有出 HK_OUT
            if (IsCheckInBGAHK(SN, DB))
            {
                code = "MSGCODE20190407224120";
                //code = "此序列號當前在烘烤區，請先掃出烘烤區!";
                return code;
            }


            //根據SN獲取最近一條未CHECK out的ID
            string ID = GetIDNotCheckoutBySN(SN, DB);




            //更新SN序列号出CHECK_OUT
            strsql = $@"UPDATE R_BGA_REWORK_RECORD SET CHECK_OUT_FLAG='1',CHECK_OUT_TIME=SYSDATE,CHECK_OUT_EMP=:CHECK_OUT_EMP,
            TRACK_NO=:TRACK_NO,RE_STATION=:RE_STATION WHERE ID=:id ";
          
            OleDbParameter[] param = new OleDbParameter[] {
                new OleDbParameter(":id", ID)
            };

            int res = DB.ExecuteNonQuery(strsql, CommandType.Text, param);
            if (res > 0)
            {
                //code = "此序列號已掃出BGA重工區!";
                code = "MSGCODE20190407224311";
                //throw new MESReturnMessage(code);
            }

            //插入一笔资料到sfc2D5DXWIPQuery表里
            strsql = $@"";
            R_2D5D_WIP_QUERY R2D5DObj =TransTo2D5DWIPQUERY(ID, BU, DB);

            T_R_2D5D_WIP_QUERY TR2D5DObj = new T_R_2D5D_WIP_QUERY(DB, DB_TYPE_ENUM.Oracle);

            TR2D5DObj.InsertR2D5DWIPQYERY(R2D5DObj,DB);


            //if (code == "")
            //{
            //    code = "MES00000001";
            //}
            return code;
        }
        /// <summary>
        /// 选择入烘烤Checkin
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public string GetHKCheckin(string SN, string HK_IN_EMP, OleExec DB)
        {
            string strsql = string.Empty;
            string code = string.Empty;
            OleDbParameter[] parameters;
            int result;
            //判断SN序列号是否进入BGA重工区且没有CHECK OUT 出去
            //boo = IsCheckInBGA(SN, DB);
            if (!IsCheckInBGA(SN, DB))
            {
                code = "MSGCODE20190407223629";
                return code;
            }

            //判斷是否已經掃入BGA烘烤區
            if (IsCheckInBGAHK(SN, DB))
            {
                code = "MSGCODE20190407224120";
                return code;
            }

            //更新數據庫,掃入烘烤區
            strsql = $@"UPDATE R_BGA_REWORK_RECORD SET HK_IN_FLAG = '1', HK_IN_TIME = SYSDATE, HK_IN_EMP = :HK_IN_EMP WHERE SN = :SN AND CHECK_OUT_FLAG IS NULL";
            parameters = new OleDbParameter[] { new OleDbParameter(":HK_IN_EMP", HK_IN_EMP) , new OleDbParameter("SN", SN) };
            result = DB.ExecuteNonQuery(strsql, CommandType.Text, parameters);
            if (result > 0)
            {
                code = "MSGCODE20190407224439";
                //code = " " + SN + ":此序列號已掃進烘烤區!";
                //throw new MESReturnMessage(code);
                
            }
            else
            {
                code = "MSGCODE20190407225717";
            }
            return code;



            //if (code == "")
            //{
            //    //???提示內容待確認
            //    code = "MES00000001";
            //}
            //return code;
        }
        /// <summary>
        /// 选择出烘烤Checkout
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public string GetHKCheckout(string SN, string HK_OUT_EMP, OleExec DB)
        {
           
            string strsql = string.Empty;
            string code = string.Empty;
            OleDbParameter[] parameters;
            int result;

            //判断SN序列号是否进入BGA重工区且没有CHECK OUT 出去
            //boo = IsCheckInBGA(SN, DB);
            if (!IsCheckInBGA(SN, DB))
            {
                code = "MSGCODE20190407223629";
                //code = " " + SN + ":此序列號未掃進BGA重工區!";
                return code;
            }

            //判斷是否已經掃入BGA烘烤區
            if (!IsCheckInBGAHK(SN, DB))
            {
                code = "MSGCODE20190407224937";
                //code = " " + SN + ":此序列號未掃進BGA烘烤區!";
                return code;
            }

            //判斷是否掃出BGA烘烤區
            if (IsCheckOutBGAHK(SN, DB))
            {
                code = "MSGCODE20190407225020";
                //code = " " + SN + ":此序列號已經掃出烘烤區,不能重複掃出!";
                return code;
            }

            //判斷烘烤時間是否大於八個小時s
            if (!IsHKTimeEnough(SN, DB))
            {
                code = "MSGCODE20190407225223";
                //code = " " + SN + ":此序列號掃進BGA烘烤區時間不足8小時,不能掃出烘烤區!";
                return code;
            }

            //更新數據庫,掃出烘烤區
            strsql = $@"UPDATE R_BGA_REWORK_RECORD SET HK_OUT_FLAG='1', HK_OUT_TIME=SYSDATE,HK_OUT_EMP='{HK_OUT_EMP}' WHERE SN='{SN}' AND CHECK_OUT_FLAG IS NULL";
            parameters = new OleDbParameter[] { new OleDbParameter("SN", SN) };
            result = DB.ExecuteNonQuery(strsql, CommandType.Text, parameters);
            if (result > 0)
            {
                code = "MSGCODE20190407225949";
                //code = " " + SN + ":此序列號已成功掃出烘烤區!";
                //throw new MESReturnMessage(code);
            }
            else
            {   
                code = "MSGCODE20190407225717";
            }
            return code;

        }
        /// <summary>
        /// 选择HKCheck后的动作
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public string GetHKCheck(string SN, int TIMES, OleExec DB)
        {
            string strsql = string.Empty;
            string code = string.Empty;
            //int result;
            //OleDbParameter[] parameters;


            if (!IsCheckInBGAHK(SN, DB))
            {
                code = "MSGCODE20190407224937";
                //code = " " + SN + ":此序列號未掃進BGA烘烤區！請CHECKIN！";
                return code;
            }




            if (TIMES >= 240000)
            {
                code = "MSGCODE20190407221427";
                //code = " " + SN + ":當前板子沒有烘烤記錄，需要烘烤！";
                //return code;
                //throw new MESReturnMessage(code);
            }
            else if (TIMES >= 72 && TIMES < 239999)
            {
                code = "MSGCODE20190407221845";
                //code = " " + SN + ":當前時間超過板子距上次進入烘烤時間超過72小時，需要烘烤！";
                //return code;
                //throw new MESReturnMessage(code);
            }
            else if (TIMES < 72)
            {
                //code = " " + SN + ":此序列號距上次進入烘烤時間未到72小時！";
                code = "MSGCODE20190407230530";
                //return code;
                //throw new MESReturnMessage(code);
            }
            else
            {
                code = "MSGCODE20190407225717";
            }

            //if (code == "")
            //{
            //    code = "MES00000001";
            //}
            return code;
        }


        /// <summary>
        /// 替换SN序列号前缀，在 C_SN_PRE_STATION_MAP表里匹配
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <return
        public string GetB29SN(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            string B29SN = string.Empty;
            //update by FQ 20190328    --add station_name=b29m   工站確認
            strsql = $@" SELECT REPLACED_SN_PRE || SUBSTR('{SN}',4,11) FROM C_SN_PRE_STATION_MAP WHERE SN_PRE =SUBSTR('{SN}',1,3) and STATION_NAME='B29M' and rownum<2 ";
            DataSet res = DB.RunSelect(strsql);
            if (res.Tables[0].Rows.Count > 0)
            {
                B29SN = res.Tables[0].Rows[0][0].ToString();
            }
            return B29SN;
        }
        /// <summary>
        /// 获取获取上次烘烤时间
        /// update by FQ 20190328  datatime 可為空
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <return
        public DateTime? GetLastHKTime(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            DateTime? LASTHKTIME=null;
            strsql = $@" SELECT HK_OUT_TIME FROM R_BGA_REWORK_RECORD WHERE SN='{SN}' ORDER BY CHECK_IN_TIME DESC ";
            DataSet res = DB.RunSelect(strsql);

            if (res.Tables[0].Rows.Count > 0)
            {
                if (res.Tables[0].Rows[0][0] != null && res.Tables[0].Rows[0][0].ToString() != "")
                {
                    LASTHKTIME = Convert.ToDateTime(res.Tables[0].Rows[0][0]);
                }
            }

            return LASTHKTIME;
        }
        /// <summary>
        /// 获取最近过AOI4时间
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <return
        public DateTime? GetAOI4Time(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            DateTime? AOI4TIME;
            strsql = $@" SELECT START_TIME FROM R_SN_STATION_DETAIL WHERE SN='{SN}' AND STATION_NAME='AOI4' ";
            DataSet res = DB.RunSelect(strsql);
            if (res.Tables[0].Rows.Count > 0)
            {
                AOI4TIME = Convert.ToDateTime(res.Tables[0].Rows[0][0]);
            }
            else
            {
                AOI4TIME = null;
            }
            return AOI4TIME;
        }
        /// <summary>
        /// 获取入BGA的次数
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <return
        public int GetCount(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            strsql = $@" SELECT COUNT(*) FROM R_BGA_REWORK_RECORD WHERE SN=:SN";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            //DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows[0][0]
          // DB.
            int result = Convert.ToInt16(DB.ExecuteScalar(strsql, CommandType.Text, parameters).ToString());
            return result;
        }
        /// <summary>
        /// 获取SN序列号的机种和当前工站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> GetSkunoStation(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            List<string> list = new List<string>();
            //获取SN序列號的機種和當前站
            strsql = $@" SELECT SKUNO,CURRENT_STATION FROM R_SN  WHERE SN='{SN}'";
            DataSet res = DB.RunSelect(strsql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < res.Tables[0].Columns.Count; j++)
                {
                    list.Add(res.Tables[0].Rows[i][j].ToString());
                }
            }
            return list;
        }
        /// <summary>
        /// 获取板子上次烘烤的小时数
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public int GetTimes(DateTime DATE, OleExec DB)
        {
            string strsql = string.Empty;
            //update by FQ ,DATE不需要-10000,,getdate中已經操作過
            //時間差計算無需訪問數據庫,代碼中即可處理
            TimeSpan TS = DateTime.Now.Subtract(DATE);
            int TIMES = Convert.ToInt32(TS.TotalHours);

            //strsql = $@" SELECT  TO_NUMBER((TRUNC(SYSDATE)-TRUNC('{DATE}'-10000))*24) FROM DUAL  ";
            //DataSet res = DB.RunSelect(strsql);
            //int TIMES = Convert.ToInt32(res.Tables[0].Rows[0][0]);
            return TIMES;
        }
        /// <summary>
        /// 获取烘烤结束时间
        /// </summary>
        /// <param name="DB"></param>
        /// <return
        public DateTime? GetHKData(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            strsql = $@"
                    SELECT * FROM (
                        SELECT  HK_OUT_TIME FROM R_BGA_REWORK_RECORD WHERE SN='{SN}'  ORDER BY CHECK_IN_TIME DESC
                    )
                    WHERE ROWNUM<2";
            DataSet res = DB.RunSelect(strsql);

            DateTime? TIMES = null;


            if (res.Tables[0].Rows.Count > 0)
            { 
            if (res.Tables[0].Rows[0][0] != null && res.Tables[0].Rows[0][0].ToString() != "")
            {
                TIMES = Convert.ToDateTime(res.Tables[0].Rows[0][0]);
            }
      
            }
            return TIMES;
        }
        /// <summary>
        /// 获取SN序列号SKUNO, CURRENT_STATION, LOCATION, DESCRIPTION信息
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <return
        public List<string> GetDetailBySN(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            List<string> list = new List<string>();
            strsql = $@"SELECT SKUNO, CURRENT_STATION, LOCATION, DESCRIPTION FROM R_BGA_REWORK_RECORD WHERE SN = '{SN}' AND CHECK_OUT_FLAG IS NULL";
            DataSet res = DB.RunSelect(strsql);
            for (int i = 0; i < res.Tables[0].Columns.Count; i++)
            {
                list.Add(res.Tables[0].Rows[0][i].ToString());
            }
            return list;
        }


        public bool IsCheckInBGA(string SN, OleExec DB)
        {           
            bool boo = true;
            string strsql = $@"SELECT * FROM R_BGA_REWORK_RECORD WHERE SN=:SN AND CHECK_IN_FLAG IS NOT NULL AND CHECK_OUT_FLAG IS NULL";

            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result == 0)
            {
                boo = false;
                //code = " " + SN + ":此序列號未掃進BGA重工區!";
                //throw new MESReturnMessage(code);              
            }
            return boo ;
        }

        public bool IsCheckInBGAHK(string SN, OleExec DB)
        {
            bool boo=false;         
            string strsql = $@"SELECT * FROM R_BGA_REWORK_RECORD WHERE SN=:SN AND HK_IN_FLAG IS NOT NULL AND HK_OUT_FLAG IS NULL";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 0)
            {
                boo = true;
                //code = " " + SN + ":此序列號已經在BGA烘烤區!請不要重複掃入!";
                //throw new MESReturnMessage(code);            
            }
            return boo;
        }

        public bool IsCheckOutBGAHK(string SN, OleExec DB)
        {
            bool boo=false;
            string strsql = $@"SELECT * FROM R_BGA_REWORK_RECORD WHERE SN='{SN}' AND HK_OUT_FLAG IS NOT NULL  AND  CHECK_OUT_FLAG IS NULL";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter("SN", SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 0)
            {   
                //已經掃出
                boo = true;
            }   
            return boo;

        }

        public bool IsHKTimeEnough(string SN, OleExec DB)
        {
            bool boo = true;

            string strsql = $@"SELECT * FROM R_BGA_REWORK_RECORD WHERE SN = :SN  AND ROUND(TO_NUMBER(SYSDATE - HK_IN_TIME) * 24) < 8 AND HK_OUT_FLAG IS NULL AND CHECK_OUT_FLAG IS NULL";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter("SN", SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 0)
            {
                boo = false;
            }

            return boo;
        }

        public bool IsSNExists(string SN,string B29SN, OleExec DB)
        {
           
            bool boo = false;
            string strsql = $@" SELECT * FROM R_SN WHERE SN = :SN OR SN=:B29SN";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN), new OleDbParameter(":B29SN", B29SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 0)
            {
                //存在
                boo = true;
            }
            return boo;
        }

        public bool IsInRepair(string SN, OleExec DB)
        {
            bool boo = false;
            string strsql = $@"  SELECT * FROM R_REPAIR_TRANSFER  WHERE SN=:SN ";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 0)
            {
                //存在
                boo = true;
            }
            return boo;       
        }

        public bool IsCheckInMoreThan3(string SN, OleExec DB)
        {
            bool boo = false;
            string strsql = $@" SELECT * FROM R_BGA_REWORK_RECORD WHERE SN=:SN AND CHECK_IN_FLAG ='3' ";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 0)
            {
                //存在
                boo = true;
            }
            return boo;
        }

        public bool IsSameLocationMoreThan2(string SN, string LOCATION, OleExec DB)
        {
            bool boo = false;
            string strsql = $@"SELECT COUNT(*) FROM R_BGA_REWORK_RECORD WHERE SN=:SN AND LOCATION=:LOCATION ";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":SN", SN), new OleDbParameter(":LOCATION", LOCATION) };
            int result = DB.ExecuteDataTable(strsql, CommandType.Text, parameters).Rows.Count;
            if (result > 1)
            {
                //存在
                boo = true;
            }
            return boo;
        }


        /// <summary>
        /// 通過BGA的ID,獲取一個R_2d5d的OBJ
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="BU"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_2D5D_WIP_QUERY TransTo2D5DWIPQUERY(string ID, string BU,OleExec DB)
        {
            R_2D5D_WIP_QUERY R2D5DObj = new R_2D5D_WIP_QUERY();
            //獲取r_bga_rework_record中信息
            R_BGA_REWORK_RECORD BGARecordObj = GetByID(ID, DB);
            //獲取工單
            T_R_SN RSNObj = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            R_SN SNObj = RSNObj.LoadSN(BGARecordObj.SN, DB);
            string WO = SNObj.WORKORDERNO;
            //獲取ID
            string R2D5DID= MesDbBase.GetNewID(DB.ORM, BU, " R_BGA_REWORK_RECORD");


            R2D5DObj.ID = R2D5DID;
            R2D5DObj.SN = BGARecordObj.SN;
            R2D5DObj.WO = WO;
            R2D5DObj.SKUNO = BGARecordObj.SKUNO;
            R2D5DObj.CURRENT_STATION = BGARecordObj.CURRENT_STATION;
            R2D5DObj.LOCATION = BGARecordObj.LOCATION;
            R2D5DObj.DESCRIPTION = BGARecordObj.DESCRIPTION;
            R2D5DObj.CHECKIN_FLAG = BGARecordObj.CHECK_IN_FLAG;
            R2D5DObj.CHECKIN_TIME = BGARecordObj.CHECK_IN_TIME;
            R2D5DObj.CHECKOUT_FLAG = BGARecordObj.CHECK_OUT_FLAG;
            R2D5DObj.CHECKOUT_TIME = BGARecordObj.CHECK_OUT_TIME;
            R2D5DObj.TRACK_NO = BGARecordObj.TRACK_NO;
            R2D5DObj.EDIT_EMP = BGARecordObj.EDIT_EMP;
            R2D5DObj.EDIT_TIME = BGARecordObj.EDIT_TIME;

            return R2D5DObj;

        }

        /// <summary>
        /// 通過ID獲取OBJ
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_BGA_REWORK_RECORD GetByID(string ID, OleExec DB)
        {
            string strSql = $@"select * from R_BGA_REWORK_RECORD where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", ID) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_R_BGA_REWORK_RECORD ret = (Row_R_BGA_REWORK_RECORD)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 獲取未checkout 的ID
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetIDNotCheckoutBySN(string SN, OleExec DB)
        {
            string ID = string.Empty;
            string SqlStr=$@"
                select * from 
                (
                select ID from R_BGA_REWORK_RECORD
                 WHERE SN =:SN AND CHECK_OUT_FLAG IS NULL 
                    order by CHECK_IN_TIME DESC)        where rownum<2         
";

            OleDbParameter[] param = new OleDbParameter[] {           
                new OleDbParameter(":SN", SN)
            };

            ID=DB.ExecuteScalar(SqlStr, CommandType.Text, param);

            return ID;

        }
        






    }
    public class Row_R_BGA_REWORK_RECORD : DataObjectBase
    {
        public Row_R_BGA_REWORK_RECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_BGA_REWORK_RECORD GetDataObject()
        {
            R_BGA_REWORK_RECORD DataObject = new R_BGA_REWORK_RECORD();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LOCATION = this.LOCATION;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.TRACK_NO = this.TRACK_NO;
            DataObject.DEPARTMENT = this.DEPARTMENT;
            DataObject.RE_STATION = this.RE_STATION;
            DataObject.CHECK_IN_FLAG = this.CHECK_IN_FLAG;
            DataObject.CHECK_IN_EMP = this.CHECK_IN_EMP;
            DataObject.CHECK_IN_TIME = this.CHECK_IN_TIME;
            DataObject.CHECK_OUT_FLAG = this.CHECK_OUT_FLAG;
            DataObject.CHECK_OUT_EMP = this.CHECK_OUT_EMP;
            DataObject.CHECK_OUT_TIME = this.CHECK_OUT_TIME;
            DataObject.HK_IN_FLAG = this.HK_IN_FLAG;
            DataObject.HK_IN_EMP = this.HK_IN_EMP;
            DataObject.HK_IN_TIME = this.HK_IN_TIME;
            DataObject.HK_OUT_FLAG = this.HK_OUT_FLAG;
            DataObject.HK_OUT_EMP = this.HK_OUT_EMP;
            DataObject.HK_OUT_TIME = this.HK_OUT_TIME;
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
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
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
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string TRACK_NO
        {
            get
            {
                return (string)this["TRACK_NO"];
            }
            set
            {
                this["TRACK_NO"] = value;
            }
        }
        public string DEPARTMENT
        {
            get
            {
                return (string)this["DEPARTMENT"];
            }
            set
            {
                this["DEPARTMENT"] = value;
            }
        }
        public string RE_STATION
        {
            get
            {
                return (string)this["RE_STATION"];
            }
            set
            {
                this["RE_STATION"] = value;
            }
        }
        public string CHECK_IN_FLAG
        {
            get
            {
                return (string)this["CHECK_IN_FLAG"];
            }
            set
            {
                this["CHECK_IN_FLAG"] = value;
            }
        }
        public string CHECK_IN_EMP
        {
            get
            {
                return (string)this["CHECK_IN_EMP"];
            }
            set
            {
                this["CHECK_IN_EMP"] = value;
            }
        }
        public DateTime? CHECK_IN_TIME
        {
            get
            {
                return (DateTime?)this["CHECK_IN_TIME"];
            }
            set
            {
                this["CHECK_IN_TIME"] = value;
            }
        }
        public string CHECK_OUT_FLAG
        {
            get
            {
                return (string)this["CHECK_OUT_FLAG"];
            }
            set
            {
                this["CHECK_OUT_FLAG"] = value;
            }
        }
        public string CHECK_OUT_EMP
        {
            get
            {
                return (string)this["CHECK_OUT_EMP"];
            }
            set
            {
                this["CHECK_OUT_EMP"] = value;
            }
        }
        public DateTime? CHECK_OUT_TIME
        {
            get
            {
                return (DateTime?)this["CHECK_OUT_TIME"];
            }
            set
            {
                this["CHECK_OUT_TIME"] = value;
            }
        }
        public string HK_IN_FLAG
        {
            get
            {
                return (string)this["HK_IN_FLAG"];
            }
            set
            {
                this["HK_IN_FLAG"] = value;
            }
        }
        public string HK_IN_EMP
        {
            get
            {
                return (string)this["HK_IN_EMP"];
            }
            set
            {
                this["HK_IN_EMP"] = value;
            }
        }
        public DateTime? HK_IN_TIME
        {
            get
            {
                return (DateTime?)this["HK_IN_TIME"];
            }
            set
            {
                this["HK_IN_TIME"] = value;
            }
        }
        public string HK_OUT_FLAG
        {
            get
            {
                return (string)this["HK_OUT_FLAG"];
            }
            set
            {
                this["HK_OUT_FLAG"] = value;
            }
        }
        public string HK_OUT_EMP
        {
            get
            {
                return (string)this["HK_OUT_EMP"];
            }
            set
            {
                this["HK_OUT_EMP"] = value;
            }
        }
        public DateTime? HK_OUT_TIME
        {
            get
            {
                return (DateTime?)this["HK_OUT_TIME"];
            }
            set
            {
                this["HK_OUT_TIME"] = value;
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
    public class R_BGA_REWORK_RECORD
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string LOCATION { get; set; }
        public string CURRENT_STATION { get; set; }
        public string DESCRIPTION { get; set; }
        public string TRACK_NO { get; set; }
        public string DEPARTMENT { get; set; }
        public string RE_STATION { get; set; }
        public string CHECK_IN_FLAG { get; set; }
        public string CHECK_IN_EMP { get; set; }
        public DateTime? CHECK_IN_TIME { get; set; }
        public string CHECK_OUT_FLAG { get; set; }
        public string CHECK_OUT_EMP { get; set; }
        public DateTime? CHECK_OUT_TIME { get; set; }
        public string HK_IN_FLAG { get; set; }
        public string HK_IN_EMP { get; set; }
        public DateTime? HK_IN_TIME { get; set; }
        public string HK_OUT_FLAG { get; set; }
        public string HK_OUT_EMP { get; set; }
        public DateTime? HK_OUT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
