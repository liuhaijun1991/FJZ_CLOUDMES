using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Constants.PublicConstants;

namespace MESStation.Config.DCN
{
    public class GetSendCZData : MesAPIBase
    {
        protected APIInfo FGetSendCZDataFromSN = new APIInfo
        {
            FunctionName = "GetSendCZDataFromSN",
            Description = "Get Send CZ Data From SN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSaveResendSN = new APIInfo
        {
            FunctionName = "SaveResendSN",
            Description = "Save Resend SN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FResendOneSN = new APIInfo
        {
            FunctionName = "ResendOneSN",
            Description = "Resend One SN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetResendData = new APIInfo
        {
            FunctionName = "GetWaitResendData",
            Description = "Get Wait Resend Data",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetSendFailLog = new APIInfo
        {
            FunctionName = "GetSendFailLog",
            Description = "Get Send Fail Log",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpSendCzData = new APIInfo
        {
            FunctionName = "UpSendCzData",
            Description = "UpSendCzData",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN60", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SN84", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSearchSendResult = new APIInfo
        {
            FunctionName = "SearchSendResult",
            Description = "SearchSendResult",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SnList", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public GetSendCZData()
        {            
            this.Apis.Add(FGetSendCZDataFromSN.FunctionName, FGetSendCZDataFromSN);
            this.Apis.Add(FSaveResendSN.FunctionName, FSaveResendSN);
            this.Apis.Add(FResendOneSN.FunctionName, FResendOneSN);
            this.Apis.Add(FGetSendFailLog.FunctionName, FGetSendFailLog);
            this.Apis.Add(FUpSendCzData.FunctionName, FUpSendCzData);
            this.Apis.Add(FSearchSendResult.FunctionName, FSearchSendResult);
        }

        public void UpSendCzData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn60_new = Data["SN60"].ToString().ToUpper().Trim();
                string sn84_new = Data["SN84"].ToString().ToUpper().Trim();
                var sn84_old = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == sn60_new).OrderBy(t=>t.EDIT_TIME,SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                var sn60_old = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn84_new && t.VALID_FLAG == 1 && t.SCANTYPE.EndsWith("S/N") && t.PARTNO.StartsWith("60-")).ToList().FirstOrDefault();
                if (sn84_old == null || sn60_old == null)
                    throw new Exception($@"sn is err!!");
                if (sn84_old.SN == sn60_old.SN)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    //StationReturn.Data = list;
                    StationReturn.MessageCode = "MES00000026";
                    return;
                }
                var sn84obj_new = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn84_new && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                var sn84obj_old = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn84_old.SN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                var res = SFCDB.ORM.Ado.UseTran(()=> {
                    sn84_old.VALID_FLAG = 0;
                    SFCDB.ORM.Updateable(sn84_old).ExecuteCommand();
                    sn84_old.VALID_FLAG = 1;
                    sn84_old.ID = $@"T{sn84_old.ID}";
                    sn84_old.SN = sn84_new;
                    sn84_old.R_SN_ID = sn84obj_new.ID;
                    SFCDB.ORM.Insertable(sn84_old).ExecuteCommand();

                    sn60_old.VALID_FLAG = 0;
                    SFCDB.ORM.Updateable(sn60_old).ExecuteCommand();
                    sn60_old.VALID_FLAG = 1;
                    sn60_old.ID = $@"T{sn60_old.ID}";
                    sn60_old.SN = sn84obj_old.SN;
                    sn60_old.R_SN_ID = sn84obj_old.ID;
                    SFCDB.ORM.Insertable(sn60_old).ExecuteCommand();

                    SFCDB.ORM.Ado.ExecuteCommand($@" INSERT INTO WWN_DATASHARING_BAK1230
                      SELECT * FROM WWN_DATASHARING A
                     WHERE exists(select*From r_sn_kp B where a.vsSN=b.value and value='{sn60_new}'AND VALID_FLAG=1)");

                    SFCDB.ORM.Ado.ExecuteCommand($@" UPDATE  WWN_DATASHARING A set csSN=(select sn From r_sn_kp B where a.vsSN=b.value and value='{sn60_new}'AND VALID_FLAG=1)
                        WHERE exists(select*From r_sn_kp B where a.vsSN=b.value and value='{sn60_new}'AND VALID_FLAG=1)");

                    SFCDB.ORM.Ado.ExecuteCommand($@" INSERT INTO WWN_DATASHARING_BAK1230
                      SELECT * FROM WWN_DATASHARING A
                     WHERE exists(select*From r_sn_kp B where a.vsSN=b.value and value='{sn60_old.VALUE}'AND VALID_FLAG=1)");

                    SFCDB.ORM.Ado.ExecuteCommand($@" UPDATE  WWN_DATASHARING A set csSN=(select sn From r_sn_kp B where a.vsSN=b.value and value='{sn60_old.VALUE}'AND VALID_FLAG=1)
                        WHERE exists(select*From r_sn_kp B where a.vsSN=b.value and value='{sn60_old.VALUE}'AND VALID_FLAG=1)");
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.Data = list;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }


        public void GetSendCZDataFromSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                //定義上傳Excel的列名 
                List<string> inputTitle = new List<string> { "SN" };
                string errTitle = "";
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception($@"Upload File Is Empty!");
                }
                if (array.Count > 999)
                {
                    throw new Exception($@"The number of Excel rows must be less than 1000!");
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool bColumnExists = false;
                foreach (string t in inputTitle)
                {
                    bColumnExists = firstData.Properties().Any(p => p.Name == t);
                    if (!bColumnExists)
                    {
                        errTitle = t;
                        break;
                    }
                }
                if (!bColumnExists)
                {
                    throw new Exception($@"Upload File Content Error,Must Contains {errTitle} Column!");
                }                
                
                string sql_sn = "";
                SFCDB = this.DBPools["SFCDB"].Borrow();
                for (int i = 0; i < array.Count; i++)
                {                   
                    if (array[i]["SN"] == null)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(array[i]["SN"].ToString()))
                    {
                        continue;
                    }
                    sql_sn += $@",'{array[i]["SN"].ToString().ToUpper().Trim()}'";
                }
                sql_sn = sql_sn.Substring(1);
                string sql_total = $@"select distinct A.SN SN,a.id  shiporderid From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ' 
                             and a.sn in ({sql_sn})
                             UNION
                             select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D where a.dn_no=b.vbeln  AND A.SN=C.SN AND C.VALUE=D.SN
                             and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' and a.sn in ( {sql_sn} ) 
                             UNION
                             select distinct D.SN SN,a.id shiporderid From r_ship_detail a,sd_to_detail b,R_SN_KP C,R_SN D,R_SN_KP E where a.dn_no=b.vbeln  
                             AND A.SN=C.SN AND C.VALUE=E.SN AND E.VALUE=D.SN
                             and b.land1='CZ' AND C.VALID_FLAG='1' AND D.VALID_FLAG='1' AND E.VALID_FLAG='1'
                             and a.sn in ( {sql_sn} ) 
                             union
                             SELECT distinct Data2 SN,data1 shiporderid FROM r_sn_log WHERE LOGTYPE = 'SendCZData_CheckSN' AND Data9 = 'N' ";

               string sql_product = $@"select distinct sysserialno,seqno,factoryid,routeid,customerid,workorderno,skuno,custpartno,eeecode,custssn,''firmware,
                                        software,servicetag,enetid,prioritycode,productfamily,productlevel,productcolor,productlangulage,
                                        shipcountry,productdesc,''orderno,''compcode,shipped,shipdate,location,whid,areaid,workordertype,packageno,systemstage,unitcost,lineseqno,reseatpre,reseat,reseatTag,lasteditby,lasteditdt,coo 
                                        from mfsysproduct mp,({sql_total}) total where mp.sysserialno=total.sn ";

                string sql_component = $@"select distinct sysserialno,partno,version,seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,installedqty,
                                        eeecode,cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,
                                       substr(originalqty, 0,6) AS orig,unitcost,replacegroup,noreplacepart,lasteditby,lasteditdt 
                                        from mfsyscomponent mp,({sql_total}) total where mp.sysserialno=total.sn ";

                string sql_syscserial = $@"select distinct sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,
                                        prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN from mfsyscserial mp,({sql_total}) total where mp.sysserialno=total.sn ";

                string sql_wwn = $@"
                select ID,
                       WSN,
                       SKU,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ('Odin','Skybolt','TYR','GEN7','Wedge'))
                                  and skuno = CSKU) then
                          CSSN
                         else
                          VSSN
                       END VSSN,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ('Odin','Skybolt','TYR','GEN7','Wedge'))
                                  and skuno = CSKU) then
                          CSKU
                         else
                          VSKU
                       END VSKU,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ('Odin','Skybolt','TYR','GEN7','Wedge'))
                                  and skuno = CSKU) then
                          'N/A'
                         else
                          CSSN
                       END CSSN,
                       case
                         when exists (select *
                                 from c_sku
                                where c_series_id in
                                      (select id
                                         from c_series
                                        where series_name in ('Odin','Skybolt','TYR','GEN7','Wedge'))
                                  and skuno = CSKU) then
                          'N/A'
                         else
                          CSKU
                       END CSKU,
                       MAC,
                       WWN,
                       MAC_BLOCK_SIZE,
                       WWN_BLOCK_SIZE,
                       LASTEDITBY,
                       LASTEDITDT,
                       MACTB0,
                       MACTB1,
                       MACTB2,
                       MACTB3,
                       MACTB4,
                       WWNTB0,
                       WWNTB1,
                       WWNTB2,
                       WWNTB3,
                       WWNTB4
                  from (select *
                          from wwn_datasharing
                         where cssn in ({sql_sn})
                        union
                        select *
                          from wwn_datasharing
                         where vssn in ({sql_sn})
                        union
                        select *
                          from wwn_datasharing
                         where wsn in ({sql_sn}))";

                DataTable dt_product = SFCDB.ORM.Ado.GetDataTable(sql_product);
                DataTable dt_component = SFCDB.ORM.Ado.GetDataTable(sql_component);
                DataTable dt_syscserial = SFCDB.ORM.Ado.GetDataTable(sql_syscserial);
                DataTable dt_wwn = SFCDB.ORM.Ado.GetDataTable(sql_wwn);

                string content_product = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt_product);
                string content_component = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt_component);
                string content_syscserial = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt_syscserial);
                string content_wwn = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt_wwn);

