using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class DCheckConfig : MesAPIBase
    {
        private APIInfo addNewRecord = new APIInfo()
        {
            FunctionName = "AddNewRecord",
            Description = "新增過站記錄",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName= "SN1Value",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "Packno",InputType ="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo getPackAndSN = new APIInfo()
        {
            FunctionName = "GetPackAndSN",
            Description = "獲取PACKSN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PackNo",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        /// <summary>
        /// 構造函數
        /// </summary>
        public DCheckConfig()
        {
            this.Apis.Add(getPackAndSN.FunctionName, getPackAndSN);
            this.Apis.Add(addNewRecord.FunctionName, addNewRecord);
        }
        /// <summary>
        /// 獲取PACK 里的SN
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetPackAndSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                string packno = Data["PackNo"].ToString().Trim();
                T_R_SN r_sn = new T_R_SN(sfcdb, DBTYPE);
                T_C_CUSTOMER c_cust = new T_C_CUSTOMER(sfcdb, DBTYPE);
                string getskuno = r_sn.GetSkuNotSE(packno, sfcdb);
                string typeskuno = c_cust.GetTypeSkuno(getskuno, sfcdb);

                List<string> station = new List<string> { "OBA", "CBS" };

                var aa = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
               .Where((rpk, rpk1, rspk, rsn) => (rpk.PACK_NO == packno || rpk1.PACK_NO == packno) && rsn.VALID_FLAG == "1" && (rsn.CURRENT_STATION == "OBA" || rsn.CURRENT_STATION == "CBS")).Select((rpk, rpk1, rspk, rsn) => rsn).ToList();
                if (aa.Count < 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"Only after OBA, CBS products can scan DOUBELCHECK, there are products in the pallet that are not in OBA status, please confirm";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                var FUNCION = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where((rpk) => rpk.SKUNO == getskuno && rpk.CATEGORY == "NOCARTON_CHECKSN").Select((rpk) => rpk).ToList();
                var bb = sfcdb.ORM.Queryable<R_PACKING>().Where((rpk) => rpk.PACK_NO == packno && rpk.PACK_TYPE == "PALLET").Select((rpk) => rpk).ToList();
                string runSql = "";
                if (bb.Count >= 1 && FUNCION.Count == 0 && (typeskuno != "SE" && typeskuno != "ARUBA" && typeskuno != "UFI"))
                {
                    runSql = $@"SELECT
	                                *
                                FROM
	                                (
		                                SELECT RP1.PACK_NO PACK_NO,
		                                RP2.PACK_NO CARTON,
                                        RP2.PACK_NO CARTONCHECK,
		                                '' SN,
		                                '' SN1,
		                                'OK' OK
	                                FROM
		                                R_PACKING RP1,
		                                R_PACKING RP2,
                                        R_MES_LOG LOG
	                                WHERE
		                                RP1.ID = RP2.PARENT_PACK_ID
		                                AND RP1.PACK_NO = '{packno}'
                                        AND RP2.PACK_NO=LOG.DATA1 
                                        AND LOG.PROGRAM_NAME = 'DOUBLECHECK'
                                        AND LOG.DATA2=RP1.PACK_NO
                                UNION
	                                SELECT
		                                RP1.PACK_NO PACK_NO,
		                                RP2.PACK_NO CARTON,
                                        '' CARTONCHECK,
		                                '' AS SN,
		                                '' SN1,
		                                '' OK
	                                FROM
		                                R_PACKING RP1,
		                                R_PACKING RP2
	                                WHERE
		                                RP1.ID = RP2.PARENT_PACK_ID
		                                AND RP1.PACK_NO = '{packno}'
		                                AND RP2.PACK_NO NOT IN (
			                                SELECT DATA1
		                                FROM
			                                R_MES_LOG LOG
		                                WHERE
			                                log.DATA1 = RP2.PACK_NO
                                            AND LOG.DATA2=RP1.PACK_NO
			                                AND log.PROGRAM_NAME = 'DOUBLECHECK')) a
                                ORDER BY
	                                a.CARTON";
                }
                else if (bb.Count >= 1 && FUNCION.Count >= 1)
                {
                    runSql = $@"SELECT * FROM (SELECT RP1.PACK_NO PACK_NO,RP2.PACK_NO  CARTON,RN.SN SN,LOG.DATA1 SN1,'OK' OK
                                  FROM R_PACKING           RP1,
                                       R_PACKING RP2,
                                       R_SN                RN,
                                       R_SN_PACKING RSP,
                                       R_MES_LOG LOG
                                 WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                       RP2.ID = RSP.PACK_ID AND
                                       RSP.SN_ID = RN.ID AND LOG.DATA1 = RN.SN
                                   AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}') AND
                                       RN.VALID_FLAG = 1 AND
                                       LOG.PROGRAM_NAME = 'DOUBLECHECK'
                                UNION
                                SELECT RP1.PACK_NO PACK_NO,RP2.PACK_NO CARTON,RN.SN AS SN,'' SN1,'' OK
                                  FROM R_PACKING           RP1,
                                       R_PACKING RP2,
                                       R_SN                RN,
                                       R_SN_PACKING RSP
                                 WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                       RP2.ID = RSP.PACK_ID AND
                                       RSP.SN_ID = RN.ID AND
                                       (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}') AND
                                       RN.VALID_FLAG = 1 AND
                                       rn.sn not in (select sn from R_MES_LOG rssd where rssd.DATA1 = rn.sn and RSSD.PROGRAM_NAME = 'DOUBLECHECK')) a order by a.sn1";
                }
                else
                {
                    runSql = $@"SELECT * FROM (SELECT RP1.PACK_NO PACK_NO,RP2.PACK_NO  CARTON,RN.SN SN,RSSD.SN SN1,'OK' OK
                                  FROM R_PACKING           RP1,
                                       R_PACKING RP2,
                                       R_SN                RN,
                                       R_SN_PACKING RSP,
                                       R_SN_STATION_DETAIL RSSD
                                 WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                       RP2.ID = RSP.PACK_ID AND
                                       RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                   AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}') AND
                                       RN.VALID_FLAG = 1 AND
                                       RSSD.STATION_NAME = 'DOUBLECHECK'
                                UNION
                                SELECT RP1.PACK_NO PACK_NO,RP2.PACK_NO CARTON,RN.SN AS SN,'' SN1,'' OK
                                  FROM R_PACKING           RP1,
                                       R_PACKING RP2,
                                       R_SN                RN,
                                       R_SN_PACKING RSP
                                 WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                       RP2.ID = RSP.PACK_ID AND
                                       RSP.SN_ID = RN.ID AND
                                       (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}') AND
                                       RN.VALID_FLAG = 1 AND
                                       rn.sn not in (select sn from R_SN_STATION_DETAIL rssd where rssd.sn = rn.sn and RSSD.STATION_NAME = 'DOUBLECHECK')) a order by a.sn1";
                }
                DataTable runDT = sfcdb.RunSelect(runSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Message = "獲取成功";
                StationReturn.Data = runDT;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetCheckShipOut(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                string packno = Data["PackNo"].ToString().Trim();
                T_R_SN r_sn = new T_R_SN(sfcdb, DBTYPE);
                T_C_CUSTOMER c_cust = new T_C_CUSTOMER(sfcdb, DBTYPE);
                string getskuno = r_sn.GetSkuNotSE(packno, sfcdb);
                string typeskuno = c_cust.GetTypeSkuno(getskuno, sfcdb);
                List<string> station = new List<string> { "OBA", "CBS" };

                //var aa = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
                //.Where((rpk, rpk1, rspk, rsn) => rpk.PACK_NO == packno && rsn.VALID_FLAG == "1" && rsn.NEXT_STATION != "OBA").Select((rpk, rpk1, rspk, rsn) => rsn).ToList();
                var FUNCION = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where((rpk) => rpk.SKUNO == getskuno && rpk.CATEGORY == "NOCARTON_CHECKSN").Select((rpk) => rpk).ToList();

                var aa = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
               .Where((rpk, rpk1, rspk, rsn) => (rpk.PACK_NO == packno || rpk1.PACK_NO == packno) && rsn.VALID_FLAG == "1" && rsn.NEXT_STATION == "SHIPOUT").Select((rpk, rpk1, rspk, rsn) => rsn).ToList();
                if (aa.Count < 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"Next Station not SHIPOUT";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                var bb = sfcdb.ORM.Queryable<R_PACKING>()
               .Where((rpk) => rpk.PACK_NO == packno && rpk.PACK_TYPE == "PALLET").Select((rpk) => rpk).ToList();
                string runSql = "";
                if (bb.Count >= 1  && FUNCION.Count == 0 && (typeskuno != "SE" && typeskuno != "ARUBA" && typeskuno != "UFI"))
                {
                    runSql = $@"SELECT
	                                *
                                FROM
	                                (
		                                SELECT RP1.PACK_NO PACK_NO,
		                                RP2.PACK_NO CARTON,
                                        RP2.PACK_NO CARTONCHECK,
		                                '' SN,
		                                '' SN1,
		                                CASE WHEN LOG.FLAG='1' THEN 'OK'
                                        ELSE 'RECHECK' END AS OK
	                                FROM
		                                R_PACKING RP1,
		                                R_PACKING RP2,
                                        R_SN_LOG LOG
	                                WHERE
		                                RP1.ID = RP2.PARENT_PACK_ID
		                                AND RP1.PACK_NO='{packno}'
                                        AND LOG.LOGTYPE = 'RECHECK'
                                        AND LOG.SN=RP2.PACK_NO
                                    ) a
                                ORDER BY
	                                a.CARTON";
                }
                else if (bb.Count >= 1 && FUNCION.Count != 0)
                {
                    runSql = $@"SELECT
	                                *
                                FROM
	                                (
		                                SELECT RP1.PACK_NO PACK_NO,
		                                RP2.PACK_NO CARTON,
                                        RSN.SN SNCHECK,
		                                RSN.SN,
		                                '' SN1,
		                                CASE WHEN LOG.FLAG='1' THEN 'OK'
                                        ELSE 'RECHECK' END AS OK
	                                FROM
	                                	R_SN RSN,
	                                	R_SN_PACKING RSNP,
		                                R_PACKING RP1,
		                                R_PACKING RP2,
                                        R_SN_LOG LOG
	                                WHERE
	                                	RSN.ID= RSNP.SN_ID AND RSNP.PACK_ID= RP2.ID AND
		                                RP1.ID = RP2.PARENT_PACK_ID
		                                AND RP1.PACK_NO='{packno}'
                                        AND LOG.LOGTYPE = 'RECHECK'
                                        AND LOG.SN=RP2.PACK_NO
                                    ) a
                                ORDER BY
	                                a.CARTON";
                }
                else
                {
                    runSql = $@"select * from (
                        select rp1.pack_no pack_no,
                               rp2.pack_no  carton,
                               rn.sn sn,
                               log.sn sn1,
                         case when log.flag='1' then 'ok'
                               else 'recheck' end as ok
                          from r_packing           rp1,
                               r_packing rp2,
                               r_sn                rn,
                               r_sn_packing rsp,
                               r_sn_log log
                         where rp1.id = rp2.parent_pack_id and
                               rp2.id = rsp.pack_id and
                               rsp.sn_id = rn.id and log.sn = rn.sn
                           and (rp1.pack_no = '{packno}' or rp2.pack_no = '{packno}') and
                               rn.valid_flag = 1 and
                               log.logtype = 'RECHECK'
                       ) a order by a.sn1";
                }
                DataTable runDT = sfcdb.RunSelect(runSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Message = "獲取成功";
                StationReturn.Data = runDT;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 新增过站记录
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddNewRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string sN1Value = Data["SN1Value"].ToString().Trim();
            string packno = Data["Packno"].ToString().Trim();

            if (string.IsNullOrEmpty(sN1Value))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "SN不能為空";
                StationReturn.MessagePara.Add("SN1Value");
                StationReturn.Data = "";
                return;
            }
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                T_R_SN r_sn = new T_R_SN(sfcdb, DBTYPE);
                T_C_CUSTOMER c_cust = new T_C_CUSTOMER(sfcdb, DBTYPE);
                T_R_PACKING R_PACKING = new T_R_PACKING(sfcdb, DBTYPE);
                string getskuno = r_sn.GetSkuNotSE(packno, sfcdb);
                string typeskuno = c_cust.GetTypeSkuno(getskuno, sfcdb);

                string sql = string.Empty;
                var FUNCION = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where((rpk) => rpk.SKUNO == getskuno && rpk.CATEGORY == "NOCARTON_CHECKSN").Select((rpk) => rpk).ToList();
                var cartonobj = sfcdb.ORM.Queryable<R_PACKING, R_PACKING>((rp1, rp2) => rp2.ID == rp1.PARENT_PACK_ID && rp2.PACK_NO == packno).Where((rp1, rp2) => rp1.PACK_NO == sN1Value && rp1.PACK_TYPE == "CARTON").Select((rp1, rp2) => rp1)
                          .ToList().FirstOrDefault();
                var snobj = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rs, rsp, rp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID).Where((rs, rsp, rp) => rs.SN == sN1Value && rs.VALID_FLAG == "1" && rp.PACK_NO == packno).Select((rs, rsp, rp) => rs)
                          .ToList().FirstOrDefault();
                if (snobj != null && FUNCION.Count == 0)
                {
                    var snRecord = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == snobj.SN && t.WORKORDERNO == snobj.WORKORDERNO && t.STATION_NAME == "DOUBLECHECK" && t.VALID_FLAG == "1")
                         .ToList();
                    if (snRecord.Count > 0)
                    {
                        StationReturn.Message = $@"SN :{sN1Value} 已扫描DOUBLECHECK";
                        this.DBPools["SFCDB"].Return(sfcdb);
                        return;
                    }
                    else
                    {
                        T_R_SN table = new T_R_SN(sfcdb, this.DBTYPE);
                        table.RecordPassStationDetail(snobj, "", "DOUBLECHECK", "DOUBLECHECK", BU, sfcdb, "0");

                        string sqlCount = $@"SELECT count(RSSD.SN) SN
                                              FROM R_PACKING           RP1,
                                                   R_PACKING RP2,
                                                   R_SN                RN,
                                                   R_SN_PACKING RSP,
                                                   R_SN_STATION_DETAIL RSSD
                                             WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                                   RP2.ID = RSP.PACK_ID AND
                                                   RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                               AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}')  AND
                                                   RN.VALID_FLAG = 1 AND
                                                   RSSD.STATION_NAME = 'DOUBLECHECK'";

                        DataTable countDT = sfcdb.RunSelect(sqlCount).Tables[0];
                        string StrCount = "0";
                        if (countDT.Rows.Count > 0)
                        {
                            StrCount = Convert.ToString(countDT.Rows[0]["SN"].ToString());
                        }
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "OK";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                }
                else if (cartonobj != null && FUNCION.Count == 0)
                {
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = TRML.GetNewID(BU, sfcdb);
                    log.PROGRAM_NAME = "DOUBLECHECK";
                    log.CLASS_NAME = "MESStation.Config.DCheckConfig";
                    log.FUNCTION_NAME = "AddNewRecord";
                    log.DATA1 = sN1Value;
                    log.DATA2 = packno;
                    log.EDIT_EMP = LoginUser.EMP_NO;
                    log.EDIT_TIME = TRML.GetDBDateTime(sfcdb);
                    TRML.InsertMESLogOld(log, sfcdb);
                    string sqlCount = $@"SELECT count(RSSD.SN) SN
                                      FROM R_PACKING           RP1,
                                           R_PACKING RP2,
                                           R_SN                RN,
                                           R_SN_PACKING RSP,
                                           R_SN_STATION_DETAIL RSSD
                                     WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                           RP2.ID = RSP.PACK_ID AND
                                           RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                       AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}')  AND
                                           RN.VALID_FLAG = 1 AND
                                           RSSD.STATION_NAME = 'DOUBLECHECK'";

                    DataTable countDT = sfcdb.RunSelect(sqlCount).Tables[0];
                    string StrCount = "0";
                    if (countDT.Rows.Count > 0)
                    {
                        StrCount = Convert.ToString(countDT.Rows[0]["SN"].ToString());
                    }
                    StationReturn.Data = StrCount;
                    StationReturn.Message = "OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else if (FUNCION.Count >= 1)
                {
                    if (typeskuno == "SE")
                    {
                        string checksnse = sN1Value.Substring(0, 1);
                        var categorylist = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == getskuno && t.CATEGORY == "SN-PREPROCESSOR").ToList();
                        if (checksnse != "S" && categorylist.Count != 0)
                        {
                            //StationReturn.Data = "";
                            //StationReturn.Message = "YOU NEED SCAN LABEL CARTON";
                            //StationReturn.Status = StationReturnStatusValue.Fail;
                            throw new Exception("YOU NEED SCAN LABEL CARTON");
                        }
                        T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(sfcdb, DBTYPE);
                        sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");
                    }
                    else if (typeskuno == "ARUBA" )
                    {
                        T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(sfcdb, DBTYPE);
                        sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");
                    }
                    else if (typeskuno == "JUNIPER")
                    {
                        T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(sfcdb, DBTYPE);
                        sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");
                    }
                    string checksn = R_PACKING.CheckSNinPallet(sN1Value, packno, sfcdb);
                    if (checksn == "")
                    {
                        StationReturn.Data = "";
                        StationReturn.Message = "Can't found " + sN1Value + " in PALLET/CARTON. Please check again";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                    else
                    {
                        R_MES_LOG log = new R_MES_LOG();
                        log.ID = TRML.GetNewID(BU, sfcdb);
                        log.PROGRAM_NAME = "DOUBLECHECK";
                        log.CLASS_NAME = "MESStation.Config.DCheckConfig";
                        log.FUNCTION_NAME = "AddNewRecord";
                        log.DATA1 = sN1Value;
                        log.DATA2 = packno;
                        log.EDIT_EMP = LoginUser.EMP_NO;
                        log.EDIT_TIME = TRML.GetDBDateTime(sfcdb);
                        TRML.InsertMESLogOld(log, sfcdb);
                        string sqlCount = $@"SELECT count(RSSD.SN) SN
                                              FROM R_PACKING RP1,
                                                   R_PACKING RP2,
                                                   R_SN RN,
                                                   R_SN_PACKING RSP,
                                                   R_SN_STATION_DETAIL RSSD
                                             WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                                   RP2.ID = RSP.PACK_ID AND
                                                   RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                               AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}')  AND
                                                   RN.VALID_FLAG = 1 AND
                                                   RSSD.STATION_NAME = 'DOUBLECHECK'";

                        DataTable countDT = sfcdb.RunSelect(sqlCount).Tables[0];
                        string StrCount = "0";
                        if (countDT.Rows.Count > 0)
                        {
                            StrCount = Convert.ToString(countDT.Rows[0]["SN"].ToString());
                        }
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "OK";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }

                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Message = "Can't found " + sN1Value + " in PALLET/CARTON. Please check CARTON/SN Correct";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 新增过站记录
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddNewRecordCheckOutReCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            string sN1Value = Data["SN1Value"].ToString().Trim();
            string packno = Data["Packno"].ToString().Trim();
            T_R_SN r_sn = new T_R_SN(sfcdb, DBTYPE);
            T_C_CUSTOMER c_cust = new T_C_CUSTOMER(sfcdb, DBTYPE);
            T_R_PACKING R_PACKING = new T_R_PACKING(sfcdb, DBTYPE);
            string getskuno = r_sn.GetSkuNotSE(packno, sfcdb);
            string typeskuno = c_cust.GetTypeSkuno(getskuno, sfcdb);
            if (string.IsNullOrEmpty(sN1Value))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "SN do not null";
                StationReturn.MessagePara.Add("SN1Value");
                StationReturn.Data = "";
                return;
            }
            try
            {
                T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                string sql = string.Empty;
                var FUNCION = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where((rpk) => rpk.SKUNO == getskuno && rpk.CATEGORY == "NOCARTON_CHECKSN").Select((rpk) => rpk).ToList();
                var cartonobj = sfcdb.ORM.Queryable<R_PACKING, R_PACKING>((rp1, rp2) => rp1.PARENT_PACK_ID == rp2.ID).Where((rp1, rp2) => rp1.PACK_NO == sN1Value && rp1.PACK_TYPE == "CARTON" && rp2.PACK_NO == packno).Select((rp1, rp2) => rp1)
                         .ToList().FirstOrDefault();
                var snobj = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rs, rsp, rp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID).Where((rs, rsp, rp) => rs.SN == sN1Value && rs.VALID_FLAG == "1" && rp.PACK_NO == packno).Select((rs, rsp, rp) => rs)
                          .ToList().FirstOrDefault();
                var snobjcarton = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
                    .Where((rpk, rpk1, rspk, rsn) => (rpk.PACK_NO == packno || rpk1.PACK_NO == packno) && rsn.VALID_FLAG == "1" && rsn.SN == sN1Value).Select((rpk, rpk1, rspk, rsn) => rsn).ToList();
                if (snobj != null && FUNCION.Count == 0)
                {
                    //var snRecord = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == snobj.SN && t.WORKORDERNO == snobj.WORKORDERNO && t.STATION_NAME == "CHECKOUTCHECK" && t.VALID_FLAG == "1")
                    //     .ToList();
                    //if (snRecord.Count > 0)
                    //{
                    //    StationReturn.Message = $@"SN :{sN1Value} 已扫描 CHECKOUTCHECK";
                    //    this.DBPools["SFCDB"].Return(sfcdb);
                    //    return;
                    //}
                    //else
                    //{
                    T_R_SN table = new T_R_SN(sfcdb, this.DBTYPE);
                    sfcdb.BeginTrain();
                    string flag = table.RecordPassStationDetail(snobj, "", "CHECKOUTCHECK", "CHECKOUTCHECK", BU,
                            sfcdb, "0");
                    if (!"1".Equals(flag))
                    {
                        sfcdb.RollbackTrain();
                        string StrCount = "0";
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "FAIL";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        return;
                    }
                    try
                    {
                        var f = sfcdb.ORM.Updateable<R_SN_LOG>().SetColumns(t => new R_SN_LOG { FLAG = "1", DATA8 = GetDBDateTime().ToString(), DATA9 = LoginUser.EMP_NO }).Where(t => t.SN == sN1Value).ExecuteCommand();
                        if (Convert.ToInt16(f) > 0)
                        {
                            string StrCount = "0";
                            StationReturn.Data = StrCount;
                            StationReturn.Message = "OK";
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            sfcdb.CommitTrain();
                        }
                        else
                        {
                            string StrCount = "0";
                            StationReturn.Data = StrCount;
                            StationReturn.Message = "FAIL";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            sfcdb.RollbackTrain();
                        }
                    }
                    catch (Exception ex)
                    {
                        string StrCount = "0";
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "FAIL " + ex.Message;
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        sfcdb.RollbackTrain();
                    }
                    //}
                }
                else if (cartonobj != null && FUNCION.Count == 0)
                {
                    try
                    {
                        var f = sfcdb.ORM.Updateable<R_SN_LOG>().SetColumns(t => new R_SN_LOG { FLAG = "1", DATA8 = GetDBDateTime().ToString(), DATA9 = LoginUser.EMP_NO }).Where(t => t.SN == sN1Value).ExecuteCommand();
                        if (Convert.ToInt16(f) > 0)
                        {
                            string StrCount = "0";
                            StationReturn.Data = StrCount;
                            StationReturn.Message = "OK";
                            StationReturn.Status = StationReturnStatusValue.Pass;
                        }
                        else
                        {
                            string StrCount = "0";
                            StationReturn.Data = StrCount;
                            StationReturn.Message = "FAIL";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                        }
                    }
                    catch (Exception ex)
                    {
                        string StrCount = "0";
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "FAIL";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                }
                else if (snobjcarton != null &&  FUNCION.Count != 0)
                {
                    try
                    {
                        T_R_PACKING r_pack = null;
                        T_R_SN_LOG r_sn_log = null;
                        List<R_PACKING> cartonlist = new List<R_PACKING>();
                        List<R_SN_LOG> loglistcarton = new List<R_SN_LOG>();
                        r_pack = new T_R_PACKING(sfcdb, DBTYPE);
                        r_sn_log = new T_R_SN_LOG(sfcdb, DBTYPE);
                        T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(sfcdb, DBTYPE);
                        if (typeskuno == "SE" )
                        {
                            string checksnse = sN1Value.Substring(0, 1);
                            var categorylist = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == getskuno && t.CATEGORY == "SN-PREPROCESSOR" ).ToList();
                            if (checksnse != "S" && categorylist.Count !=0)
                            {
                                //StationReturn.Data = "";
                                //StationReturn.Message = "YOU NEED SCAN LABEL CARTON";
                                //StationReturn.Status = StationReturnStatusValue.Fail;
                                throw new Exception("YOU NEED SCAN LABEL CARTON");
                            }
                            sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");
                        }
                        else if (typeskuno == "ARUBA")
                        {
                            sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");
                        }
                        else if (typeskuno == "JUNIPER")
                        {
                            sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");
                        }
                        //sN1Value = c_sku_detail.SNPreprocessor(sfcdb, getskuno, sN1Value, "PACKING");

                        var checksninpl = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
                        .Where((rpk, rpk1, rspk, rsn) => (rpk.PACK_NO == packno || rpk1.PACK_NO == packno) && rsn.VALID_FLAG == "1" && rsn.SN == sN1Value).Select((rpk, rpk1, rspk, rsn) => rsn).ToList();
                        if (checksninpl.Count == 0)
                        {
                            throw new Exception($@"This is SN: {sN1Value} not in {packno}");
                        }
                        
                        cartonlist = r_pack.GetCarton(sN1Value, sfcdb);
                        if (cartonlist != null)
                        {
                            string carton = cartonlist[0].PACK_NO;
                            var qty = cartonlist[0].QTY;
                            loglistcarton = r_sn_log.GetListCartonLog(carton, sfcdb);
                            var flag = loglistcarton[0].FLAG;
                            string data7 = loglistcarton[0].DATA7;
                            if (flag == "1")
                            {
                                string StrCount = "0";
                                StationReturn.Data = StrCount;
                                StationReturn.Message = "OK";
                                StationReturn.Status = StationReturnStatusValue.Pass;
                            }
                            else
                            {
                                var qtydata7 = Double.Parse(data7);
                                if (qty == qtydata7)
                                {
                                    var f = sfcdb.ORM.Updateable<R_SN_LOG>().SetColumns(t => new R_SN_LOG { FLAG = "1", DATA8 = GetDBDateTime().ToString(), DATA9 = LoginUser.EMP_NO }).Where(t => t.SN == carton).ExecuteCommand();
                                    if (Convert.ToInt16(f) > 0)
                                    {
                                        string StrCount = "0";
                                        StationReturn.Data = StrCount;
                                        StationReturn.Message = "OK";
                                        StationReturn.Status = StationReturnStatusValue.Pass;
                                    }
                                    else
                                    {
                                        string StrCount = "0";
                                        StationReturn.Data = StrCount;
                                        StationReturn.Message = "FAIL";
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                    }
                                }
                                else
                                {
                                    var qtydata7_ = qtydata7 + 1;
                                    string data7_ = qtydata7_.ToString();
                                    if (qty == qtydata7_)
                                    {
                                        var f1 = sfcdb.ORM.Updateable<R_SN_LOG>().SetColumns(t => new R_SN_LOG { FLAG = "1", DATA7 = data7_, DATA8 = GetDBDateTime().ToString(), DATA9 = LoginUser.EMP_NO }).Where(t => t.SN == carton).ExecuteCommand();
                                        if (Convert.ToInt16(f1) > 0)
                                        {
                                            string StrCount = "0";
                                            StationReturn.Data = StrCount;
                                            StationReturn.Message = "OK";
                                            StationReturn.Status = StationReturnStatusValue.Pass;
                                        }
                                        else
                                        {
                                            string StrCount = "0";
                                            StationReturn.Data = StrCount;
                                            StationReturn.Message = "FAIL";
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                        }
                                    }
                                    else
                                    {
                                        var f = sfcdb.ORM.Updateable<R_SN_LOG>().SetColumns(t => new R_SN_LOG { DATA7 = data7_, DATA8 = GetDBDateTime().ToString(), DATA9 = LoginUser.EMP_NO }).Where(t => t.SN == carton).ExecuteCommand();
                                        if (Convert.ToInt16(f) > 0)
                                        {
                                            string StrCount = "0";
                                            StationReturn.Data = StrCount;
                                            StationReturn.Message = "OK";
                                            StationReturn.Status = StationReturnStatusValue.Pass;
                                        }
                                        else
                                        {
                                            string StrCount = "0";
                                            StationReturn.Data = StrCount;
                                            StationReturn.Message = "FAIL";
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                        }

                                    }

                                }
                            }
                        }
                        else
                        {
                            StationReturn.Data = "";
                            StationReturn.Message = "Can't found " + sN1Value + " in PALLET/CARTON. Please check CARTON/SN Correct";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                        }

                    }
                    catch (Exception ex)
                    {
                        string StrCount = "0";
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "Can't found " + sN1Value + " in PALLET/CARTON. Please check CARTON/SN Correct";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Message = "Can't found " + sN1Value + " in PALLET/CARTON. Please check CARTON/SN Correct";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        static string _CATEGORY_SKUNO = "BE600M1,BE650G1,BR1500MS,BR1500MS2,BR700G,BX1500M,S9700-23D-J80,S9700-23D-JB4,S9700-53DX-JB4,S9705-48D-480,S9705-48D-4B4";
        /// <summary>
        /// Gel list Carton/SN
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetPackAndSNCanReset(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string packno = Data["PackNo"].ToString().Trim();

            try
            {

                List<string> station = new List<string> { "OBA", "CBS" };
                sfcdb = this.DBPools["SFCDB"].Borrow();
                //var aa = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
                //.Where((rpk, rpk1, rspk, rsn) => rpk.PACK_NO == packno && rsn.VALID_FLAG == "1" && rsn.NEXT_STATION != "OBA").Select((rpk, rpk1, rspk, rsn) => rsn).ToList();

                var aa = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rpk, rpk1, rspk, rsn) => rpk.ID == rpk1.PARENT_PACK_ID && rpk1.ID == rspk.PACK_ID && rspk.SN_ID == rsn.ID)
               .Where((rpk, rpk1, rspk, rsn) => (rpk.PACK_NO == packno || rpk1.PACK_NO == packno) && rsn.VALID_FLAG == "1" && (rsn.CURRENT_STATION == "OBA" || rsn.CURRENT_STATION == "CBS")).Select((rpk, rpk1, rspk, rsn) => rsn).ToList();
                if (aa.Count < 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = $@"Only after OBA, CBS products can scan CHECK, there are products in the pallet that are not in OBA status, please confirm";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                var bb = sfcdb.ORM.Queryable<R_PACKING>()
               .Where((rpk) => rpk.PACK_NO == packno && rpk.PACK_TYPE == "PALLET").Select((rpk) => rpk).ToList().FirstOrDefault();
                string runSql = "";
                if (bb != null && _CATEGORY_SKUNO.IndexOf(bb.SKUNO) < 0)
                {
                    runSql = $@"SELECT
	                                *
                                FROM
	                                (
		                                SELECT RP1.PACK_NO PACK_NO,
		                                RP2.PACK_NO CARTON,
                                        RP2.PACK_NO CARTONCHECK,
		                                '' SN,
		                                '' SN1,
		                                'OK' OK
	                                FROM
		                                R_PACKING RP1,
		                                R_PACKING RP2,
                                        R_MES_LOG LOG
	                                WHERE
		                                RP1.ID = RP2.PARENT_PACK_ID
		                                AND RP1.PACK_NO = '{packno}'
                                        AND RP2.PACK_NO=LOG.DATA1 
                                        AND LOG.PROGRAM_NAME = 'RESETCHECK'
                                        AND LOG.DATA2=RP1.PACK_NO
                                UNION
	                                SELECT
		                                RP1.PACK_NO PACK_NO,
		                                RP2.PACK_NO CARTON,
                                        '' CARTONCHECK,
		                                '' AS SN,
		                                '' SN1,
		                                '' OK
	                                FROM
		                                R_PACKING RP1,
		                                R_PACKING RP2
	                                WHERE
		                                RP1.ID = RP2.PARENT_PACK_ID
		                                AND RP1.PACK_NO = '{packno}'
		                                AND RP2.PACK_NO NOT IN (
			                                SELECT DATA1
		                                FROM
			                                R_MES_LOG LOG
		                                WHERE
			                                log.DATA1 = RP2.PACK_NO
                                            AND LOG.DATA2=RP1.PACK_NO
			                                AND log.PROGRAM_NAME = 'RESETCHECK')) a
                                ORDER BY
	                                a.CARTON";
                }
                else
                {
                    runSql = $@"SELECT * FROM (SELECT RP1.PACK_NO PACK_NO,RP2.PACK_NO  CARTON,RN.SN SN,RSSD.SN SN1,'OK' OK
          FROM R_PACKING           RP1,
               R_PACKING RP2,
               R_SN                RN,
               R_SN_PACKING RSP,
               R_SN_STATION_DETAIL RSSD
         WHERE RP1.ID = RP2.PARENT_PACK_ID AND
               RP2.ID = RSP.PACK_ID AND
               RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
           AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}') AND
               RN.VALID_FLAG = 1 AND
               RSSD.VALID_FLAG = 1 AND
               RSSD.STATION_NAME = 'RESETCHECK'
        UNION
        SELECT RP1.PACK_NO PACK_NO,RP2.PACK_NO CARTON,RN.SN AS SN,'' SN1,'' OK
          FROM R_PACKING           RP1,
               R_PACKING RP2,
               R_SN                RN,
               R_SN_PACKING RSP
         WHERE RP1.ID = RP2.PARENT_PACK_ID AND
               RP2.ID = RSP.PACK_ID AND
               RSP.SN_ID = RN.ID AND
               (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}') AND
               RN.VALID_FLAG = 1 AND
               rn.sn not in (select sn from R_SN_STATION_DETAIL rssd where rssd.sn = rn.sn and RSSD.STATION_NAME = 'RESETCHECK' AND RSSD.VALID_FLAG = 1 )) a order by a.sn1";
                }
                DataTable runDT = sfcdb.RunSelect(runSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Message = "獲取成功";
                StationReturn.Data = runDT;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        /// <summary>
        /// Method insert data to Database
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddNewRecordCanReset(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string sN1Value = Data["SN1Value"].ToString().Trim();
            string packno = Data["Packno"].ToString().Trim();
            if (string.IsNullOrEmpty(sN1Value))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "SN不能為空";
                StationReturn.MessagePara.Add("SN1Value");
                StationReturn.Data = "";
                return;
            }
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                string sql = string.Empty;
                var cartonobj = sfcdb.ORM.Queryable<R_PACKING, R_PACKING>((rp1, rp2) => rp2.ID == rp1.PARENT_PACK_ID && rp2.PACK_NO == packno).Where((rp1, rp2) => (rp1.PACK_NO == sN1Value && rp1.PACK_TYPE == "CARTON") || rp2.PACK_NO == packno).Select((rp1, rp2) => rp1)
                          .ToList().FirstOrDefault();
                var snobj = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rs, rsp, rp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID).Where((rs, rsp, rp) => rs.SN == sN1Value && rs.VALID_FLAG == "1" && rp.PACK_NO == packno).Select((rs, rsp, rp) => rs)
                          .ToList().FirstOrDefault();
                if (snobj != null)
                {
                    T_R_SN table = new T_R_SN(sfcdb, this.DBTYPE);
                    table.RecordPassStationDetail(snobj, "", "RESETCHECK", "RESETCHECK", BU,
                          sfcdb, "0");

                    string sqlCount = $@"SELECT count(RSSD.SN) SN
                                          FROM R_PACKING           RP1,
                                               R_PACKING RP2,
                                               R_SN                RN,
                                               R_SN_PACKING RSP,
                                               R_SN_STATION_DETAIL RSSD
                                         WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                               RP2.ID = RSP.PACK_ID AND
                                               RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                           AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}')  AND
                                               RN.VALID_FLAG = 1 AND
                                               RSSD.VALID_FLAG = 1 AND
                                               RSSD.STATION_NAME = 'RESETCHECK'";

                    DataTable countDT = sfcdb.RunSelect(sqlCount).Tables[0];
                    string StrCount = "0";
                    if (countDT.Rows.Count > 0)
                    {
                        StrCount = Convert.ToString(countDT.Rows[0]["SN"].ToString());
                    }
                    StationReturn.Data = StrCount;
                    StationReturn.Message = "OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    this.DBPools["SFCDB"].Return(sfcdb);

                }
                else if (cartonobj != null && _CATEGORY_SKUNO.IndexOf(cartonobj.SKUNO) < 0)
                {
                    R_MES_LOG log = new R_MES_LOG();
                    log.ID = TRML.GetNewID(BU, sfcdb);
                    log.PROGRAM_NAME = "RESETCHECK";
                    log.CLASS_NAME = "MESStation.Config.DCheckConfig";
                    log.FUNCTION_NAME = "AddNewRecordCanReset";
                    log.DATA1 = sN1Value;
                    log.DATA2 = packno;
                    log.EDIT_EMP = LoginUser.EMP_NO;
                    log.EDIT_TIME = TRML.GetDBDateTime(sfcdb);
                    TRML.InsertMESLogOld(log, sfcdb);
                    string sqlCount = $@"SELECT count(RSSD.SN) SN
                                          FROM R_PACKING           RP1,
                                               R_PACKING RP2,
                                               R_SN                RN,
                                               R_SN_PACKING RSP,
                                               R_SN_STATION_DETAIL RSSD
                                         WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                               RP2.ID = RSP.PACK_ID AND
                                               RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                           AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}')  AND
                                               RN.VALID_FLAG = 1 AND
                                               RSSD.VALID_FLAG = 1 AND
                                               RSSD.STATION_NAME = 'RESETCHECK'";

                    DataTable countDT = sfcdb.RunSelect(sqlCount).Tables[0];
                    string StrCount = "0";
                    if (countDT.Rows.Count > 0)
                    {
                        StrCount = Convert.ToString(countDT.Rows[0]["SN"].ToString());
                    }
                    StationReturn.Data = StrCount;
                    StationReturn.Message = "OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else if (cartonobj != null && _CATEGORY_SKUNO.IndexOf(cartonobj.SKUNO) > -1)
                {
                    var snobj1 = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rs, rsp, rp, rp1) => rs.ID == rsp.SN_ID && rp.PARENT_PACK_ID == rp1.ID && rsp.PACK_ID == rp.ID).Where((rs, rsp, rp, rp1) => rs.SN == sN1Value && rs.VALID_FLAG == "1" && rp1.PACK_NO == packno).Select((rs, rsp, rp) => rs)
                         .ToList().FirstOrDefault();
                    if (snobj1 != null)
                    {
                        T_R_SN table = new T_R_SN(sfcdb, this.DBTYPE);
                        table.RecordPassStationDetail(snobj1, "", "RESETCHECK", "RESETCHECK", BU,
                              sfcdb, "0");

                        string sqlCount = $@"SELECT count(RSSD.SN) SN
                                          FROM R_PACKING           RP1,
                                               R_PACKING RP2,
                                               R_SN                RN,
                                               R_SN_PACKING RSP,
                                               R_SN_STATION_DETAIL RSSD
                                         WHERE RP1.ID = RP2.PARENT_PACK_ID AND
                                               RP2.ID = RSP.PACK_ID AND
                                               RSP.SN_ID = RN.ID AND RSSD.SN = RN.SN
                                           AND (RP1.PACK_NO = '{packno}' OR RP2.PACK_NO = '{packno}')  AND
                                               RN.VALID_FLAG = 1 AND
                                               RSSD.VALID_FLAG = 1 AND
                                               RSSD.STATION_NAME = 'RESETCHECK'";

                        DataTable countDT = sfcdb.RunSelect(sqlCount).Tables[0];
                        string StrCount = "0";
                        if (countDT.Rows.Count > 0)
                        {
                            StrCount = Convert.ToString(countDT.Rows[0]["SN"].ToString());
                        }
                        StationReturn.Data = StrCount;
                        StationReturn.Message = "OK";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        this.DBPools["SFCDB"].Return(sfcdb);

                    }
                }
                else
                {
                    StationReturn.Data = "0";
                    StationReturn.Message = "Can't found " + sN1Value + " in PALLET/CARTON. Please check CARTON/SN Correct";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception ex)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
        }


        public void DeleteRecordCanReset(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string sN1Value = Data["SN1Value"].ToString().Trim();
            string packno = Data["Packno"].ToString().Trim();
            if (string.IsNullOrEmpty(packno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Mã pallet hoặc carton không được bỏ trống";
                StationReturn.MessagePara.Add("Packno");
                StationReturn.Data = "";
                return;
            }
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var snobjPallet = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rs, rsp, rp, rp1) => rs.ID == rsp.SN_ID && rp.PARENT_PACK_ID == rp1.ID && rsp.PACK_ID == rp.ID).Where((rs, rsp, rp, rp1) => (rs.VALID_FLAG == "1" && rp1.PACK_NO == packno)).Select((rs, rsp, rp) => rs)
                         .ToList();
                var snobj = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rs, rsp, rp, rp1) => rs.ID == rsp.SN_ID && rp.PARENT_PACK_ID == rp1.ID && rsp.PACK_ID == rp.ID).Where((rs, rsp, rp, rp1) => (rs.VALID_FLAG == "1" && rp.PACK_NO == packno)).Select((rs, rsp, rp) => rs)
                         .ToList();
                if ((snobj.Count > 0 && snobjPallet.Count == 0))
                {
                    foreach (R_SN rs in snobj)
                    {
                        var tf = sfcdb.ORM.Updateable<R_SN_STATION_DETAIL>().SetColumns(t => new R_SN_STATION_DETAIL { VALID_FLAG = "0" }).Where(t => t.SN == rs.SN && t.STATION_NAME == "RESETCHECK" && t.VALID_FLAG == "1").ExecuteCommand();
                    }
                    StationReturn.Data = "";
                    StationReturn.Message = "RESET OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;

                }
                else if (snobjPallet.Count > 0 && _CATEGORY_SKUNO.IndexOf(snobjPallet[0].SKUNO) > -1)
                {
                    foreach (R_SN rs in snobjPallet)
                    {
                        var tf = sfcdb.ORM.Updateable<R_SN_STATION_DETAIL>().SetColumns(t => new R_SN_STATION_DETAIL { VALID_FLAG = "0" }).Where(t => t.SN == rs.SN && t.STATION_NAME == "RESETCHECK" && t.VALID_FLAG == "1").ExecuteCommand();
                    }
                    StationReturn.Data = "";
                    StationReturn.Message = "RESET OK";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    var f = sfcdb.ORM.Deleteable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "RESETCHECK" && t.DATA2 == packno && t.FUNCTION_NAME == "AddNewRecordCanReset").ExecuteCommand();
                    if ("1".Equals(f.ToString()))
                    {
                        StationReturn.Data = "";
                        StationReturn.Message = "RESET OK";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Message = "RESET FAIL";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Message = "RESET FAIL";
                StationReturn.Status = StationReturnStatusValue.Fail;
                this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }

        }

        public void ResetDOUBLECHECK(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string packno = Data["pallet"].ToString().Trim();
            if (string.IsNullOrEmpty(packno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Mã pallet hoặc carton không được bỏ trống";
                StationReturn.MessagePara.Add("Packno");
                StationReturn.Data = "";
                return;
            }
            try
            {
                string sqlcheck;
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sqlcheck = $@"  delete R_MES_LOG WHERE  PROGRAM_NAME = 'DOUBLECHECK' AND DATA1 IN (
                                  SELECT SN FROM R_SN SN, R_SN_PACKING RSP, R_PACKING CT, R_PACKING PL WHERE SN.VALID_FLAG=1 AND 
                                  SN.ID=RSP.SN_ID AND RSP.PACK_ID= CT.ID AND  CT.PARENT_PACK_ID= PL.ID AND PL.PACK_NO ='{packno}')";
                int a = sfcdb.ExecSqlNoReturn(sqlcheck, null);
                StationReturn.Status = StationReturnStatusValue.Pass;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Message = "RESET FAIL";
                StationReturn.Status = StationReturnStatusValue.Fail;
                this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }

        }
    }
}
