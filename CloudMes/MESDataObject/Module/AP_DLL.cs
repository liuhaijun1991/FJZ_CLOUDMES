using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class AP_DLL
    {
        public List<DataRow> C_Product_Config_GetBYSkuAndVerson_like(string skuno, string skuverson, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes1.c_product_config where p_no=:sku and p_version like:skuverson ||'%'";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno),
                    new OleDbParameter(":skuverson", skuverson)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        public List<DataRow> C_Product_Config_GetBYSkuAndVerson(string skuno, string skuverson, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes1.c_product_config where p_no=:sku and p_version=:skuverson";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno),
                    new OleDbParameter(":skuverson", skuverson)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }
        
        public List<DataRow> R_PCBA_LINK_GetBYSku(string skuno, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_pcba_link where skuno=:sku";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        public List<DataRow> R_PCBA_LINK_GetBYPno(string skuno, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_pcba_link where p_no=:sku";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        public List<DataRow> C_STATION_KP_GetBYPno(string skuno,string skuverson, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from MES1.C_STATION_KP where p_no=:sku and STATION_NAME='SMTLOADING' and p_version like :skuverson ||'%' AND ROWNUM=1";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno),
                    new OleDbParameter(":skuverson", skuverson)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        public List<DataRow> R_TR_SN_GetBYTR_SN(string TRSN, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_tr_sn where tr_sn=:trsn";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":trsn", TRSN)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }
        public List<DataRow> R_TR_SN_WIP_GetBYTR_SN(string TRSN, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn=:trsn";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":trsn", TRSN)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        public string APUpdatePanlSN(string PanelSN, string SN, OleExec DB)
        {
            //string ErrMessage = "";
            //Psn = PanelSession.InputValue.ToString();

            OleDbParameter[] PanelReplaceSP = new OleDbParameter[3];
            PanelReplaceSP[0] = new OleDbParameter("G_PANEL", PanelSN);
            PanelReplaceSP[1] = new OleDbParameter("G_PSN", SN);
            PanelReplaceSP[2] = new OleDbParameter();
            PanelReplaceSP[2].Size = 1000;
            PanelReplaceSP[2].ParameterName = "RES";
            PanelReplaceSP[2].Direction = System.Data.ParameterDirection.Output;
            string result = DB.ExecProcedureNoReturn("MES1.Z_PANEL_REPLACE_SP", PanelReplaceSP);
            return result;
        }


        public List<string> GetLocationList(string SKUNO, OleExec DB)
        {
            string strSql = string.Empty;
            List<string> result = new List<string>();
            strSql = $@" SELECT DISTINCT(A.LOCATION) FROM MES1.C_SMT_AP_LOCATION A,MES1.C_SMT_AP_PRODUCT B
                            WHERE A.SMT_CODE =B.SMT_CODE AND B.P_NO='{SKUNO}' ";

            DataTable res = DB.ExecSelect(strSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    result.Add(res.Rows[i][0].ToString());
                }
            }
            return result;
        }

        public List<string> CheckLocationExist(string SKUNO, string LOCATION, OleExec DB)
        {
            string strSql = string.Empty;
            List<string> result = new List<string>();
            strSql = $@" SELECT DISTINCT(A.LOCATION) FROM MES1.C_SMT_AP_LOCATION A,MES1.C_SMT_AP_PRODUCT B
                            WHERE A.SMT_CODE =B.SMT_CODE AND B.P_NO='{SKUNO}' AND A.LOCATION ='{LOCATION}' ";

            DataTable res = DB.ExecSelect(strSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    result.Add(res.Rows[i][0].ToString());
                }
            }
            return result;
        }

        public int APUpdateUndoSmtloading(string panel, OleExec DB)
        {
            string strSql = $@"update mes4.r_tr_product_detail set wo = '#' || substr(wo,2,11), p_sn = '#' || p_sn, tr_code = '#' || tr_code where p_sn in (select p_sn from mes4.r_sn_link where panel_no =:panel_no )";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":panel_no", panel) };
            int i = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);

            strSql = $@"update mes4.r_sn_link set p_sn = '#' || p_sn, wo = '#' || substr(wo,2,11), panel_no = '#' || panel_no where panel_no =:panel_no ";
            paramet = new OleDbParameter[] { new OleDbParameter(":panel_no", panel) };
            i = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);

            return i;
        }

        /// <summary>
        /// 檢查 SMT 物料上料情況
        /// </summary>
        /// <param name="PSn"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string CheckSmtMaterial(string PSn, string Station, OleExec DB)
        {
            List<SqlSugar.SugarParameter> parameters = new List<SqlSugar.SugarParameter>();
            parameters.Add(new SqlSugar.SugarParameter("G_PSN", PSn));
            parameters.Add(new SqlSugar.SugarParameter("G_EVENT", Station));
            var result = new SqlSugar.SugarParameter("RES", null, true);
            parameters.Add(result);
            DB.ORM.Ado.UseStoredProcedure().GetString("MES1.CHECK_PSN_MATERIAL_TOP", parameters);
            return result.Value.ToString();
        }

        public string GetNextPanelCode(OleExec DB)
        {
            string sql = $@"SELECT * FROM MES4.R_SN_LINK WHERE INSTR(SN_CODE,TO_CHAR(SYSDATE,'YYMMDD'))=1 ORDER BY SN_CODE DESC";
            DataTable dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return (Int32.Parse(dt.Rows[0]["SN_CODE"].ToString()) + 1).ToString();
            }
            else
            {
                return DateTime.Now.ToString("yyMMdd") + "0001";
            }
        }

        public bool CheckStationKp(string PSn, string Station, OleExec DB)
        {
            bool Completed = false;
            string _Station = ChangeStation(Station);
            string sql = $@"SELECT C.* FROM MES4.R_WO_BASE A,MES4.R_SN_LINK B,MES1.C_STATION_KP C WHERE A.WO=B.WO AND B.P_SN='{PSn}' 
                        AND A.P_NO=C.P_NO AND A.P_VERSION=C.P_VERSION AND C.STATION_NAME='{_Station}'";
            DataTable dt = DB.ExecSelect(sql, null).Tables[0];

            sql = $@"SELECT A.* FROM MES4.R_TR_CODE_DETAIL A,MES4.R_TR_PRODUCT_DETAIL B,MES1.C_STATION_KP C
                    WHERE A.P_NO=C.P_NO AND A.P_VERSION=C.P_VERSION AND A.TR_CODE=B.TR_CODE AND B.P_SN='{PSn}' AND A.KP_NO=C.KP_NO
                    AND C.STATION_NAME='{_Station}'";
            if (dt.Rows.Count <= DB.ExecSelect(sql, null).Tables[0].Rows.Count)
            {
                Completed = true;
            }
            return Completed;
        }

        public string ChangeStation(string Station)
        {
            switch (Station.ToUpper())
            {
                case "PTH":
                    return "PTH1";
                case "PFT":
                    return "PTH2";
                case "PFB":
                    return "PTH3";
                default:
                    return Station;
            }
        }

        //add  by LLF 2018-07-04 begin
        public List<DataRow> C_Station_KP_GetBYSku(string skuno, string Ver, string Station, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from MES1.C_STATION_KP where p_no=:sku and p_version=:ver and station_name=:Station";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno),
                    new OleDbParameter(":ver", Ver),
                    new OleDbParameter(":Station", Station)
            };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }
        //add by LLF 2018-07-04 end
        
        public string LH_NSDI_GetAPTrCode(string TrSN, string WorkOrderNo, string MacAddress, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string Ext_Qty = string.Empty;
            string APSp = "MES1.GET_TRCODE";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetTRCode = new OleDbParameter[5];

                ParasForGetTRCode[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetTRCode[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetTRCode[2] = new OleDbParameter("G_MAC_ADDRESS", MacAddress);
                ParasForGetTRCode[2].Direction = ParameterDirection.Input;

                ParasForGetTRCode[3] = new OleDbParameter("G_EXT_QTY", OleDbType.VarChar);
                ParasForGetTRCode[3].Direction = ParameterDirection.Output;
                ParasForGetTRCode[3].Size = 2000;

                ParasForGetTRCode[4] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetTRCode[4].Direction = ParameterDirection.Output;
                ParasForGetTRCode[4].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic(APSp, ParasForGetTRCode);
                Ext_Qty = GetTRCodeDic["G_EXT_QTY"].ToString();

                result = GetTRCodeDic["RES"].ToString();
                if (result.ToUpper().Substring(0, 2) != "OK")
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
            return Ext_Qty;
        }

        public string LH_NSDI_GetAPSNCode(string IP, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string result = string.Empty;
            string SNCode = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string Ext_Qty = string.Empty;
            string StrAPSp = "MES1.GET_SNCODE_IPCBU";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetSNCode = new OleDbParameter[3];

                ParasForGetSNCode[0] = new OleDbParameter("G_MAC_ADDRESS", IP);
                ParasForGetSNCode[0].Direction = ParameterDirection.Input;

                ParasForGetSNCode[1] = new OleDbParameter("G_SNCODE", OleDbType.VarChar);
                ParasForGetSNCode[1].Direction = ParameterDirection.Output;
                ParasForGetSNCode[1].Size = 2000;

                ParasForGetSNCode[2] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetSNCode[2].Direction = ParameterDirection.Output;
                ParasForGetSNCode[2].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic(StrAPSp, ParasForGetSNCode);
                SNCode = GetTRCodeDic["G_SNCODE"].ToString();
                if (ParasForGetSNCode[2].Value.ToString() != "OK")
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000246", new string[] { StrAPSp + ParasForGetSNCode[2].Value.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
                else
                {
                    SNCode = GetTRCodeDic["G_SNCODE"].ToString();
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SNCode;
        }

        public string LH_NSDI_Z_INSERT_SN_LINK(string WO, string IP, string SN, out string Message, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string result = string.Empty;
            string SNCode = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string Ext_Qty = string.Empty;
            Message = "OK";
            string StrAPSp = "MES1.Z_INSERT_SN_LINK";
            try
            {
                if (DBType == DB_TYPE_ENUM.Oracle)
                {
                    OleDbParameter[] ParasForGetSNCode = new OleDbParameter[5];

                    ParasForGetSNCode[0] = new OleDbParameter("G_WO", WO);
                    ParasForGetSNCode[0].Direction = ParameterDirection.Input;

                    ParasForGetSNCode[1] = new OleDbParameter("G_MAC_ADDRESS", IP);
                    ParasForGetSNCode[1].Direction = ParameterDirection.Input;

                    ParasForGetSNCode[2] = new OleDbParameter("G_PSN", SN);
                    ParasForGetSNCode[2].Direction = ParameterDirection.Input;

                    ParasForGetSNCode[3] = new OleDbParameter("G_EXT_QTY", OleDbType.VarChar);
                    ParasForGetSNCode[3].Direction = ParameterDirection.Output;
                    ParasForGetSNCode[3].Size = 2000;

                    ParasForGetSNCode[4] = new OleDbParameter("RES", OleDbType.VarChar);
                    ParasForGetSNCode[4].Direction = ParameterDirection.Output;
                    ParasForGetSNCode[4].Size = 2000;

                    GetTRCodeDic = DB.ExecProcedureReturnDic(StrAPSp, ParasForGetSNCode);
                    Ext_Qty = GetTRCodeDic["G_EXT_QTY"].ToString();
                    if (ParasForGetSNCode[4].Value.ToString() != "OK")
                    {
                        Message = MESReturnMessage.GetMESReturnMessage("MES00000246", new string[] { StrAPSp + "," + ParasForGetSNCode[4].Value.ToString() });
                    }
                    else
                    {
                        Ext_Qty = GetTRCodeDic["G_EXT_QTY"].ToString();
                    }
                }
                else
                {
                    Message = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                }
            }
            catch (Exception ex)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000246", new string[] { StrAPSp + "," + ex.Message.ToString() });
            }
            return Ext_Qty;
        }

        /// <summary>
        /// AOI Allparts資料檢查
        /// </summary>
        /// <param name="g_psn"></param>
        /// <param name="g_event"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AOIDataCheck(string g_psn, string g_event, OleExec DB)
        {
            OleExec apdb = DB;
            string Psn = g_psn;
            string Event = g_event;
            OleDbParameter[] AOISP = new OleDbParameter[3];
            AOISP[0] = new OleDbParameter("g_psn", Psn);
            AOISP[1] = new OleDbParameter("g_event", Event);
            AOISP[2] = new OleDbParameter();
            AOISP[2].Size = 1000;
            AOISP[2].ParameterName = "res";
            AOISP[2].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.check_psn_material_AOI", AOISP);
            return result;
        }

        /// <summary>
        /// HWT過站檢查allpart資料
        /// add by hgb 2019.05.29
        /// </summary>
        /// <param name="g_psn"></param>
        /// <param name="g_event"></param>
        /// <param name="g_line"></param>
        /// <param name="mac_address"></param>
        /// <param name="g_wo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string CHECK_PSN_MATERIAL_AOI_NEW(string g_psn, string g_event, string g_line, string mac_address, string g_wo, OleExec DB)
        {
            OleExec apdb = DB;
            string Psn = g_psn;
            string Event = g_event;
            OleDbParameter[] AOISP = new OleDbParameter[6];
            AOISP[0] = new OleDbParameter("g_psn", Psn);
            AOISP[1] = new OleDbParameter("g_event", Event);
            AOISP[2] = new OleDbParameter("g_line", g_line);
            AOISP[3] = new OleDbParameter("mac_address", mac_address);
            AOISP[4] = new OleDbParameter("g_wo", g_wo);
            AOISP[5] = new OleDbParameter();
            AOISP[5].Size = 1000;
            AOISP[5].ParameterName = "res";
            AOISP[5].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_PSN_MATERIAL_AOI_NEW", AOISP);
           
            return result;
        }

        /// <summary>
        /// HWT檢查是否需要上輔料
        /// add by hgb 2019.05.29
        /// </summary>
        /// <param name="g_event"></param>
        /// <param name="g_wo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string SFC_CHECK_ASSIST_KP(string g_event, string g_wo, OleExec DB)
        {
            OleExec apdb = DB;
            OleDbParameter[] AOISP = new OleDbParameter[3];
            AOISP[0] = new OleDbParameter("g_event", g_event);
            AOISP[1] = new OleDbParameter("g_wo", g_wo);
            AOISP[2] = new OleDbParameter();
            AOISP[2].Size = 1000;
            AOISP[2].ParameterName = "var_o_message";
            AOISP[2].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.SFC_CHECK_ASSIST_KP", AOISP);
            return result;
        }



        /// <summary>
        /// 檢查是否配置有需要檢查虛擬條碼
        /// add by hgb 2019.05.29
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckIsNeedCheckVirtualSn(string skuno, OleExec DB)
        {
            string sql = $@" 
       SELECT *
        FROM mes1.c_program_parameter
       WHERE program_type = 'SP'
         AND program_name = 'CMC_AUDITUNIT_SP'
         AND function_object = 'CHECK-VIRTUAL'
         AND function_value1 = 'Y'
         AND instr(data3,'{skuno}') > 0";
            DataTable dt = DB.ExecSelect(sql, null).Tables[0];

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
        /// 檢查上一輪是否有掃完
        /// add by hgb 2019.05.29
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SKUNO"></param>
        /// <param name="StationName"></param>
        /// <param name="var_ipaddress"></param>
        /// <param name="var_workorderno"></param>
        /// <param name="DB"></param>
        public void CheckVirtualSnIsFihishScan(string SN, string SKUNO, string StationName, string var_ipaddress, string var_workorderno, OleExec DB)
        {
            string sql = $@" 
       SELECT *
              FROM mes4.r_ap_temp
             WHERE data1 = '{var_ipaddress}'
               AND data2 =  '{StationName}'
               AND data4 = '00' || substr('{var_workorderno}', 3, 10) ";
            DataTable dt = DB.ExecSelect(sql, null).Tables[0];

            if (dt.Rows.Count > 0)
            {
                //data4, data3, data5 into INTO l_wo, var_sn_tmp, var_rowcount
                sql = $@" 
       SELECT data4, data3, data5                
                FROM mes4.r_ap_temp
               WHERE data1 = '{var_ipaddress}'
                 AND data2 =  '{StationName}'
                 AND data4 = '00' || substr('{var_workorderno}', 3, 10) ";
                dt = DB.ExecSelect(sql, null).Tables[0];

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091218", new string[] { dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString() }));

            }
            MakeVirtualSnRecord(SN, SKUNO, StationName, var_ipaddress, var_workorderno, DB);


        }



        /// <summary>
        ///  虛擬工單顯示待掃描實條碼數量
        /// add by hgb 2019.05.30
        /// </summary>
        /// <param name="StationName"></param>
        /// <param name="var_ipaddress"></param>
        /// <param name="var_workorderno"></param>
        /// <param name="SFCDB"></param>
        /// <param name="APDB"></param>
        /// <returns></returns>
        public string ShowWaitScanRealSnQty(string StationName, string var_ipaddress, string var_workorderno, OleExec SFCDB, OleExec APDB)
        {

            string l_wo = "99" + var_workorderno.Substring(2, 10);

            string sql = $@" 
       SELECT * FROM SFCRUNTIME.R_WO_BASE WHERE WORKORDERNO = '{l_wo}' ";
            DataTable dt = SFCDB.ExecSelect(sql, null).Tables[0];

            if (dt.Rows.Count > 0 || var_workorderno.Substring(0, 2) == "99")//條件之一滿足就說明是虛擬工單，顯示待掃描實條碼數量
            {

                sql = $@" 
        SELECT data5 - nvl(data6, '0')       
        FROM mes4.r_ap_temp
       WHERE data1 = '{var_ipaddress}'
         AND data2 ='{StationName}' ";
                dt = APDB.ExecSelect(sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    string qty = dt.Rows[0][0].ToString();
                    return qty;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }



        /// <summary>
        /// 檢查是否已掃描虛擬條碼
        /// add by hgb 2019.05.29
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SKUNO"></param>
        /// <param name="StationName"></param>
        /// <param name="var_ipaddress"></param>
        /// <param name="var_workorderno"></param>
        /// <param name="SFCDB"></param>
        /// <param name="APDB"></param>
        public void CheckVirtualSnIsScan(string SN, string SKUNO, string StationName, string var_ipaddress, string var_workorderno, OleExec SFCDB,OleExec APDB)
        {
            string l_wo = "99" + var_workorderno.Substring(2, 10);

            string sql = $@" 
       SELECT * FROM SFCRUNTIME.R_WO_BASE WHERE WORKORDERNO = '{l_wo}'";
            DataTable dt = SFCDB.ExecSelect(sql, null).Tables[0];

            if (dt.Rows.Count > 0)
            {
                l_wo = "00" + var_workorderno.Substring(2, 10);
                sql = $@" 
       SELECT *               
                FROM mes4.r_ap_temp
               WHERE data1 = '{var_ipaddress}'
                 AND data2 =  '{StationName}'
                 AND data4 = '{l_wo}' ";
                dt = APDB.ExecSelect(sql, null).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091219", new string[] { }));
                }
                MakeVirtualSnRecord(SN, SKUNO, StationName, var_ipaddress, var_workorderno, APDB);
            }


        }

        /// <summary>
        /// 生成虛擬條碼記錄
        /// add by hgb 2019.05.29
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SKUNO"></param>
        /// <param name="StationName"></param>
        /// <param name="var_ipaddress"></param>
        /// <param name="var_workorderno"></param>
        /// <param name="DB"></param>
        public void MakeVirtualSnRecord(string SN, string SKUNO, string StationName, string var_ipaddress, string var_workorderno, OleExec DB)
        {
            string l_wo = "00" + var_workorderno.Substring(2, 10);

            string sql = $@" 
        SELECT data4
      FROM mes4.r_ap_temp
     WHERE data1 = '{var_ipaddress}'
       AND data2 = '{StationName}'
       AND data4 = '{l_wo}'
       AND rownum < 2";
            DataTable dt = DB.ExecSelect(sql, null).Tables[0];

            sql = $@" 
       select mes1.get_customer_partno('KP_NO', '{SKUNO}') from dual ";
            DataTable dt_temp = DB.ExecSelect(sql, null).Tables[0];
            string cust_kp = string.Empty;
            if (dt_temp.Rows.Count > 0)
            {
                 cust_kp = dt_temp.Rows[0][0].ToString();
            }
            string test1 = cust_kp.Substring(cust_kp.Length - 4, 4);
            string test2 = SN.Substring(0, 4);
            if (dt.Rows.Count == 0 && SN.Length == 10 && cust_kp.Substring(cust_kp.Length - 4, 4) == SN.Substring(0, 4))
            {
                sql = $@" 
       SELECT link_qty               
              FROM mes1.c_product_config
             WHERE p_no ='{SKUNO}'
               AND p_version IN
                   (SELECT p_version
                      FROM mes4.r_wo_base
                     WHERE wo ='{l_wo}')
               AND rownum < 2 ";
                dt = DB.ExecSelect(sql, null).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    string var_link_qty = dt.Rows[0][0].ToString();

                    sql = $@" 
       INSERT INTO mes4.r_ap_temp
              (data1, data2, data3, data4, data5, data6, work_time)
            VALUES
              ('{var_ipaddress}',
               '{StationName}',
               '{SN}',
               '{l_wo}',
               '{var_link_qty}',
               '0',
               SYSDATE) ";                 
                    //DB.ExecSQL(sql);
                    DB.ExecSqlNoReturn(sql, null);
                    DB.CommitTrain();

                }
            }


        }


        /// <summary>
        /// AOI Allparts資料檢查
        /// </summary>
        /// <param name="g_psn"></param>
        /// <param name="g_event"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string PTHDataCheck(string G_PSN, string G_STATION, string G_LINE, OleExec DB)
        {
            OleExec apdb = DB;
            string Psn = G_PSN;
            string Event = G_STATION;
            string Line = G_LINE;
            OleDbParameter[] AOISP = new OleDbParameter[4];
            AOISP[0] = new OleDbParameter("G_PSN", Psn);
            AOISP[1] = new OleDbParameter("G_STATION", Event);
            AOISP[2] = new OleDbParameter("G_LINE", Line);
            AOISP[3] = new OleDbParameter();
            AOISP[3].Size = 1000;
            AOISP[3].ParameterName = "res";
            AOISP[3].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.Z_Sn_PTHASS_Sp", AOISP);
            return result;
        }


        public string SnMaterialPTHAll(string g_psn, OleExec DB)
        {
            OleExec apdb = DB;
            string Psn = g_psn;
            OleDbParameter[] PTHSP = new OleDbParameter[2];
            PTHSP[0] = new OleDbParameter("g_psn", Psn);
            PTHSP[1] = new OleDbParameter();
            PTHSP[1].Size = 1000;
            PTHSP[1].ParameterName = "res";
            PTHSP[1].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.check_psn_material_PTH_ALL", PTHSP);
            return result;
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="PanelSN"></param>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string PCBAAPMaterial(string SSN, string Loaction, ref Dictionary<string, string> DicAPMaterial, OleExec DB)
        {
            System.Data.OleDb.OleDbParameter[] PanelReplaceSP = new System.Data.OleDb.OleDbParameter[]
                {
                new System.Data.OleDb.OleDbParameter("MYPSN", SSN),
                new System.Data.OleDb.OleDbParameter("MYLOCATION", Loaction),
                new System.Data.OleDb.OleDbParameter("g_tr_sn", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_kp_no", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_mfr_kp_no", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_mfr_code", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_mfr_name", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_date_code", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_lot_code", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("g_kp_desc", System.Data.OleDb.OleDbType.VarChar, 5000),
                new System.Data.OleDb.OleDbParameter("RES", System.Data.OleDb.OleDbType.VarChar, 5000),
                };
            for (int i = 2; i < PanelReplaceSP.Length; i++)
            {
                PanelReplaceSP[i].Direction = ParameterDirection.Output;
            }
            string SqlError = DB.ExecProcedureNoReturn("mes1.get_kp_message_new", PanelReplaceSP);
            DicAPMaterial["TR_SN"] = PanelReplaceSP[2].Value.ToString()/*"g_tr_sn"*/;
            DicAPMaterial["KP_NO"] = PanelReplaceSP[3].Value.ToString()/*"g_tr_sn"*/;
            DicAPMaterial["MFR_KP_NO"] = PanelReplaceSP[4].Value.ToString();
            DicAPMaterial["MFR_CODE"] = PanelReplaceSP[5].Value.ToString();
            DicAPMaterial["MFR_NAME"] = PanelReplaceSP[6].Value.ToString();
            DicAPMaterial["DATE_CODE"] = PanelReplaceSP[7].Value.ToString();
            DicAPMaterial["LOT_CODE"] = PanelReplaceSP[8].Value.ToString();
            DicAPMaterial["KP_DESC"] = PanelReplaceSP[9].Value.ToString();
            DicAPMaterial["RES"] = PanelReplaceSP[10].Value.ToString();
            return SqlError;
        }
        public string OraclOleDbParameter(string OraclSP, OleDbParameter[] INPUTOUTOleDbParameter, ref Dictionary<string, string> DicAPMaterial,/*ref OleDbParameter[] OUTOleDbParameter,*/ OleExec DB)
        {
            string SqlError = DB.ExecProcedureNoReturn(OraclSP, INPUTOUTOleDbParameter);
            //for (int i = 2; i < INPUTOUTOleDbParameter.Length; i++)
            //{
            //    OUTOleDbParameter[i] = INPUTOUTOleDbParameter[i];
            //}
            for (int i = 0; i < INPUTOUTOleDbParameter.Length; i++)
            {
                DicAPMaterial[INPUTOUTOleDbParameter[i].ToString()] = INPUTOUTOleDbParameter[i].Value.ToString();
            }
            return SqlError;
        }

        public DataRow GETSkuVerByWoFromAP(string Wo, OleExec DB)
        {
            DataRow AP_WO_DataRow = null;
            string strSql = $@"select * from mes4.r_wo_base where wo=:Wo and rownum=1";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":Wo", Wo)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                AP_WO_DataRow = res.Rows[0];
            }
            return AP_WO_DataRow;
        }

        /// <summary>
        /// Pallet Rohs資料檢查
        /// </summary>
        /// <param name="TRANTYPE"></param>
        /// <param name="PalletNo"></param>
        /// <param name="LANGUAGE_CODE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string PalletRohsDataCheckbySP(string TRANTYPE, string PalletNo, string LANGUAGE_CODE, OleExec DB)
        {
            OleExec SFCdb = DB;
            //string Psn = G_PSN;
            //string Event = G_STATION;
            //string Line = G_LINE;
            //SKU_VER in varchar2,OLD_SKU_VER in varchar2,LANGUAGE_CODE in varchar2,OUT_RES out varchar2
            OleDbParameter[] Paras = new OleDbParameter[7];
            Paras[0] = new OleDbParameter("TRANTYPE", TRANTYPE);
            Paras[1] = new OleDbParameter("SN", PalletNo);
            Paras[2] = new OleDbParameter("SKUNO", "");
            Paras[3] = new OleDbParameter("SKU_VER", "");
            Paras[4] = new OleDbParameter("OLD_SKU_VER", "");
            Paras[5] = new OleDbParameter("LANGUAGE_CODE", "");
            Paras[6] = new OleDbParameter();
            Paras[6].Size = 1000;
            Paras[6].ParameterName = "OUT_RES";
            Paras[6].Direction = System.Data.ParameterDirection.Output;
            string result = SFCdb.ExecProcedureNoReturn("SFC.CHECK_ROHS", Paras);
            return result;
        }

        /// <summary>
        /// Pallet AutoTest資料檢查
        /// </summary>
        /// <param name="g_psn"></param>
        /// <param name="g_event"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string PalletAutoTestDataCheckbySP(string PalletNo, string BU, string LANGUAGE_CODE, OleExec DB)
        {
            OleExec SFCdb = DB;
            OleDbParameter[] Paras = new OleDbParameter[4];
            Paras[0] = new OleDbParameter("PALLET_NO", PalletNo);
            Paras[1] = new OleDbParameter("BU", BU);
            Paras[2] = new OleDbParameter("LANGUAGE_CODE", LANGUAGE_CODE);
            Paras[3] = new OleDbParameter();
            Paras[3].Size = 1000;
            Paras[3].ParameterName = "OUT_RES";
            Paras[3].Direction = System.Data.ParameterDirection.Output;
            string result = SFCdb.ExecProcedureNoReturn("SFC.CHECK_AUTO_TEST_CBS", Paras);
            return result;
        }




        /// <summary>
        /// 獲取AP處link的SN,AOI自動過站用
        /// </summary>
        /// <param name="FileSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<DataRow> GETAOILinkSN(string FileSN, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql;
            //根絕SN長度區分SQL
            if (FileSN.Length == 9)
            {
                strSql = $@"select p_sn, isnull(error_code, '') as error_code from mes4.r_temp_replace where temp_sn =:filesn";
            }
            else
            {
                strSql = $@"select p_sn, '' as error_code from mes4.r_sn_link where sn_code in (select sn_code from mes4.r_sn_link where p_sn = :filesn union select :filesn from dual)";
            }

            OleDbParameter[] paramet = new OleDbParameter[]
               {
                    new OleDbParameter(":filesn", FileSN)};

            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190324171936"));

            }
            return datarowlist;
        }

        /// <summary>
        /// 獲取AP相關數據
        /// </summary>
        /// <param name="TR_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSMTCheckListByTRSN(string TR_SN, OleExec DB)
        {
            string SqlStr = $@"
                SELECT MFR_NAME,DATE_CODE,LOT_CODE
                FROM MES4.R_TR_SN A, MES1.C_MFR_CONFIG B
                WHERE TR_SN =:tr_sn AND A.MFR_CODE=B.MFR_CODE
            ";
            OleDbParameter[] paramet = new OleDbParameter[]
              {
                    new OleDbParameter(":tr_sn", TR_SN)};

            DataTable res = DB.ExecuteDataTable(SqlStr, CommandType.Text, paramet);

            if (res.Rows.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190324164737", new string[] { TR_SN }));
            }


            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (DataColumn col in res.Columns)
            {
                dic.Add(col.ColumnName.ToString(), res.Rows[0][col.ColumnName].ToString());
            }
            return dic;
        }
        public string GetMaxSNSave(string SSN, OleExec DB)
        {
            OleDbParameter[] PanelReplaceSP = new OleDbParameter[10];
            PanelReplaceSP[0] = new OleDbParameter("MYPSN", SSN);
            PanelReplaceSP[1] = new OleDbParameter();
            PanelReplaceSP[1].Size = 5000;
            PanelReplaceSP[1].ParameterName = "RES";
            PanelReplaceSP[12].Direction = System.Data.ParameterDirection.Output;
            string SqlError = DB.ExecProcedureNoReturn("mes1.get_kp_message_new", PanelReplaceSP);
            return SqlError;
        }

        /// <summary>
        /// HWT SMTLOADING掃描TRSN時調用AP SP判斷並生成TR_CODE
        /// </summary>
        /// <param name="PanelSN"></param>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AP_get_trcode(string TRSN, string WO, string IP, string EMP_NO, string PROCESS, OleExec DB)
        {
            //string ErrMessage = "";
            //Psn = PanelSession.InputValue.ToString();

            OleDbParameter[] PanelReplaceSP = new OleDbParameter[7];
            PanelReplaceSP[0] = new OleDbParameter("g_trsn", TRSN);
            PanelReplaceSP[1] = new OleDbParameter("g_wo", WO);
            PanelReplaceSP[2] = new OleDbParameter("mac_address", IP);
            PanelReplaceSP[3] = new OleDbParameter("g_emp_no", EMP_NO);
            PanelReplaceSP[4] = new OleDbParameter("g_process", PROCESS);
            PanelReplaceSP[5] = new OleDbParameter();
            PanelReplaceSP[5].Size = 1000;
            PanelReplaceSP[5].ParameterName = "v_trcode";
            PanelReplaceSP[5].Direction = System.Data.ParameterDirection.Output;
            PanelReplaceSP[6] = new OleDbParameter();
            PanelReplaceSP[6].Size = 1000;
            PanelReplaceSP[6].ParameterName = "res";
            PanelReplaceSP[6].Direction = System.Data.ParameterDirection.Output;
            string result = DB.ExecProcedureNoReturn("MES1.GET_TRCODE", PanelReplaceSP);
            return result;
        }

        public string AP_z_insert_panel_snlink_new(string TRSN, string WO, string IP, string EMP_NO, string SN,string TRCODE, OleExec DB)
        {
            //string ErrMessage = "";
            //Psn = PanelSession.InputValue.ToString();

            OleDbParameter[] PanelReplaceSP = new OleDbParameter[10];
            PanelReplaceSP[0] = new OleDbParameter("g_trsn", TRSN);
            PanelReplaceSP[1] = new OleDbParameter("g_wo", WO);
            PanelReplaceSP[2] = new OleDbParameter("mac_address", IP);
            PanelReplaceSP[3] = new OleDbParameter("g_emp_no", EMP_NO);
            PanelReplaceSP[4] = new OleDbParameter("g_trcode", TRCODE);
            PanelReplaceSP[5] = new OleDbParameter("g_panelno", SN);
            PanelReplaceSP[6] = new OleDbParameter("g_link_qty", "");
            PanelReplaceSP[7] = new OleDbParameter("g_process", "");

            PanelReplaceSP[8] = new OleDbParameter();
            PanelReplaceSP[8].Size = 1000;
            PanelReplaceSP[8].ParameterName = "v_ext_qty";
            PanelReplaceSP[8].Direction = System.Data.ParameterDirection.Output;
            PanelReplaceSP[9] = new OleDbParameter();
            PanelReplaceSP[9].Size = 1000;
            PanelReplaceSP[9].ParameterName = "res";
            PanelReplaceSP[9].Direction = System.Data.ParameterDirection.Output;
            string result = DB.ExecProcedureNoReturn("MES1.z_insert_panel_snlink_new", PanelReplaceSP);
            return result;
        }

        public string AP_GET_LINKQTY(string WO, OleExec DB)
        {
            string result="",strsql;
            strsql = @"SELECT link_qty
                          FROM mes1.c_product_config t
                         WHERE (t.p_no, t.p_version) IN
                               (SELECT a.p_no, a.p_version
                                  FROM mes4.r_wo_base a
                                 WHERE a.wo = '"+ WO + "')";
            DataTable dt = DB.ExecSelect(strsql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0][0].ToString();
            }
            else
            {
                result = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163415");// "請聯繫PE檢查ALLPART機種配置資料！";
            }
            return result;
        }

        public string AP_R_FIXTURE_DETAIL_EVENT(string SN, string LINE, string STATIONNAME, string EMPNO, OleExec DB)
        {
            string result = "", strsql;

            strsql = "select * from mes1.c_fixture_base where fixture_sn = '" + SN + "'";
            DataTable dt = DB.ExecSelect(strsql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SN }));
            }

            strsql = "select *\n" +
            "  from mes4.r_fixture_detail a\n" +
            " where a.fixture_sn = '" + SN + "'\n" +
            "   and a.use_flag = 'EVENT'\n" +
            "   and a.start_time > sysdate - 5 / 60 / 1440";
            dt = DB.ExecSelect(strsql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                //載具記數，5分鍾內不記
                strsql = "update mes1.c_fixture_base set total_use_times = total_use_times + 1 where fixture_sn = '" + SN + "'";
                result = DB.ExecSQL(strsql);
            }
            //更新線體之前的載具
            strsql = "update mes4.r_fixture_detail set end_emp = '"+ EMPNO + "',end_time = sysdate where line_name =  '" + LINE + "' and use_flag = 'EVENT' and end_time is null";
            result = DB.ExecSQL(strsql);
            //記錄新的載具信息
            strsql = "insert into mes4.r_fixture_detail(fixture_sn,line_name,station_name,use_flag,start_emp,start_time) values('" + SN + "','" + LINE + "','" + STATIONNAME + "','EVENT','" + EMPNO + "',sysdate)";
            result = DB.ExecSQL(strsql);
            return result;

        }

        public int InsertRSNLink(string panel, List<string> Sns, string wo, string emp_no,OleExec DB)
        {
            int result = 0;
            Sns.ForEach(sn =>
            {
                var sql = $@"INSERT INTO MES4.R_SN_LINK(SN_CODE,P_SN,WO,WORK_TIME,EMP_NO) VALUES
                    ('{panel}','{sn}','{wo}',sysdate,'{emp_no}')";
                result+=DB.ExecuteNonQuery(sql,CommandType.Text,null);
            });
            return result;
        }

        /// <summary>
        /// 獲取MES1.C_PRODUCT_CONFIG表資料，DCN的機種版本邏輯比較奇葩
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="version"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<DataRow> GetCProductConfigBySkuAndVer(string skuno, string version, OleExec DB)
        {
            List<DataRow> dataRowList = new List<DataRow>();
            string strSql = $@"select * from mes1.c_product_config where p_no='{skuno}' and p_version='{version}'";
            if (version.Length == 1)
            {
                strSql = $@"select * from mes1.c_product_config where p_no='{skuno}' and (p_version='{version}' or p_version='{version}0')";
            }
            else if (version.Length == 3)
            {
                strSql = $@"select * from mes1.c_product_config where p_no='{skuno}' and (p_version='{version}' or p_version=substr('{version}',2,2))";
            }
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, null);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    dataRowList.Add(res.Rows[i]);
                }
            }
            return dataRowList;
        }

        /// <summary>
        /// 獲取Allpart料表，從DCN網站WebStation照搬過來
        /// </summary>
        /// <param name="skuNo"></param>
        /// <param name="version"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<DataRow> GetAllpartKpConfigBySkuAndVer(string skuNo, string version, OleExec DB)
        {
            List<DataRow> dataRowList = new List<DataRow>();
            string strSql = $@"
            select b.replace_kp_no, a.*
              from mes1.c_station_kp a
              left join mes1.c_replace_kp b
                on a.p_no = b.p_no
               and a.p_version = b.p_version
               and a.kp_no = b.kp_no
             where a.station_name = 'SMTLOADING'
               and a.p_no = '{skuNo}'
               and (a.p_version = '{version}' or a.p_version = '{version}0' or a.p_version = substr('{version}', 2, 2))";
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, null);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    dataRowList.Add(res.Rows[i]);
                }
            }
            return dataRowList;
        }

        /// <summary>
        /// 獲取Link工單
        /// </summary>
        /// <param name="wo"></param>
        /// <returns></returns>
        public List<DataRow> GetRVWOByWO(string wo, OleExec DB)
        {
            List<DataRow> dataRowList = new List<DataRow>();
            string strSql = $@"select * from mes4.r_v_wo a where exists(select 1 from mes4.r_v_wo b where a.v_wo=b.v_wo and b.t_wo= '{wo}')";
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, null);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    dataRowList.Add(res.Rows[i]);
                }
            }
            return dataRowList;
        }


        /// <summary>
        /// 把輸入的二維碼，PKG_ID轉TRSN
        /// </summary>
        /// <param name="trsn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetR2DSNRelation(string trsn, OleExec DB)
        {
            //List<DataRow> dataRowList = new List<DataRow>();
            string outputData = trsn;
            string strSql = $@"select * from mes4.r_2d_sn_relation where pkg_id = '{trsn}' or barcode_2d = '{trsn}'";
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, null);
            if (res.Rows.Count > 0)
            {
                outputData = res.Rows[0]["tr_sn"].ToString();
            }
            return outputData;
        }
    }
}