                string file_product = "product_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string file_component = "component_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string file_syscserial = "syscserial_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string file_wwn = "wwn_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

                List<object> list = new List<object>();
                list.Add(new { FileName = file_product, Content = content_product });
                list.Add(new { FileName = file_component, Content = content_component });
                list.Add(new { FileName = file_syscserial, Content = content_syscserial });
                list.Add(new { FileName = file_wwn, Content = content_wwn });

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = list;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void SaveResendSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                //定義上傳Excel的列名 
                List<string> inputTitle = new List<string> { "SN" };
                string errTitle = "";
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception($@"Upload File Is Empty!");
                }               
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool bColumnExists = false;
                foreach (string t in inputTitle)
                {
                    bColumnExists = firstData.Properties().Any(p => p.Name == t);
                    if (!bColumnExists)
                    {
                        errTitle = t;
                        break;
                    }
                }
                if (!bColumnExists)
                {
                    throw new Exception($@"Upload File Content Error,Must Contains {errTitle} Column!");
                }
                
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(SFCDB, this.DBTYPE);
                string sn = "";
                string fail_sn = "";
                string success_sn = "";
                string message = "";
                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i]["SN"] == null)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(array[i]["SN"].ToString()))
                    {
                        continue;
                    }
                    sn = array[i]["SN"].ToString();
                    bool bExists = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.LOGTYPE == "SendCZData" && r.FLAG == "N" && r.SN == sn).Any();
                    if (bExists)
                    {
                        fail_sn += $@",{sn}";
                        continue;
                    }
                    R_SN_LOG log = new R_SN_LOG();
                    log.ID = t_r_sn_log.GetNewID(this.BU, SFCDB);
                    log.LOGTYPE = "SendCZData";
                    log.SN = sn;
                    log.CREATEBY = LoginUser.EMP_NO;
                    log.CREATETIME = SFCDB.ORM.GetDate();
                    log.FLAG = "N";
                    SFCDB.ORM.Insertable<R_SN_LOG>(log).ExecuteCommand();
                    success_sn += $@",{sn}";
                }
                if (string.IsNullOrEmpty(fail_sn))
                {
                    message = $@"All SN upload success,please wait for resend.";
                }
                else
                {
                    if (string.IsNullOrEmpty(success_sn))
                    {
                        message = "All SN upload fail,already waitting for resend.";
                    }
                    else
                    {
                        success_sn = success_sn.Substring(1);
                        fail_sn = fail_sn.Substring(1);
                        message = $@"[{success_sn}] upload success,waiting for resend.[{fail_sn}] upload fail,already waitting for resend.";
                    }                    
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = message;
                StationReturn.Message = message;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void ResendOneSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            if (Data["SN"] == null)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Message = "Please Input SN";
                StationReturn.Data = "Please Input SN";
                return;
            }
            string sn = Data["SN"].ToString();
            OleExec MESDB = null;
            OleExec CZDB = null;            

            try
            {
                MESDB = this.DBPools["SFCDB"].Borrow();
                CZDB = new OleExec($@"server =10.3.34.45,1433; uid = vn_import; pwd = TCG*f/spMI26EVYQG5\XqpOt; database = CZ22PRDCZ_EXT", SqlSugar.DbType.SqlServer);
                MESDB.ThrowSqlExeception = true;
                CZDB.ThrowSqlExeception = true;

                string sql = $@"select distinct A.SN SN,a.id  shiporderid From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ' and a.sn='{sn}'";
                DataTable dt = MESDB.ORM.Ado.GetDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"{sn} error or not shipped.");
                }

                MESDB.BeginTrain();
                CZDB.BeginTrain();
                string _sn = dt.Rows[0]["SN"].ToString();
                string shiporderid = dt.Rows[0]["shiporderid"].ToString();
                string result = "";
                sql = "";

                //1.本階
                sql = $@" select * from R_SN_KP a where a.sn='{_sn}' and a.valid_flag='1' ";
                DataTable kp_table_1 = MESDB.ORM.Ado.GetDataTable(sql);
                if (kp_table_1.Rows.Count > 0)
                {
                    result = InsertMES(_sn, shiporderid, MESDB);
                    if (result != "OK")
                    {
                        throw new Exception(result);
                    }
                    result = InsertCZ(_sn, CZDB, MESDB);
                    if (result != "OK")
                    {
                        throw new Exception(result);
                    }
                    UpdateMES(_sn, MESDB);
                }

                //2.2階KP
                sql = $@"select a.value from R_SN_KP a where a.sn='{_sn}' and a.valid_flag='1'	
                                and exists (select * from r_sn b where a.value=b.sn and b.valid_flag='1')";
                DataTable kp_table_2 = MESDB.ORM.Ado.GetDataTable(sql);
                string sn_kp_2 = "";
                if (kp_table_2.Rows.Count > 0)
                {
                    foreach (DataRow kp_1 in kp_table_2.Rows)
                    {
                        sn_kp_2 = kp_1["VALUE"].ToString();
                        result = InsertMES(sn_kp_2, shiporderid, MESDB);
                        if (result != "OK")
                        {
                            throw new Exception(result);
                        }
                        result = InsertCZ(sn_kp_2, CZDB, MESDB);
                        if (result != "OK")
                        {
                            throw new Exception(result);
                        }
                        UpdateMES(sn_kp_2, MESDB);
                    }
                }

                //3.3階KP
                sql = $@"select n.value from R_SN_KP m,R_SN_KP n where m.sn='{_sn}' and m.valid_flag='1'	
                                    and m.value=n.sn and n.valid_flag='1' 
                                    and  exists (select * from r_sn sn where n.value=sn.sn and sn.valid_flag='1')";
                DataTable kp_table_3 = MESDB.ORM.Ado.GetDataTable(sql);
                string sn_kp_3 = "";
                if (kp_table_3.Rows.Count > 0)
                {
                    foreach (DataRow kp_3 in kp_table_3.Rows)
                    {
                        sn_kp_3 = kp_3["VALUE"].ToString();
                        result = InsertMES(sn_kp_3, shiporderid, MESDB);
                        if (result != "OK")
                        {
                            throw new Exception(result);
                        }
                        result = InsertCZ(sn_kp_3, CZDB, MESDB);
                        if (result != "OK")
                        {
                            throw new Exception(result);
                        }
                        UpdateMES(sn_kp_3, MESDB);
                    }
                }

                //4.wwwn                    
                string Series = $@"'Odin','Skybolt','TYR','GEN7','Wedge'";
                string wwn_sql = $@"
                                select ID,
                                       WSN,
                                       SKU,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          CSSN
                                         else
                                          VSSN
                                       END VSSN,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          CSKU
                                         else
                                          VSKU
                                       END VSKU,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          'N/A'
                                         else
                                          CSSN
                                       END CSSN,
                                       case
                                         when exists (select *
                                                 from c_sku
                                                where c_series_id in
                                                      (select id
                                                         from c_series
                                                        where series_name in ({Series}))
                                                  and skuno = CSKU) then
                                          'N/A'
                                         else
                                          CSKU
                                       END CSKU,
                                       MAC,
                                       WWN,
                                       MAC_BLOCK_SIZE,
                                       WWN_BLOCK_SIZE,
                                       LASTEDITBY,
                                       LASTEDITDT,
                                       MACTB0,
                                       MACTB1,
                                       MACTB2,
                                       MACTB3,
                                       MACTB4,
                                       WWNTB0,
                                       WWNTB1,
                                       WWNTB2,
                                       WWNTB3,
                                       WWNTB4
                                  from (select *
                                          from wwn_datasharing
                                         where cssn='{_sn}'
                                        union
                                        select *
                                          from wwn_datasharing
                                         where vssn ='{_sn}'
                                        union
                                        select *
                                          from wwn_datasharing
                                         where wsn ='{_sn}')";
                wwn_sql = $@"select WSN, SKU, VSSN, VSKU, CSSN, CSKU, MAC, WWN, MAC_block_size, WWN_block_size, lasteditby,
                                    lasteditdt, MACTB0, MACTB1, MACTB2, MACTB3, MACTB4, WWNTB0, WWNTB1, WWNTB2, WWNTB3, WWNTB4,sysdate
                                    from ({wwn_sql})";
                string insert_wwn = "";
                try
                {
                    DataTable wwn_table = MESDB.ORM.Ado.GetDataTable(wwn_sql);
                    if (wwn_table.Rows.Count > 0)
                    {
                        string delete_wwn = $@"delete from wwn_datasharing_TEMP where cssn = '{_sn}'";
                        CZDB.ExecSQL(delete_wwn);

                        delete_wwn = $@"delete from wwn_datasharing_TEMP where vssn = '{_sn}'";
                        CZDB.ExecSQL(delete_wwn);

                        delete_wwn = $@"delete from wwn_datasharing_TEMP where wsn = '{_sn}'";
                        CZDB.ExecSQL(delete_wwn);

                        MES_DCN.DBTableP dbtable_wwn = new MES_DCN.DBTableP("wwn_datasharing_TEMP", CZDB);
                        dbtable_wwn.myDBTYPE = 0;
                        dbtable_wwn.analyse();

                        foreach (DataRow row in wwn_table.Rows)
                        {
                            insert_wwn = dbtable_wwn.GetInsertSql(row);
                            CZDB.ExecSQL(insert_wwn);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($@"SN:{_sn};SQL:{insert_wwn};Error:{ex.Message}");
                }

                string update_log = $@" update r_sn_log  set flag='Y' where LOGTYPE = 'SendCZData' and flag='N' and sn='{_sn}'";
                MESDB.ORM.Ado.ExecuteCommand(update_log);

                MESDB.CommitTrain();
                CZDB.CommitTrain();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "OK";
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                if (MESDB != null)
                {
                    MESDB.RollbackTrain();
                }
                if (CZDB != null)
                {
                    CZDB.RollbackTrain();
                }               
                OleExec MESDB_LOG = this.DBPools["SFCDB"].Borrow();
                try
                {
                    string sql = $@"select distinct A.SN SN,a.id  shiporderid From r_ship_detail a,sd_to_detail b where a.dn_no=b.vbeln and b.land1='CZ' and a.sn='{sn}'";
                    DataTable dt = MESDB_LOG.ORM.Ado.GetDataTable(sql);
                    if (dt.Rows.Count != 0)
                    {
                        T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(MESDB_LOG, this.DBTYPE);
                        R_MES_LOG log = new R_MES_LOG();
                        log.ID=t_r_mes_log.GetNewID(this.BU, MESDB_LOG);
                        log.PROGRAM_NAME = "SendCZData";
                        log.CLASS_NAME = "MESStation.Config.DCN.GetSendCZData";
                        log.FUNCTION_NAME = "ResendOneSN";
                        log.LOG_MESSAGE = "Resend One SN To CZ Fail";
                        log.LOG_SQL = ee.Message.Length > 1000 ? ee.Message.Substring(0, 950) : ee.Message;
                        log.DATA1 = sn;
                        log.EDIT_EMP = LoginUser.EMP_NO;
                        log.EDIT_TIME = MESDB_LOG.ORM.GetDate();
                        MESDB_LOG.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                    }
                }
                catch (Exception)
                {

                }
                if (MESDB_LOG != null)
                {
                    this.DBPools["SFCDB"].Return(MESDB_LOG);
                }
            }
            finally
            {
                if (MESDB != null)
                {
                    this.DBPools["SFCDB"].Return(MESDB);
                }
                CZDB.FreeMe();
            }
        }

        public void GetWaitResendData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {                
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.LOGTYPE == "SendCZData" && r.FLAG == "N")
                    .Select(r => new { r.SN, r.FLAG, r.CREATETIME, r.CREATEBY }).OrderBy(r=>r.CREATETIME,SqlSugar.OrderByType.Desc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = list;
                StationReturn.Message = "OK";
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetSendFailLog(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list = SFCDB.ORM.Queryable<R_MES_LOG>().Where(r => r.PROGRAM_NAME == "SendCZData")
                    .Select(r => new { r.PROGRAM_NAME, r.FUNCTION_NAME, SN = r.DATA1, ERROR_MSSAGE = r.LOG_SQL, r.EDIT_TIME, r.EDIT_EMP })
                    .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = list;
                StationReturn.Message = "OK";
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void SearchSendResult(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string sn = Data["SnList"].ToString();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var listComponent = SFCDB.ORM.Queryable<MFSYSCOMPONENT>().Where(r => r.NOREPLACEPART == "1" && r.SYSSERIALNO == sn).ToList();
                if (listComponent.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = new { 
                        SendState = "Fail", 
                        SendTime = "", 
                        Component =new List<MFSYSCOMPONENT>(),
                        Cserial = new List<mfsyscserial>(),
                        Product = new List<MFSYSPRODUCT>(),
                        WWN = new List<MESDataObject.Module.DCN.WWN_DATASHARING>()
                    };
                    StationReturn.Message = "OK";
                    StationReturn.MessageCode = "MES00000026";
                }
                else
                {
                    var shippedCserial = SFCDB.ORM.Queryable<mfsyscserial>().Where(r => r.SYSSERIALNO == sn && r.PRODTYPE == "OK").ToList();
                    var allCserial = GetAllCserial(SFCDB, shippedCserial).Select(r=>new {                        
                        r.SYSSERIALNO,
                        r.CSERIALNO,
                        r.EVENTPOINT,
                        r.CUSTPARTNO,
                        r.EEECODE,
                        r.PARTNO,
                        r.SEQNO,
                        r.CATEGORYNAME,
                        r.PRODCATEGORYNAME,
                        r.PRODTYPE,
                        r.ORIGINALCSN,
                        r.SCANBY,
                        r.SCANDT,
                        r.LASTEDITBY,
                        r.LASTEDITDT,
                        r.MDSGET,
                        r.MPN,
                        r.OLDMPN
                    });               
                    List<string> listSysserialno = allCserial.Select(r => r.SYSSERIALNO).Distinct().ToList();
                    var allComponent = SFCDB.ORM.Queryable<MFSYSCOMPONENT>().Where(r =>r.NOREPLACEPART=="1" && listSysserialno.Contains(r.SYSSERIALNO))
                        .Select(r=>new {
                            r.SYSSERIALNO,
                            r.PARTNO,
                            r.VERSION,
                            r.SEQNO,
                            r.QTY,
                            r.CUSTPARTNO,
                            r.REPLACENO,
                            r.REPLACETOPARTNO,
                            r.KEYPART,
                            r.INSTALLED,
                            r.INSTALLEDQTY,
                            r.EEECODE,
                            r.CSERIALNO1,
                            r.CSERIALNO2,
                            r.CSERIALNO3,
                            r.CSERIALNO4,
                            r.CATEGORYNAME,
                            r.PRODCATEGORYNAME,
                            r.PRODTYPE,
                            r.ORIGINALQTY,
                            r.UNITCOST,
                            r.REPLACEGROUP,
                            r.NOREPLACEPART,
                            r.LASTEDITBY,
                            r.LASTEDITDT
                        }).ToList();
                    var allProduct = SFCDB.ORM.Queryable<MFSYSPRODUCT>().Where(r => r.RESEAT=="1" && listSysserialno.Contains(r.SYSSERIALNO))
                        .Select(r=>new 
                        { 
                            r.SYSSERIALNO,
                            r.SEQNO,
                            r.FACTORYID,
                            r.ROUTEID,
                            r.CUSTOMERID,
                            r.WORKORDERNO,
                            r.SKUNO,
                            r.CUSTPARTNO,
                            r.EEECODE,
                            r.CUSTSSN,
                            r.FIRMWARE,
                            r.SOFTWARE,
                            r.SERVICETAG,
                            r.ENETID,
                            r.PRIORITYCODE,
                            r.PRODUCTFAMILY,
                            r.PRODUCTLEVEL,
                            r.PRODUCTCOLOR,
                            r.PRODUCTLANGULAGE,
                            r.SHIPCOUNTRY,
                            r.PRODUCTDESC,
                            r.ORDERNO,
                            r.COMPCODE,
                            r.SHIPPED,
                            r.SHIPDATE,
                            r.LOCATION,
                            r.WHID,
                            r.AREAID,
                            r.WORKORDERTYPE,
                            r.PACKAGENO,
                            r.SYSTEMSTAGE,
                            r.UNITCOST,
                            r.LINESEQNO,
                            r.RESEATPRE,
                            r.RESEAT,
                            r.RESEATTAG,
                            r.LASTEDITBY,
                            r.LASTEDITDT,
                            r.COO,
                            r.FIELD1 
                        }).ToList();
                    var wwnData = SFCDB.ORM.Queryable<MESDataObject.Module.DCN.WWN_DATASHARING>().Where(r => r.CSSN == sn || r.VSSN == sn)
                        .Select(r=>new {
                            r.WSN,
                            r.SKU,
                            r.VSSN,
                            r.VSKU,
                            r.CSSN,
                            r.CSKU,
                            r.MAC,
                            r.WWN,
                            r.MAC_BLOCK_SIZE,
                            r.WWN_BLOCK_SIZE,
                            r.LASTEDITBY,
                            r.LASTEDITDT,
                            r.MACTB0,
                            r.MACTB1,
                            r.MACTB2,
                            r.MACTB3,
                            r.MACTB4,
                            r.WWNTB0,
                            r.WWNTB1,
                            r.WWNTB2,
                            r.WWNTB3,
                            r.WWNTB4
                        }).ToList();

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = new
                    {
                        SendState = "Success",
                        SendTime = listComponent.FirstOrDefault().LASTEDITDT,
                        Component = allComponent,
                        Cserial = allCserial,
                        Product = allProduct,
                        WWN = wwnData
                    };
                    StationReturn.Message = "OK";
                    StationReturn.MessageCode = "MES00000026";
                }
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public string InsertMES(string Serial, string shiporderid, OleExec oleExec)
        {
            string delete_sql = "";
            string insert_sql = "";
            string result = "OK";
            try
            {
                //mfsysproduct
                delete_sql = $@"delete mfsysproduct where SYSSERIALNO='{Serial}' ";
                oleExec.ExecSQL(delete_sql);
                insert_sql = $@"   insert into mfsysproduct
                  select 'PRO'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}',a.sn,'99',a.plant,a.route_id,e.sku_series ,a.workorderno,a.skuno,a.skuno,a.workorderno,a.start_time,'','','','','99',
                  e.production_type,'DEFAULT',e.START_STATION,'','','','','',a.SHIPPED_FLAG,a.shipdate,  case when exists(select*From r_sn_packing c,r_packing d 
                  where a.id=c.sn_id and c.pack_id=d.id )then (select dd.PACK_NO From r_sn_packing cc,r_packing dd 
                  where a.id=cc.sn_id and cc.pack_id=dd.id) else '' end  as PACK_NO,'','',e.wo_type,'','','','','','0','0',a.edit_emp,a.edit_time,'',''
                  from r_sn a ,r_wo_base e  where  a.valid_flag='1'   and a.workorderno=e.workorderno and a.sn ='{Serial}'";
                oleExec.ExecSQL(insert_sql);

                //MFSYSCOMPONENT
                delete_sql = $@"delete MFSYSCOMPONENT where SYSSERIALNO='{Serial}' ";
                oleExec.ExecSQL(delete_sql);
                insert_sql = $@"  insert into MFSYSCOMPONENT
                         select 'COM'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', aa.sn,aa.PARTNO,aa.revlv,'0',aa.PARTS,aa.PARTNO,'1',aa.PARTNO,'1','1',
                         aa.parts,aa.station,'','','','',aa.scantype,aa.kp_name,aa.auart,'0',  '0','','0','SYSTEM',sysdate From (select distinct
                         a.SN,c.revlv,c.parts,a.PARTNO,a.station, a.scantype,a.kp_name,substr(c.auart, 0, 2) auart
                         From r_sn_kp A, R_sn b, R_WO_ITEM c where a.sn = '{Serial}' and a.sn = b.sn and a.SCANTYPE<>'AUTOAP'
                         and b.workorderno = c.aufnr and a.partno = c.matnr and a.valid_flag = 1 and b.valid_flag='1' and a.edit_time>b.START_TIME ) aa";
                oleExec.ExecSelect(insert_sql);

                //mfsyscserial
                delete_sql = $@"delete mfsyscserial where SYSSERIALNO='{Serial}' ";
                oleExec.ExecSQL(delete_sql);

                insert_sql = $@"insert into mfsyscserial
                                                    select 'CSE'||SFC.SEQ_C_ID.NEXTVAL,'{shiporderid}', sn,value,
                                                    station,partno,CASE WHEN SCANTYPE in( 'AUTOAP','APTRSN') THEN LOCATION WHEN SCANTYPE='PN' and KP_NAME='PCBA' THEN KP_NAME || '-' || location  WHEN KP_NAME='PS' THEN 'POWER-'||seqno
                                                    else  KP_NAME||'-'||seqno END  as eeecode,partno,detailseq,scantype,'CSERIALNO', '',value,edit_emp,to_char(edit_time,'yyyy/mm/dd hh24:mi:ss') as SCANDT,edit_emp,edit_time,'1',mpn,'' 
                                                    from (select a.*,row_number() over(partition by a.partno order by a.id) seqno
                                                    from r_sn_kp a,r_sn b where a.sn='{Serial}'  AND A.SN=B.SN AND a.R_SN_ID=b.id  and a.valid_flag=1 AND b.valid_flag=1 )";
                oleExec.ExecSelect(insert_sql);
            }
            catch (Exception ex)
            {
                result = $@"SN:{Serial};SQL:{insert_sql};Error:{ex.Message}";
            }
            return result;
        }

        public void UpdateMES(string sn, OleExec oleExec)
        {
            string update_sql = $@"update mfsysproduct set Reseat=1 where Reseat=0 and sysserialno='{sn}' ";
            oleExec.ORM.Ado.ExecuteCommand(update_sql);

            update_sql = $@"update mfsyscomponent set noreplacepart=1 where noreplacepart=0 and  sysserialno='{sn}'";
            oleExec.ORM.Ado.ExecuteCommand(update_sql);

            update_sql = $@"update mfsyscserial set prodtype='OK' where ( prodtype is null or prodtype='' ) and sysserialno='{sn}' ";
            oleExec.ORM.Ado.ExecuteCommand(update_sql);
        }
        public string InsertCZ(string sn, OleExec cz_db, OleExec mes_db)
        {
            string result = "OK";
            string select_sql = "";
            string insert_sql = "";
            string delete_sql = "";
            DataTable data_table = null;
            try
            {
                //mfsysproduct_temp
                delete_sql = $@"delete mfsysproduct_temp where SYSSERIALNO='{sn}' ";
                cz_db.ExecSQL(delete_sql);

                MES_DCN.DBTableP dbtable_product = new MES_DCN.DBTableP("mfsysproduct_temp", cz_db);
                dbtable_product.myDBTYPE = 0;
                dbtable_product.analyse();
                select_sql = $@"select sysserialno,seqno,factoryid,routeid,customerid,workorderno,skuno,custpartno,eeecode,custssn,''firmware,
                                        software,servicetag,enetid,prioritycode,productfamily,productlevel,productcolor,productlangulage,
                                        shipcountry,productdesc,''orderno,''compcode,shipped,shipdate,location,whid,areaid,workordertype,packageno,systemstage,unitcost,lineseqno,reseatpre,reseat,reseatTag,lasteditby,lasteditdt,coo 
                                        from mfsysproduct where Reseat=0 and sysserialno='{sn}'  ";
                data_table = mes_db.RunSelect(select_sql).Tables[0];
                foreach (DataRow row in data_table.Rows)
                {
                    insert_sql = dbtable_product.GetInsertSql(row);
                    cz_db.ExecSQL(insert_sql);
                }

                //mfsyscomponent_temp
                delete_sql = $@"delete mfsyscomponent_temp where SYSSERIALNO='{sn}' ";
                cz_db.ExecSQL(delete_sql);

                MES_DCN.DBTableP dbtable_component = new MES_DCN.DBTableP("mfsyscomponent_temp", cz_db);
                dbtable_component.myDBTYPE = 0;
                dbtable_component.analyse();
                select_sql = $@"select sysserialno,partno,version,seqno,qty,custpartno,replaceno,replacetopartno,keypart,installed,installedqty,
                                        eeecode,cserialno1,cserialno2,cserialno3,cserialno4,categoryname,prodcategoryname,prodtype,
                                       substr(originalqty, 0,6) AS orig,unitcost,replacegroup,noreplacepart,lasteditby,lasteditdt 
                                        from mfsyscomponent  where noreplacepart=0 and  sysserialno='{sn}'";
                data_table = mes_db.RunSelect(select_sql).Tables[0];
                foreach (DataRow row in data_table.Rows)
                {
                    insert_sql = dbtable_component.GetInsertSql(row);
                    cz_db.ExecSQL(insert_sql);
                }
                //mfsyscserial_temp
                delete_sql = $@"delete mfsyscserial_temp where SYSSERIALNO='{sn}' ";
                cz_db.ExecSQL(delete_sql);

                MES_DCN.DBTableP dbtable_cserial = new MES_DCN.DBTableP("mfsyscserial_temp", cz_db);
                dbtable_cserial.myDBTYPE = 0;
                dbtable_cserial.analyse();
                select_sql = $@"select sysserialno,cserialno,eventpoint,custpartno,eeecode,partno,seqno,categoryname,prodcategoryname,
                                        prodtype,OriginalCSN,scanby,scandt,lasteditby,lasteditdt,MDSGet,MPN,OldMPN from mfsyscserial 
                                        where ( prodtype is null or prodtype='' ) and sysserialno ='{sn}'";
                data_table = mes_db.RunSelect(select_sql).Tables[0];
                foreach (DataRow row in data_table.Rows)
                {
                    insert_sql = dbtable_cserial.GetInsertSql(row);
                    cz_db.ExecSQL(insert_sql);
                }
            }
            catch (Exception ex)
            {
                result = $@"SN:{sn};SQL:{insert_sql};Error:{ex.Message}";
            }
            return result;
        }

        public List<mfsyscserial> GetAllCserial(OleExec sfcdb,List<mfsyscserial> listCserials)
        {
            List<mfsyscserial> list = new List<mfsyscserial>();
            list.AddRange(listCserials);
            foreach (var item in listCserials)
            {
                var temp = sfcdb.ORM.Queryable<mfsyscserial>().Where(r => r.SYSSERIALNO == item.CSERIALNO && r.PRODTYPE == "OK").ToList();
                if(temp.Count>0)
                {
                    list.AddRange(GetAllCserial(sfcdb, temp));
                }
            }
            return list;
        }        
    }
}
