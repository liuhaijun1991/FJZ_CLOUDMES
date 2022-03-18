using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class RepairKPReplace : MesAPIBase
    {
        protected APIInfo FGetRepairSNKP = new APIInfo()
        {
            FunctionName = "GetRepairSNKP",
            Description = "獲取維修SN的R_SN_KP表信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "KpNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Location", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetRepairMfrInfo = new APIInfo()
        {
            FunctionName = "GetRepairMfrInfo",
            Description = "獲取維修SN的MFR信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "KPNO", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCheckExistsESS = new APIInfo()
        {
            FunctionName = "CheckExistsESS",
            Description = "檢查維修SN的路由是否存在ESS工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "RepairSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "STATION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetRepairTRSNInfo = new APIInfo()
        {
            FunctionName = "GetRepairTRSNInfo",
            Description = "獲取維修替換時輸入的Allpart條碼信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "TRSN", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetRepairKPSNInfo = new APIInfo()
        {
            FunctionName = "GetRepairKPSNInfo",
            Description = "獲取維修替換時輸入KP信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "KPSN", InputType = "string", DefaultValue = "" } ,
                new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" } ,
                new APIInputInfo() { InputName = "LOCATION1", InputType = "string", DefaultValue = "" } ,
            },
            
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCheckExistsDC = new APIInfo()
        {
            FunctionName = "CheckExistsDC",
            Description = "檢查維修替換時輸入的DateCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "DATECODE", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateKPReplaceInfo = new APIInfo()
        {
            FunctionName = "UpdateKPReplaceInfo",
            Description = "更新維修替換KeyPart信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "KpNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Location", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "OldCode", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "NewCode", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() { InputName = "OldMPN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "NewMPN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCheckAllpartBySNLocation = new APIInfo()
        {
            FunctionName = "CheckAllpartBySNLocation",
            Description = "檢查該位置在MES4.R_TR_CODE_DETAIL、MES4.R_TR_PRODUCT_DETAIL表信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "RepairSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Location", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FRepairCheckOut = new APIInfo()
        {
            FunctionName = "RepairCheckOut",
            Description = "RepairCheckOut",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" }                
            },
            Permissions = new List<MESPermission>()
        };

        public RepairKPReplace()
        {
            this.Apis.Add(FGetRepairSNKP.FunctionName, FGetRepairSNKP);
            this.Apis.Add(FGetRepairMfrInfo.FunctionName, FGetRepairMfrInfo);
            this.Apis.Add(FCheckExistsESS.FunctionName, FCheckExistsESS);
            this.Apis.Add(FGetRepairTRSNInfo.FunctionName, FGetRepairTRSNInfo);
            this.Apis.Add(FGetRepairKPSNInfo.FunctionName, FGetRepairKPSNInfo);
            this.Apis.Add(FCheckExistsDC.FunctionName, FCheckExistsDC);
            this.Apis.Add(FUpdateKPReplaceInfo.FunctionName, FUpdateKPReplaceInfo);
            this.Apis.Add(FCheckAllpartBySNLocation.FunctionName, FCheckAllpartBySNLocation);
            this.Apis.Add(FRepairCheckOut.FunctionName, FRepairCheckOut);
        }

        /// <summary>
        /// 獲取維修SN的R_SN_KP表信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairSNKP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            OleExec APDB = null;
            try
            {
                APDB = this.DBPools["APDB"].Borrow();
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string sn = Data["SN"].ToString();
                string kpNo = Data["KpNo"].ToString();
                string location = Data["Location"].ToString();

                R_SN snObject = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn || t.BOXSN == sn).Where(t => t.VALID_FLAG == "1")
                    .ToList().FirstOrDefault();
                if (snObject == null)
                {
                    //throw new Exception("SN:" + sn + " 不存在或無效!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN: " + sn }));
                }

                string sql = "";
                string category = "PCBA";
                DataTable resDT = new DataTable();
                DataTable woTypeDT = woTypeDT = SFCDB.ORM.Queryable<R_WO_TYPE>().Where(t => t.PREFIX == snObject.WORKORDERNO.Substring(0, 6)).Select(t => t.CATEGORY).ToDataTable();
                if (woTypeDT.Rows.Count > 0)
                {
                    category = woTypeDT.Rows[0]["CATEGORY"].ToString();
                }
                if (category == "PCBA")
                {
                    sql = $@"
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time
                    from r_sn_kp where sn='{snObject.SN}' and partno='{kpNo}' and exvalue1 = '{location}'  and valid_flag='1'";
                    
                }
                else if (category == "VANILLA")
                {
                    sql = $@"
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn='{snObject.SN}' and partno='{kpNo}'  and valid_flag='1'
                    union all
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn in (select value from r_sn_kp where sn='{snObject.SN}') and partno='{kpNo}'  and valid_flag='1'
                    union all
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn in (select value from r_sn_kp where sn in (select value from r_sn_kp where sn='{snObject.SN}')) and partno='{kpNo}'  and valid_flag='1'";
                    resDT = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                }
                else if (category == "MODEL")
                {
                    sql = $@"
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn='{snObject.SN}' and partno='{kpNo}'  and valid_flag='1'
                    union all
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn in (select value from r_sn_kp where sn='{snObject.SN}') and partno='{kpNo}'  and valid_flag='1'
                    union all
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn in (select value from r_sn_kp where sn in (select value from r_sn_kp where sn='{snObject.SN}')) and partno='{kpNo}'  and valid_flag='1'
                    union all
                    select sn,partno,value,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,1,instr(value,'/')-1) else value end date_code,
                    case when instr(value,'/') > 0 and value <> 'N/A' then substr(value,instr(value,'/')+1) else 'N/A' end lot_code,
                    station,exvalue1,nvl(scantype,exvalue1) scantype,mpn,LOCATION,edit_time 
                    from r_sn_kp where sn in (select value from r_sn_kp where sn in (select value from r_sn_kp where sn in (select value from r_sn_kp where sn='{snObject.SN}'))) and partno='{kpNo}'  and valid_flag='1'";
                }
                resDT = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (resDT.Rows.Count == 0)
                {
                    //throw new Exception("SN:" + sn + " 無綁定KP信息!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814181159", new string[] {sn }));
                }
                resDT.Columns.Add("VENDER");
                for (int i = 0; i < resDT.Rows.Count; i++)
                {
                    if (resDT.Rows[i]["MPN"].ToString().Length > 0)
                    {
                        //string apSql = $@"select b.mfr_name as vender from mes1.c_mfr_kp_relation a , mes1.c_mfr_config b where a.mfr_code=b.mfr_code and a.cust_kp_no='{kpNo}'";
                        //有時候QE亂搞，設置KP時在KP後面加上'S/N'或者'P/N'之類，這就導致語句抓不到資料
                        string apSql = $@"select b.mfr_name as vender from mes1.c_mfr_kp_relation a , mes1.c_mfr_config b where a.mfr_code=b.mfr_code and substr('{kpNo}', 1, length(a.cust_kp_no))=a.cust_kp_no";
                        DataTable apDT = APDB.ExecuteDataTable(apSql, CommandType.Text, null);
                        if (apDT.Rows.Count == 0)
                        {
                            //MESStationBase Station = new MESStationBase();
                            //UIInputData O = new UIInputData()
                            //{
                            //    Timeout = 600000,
                            //    UIArea = new string[] { "50%", "40%" },
                            //    IconType = IconType.None,
                            //    Message = "確認",
                            //    Tittle = "Tip",
                            //    Type = UIInputType.Confirm,
                            //    Name = "",
                            //    ErrMessage = "請點擊確認按鈕！"
                            //};
                            //O.OutInputs.Add(new DisplayOutPut() { Name = "溫馨提示：", DisplayType = UIOutputType.TextArea.ToString(), Value = "THE MPN IS ERROR！" });
                            ////O.GetUiInput(Station.API, UIInput.Normal, Station);
                            //var inputObj = O.GetUiInput(this, UIInput.Normal);

                            //用GetUiInput報錯，直接報出來吧: 料號: {0} 在 Allpart 找不到 MPN 資料[c_mfr_kp_relation , c_mfr_config]!
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211102103846", new string[] { kpNo }));
                        }
                        resDT.Rows[i]["VENDER"] = apDT.Rows[0]["VENDER"].ToString();

                        //this.DBPools["APDB"].Return(APDB);
                    }
                    else
                    {
                        resDT.Rows[i]["VENDER"] = "";
                    }
                }

                StationReturn.Data = resDT;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                if (APDB != null)
                {
                    this.DBPools["APDB"].Return(APDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// 獲取維修SN的MFRPN表信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairMfrInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec APDB = null;
            try
            {
                APDB = this.DBPools["APDB"].Borrow();
                
                string kpNo = Data["KPNO"].ToString();
                string sql = "";
                sql = string.Format(@"
                    SELECT DISTINCT A.MFR_KP_NO AS MPN,
                                    B.MFR_NAME  AS VENDER
                      FROM MES1.C_MFR_KP_RELATION A, MES1.C_MFR_CONFIG B
                     WHERE A.MFR_CODE = B.MFR_CODE
                       AND A.CUST_KP_NO = '{0}'", kpNo);

                DataTable dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["MPN"] = "N/A";
                    dr["VENDER"]= "N/A";
                    dt.Rows.Add(dr);
                }

                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(APDB);
            }
            catch (Exception ex)
            {
                if (APDB != null)
                {
                    this.DBPools["APDB"].Return(APDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// 檢查維修SN的路由是否存在ESS工站
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CheckExistsESS(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string repairSN = Data["RepairSN"].ToString();
                R_SN snObject = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == repairSN || t.BOXSN == repairSN).Where(t => t.VALID_FLAG == "1")
                    .ToList().FirstOrDefault();
                if (snObject == null)
                {
                    //throw new Exception("SN:" + repairSN + " 不存在或無效!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151201", new string[] { repairSN }));

                }

                string station = Data["STATION"].ToString();
                DataTable dt = SFCDB.ORM.Queryable<R_SN, C_ROUTE_DETAIL, C_ROUTE_DETAIL>((r, s, t) => r.ROUTE_ID == s.ROUTE_ID && r.ROUTE_ID == t.ROUTE_ID)
                    .Where((r, s, t) => t.STATION_NAME == "ESS" && s.STATION_NAME == station && r.VALID_FLAG == "1" && r.SN == repairSN)
                    .Select((r, s, t) => new { SEQ_REPAIR = s.SEQ_NO, SEQ_ESS = t.SEQ_NO })
                    .ToDataTable();

                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// 獲取維修替換時輸入的Allpart條碼信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairTRSNInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec APDB = null;
            try
            {
                APDB = this.DBPools["APDB"].Borrow();

                string trSN = Data["TRSN"].ToString();
                string sql = "";
                sql = string.Format(@"
                    SELECT NVL(A.CUST_KP_NO, '') AS CUST_KP_NO,
                           NVL(A.MFR_KP_NO, '') AS MFR_KP_NO,
                           NVL(A.DATE_CODE, 'N/A') AS DATE_CODE,
                           NVL(A.LOT_CODE, 'N/A') AS LOT_CODE,
                           NVL(A.WORK_FLAG, 'N/A') AS WORK_FLAG,
                           NVL(A.LOCATION_FLAG, 'N/A') AS LOCATION_FLAG,
                           A.EXT_QTY,
                           A.QTY,
                           NVL(A.MFR_CODE, 'N/A') AS MFR_CODE,
                           NVL(B.MFR_NAME, 'N/A') AS MFR_NAME
                      FROM MES4.R_TR_SN A, MES1.C_MFR_CONFIG B 
                     WHERE A.MFR_CODE = B.MFR_CODE AND A.TR_SN = '{0}'", trSN);

                DataTable dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    //throw new Exception("無此Allpart條碼信息!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151201", new string[] { trSN }));
                }

                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(APDB);
            }
            catch (Exception ex)
            {
                if (APDB != null)
                {
                    this.DBPools["APDB"].Return(APDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
        public void GetRepairKPSNInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string KPSN = Data["KPSN"].ToString();
                string SERIALNO = Data["SN"].ToString();
                string LOCATION = Data["LOCATION1"].ToString();
                bool b = false;
                var bRegex = SFCDB.ORM.Queryable<R_SN_KP>().Where(x => x.SN == SERIALNO && x.VALID_FLAG == 1 && x.LOCATION == LOCATION).ToList().FirstOrDefault();
                if (!bRegex.KP_NAME.ToUpper().StartsWith("FAN"))
                {
                    var rsn = SFCDB.ORM.Queryable<R_SN>().Where(x => x.SN == KPSN && x.VALID_FLAG == "1").ToList().FirstOrDefault();
                    if (rsn != null)
                    {
                        if (rsn.COMPLETED_FLAG == "0")
                        {
                            StationReturn.MessagePara.Add($@"{KPSN}->未完工！");
                        }
                        else if (rsn.SHIPPED_FLAG == "1")
                        {
                            StationReturn.MessagePara.Add($@"{KPSN}->已LINK 其它SN,R_SN  SHIPPED_FLAG =1！");
                        }
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037";
                    }
                    var ss = SFCDB.ORM.Queryable<R_SN_KP>().Where(x => x.VALUE == KPSN && x.VALID_FLAG == 1 && x.LOCATION == LOCATION).Any();
                    if (ss)
                    {
                    
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037";
                        StationReturn.MessagePara.Add($@"{KPSN}->已LINK 其它SN！ R_SN_KP VALUE ");
                    }
                }
                b = Regex.IsMatch(KPSN, bRegex.REGEX);
                if (!b)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add($@"{KPSN}->與編碼規則不匹配,請確認！");
                }

                this.DBPools["SFCDB"].Return(SFCDB);

            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// 檢查維修替換時輸入的DateCode信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CheckExistsDC(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                string dateCode = Data["DATECODE"].ToString();
                DataTable dt = SFCDB.ORM.Queryable<R_SN_KP>()
                    .Where(t => t.VALUE == dateCode)
                    .ToDataTable();

                if (dt.Rows.Count > 0)
                {
                    //throw new Exception("該CSN在系統中已存在,請確認!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814182042", new string[] { dateCode }));
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// 更新維修替換KeyPart信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateKPReplaceInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                
                string sn = Data["SN"].ToString();
                string kpNo = Data["KpNo"].ToString();
                string location = Data["Location"].ToString();
                string oldCode= Data["OldCode"].ToString();
                string newCode = Data["NewCode"].ToString();
                //string oldMPN = Data["OldMPN"].ToString();
                string newMPN = Data["NewMPN"].ToString();
                DateTime dateTime = GetDBDateTime();
                T_WWN_DATASHARING t_wwn = new T_WWN_DATASHARING(SFCDB, DB_TYPE_ENUM.Oracle);
                var kpObj = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.PARTNO == kpNo && t.VALUE == oldCode && t.VALID_FLAG == 1)
                    .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(location), t => t.EXVALUE1 == location)
                    .ToList().FirstOrDefault();
                if (kpObj == null)
                {
                    throw new Exception($@"{sn},{kpNo},{oldCode},{location} Not In Keypart!");
                }
                //SFCDB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => new R_SN_KP
                //{
                //    VALUE = newCode,
                //    MPN = newMPN,
                //    EDIT_TIME = dateTime,
                //    EDIT_EMP = LoginUser.EMP_NO
                //}).Where(t => t.SN == sn && t.PARTNO == kpNo && t.VALUE == oldCode && t.EXVALUE1 == location).ExecuteCommand();

                kpObj.VALUE = newCode;
                kpObj.MPN = newMPN;
                kpObj.EDIT_EMP = LoginUser.EMP_NO;
                kpObj.EDIT_TIME = dateTime;
                int result = SFCDB.ORM.Updateable<R_SN_KP>(kpObj).Where(r => r.ID == kpObj.ID).ExecuteCommand();
                if (result == 0)
                {
                    throw new Exception($@"Update R_SN_KP Fail!");
                }
              t_wwn.ReplaceSnWWN(newCode, oldCode, SFCDB);
             
                T_R_SN t_r_sn = new T_R_SN(SFCDB, DBTYPE);
                var oldSNObj = t_r_sn.LoadSN(oldCode, SFCDB); ;
                if (oldSNObj != null)
                {
                    oldSNObj.SHIPDATE = null;
                    oldSNObj.SHIPPED_FLAG = "0";
                    oldSNObj.EDIT_EMP = LoginUser.EMP_NO;
                    oldSNObj.EDIT_TIME = dateTime;
                    t_r_sn.Update(oldSNObj, SFCDB);
                }
                var newSNObj = t_r_sn.LoadSN(newCode, SFCDB);
                if (newSNObj != null)
                {
                    if (newSNObj.SHIPPED_FLAG == "1")
                    {
                        throw new Exception($@"{newCode} Has Been Shipped");
                    }
                    if (newSNObj.SCRAPED_FLAG == "1")
                    {
                        throw new Exception($@"{newCode} Has Been Scraped");
                    }
                    if (newSNObj.REPAIR_FAILED_FLAG == "1")
                    {
                        throw new Exception($@"{newCode} Is In Repair");
                    }
                    if (newSNObj.COMPLETED_FLAG != "1")
                    {
                        throw new Exception($@"{newCode} Is Not Complete");
                    }
                    if (newSNObj.NEXT_STATION == "REWORK")
                    {
                        throw new Exception($@"{newCode} Is In Rework");
                    }
                    newSNObj.SHIPDATE = dateTime;
                    newSNObj.SHIPPED_FLAG = "1";
                    newSNObj.EDIT_EMP = LoginUser.EMP_NO;
                    newSNObj.EDIT_TIME = dateTime;
                    t_r_sn.Update(newSNObj, SFCDB);
                }
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(SFCDB, this.DBTYPE);
                R_SN_LOG snLog = new R_SN_LOG();
                snLog.ID = t_r_sn_log.GetNewID(this.BU, SFCDB);
                snLog.SN = kpObj.SN;
                snLog.SNID = kpObj.R_SN_ID;
                snLog.LOGTYPE = "REPLACE_KP";
                snLog.DATA1 = oldCode;
                snLog.DATA2 = newCode;
                snLog.DATA3 = kpObj.PARTNO;
                snLog.DATA4 = kpNo;
                snLog.DATA5 = kpObj.MPN;
                snLog.DATA6 = newMPN;
                snLog.FLAG = "1";
                snLog.CREATETIME = dateTime;
                snLog.CREATEBY = LoginUser.EMP_NO;
                t_r_sn_log.Save(SFCDB, snLog);
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        /// <summary>
        /// 檢查該位置在MES4.R_TR_CODE_DETAIL、MES4.R_TR_PRODUCT_DETAIL表信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CheckAllpartBySNLocation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec APDB = null;
            try
            {
                APDB = this.DBPools["APDB"].Borrow();

                string sql = string.Empty;
                string repairSN = Data["RepairSN"].ToString();
                string location = Data["Location"].ToString().Trim().ToUpper();

                #region SQL語句
                sql = string.Format(@"
                        SELECT DE.KP_NO,
                               DE.MFR_KP_NO,
                               DE.MFR_CODE,
                               DE.TR_SN,
                               DE.DATE_CODE,
                               DE.LOT_CODE,
                               DE.TR_CODE,
                               DE.SMT_CODE,
                               SUBSTR(PR.TR_CODE, 1, 8) AS STATION,
                               DE.P_NO,
                               DE.P_VERSION,
                               DE.SLOT_NO
                          FROM MES4.R_TR_PRODUCT_DETAIL PR,
                               MES4.R_TR_CODE_DETAIL    DE,
                               MES1.C_SMT_AP_LOCATION   LO
                         Where PR.P_SN = '{0}'
                           And LO.LOCATION = '{1}'
                           And PR.SMT_CODE = LO.SMT_CODE
                           AND LO.KP_NO = DE.KP_NO
                           AND PR.TR_CODE = DE.TR_CODE
                        Union
                        SELECT DE.KP_NO,
                               DE.MFR_KP_NO,
                               DE.MFR_CODE,
                               DE.TR_SN,
                               DE.DATE_CODE,
                               DE.LOT_CODE,
                               DE.TR_CODE,
                               DE.SMT_CODE,
                               SUBSTR(PR.TR_CODE, 1, 8) AS STATION,
                               DE.P_NO,
                               DE.P_VERSION,
                               DE.SLOT_NO
                          FROM MES4.R_TR_PRODUCT_DETAIL PR,
                               MES4.R_TR_CODE_DETAIL    DE,
                               MES1.C_STATION_KP        LO
                         Where PR.P_SN = '{0}'
                           And InStr(LO.LOCATION, '{1}') > 0
                           And DE.P_NO = LO.P_NO
                           And DE.P_VERSION = LO.P_VERSION
                           AND LO.KP_NO = DE.KP_NO
                           AND PR.TR_CODE = DE.TR_CODE", repairSN, location);
                #endregion
                var dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    //throw new Exception("Allparts無數據, 請確認選擇的PCBA以及輸入的Location是否正確!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814182419", new string[] { repairSN }));
                }
                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["APDB"].Return(APDB);
            }
            catch (Exception ex)
            {
                if (APDB != null)
                {
                    this.DBPools["APDB"].Return(APDB);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }
}
