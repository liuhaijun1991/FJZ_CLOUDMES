using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Text.RegularExpressions;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_OFFLINE : DataObjectTable
    {
        public T_R_REPAIR_OFFLINE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_OFFLINE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_OFFLINE);
            TableName = "R_REPAIR_OFFLINE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 添加信息 for R_REPAIR_OFFLINE    by  zc 2019/04/30
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SKUNO"></param>
        /// <param name="STATION_NAME"></param>
        /// <param name="LOCATION"></param>
        /// <param name="COMPONENT_ID"></param>
        /// <param name="CPN"></param>
        /// <param name="REPAIR_EMP"></param>
        /// <param name="FAIL_CODE"></param>
        /// <param name="ACTION_CODE"></param>
        /// <param name="REMARK"></param>
        /// <param name="ROOT_CAUSE"></param>
        /// <param name="EDIT_TIME"></param>
        /// <param name="PROCESS_SITE"></param>
        /// <param name="DATA_CODE"></param>
        /// <param name="LOT_CODE"></param>
        /// <param name="VEND_NAME"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddDetail(string ID, string SN, string R_SN_ID, string SKUNO, string STATION_NAME, string LOCATION, string COMPONENT_ID, string CPN, string REPAIR_EMP, string FAIL_CODE, string ACTION_CODE, string REMARK, string ROOT_CAUSE, string EDIT_TIME, string PROCESS_SITE, string DATA_CODE, string LOT_CODE, string VEND_NAME,  OleExec DB)
        {
            string strsql = string.Empty;
            int result;
            T_R_REPAIR_OFFLINE REPAIROFF = new T_R_REPAIR_OFFLINE(DB,DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_OFFLINE row=(Row_R_REPAIR_OFFLINE)REPAIROFF.NewRow();
            row.ID = ID;
            row.R_SN_ID = R_SN_ID;
            row.SN = SN;
            row.SKUNO = SKUNO;
            row.STATION_NAME = STATION_NAME;
            row.FAIL_CODE = FAIL_CODE;
            row.ACTION_CODE = ACTION_CODE;
            row.LOCATION = LOCATION;
            row.ROOT_CAUSE = ROOT_CAUSE;
            row.PROCESS_SITE = PROCESS_SITE;
            row.CPN = CPN;
            row.DATA_CODE = DATA_CODE;
            row.LOT_CODE = LOT_CODE;
            row.VEND_NAME = VEND_NAME;
            row.REPAIR_EMP = REPAIR_EMP;
            row.REMARK = REMARK;
            row.COMPONENT_ID = COMPONENT_ID;
            row.EDIT_EMP = REPAIR_EMP;
            row.EDIT_TIME = Convert.ToDateTime(EDIT_TIME);
            //DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text, paramet);
            //result = res.Rows.Count;
            string Sql=row.GetInsertString(DB_TYPE_ENUM.Oracle);
            try
            {
                return result = Int32.Parse(DB.ExecSQL(Sql));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 刪除選中行by SN    by  zsh  2019-04-12 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteByID(string ID, OleExec DB)
        {
            string strsql = string.Empty;
            int result;
            strsql = $@"  DELETE FROM R_REPAIR_OFFLINE WHERE ID=:ID ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":ID", ID) };
            result = DB.ExecuteNonQuery(strsql, CommandType.Text, paramet);
           // result = res.Rows.Count;
            return result;
        }
        /// <summary>
        /// Check SN  for R_REPAIR_SELF    by   zsh  2019-04-11
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable CheckSNForRepairSelf(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            strsql = $@"SELECT * FROM R_REPAIR_SELF WHERE SN='{SN}' AND IN_FLAG=1 AND OUT_FLAG=0";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text, null);
            return res;
        }
        /// <summary>
        /// Check SN   for R_REPAIR_TRANSFER    by  zsh  2019-04-11
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable CheckSNForRepairTransfer(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            strsql = $@" SELECT * FROM R_REPAIR_TRANSFER WHERE SN='{SN}'  AND CLOSED_FLAG=1";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text, null);
            return res;
        }
        /// <summary>
        /// 根據SN AND LOCATION帶出AllPart物料信息    by  zsh  2019-04-12
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="LOCATION"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetBySNAndLOCATION(string SN, string LOCATION, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetCode = null;

            List<string> Data = new List<string>();

            string APSp = "MES1.GET_KP_MESSAGE_NEW";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetTRCode = new OleDbParameter[11];

                ParasForGetTRCode[0] = new OleDbParameter("MYPSN", SN);
                ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetTRCode[1] = new OleDbParameter("MYLOCATION", LOCATION);
                ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetTRCode[2] = new OleDbParameter("G_TR_SN", null);
                ParasForGetTRCode[2].Direction = ParameterDirection.Output;

                ParasForGetTRCode[3] = new OleDbParameter("G_KP_NO", null);
                ParasForGetTRCode[3].Direction = ParameterDirection.Output;
                ParasForGetTRCode[3].Size = 2000;

                ParasForGetTRCode[4] = new OleDbParameter("G_MFR_KP_NO", null);
                ParasForGetTRCode[4].Direction = ParameterDirection.Output;
                ParasForGetTRCode[4].Size = 2000;

                ParasForGetTRCode[5] = new OleDbParameter("G_MFR_CODE", null);
                ParasForGetTRCode[5].Direction = ParameterDirection.Output;
                ParasForGetTRCode[5].Size = 2000;

                ParasForGetTRCode[6] = new OleDbParameter("G_MFR_NAME", null);
                ParasForGetTRCode[6].Direction = ParameterDirection.Output;
                ParasForGetTRCode[6].Size = 2000;

                ParasForGetTRCode[7] = new OleDbParameter("G_DATE_CODE", null);
                ParasForGetTRCode[7].Direction = ParameterDirection.Output;
                ParasForGetTRCode[7].Size = 2000;

                ParasForGetTRCode[8] = new OleDbParameter("G_LOT_CODE", null);
                ParasForGetTRCode[8].Direction = ParameterDirection.Output;
                ParasForGetTRCode[8].Size = 2000;

                ParasForGetTRCode[9] = new OleDbParameter("G_KP_DESC", null);
                ParasForGetTRCode[9].Direction = ParameterDirection.Output;
                ParasForGetTRCode[9].Size = 2000;

                ParasForGetTRCode[10] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetTRCode[10].Direction = ParameterDirection.Output;
                ParasForGetTRCode[10].Size = 2000;

                GetCode = DB.ExecProcedureReturnDic(APSp, ParasForGetTRCode);
                //string str_g_kp_no = GetCode["G_KP_NO"].ToString().Trim();
                //string strmfrname = GetCode["G_MFR_NAME"].ToString().Trim();
                //string strDataCode = GetCode["G_DATE_CODE"].ToString().Trim();
                //string strLotCode = GetCode["G_LOT_CODE"].ToString().Trim();
                //Data.Add(GetCode["G_KP_NO"].ToString().Trim());
                //Data.Add(GetCode["G_MFR_NAME"].ToString().Trim());
                //Data.Add(GetCode["G_DATE_CODE"].ToString().Trim());
                //Data.Add(GetCode["G_LOT_CODE"].ToString().Trim());
                //Data.Add(GetCode["RES"].ToString().Trim());
                result = GetCode["RES"].ToString();
                if (result.ToUpper().Trim() != "OK;G1")
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000263", new string[] { result.ToString() + " " + APSp });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return GetCode;
        }
        /// <summary>
        /// 根據時間、機種、SN或工站名查詢信息
        /// </summary>
        /// <param name="EDIT_TIME"></param>
        /// <param name="SKUNO"></param>
        /// <param name="SN"></param>
        /// <param name="STATION_NAME"></param>
        /// <param name="REPAIR_EMP"></param>
        /// <param name="DB"></param>
        public object SelectDate(DateTime STIME, DateTime FTIME, string SKUNO, string SN, string STATION_NAME, string REPAIR_EMP, OleExec DB)
        {
            string strsql = string.Empty;    
            string START_TIME = STIME.ToString("yyyy-MM-dd HH:mm:ss");
            string FINISH_TIME = FTIME.ToString("yyyy-MM-dd HH:mm:ss");

            strsql = $@"  SELECT DISTINCT A.ID,A.EDIT_TIME AS WORKDAY,D.SKUNO ,G.SKU_NAME AS CODENAME,A.SN AS SYSSERAILNO, A.EDIT_TIME AS CREATEDATE,''AS TESTBY,
          A.STATION_NAME AS EVENTPOINT, A.FAIL_CODE AS FAILSYMPTOM, A.ROOT_CAUSE AS ROOT_CAUSE,B.ENGLISH_DESCRIPTION AS FAILURECODE ,
           A.ACTION_CODE,A.PROCESS_SITE AS PROCESS,'' AS CSERIALNO, A.LOCATION,A.REPAIR_EMP AS REPAIRBY,'' AS LASTEDITBY,'' AS REPAIRDATE
            , A.CPN AS COMPONENTCODE,A.VEND_NAME AS VENDORCODE ,A.DATA_CODE AS DATACODE,A.LOT_CODE AS LOTCODE,A.COMPONENT_ID AS SOLUTION,A.REMARK AS DESCRIPTION,'' AS PARTDES, D.WORKORDERNO,'' AS SHIFT,
              F.LINE AS PRODUCTIONLINE,'' AS FAILLOCATION
            FROM R_REPAIR_OFFLINE A
           LEFT JOIN(SELECT ERROR_CODE, ENGLISH_DESCRIPTION FROM C_ERROR_CODE) B ON A.FAIL_CODE = B.ERROR_CODE
           LEFT JOIN R_SN D ON A.SN = D.SN
           LEFT JOIN R_SN_STATION_DETAIL F ON A.SN = F.SN AND A.STATION_NAME = F.STATION_NAME
           LEFT JOIN C_SKU G ON D.SKUNO = G.SKUNO WHERE 1=1   ";
            if (SN != "")
            {
                strsql += $@" and  a.SN = '{SN}' ";
            }
            else
            {
                if (FTIME >= STIME)
                {
                    strsql += $@" and a.EDIT_TIME between TO_DATE('{START_TIME}','yyyy-mm-dd HH24:MI:SS') AND TO_DATE('{FINISH_TIME}','yyyy-mm-dd HH24:MI:SS')";
                }
                if (SKUNO != "")
                {
                    strsql += $@" and  a.SKUNO = '{SKUNO}' ";
                }
                if (STATION_NAME != "")
                {
                    strsql += $@" and  a.STATION_NAME = '{STATION_NAME}' ";
                }
                if (REPAIR_EMP != "")
                {
                    strsql += $@" and  a.REPAIR_EMP = '{REPAIR_EMP}' ";
                }
            }
            strsql += " order by a.EDIT_TIME desc ";
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text, null);
            var rows = res.AsEnumerable().Select(tmp => new
            {
                ID = tmp["ID"].ToString(),//20170815
                WORKDAY = tmp["WORKDAY"].ToString(),
                SKUNO = tmp["SKUNO"].ToString(),
                CODENAME = tmp["CODENAME"].ToString(),
                SYSSERAILNO = tmp["SYSSERAILNO"].ToString(),
                CREATEDATE = tmp["CREATEDATE"].ToString(),
                TESTBY = tmp["TESTBY"].ToString(),
                EVENTPOINT = tmp["EVENTPOINT"].ToString(),
                FAILSYMPTOM = tmp["FAILSYMPTOM"].ToString(),
                ROOT_CAUSE = tmp["ROOT_CAUSE"].ToString(),
                FAILURECODE = tmp["FAILURECODE"].ToString(),
                ACTION_CODE = tmp["ACTION_CODE"].ToString(),
                PROCESS = tmp["PROCESS"].ToString(),
                CSERIALNO = tmp["CSERIALNO"].ToString(),
                LOCATION = tmp["LOCATION"].ToString(),
                REPAIRBY = tmp["REPAIRBY"].ToString(),
                LASTEDITBY = tmp["LASTEDITBY"].ToString(),
                REPAIRDATE = tmp["REPAIRDATE"].ToString(),
                COMPONENTCODE = tmp["COMPONENTCODE"].ToString(),
                VENDORCODE = tmp["VENDORCODE"].ToString(),
                DATACODE = tmp["DATACODE"].ToString(),
                LOTCODE = tmp["LOTCODE"].ToString(),
                SOLUTION = tmp["SOLUTION"].ToString(),
                DESCRIPTION = tmp["DESCRIPTION"].ToString(),
                PARTDES = tmp["PARTDES"].ToString(),
                WORKORDERNO = tmp["WORKORDERNO"].ToString(),
                SHIFT = tmp["SHIFT"].ToString(),
                PRODUCTIONLINE = tmp["PRODUCTIONLINE"].ToString(),
                FAILLOCATION = tmp["FAILLOCATION"].ToString()
            });
            return rows.Select(t => t.ID).ToList().Count == 0 ? null : rows;
            
            //return rows.Select(t => t.ID).ToList().Count == 0 ? null : rows;
           // return res;
        }
        /// <summary>
        /// 頁面加載時顯示信息    by  zsh  2019-04-17  
        /// </summary>
        /// <param name="START_TIME"></param>
        /// <param name="FINISH_TIME"></param>
        /// <param name="SKUNO"></param>
        /// <param name="SN"></param>
        /// <param name="STATION_NAME"></param>
        /// <param name="REPAIR_EMP"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<RepairOfflineInf>  SelectDateForLoad(OleExec DB)
        {
            string strsql = string.Empty;
            var list = new List<R_REPAIR_OFFLINE>();
            // List<R_REPAIR_OFFLINE> result = new List<R_REPAIR_OFFLINE>();

            strsql = $@"   SELECT DISTINCT A.ID,A.EDIT_TIME  AS WORKDAY,D.SKUNO ,G.SKU_NAME AS CODENAME,A.SN AS SYSSERAILNO, A.EDIT_TIME AS CREATEDATE,''AS TESTBY  ,
           A.STATION_NAME  AS EVENTPOINT, A.FAIL_CODE AS FAILSYMPTOM, A.ROOT_CAUSE AS ROOT_CAUSE,B.ENGLISH_DESCRIPTION AS FAILURECODE ,
           A.ACTION_CODE,A.PROCESS_SITE AS PROCESS,'' AS CSERIALNO,A.LOCATION,A.REPAIR_EMP AS REPAIRBY,'' AS LASTEDITBY,'' AS REPAIRDATE
           ,A.CPN AS COMPONENTCODE,A.VEND_NAME AS VENDORCODE ,A.DATA_CODE AS DATACODE,A.LOT_CODE AS LOTCODE,A.COMPONENT_ID AS SOLUTION,A.REMARK AS DESCRIPTION,'' AS PARTDES,D.WORKORDERNO,'' AS SHIFT,
           F.LINE AS PRODUCTIONLINE,'' AS FAILLOCATION
            FROM R_REPAIR_OFFLINE A
           LEFT JOIN (SELECT ERROR_CODE,ENGLISH_DESCRIPTION FROM C_ERROR_CODE ) B ON A.FAIL_CODE=B.ERROR_CODE 
           LEFT JOIN R_SN D ON A.SN=D.SN
           LEFT JOIN R_SN_STATION_DETAIL F ON A.SN= F.SN AND  A.STATION_NAME=F.STATION_NAME
           LEFT JOIN C_SKU G ON D.SKUNO=G.SKUNO  WHERE ROWNUM<=100 ";
                   
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text, null);
            var data = (from tmp in res.AsEnumerable()
                        select new RepairOfflineInf
                    {
                            ID = tmp["ID"].ToString(),//20170815
                            WORKDAY = tmp["WORKDAY"].ToString(),
                            SKUNO = tmp["SKUNO"].ToString(),
                            CODENAME = tmp["CODENAME"].ToString(),
                            SYSSERAILNO = tmp["SYSSERAILNO"].ToString(),
                            CREATEDATE = tmp["CREATEDATE"].ToString(),
                            TESTBY = tmp["TESTBY"].ToString(),
                            EVENTPOINT = tmp["EVENTPOINT"].ToString(),
                            FAILSYMPTOM = tmp["FAILSYMPTOM"].ToString(),
                            ROOT_CAUSE = tmp["ROOT_CAUSE"].ToString(),
                            FAILURECODE = tmp["FAILURECODE"].ToString(),
                            ACTION_CODE = tmp["ACTION_CODE"].ToString(),
                            PROCESS = tmp["PROCESS"].ToString(),
                            CSERIALNO = tmp["CSERIALNO"].ToString(),
                            LOCATION = tmp["LOCATION"].ToString(),
                            REPAIRBY = tmp["REPAIRBY"].ToString(),
                            LASTEDITBY = tmp["LASTEDITBY"].ToString(),
                            REPAIRDATE = tmp["REPAIRDATE"].ToString(),
                            COMPONENTCODE = tmp["COMPONENTCODE"].ToString(),
                            VENDORCODE = tmp["VENDORCODE"].ToString(),
                            DATACODE = tmp["DATACODE"].ToString(),
                            LOTCODE = tmp["LOTCODE"].ToString(),
                            SOLUTION = tmp["SOLUTION"].ToString(),
                            DESCRIPTION = tmp["DESCRIPTION"].ToString(),
                            PARTDES = tmp["PARTDES"].ToString(),
                            WORKORDERNO = tmp["WORKORDERNO"].ToString(),
                            SHIFT = tmp["SHIFT"].ToString(),
                            PRODUCTIONLINE = tmp["PRODUCTIONLINE"].ToString(),
                            FAILLOCATION = tmp["FAILLOCATION"].ToString()
                        }).ToList();
            return data;
        }

        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetBySNActionCode(string sn, OleExec DB)
        {
            bool res = false;
            string strSQL = $@"SELECT * FROM R_REPAIR_OFFLINE WHERE SN='{sn}' AND ACTION_CODE IN (SELECT CONTROL_VALUE FROM C_CONTROL WHERE CONTROL_NAME='CHECK_ICT_RETEST_CODE')";
            DataTable Dt = DB.ExecSelect(strSQL).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }
            //int Num = DB.ExecSqlNoReturn(strSQL, null);
            //if (Num > 0)
            //{
            //    res = true;
            //}
            return res;
        }
        public List<R_REPAIR_OFFLINE> GetSNComponent(string SN, string Component, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_OFFLINE>().Where(t => t.SN == SN && t.COMPONENT_ID == Component).ToList();
        }
        public int InRepairSN(Row_R_REPAIR_OFFLINE RepairINFO, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_OFFLINE>(RepairINFO).ExecuteCommand(); ;
        }
        /// <summary>
        /// WZW 维修保存按钮需要
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void AutoRepairSave(Dictionary<string, string> DicStation, Dictionary<string, OleExec> DicDB, ref Dictionary<string, string> DicRef)
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
            string Line = DicStation["LINE"];
            string StationName = DicStation["STATIONNAME"];
            int StrSolderNo = Convert.ToInt32(DicStation["SolderNo"].ToString());

            OleExec DB = DicDB["SFCDB"];
            OleExec APDB = DicDB["APDB"];

            string CodeDesc = "";
            Regex lotReg = new Regex(@"/[^x00-xff]/g");
            if (!lotReg.IsMatch(FailSysPtom))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105174438", new string[] { FailSysPtom }));
            }
            else if (FailSysPtom == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105181605", new string[] { }));
            }
            else
            {
                //if not exists(select * from RC_DefectCode (nolock) where defectcategory = @ActionCode){}  OK
                T_C_ACTION_CODE CActionCode = new T_C_ACTION_CODE(DB, DB_TYPE_ENUM.Oracle);
                C_ACTION_CODE ActionCodeInFo = CActionCode.GetByActionCode(ActionCode, DB);
                if (ActionCodeInFo == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181107085152", new string[] { ActionCode }));
                }
                T_C_ERROR_CODE CErrorCode = new T_C_ERROR_CODE(DB, DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE ErrorCode = CErrorCode.GetByErrorCode(RootCause, DB);
                if (ErrorCode == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181106082033", new string[] { RootCause }));
                }
                if (Location == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181106083739", new string[] { }));
                }
                if (TrackNo == "" && TrackNo != "N/A" && TrackNo != null)
                {
                    //T_R_REPAIR_OFFLINE RRepairOffline = new T_R_REPAIR_OFFLINE(DB,DB_TYPE_ENUM.Oracle);
                    List<R_REPAIR_OFFLINE> ListRRepairOffline = GetSNComponent(SN, TrackNo, DB);
                    if (ListRRepairOffline.Count > 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105092618", new string[] { TrackNo }));
                    }
                }
                T_R_SN RSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
                List<R_SN> ListRSN = RSN.GetRSNbySN(SN, DB);
                T_R_WO_BASE RWOBase = new T_R_WO_BASE(DB, DB_TYPE_ENUM.Oracle);
                R_WO_BASE RWOBaseinfo = RWOBase.GetWoByWoNo(ListRSN[0].WORKORDERNO, DB);
                if (RootCause == "E206" && TrackNo.Substring(0, 1) != "T")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181106084128", new string[] { }));
                }
                if (RootCause != "")
                {
                    C_ERROR_CODE ErrorCodeName = CErrorCode.GetByErrorCode(RootCause, DB);
                    if (ErrorCodeName != null)
                    {
                        CodeDesc = ErrorCodeName.ENGLISH_DESC;
                    }
                    else
                    {
                        RootCause = FailSysPtom;
                    }
                    T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
                    List<R_TESTRECORD> ListRTestRecord = RTestRecord.GetListBYSNDesc(SN, DB);
                    //Row_R_REPAIR_OFFLINE RowRRepairOffline = GetSNBYSN(SN, DB);
                    Row_R_REPAIR_OFFLINE RowRRepairOfflineSNInFo = null;
                    string TableID = GetNewID(BU, DB);
                    RowRRepairOfflineSNInFo.ID = TableID;
                    RowRRepairOfflineSNInFo.R_SN_ID = ListRSN[0].ID;
                    RowRRepairOfflineSNInFo.SN = SN;
                    RowRRepairOfflineSNInFo.SKUNO = ListRSN[0].SKUNO;
                    RowRRepairOfflineSNInFo.STATION_NAME = ListRSN[0].CURRENT_STATION;
                    RowRRepairOfflineSNInFo.FAIL_CODE = CodeDesc;
                    RowRRepairOfflineSNInFo.ACTION_CODE = ActionCode;
                    RowRRepairOfflineSNInFo.LOCATION = Location;
                    RowRRepairOfflineSNInFo.ROOT_CAUSE = RootCause;
                    RowRRepairOfflineSNInFo.PROCESS_SITE = Process;
                    RowRRepairOfflineSNInFo.CPN = Component;
                    RowRRepairOfflineSNInFo.DATA_CODE = Date_Code;
                    RowRRepairOfflineSNInFo.LOT_CODE = Lot_Code;
                    RowRRepairOfflineSNInFo.VEND_NAME = VendName;
                    RowRRepairOfflineSNInFo.REMARK = Description;
                    RowRRepairOfflineSNInFo.COMPONENT_ID = TrackNo;
                    RowRRepairOfflineSNInFo.EDIT_EMP = EMP;
                    RowRRepairOfflineSNInFo.EDIT_TIME = GetDBDateTime(DB);
                    int InNum = InRepairSN(RowRRepairOfflineSNInFo, DB);
                    if (InNum < 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                    }
                    T_C_WORK_CLASS CWorkClass = new T_C_WORK_CLASS(DB, DB_TYPE_ENUM.Oracle);
                    string Name = CWorkClass.GetWorkClass(DB);
                    T_R_SN_STATION_DETAIL RSNStationDetail = new T_R_SN_STATION_DETAIL(DB, DB_TYPE_ENUM.Oracle);
                    Row_R_SN_STATION_DETAIL RowRSNStationDetail = null;
                    string RSNSDTableID = RSNStationDetail.GetNewID(BU, DB);
                    RowRRepairOfflineSNInFo.ID = RSNSDTableID;
                    RowRSNStationDetail.R_SN_ID = ListRSN[0].ID;
                    RowRSNStationDetail.SN = SN;
                    RowRSNStationDetail.SKUNO = ListRSN[0].SKUNO;
                    RowRSNStationDetail.WORKORDERNO = ListRSN[0].WORKORDERNO;
                    RowRSNStationDetail.PLANT = "ACEA";
                    RowRSNStationDetail.CLASS_NAME = Name;
                    RowRSNStationDetail.ROUTE_ID = RWOBaseinfo.ROUTE_ID;
                    RowRSNStationDetail.LINE = Line;
                    RowRSNStationDetail.STARTED_FLAG = ListRSN[0].STARTED_FLAG;
                    RowRSNStationDetail.START_TIME = ListRSN[0].START_TIME;
                    RowRSNStationDetail.PACKED_FLAG = ListRSN[0].PACKED_FLAG;
                    RowRSNStationDetail.PACKED_TIME = ListRSN[0].PACKDATE;
                    RowRSNStationDetail.COMPLETED_FLAG = ListRSN[0].COMPLETED_FLAG;
                    RowRSNStationDetail.COMPLETED_TIME = ListRSN[0].COMPLETED_TIME;
                    RowRSNStationDetail.SHIPPED_FLAG = ListRSN[0].SHIPPED_FLAG;
                    RowRSNStationDetail.SHIPDATE = ListRSN[0].SHIPDATE;
                    RowRSNStationDetail.REPAIR_FAILED_FLAG = ListRSN[0].REPAIR_FAILED_FLAG;
                    RowRSNStationDetail.CURRENT_STATION = ListRSN[0].CURRENT_STATION;
                    RowRSNStationDetail.NEXT_STATION = ListRSN[0].NEXT_STATION;
                    RowRSNStationDetail.KP_LIST_ID = ListRSN[0].KP_LIST_ID;
                    RowRSNStationDetail.PO_NO = ListRSN[0].PO_NO;
                    RowRSNStationDetail.CUST_ORDER_NO = ListRSN[0].CUST_ORDER_NO;
                    RowRSNStationDetail.CUST_PN = ListRSN[0].CUST_PN;
                    RowRSNStationDetail.BOXSN = ListRSN[0].BOXSN;
                    RowRSNStationDetail.DEVICE_NAME = StationName;
                    RowRSNStationDetail.STATION_NAME = StationName;
                    RowRSNStationDetail.SCRAPED_FLAG = ListRSN[0].SCRAPED_FLAG;
                    RowRSNStationDetail.SCRAPED_TIME = ListRSN[0].SCRAPED_TIME;
                    RowRSNStationDetail.PRODUCT_STATUS = ListRSN[0].PRODUCT_STATUS;
                    RowRSNStationDetail.REWORK_COUNT = ListRSN[0].REWORK_COUNT;
                    RowRSNStationDetail.VALID_FLAG = ListRSN[0].VALID_FLAG;
                    RowRSNStationDetail.EDIT_EMP = EMP;
                    RowRSNStationDetail.EDIT_TIME = RSNStationDetail.GetDBDateTime(DB);
                    int RSNsTATIONdETAILNum = RSNStationDetail.INSNStationDetail(RowRSNStationDetail, DB);
                    if (RSNsTATIONdETAILNum < 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                    }
                    Row_R_SN RRSN = RSN.getR_SNbySN(SN, DB);
                    RRSN.REPAIR_FAILED_FLAG = "0";
                    RRSN.EDIT_EMP = EMP;
                    RRSN.EDIT_TIME = RSNStationDetail.GetDBDateTime(DB);
                    int RSNNum = RSN.UpdateR_SN(RRSN, SN, "1", DB);
                    if (RSNNum < 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { SN }));
                    }
                    T_R_REPAIR_MAIN RRepairMain = new T_R_REPAIR_MAIN(DB, DB_TYPE_ENUM.Oracle);
                    R_REPAIR_MAIN RowRRepairMain = RRepairMain.GetSNBySN(SN, DB);
                    RowRRepairMain.CLOSED_FLAG = "1";
                    RowRRepairMain.REPAIRING_FLAG = "0";
                    RowRRepairMain.EDIT_EMP = EMP;
                    RowRRepairMain.EDIT_TIME = RRepairMain.GetDBDateTime(DB);
                    int RowRRepairMainNum = RRepairMain.UpdateRepairSN(RowRRepairMain, SN, "0", DB);
                    if (RowRRepairMainNum < 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { SN }));
                    }
                }
            }
        }
        public List<R_REPAIR_OFFLINE> GetBySNRepriSN(string RelSn, OleExec DB)
        {
            string strSql = $@" SELECT * FROM R_REPAIR_OFFLINE WHERE SN='{RelSn}' AND EDIT_TIME >=      
    (SELECT MAX(EDIT_TIME) FROM R_REPAIR_TRANSFER  WHERE SN='{RelSn}') ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_OFFLINE> listSn = new List<R_REPAIR_OFFLINE>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_OFFLINE ret = (Row_R_REPAIR_OFFLINE)NewRow();
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
    }
    public class Row_R_REPAIR_OFFLINE : DataObjectBase
    {
        public Row_R_REPAIR_OFFLINE(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_OFFLINE GetDataObject()
        {
            R_REPAIR_OFFLINE DataObject = new R_REPAIR_OFFLINE();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.ACTION_CODE = this.ACTION_CODE;
            DataObject.LOCATION = this.LOCATION;
            DataObject.ROOT_CAUSE = this.ROOT_CAUSE;
            DataObject.PROCESS_SITE = this.PROCESS_SITE;
            DataObject.CPN = this.CPN;
            DataObject.DATA_CODE = this.DATA_CODE;
            DataObject.LOT_CODE = this.LOT_CODE;
            DataObject.VEND_NAME = this.VEND_NAME;
            DataObject.REPAIR_EMP = this.REPAIR_EMP;
            DataObject.REMARK = this.REMARK;
            DataObject.COMPONENT_ID = this.COMPONENT_ID;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
        public string FAIL_CODE
        {
            get
            {
                return (string)this["FAIL_CODE"];
            }
            set
            {
                this["FAIL_CODE"] = value;
            }
        }
        public string ACTION_CODE
        {
            get
            {
                return (string)this["ACTION_CODE"];
            }
            set
            {
                this["ACTION_CODE"] = value;
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
        public string ROOT_CAUSE
        {
            get
            {
                return (string)this["ROOT_CAUSE"];
            }
            set
            {
                this["ROOT_CAUSE"] = value;
            }
        }
        public string PROCESS_SITE
        {
            get
            {
                return (string)this["PROCESS_SITE"];
            }
            set
            {
                this["PROCESS_SITE"] = value;
            }
        }
        public string CPN
        {
            get
            {
                return (string)this["CPN"];
            }
            set
            {
                this["CPN"] = value;
            }
        }
        public string DATA_CODE
        {
            get
            {
                return (string)this["DATA_CODE"];
            }
            set
            {
                this["DATA_CODE"] = value;
            }
        }
        public string LOT_CODE
        {
            get
            {
                return (string)this["LOT_CODE"];
            }
            set
            {
                this["LOT_CODE"] = value;
            }
        }
        public string VEND_NAME
        {
            get
            {
                return (string)this["VEND_NAME"];
            }
            set
            {
                this["VEND_NAME"] = value;
            }
        }
        public string REPAIR_EMP
        {
            get
            {
                return (string)this["REPAIR_EMP"];
            }
            set
            {
                this["REPAIR_EMP"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public string COMPONENT_ID
        {
            get
            {
                return (string)this["COMPONENT_ID"];
            }
            set
            {
                this["COMPONENT_ID"] = value;
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
    public class R_REPAIR_OFFLINE
    {
        public string ID;
        public string R_SN_ID;
        public string SN;
        public string SKUNO;
        public string STATION_NAME;
        public string FAIL_CODE;
        public string ACTION_CODE;
        public string LOCATION;
        public string ROOT_CAUSE;
        public string PROCESS_SITE;
        public string CPN;
        public string DATA_CODE;
        public string LOT_CODE;
        public string VEND_NAME;
        public string REPAIR_EMP;
        public string REMARK;
        public string COMPONENT_ID;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
    public class RepairOfflineInf
    {
        public string ID;
        public string WORKDAY;
        public string SKUNO;
        public string CODENAME;
        public string SYSSERAILNO;
        public string CREATEDATE;
        public string TESTBY;
        public string EVENTPOINT;
        public string FAILSYMPTOM;
        public string ROOT_CAUSE;
        public string FAILURECODE;
        public string ACTION_CODE;
        public string PROCESS;
        public string CSERIALNO;
        public string LOCATION;
        public string REPAIRBY;
        public string LASTEDITBY;
        public string REPAIRDATE;
        public string COMPONENTCODE;
        public string VENDORCODE;
        public string DATACODE;
        public string LOTCODE;
        public string SOLUTION;
        public string DESCRIPTION;
        public string PARTDES;
        public string WORKORDERNO;
        public string SHIFT;
        public string PRODUCTIONLINE;
        public string FAILLOCATION;
    }
}