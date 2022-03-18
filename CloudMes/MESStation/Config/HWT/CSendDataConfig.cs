using MESDataObject.Module;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.HWT
{
    public class CSendDataConfig : MesAPIBase
    {
        protected APIInfo FInitDNList = new APIInfo()
        {
            FunctionName = "InitDNList",
            Description = "Init Wait Send DN List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "DNMODE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSendNormal = new APIInfo()
        {
            FunctionName = "SendNormal",
            Description = "Send CD Normal",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "DNNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "PONO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "EMSNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSendAll = new APIInfo()
        {
            FunctionName = "SendAll",
            Description = "Send CD All",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "DNList", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CSendDataConfig()
        {
            this.Apis.Add(FInitDNList.FunctionName, FInitDNList);
            this.Apis.Add(FSendNormal.FunctionName, FSendNormal);
            this.Apis.Add(FSendAll.FunctionName, FSendAll);
        }

        public void InitDNList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var dnList = new List<R_DN_STATUS>();
                var dnMode = Data["DNMODE"].ToString().Trim();
                dnList = GetDNList(SFCDB, dnMode);
                if (dnList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = dnList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = dnMode == "1" ? "沒有查詢到CQC工站CHECK過的DN!" : "沒有查詢到有掃SHIPPING的待傳DN!";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

        public void SendNormal(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var dnNo = Data["DNNO"].ToString().Trim();
                var poNo = Data["PONO"].ToString().Trim();
                var emsNo = Data["EMSNO"].ToString().Trim();

                CheckSkuNo(dnNo, SFCDB);
                var showDT = GetShowDT(dnNo, poNo, SFCDB);
                if (showDT.Rows.Count == 0)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "該DN["+ dnNo + "]無數據!";
                    StationReturn.Data = "";
                    return;
                }

                var result = SendBarcode("BY_DN", "1", this.IP, emsNo, poNo, dnNo, "HWTMES", SFCDB);
                if (!result.StartsWith("OK"))
                {
                    //不記LOG了,叫再說
                    //MESDBHelper.OleExec APDB = this.DBPools["APDB"].Borrow();
                    //var sql = string.Format(@"insert into mes4.r_ems_ship_log@hwapdb (p_sn, upload_log, work_emp) values ('{0}','{1}','{2}')", dnNo, result, LoginUser.EMP_NO);
                    //APDB.ExecSQL(sql);
                    //this.DBPools["APDB"].Return(APDB);

                    this.DBPools["SFCDB"].Return(SFCDB);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = result;
                    StationReturn.Data = showDT;
                    return;
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(showDT.Rows.Count);
                StationReturn.Data = showDT;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

        public void SendAll(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var dtAll = new DataTable();
                var dnList = Data["DNList"].ToArray();
                for (int i = 0; i < dnList.Length; i++)
                {
                    var dnNo = dnList[i].ToString().Trim();
                    var dnStatuslList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == dnNo).ToList().ToArray();
                    if (dnStatuslList.Length == 0)
                    {
                        this.DBPools["SFCDB"].Return(SFCDB);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "該DN[" + dnNo + "]無信息,請先Download DN![R_DN_STATUS]";
                        StationReturn.Data = "";
                        return;
                    }
                    else if (dnStatuslList.Length > 1)
                    {
                        this.DBPools["SFCDB"].Return(SFCDB);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "該DN[" + dnNo + "]存在多筆信息,請聯繫交管及IT![R_DN_STATUS]";
                        StationReturn.Data = "";
                        return;
                    }

                    var poNo = dnStatuslList[0].PO_NO.ToString().Trim();
                    var emsNo = dnStatuslList[0].PO_NO.ToString().Trim();

                    CheckSkuNo(dnNo, SFCDB);
                    var showDT = GetShowDT(dnNo, poNo, SFCDB);
                    if (showDT.Rows.Count == 0)
                    {
                        this.DBPools["SFCDB"].Return(SFCDB);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "該DN[" + dnNo + "]無數據!";
                        StationReturn.Data = "";
                        return;
                    }

                    var result = SendBarcode("BY_DN", "1", this.IP, emsNo, poNo, dnNo, "HWTMES", SFCDB);
                    if (!result.StartsWith("OK"))
                    {
                        //不記LOG了,叫再說
                        //MESDBHelper.OleExec APDB = this.DBPools["APDB"].Borrow();
                        //var sql = string.Format(@"insert into mes4.r_ems_ship_log@hwapdb (p_sn, upload_log, work_emp) values ('{0}','{1}','{2}')", dnNo, result, LoginUser.EMP_NO);
                        //APDB.ExecSQL(sql);
                        //this.DBPools["APDB"].Return(APDB);

                        this.DBPools["SFCDB"].Return(SFCDB);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = result;
                        StationReturn.Data = showDT;
                        return;
                    }

                    if (dtAll.Rows.Count == 0)
                        dtAll = showDT.Copy();
                    else
                        dtAll.Merge(showDT);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(dtAll.Rows.Count);
                StationReturn.Data = dtAll;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

        /// <summary>
        /// 獲取待傳CD的DN列表. 0表示正常傳送;1表示CQC Check;2表示補傳
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="dnMode"></param>
        /// <returns></returns>
        private List<R_DN_STATUS> GetDNList(MESDBHelper.OleExec SFCDB, String dnMode)
        {
            var dnList = new List<R_DN_STATUS>();
            if (dnMode == "1")
            {

            }
            else if (dnMode == "2")
            {
                dnList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(r => !SqlSugar.SqlFunc.StartsWith(r.DN_NO, "52") && r.DN_FLAG != "0" && r.DN_FLAG == "SENDCD").OrderBy(r => r.DN_NO).ToList();
            }
            else
            {
                dnList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(r => !SqlSugar.SqlFunc.StartsWith(r.DN_NO, "52") && r.DN_FLAG != "0" && r.DN_FLAG != "SENDCD").OrderBy(r => r.DN_NO).ToList();
            }
            return dnList;
        }

        /// <summary>
        /// 檢查DN內機種各項配置
        /// </summary>
        /// <param name="skuArray">根據DN取得的機種數組</param>
        /// <param name="SFCDB"></param>
        private void CheckSkuNo(string dnNo, MESDBHelper.OleExec SFCDB)
        {
            var skuArray = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN>((sd, s) => sd.SN == s.SN)
                    .Where((sd, s) => s.VALID_FLAG == "1" && sd.DN_NO == dnNo).Select((sd, s) => s.SKUNO).ToList().Distinct().ToArray();

            if (skuArray.Length == 0)
                throw new Exception("該DN["+ dnNo + "]無數據![R_SHIP_DETAIL,R_SN]");
            else if (skuArray.Length == 1)
            {
                var skuNo = skuArray[0].ToString();
                var relationList = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(c => c.CATEGORY == "RELATION" && c.SKUNO == skuNo).ToList().FirstOrDefault();
                if (relationList == null)
                    throw new Exception("該DN[" + dnNo + "]對應的機種[" + skuNo + "]未配置父子項關係![C_SKU_DETAIL]");

                var softver = SFCDB.ORM.Queryable<C_SKU_SOFT_CONFIG>().Where(c => c.P_NO == skuNo).ToList().FirstOrDefault();
                if (softver == null)
                    throw new Exception("該DN[" + dnNo + "]對應的機種[" + skuNo + "]未配置軟件版本![C_SKU_SOFT_CONFIG]");
            }
            else
                throw new Exception("該DN[" + dnNo + "]對應多個機種![R_SHIP_DETAIL,R_SN]");
        }

        /// <summary>
        /// 獲取顯示數據.除了表不一樣,其他包括字段欄位規則什麽的都是照搬舊傳CD程式
        /// </summary>
        /// <param name="dnNo"></param>
        /// <param name="poNo"></param>
        /// <param name="SFCDB"></param>
        /// <returns></returns>
        private DataTable GetShowDT(string dnNo, string poNo, MESDBHelper.OleExec SFCDB)
        {
            var dt = new DataTable();
            var sql = string.Format(@"
                        SELECT '10014' AS USERNAME,'`' AS NUM1,'0157' AS COMPANY_CODE,
                                A.PO_NO AS PO_NO,'' AS TO_NO,'`' AS NUM2,B.SN AS SYSSERIALNO,
                                '``' AS NUM3,C.VALUE AS KEYPART_SN,'`' AS UNIT,A.DN_NO AS DN_NO,
                                A.SKUNO AS P_NO,F.PACK_NO AS PALLET_NO,DECODE(SUBSTR(A.SKUNO, 1, 2), 'WH', G.CUST_PN, '{0}') PO
                            FROM R_DN_STATUS A,R_SHIP_DETAIL B,R_SN_KP C,R_SN_PACKING D,
                                R_PACKING E,R_PACKING F,R_WO_BASE G
                            WHERE A.DN_NO = B.DN_NO AND B.SN = C.SN AND B.ID = D.SN_ID
                            AND D.PACK_ID = E.ID AND E.PARENT_PACK_ID = F.ID AND A.SKUNO = G.SKUNO AND A.DN_NO = '{1}'
                        UNION 
                        SELECT '10014' AS USERNAME,'`' AS NUM1,'0157' AS COMPANY_CODE,
                                A.PO_NO AS PO_NO,'' AS TO_NO,'`' AS NUM2,B.SN AS SYSSERIALNO,
                                '``' AS NUM3,C.VALUE AS KEYPART_SN,'`' AS UNIT,A.DN_NO AS DN_NO,
                                A.SKUNO AS P_NO,F.PACK_NO AS PALLET_NO,DECODE(SUBSTR(A.SKUNO, 1, 2), 'WH', G.CUST_PN, '{0}') PO
                            FROM R_DN_STATUS A,R_SHIP_DETAIL B,R_SN_KP C,R_SN_PACKING D,
                                R_PACKING E,R_PACKING F,R_WO_BASE G
                            WHERE A.DN_NO = B.DN_NO AND B.SN = C.SN(+) AND C.VALUE IS NULL AND B.ID = D.SN_ID
                            AND D.PACK_ID = E.ID AND E.PARENT_PACK_ID = F.ID AND A.SKUNO = G.SKUNO AND A.DN_NO = '{1}'", poNo, dnNo);
            dt = SFCDB.RunSelect(sql).Tables[0];

            return dt;
        }

        /// <summary>
        /// 調用傳CD的SP
        /// </summary>
        /// <param name="sendType"></param>
        /// <param name="emsFlag">默認=1,表示:Send EMS data to hwems db</param>
        /// <param name="ip"></param>
        /// <param name="emsNo"></param>
        /// <param name="poNo"></param>
        /// <param name="dnNo"></param>
        /// <param name="dsn"></param>
        /// <param name="SFCDB"></param>
        /// <returns></returns>
        private string SendBarcode(string sendType, string emsFlag, string ip, string emsNo, string poNo, string dnNo, string dsn, MESDBHelper.OleExec SFCDB)
        {
            var result = "";
            try
            {
                if (DBTYPE == MESDataObject.DB_TYPE_ENUM.Oracle)
                {
                    OleDbParameter[] parameters = new OleDbParameter[9];
                    parameters[0] = new OleDbParameter("var_o_message", OleDbType.VarChar, 2000);
                    parameters[0].Direction = ParameterDirection.Output;

                    parameters[1] = new OleDbParameter("var_type", sendType);
                    parameters[1].Direction = ParameterDirection.Input;

                    parameters[2] = new OleDbParameter("var_linkems_flag", emsFlag);
                    parameters[2].Direction = ParameterDirection.Input;

                    parameters[3] = new OleDbParameter("var_ip", ip);
                    parameters[3].Direction = ParameterDirection.Input;

                    parameters[4] = new OleDbParameter("var_emsno", emsNo);
                    parameters[4].Direction = ParameterDirection.Input;

                    parameters[5] = new OleDbParameter("var_po", poNo);
                    parameters[5].Direction = ParameterDirection.Input;

                    parameters[6] = new OleDbParameter("var_dn", dnNo);
                    parameters[6].Direction = ParameterDirection.Input;

                    parameters[7] = new OleDbParameter("var_dsn", dsn);
                    parameters[7].Direction = ParameterDirection.Input;

                    parameters[8] = new OleDbParameter("var_emp", LoginUser.EMP_NO);
                    parameters[8].Direction = ParameterDirection.Input;

                    var spResutl = SFCDB.ExecProcedureReturnDic("sfc.get_ems_ship_data_sp_new", parameters);
                    result = spResutl["var_o_message"].ToString() == null ? "N/A" : spResutl["var_o_message"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("執行SP出錯:" + ex.Message);
            }

            return result;
        }
    }
}
