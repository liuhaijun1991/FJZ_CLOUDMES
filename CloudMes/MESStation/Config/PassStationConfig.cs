using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using System.Data;

namespace MESStation.Config
{
    public class PassStationConfig : MesAPIBase
    {
        protected APIInfo FPass = new APIInfo
        {
            FunctionName = "DoPass",
            Description = "Do Pass by wo or lotno or sn",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "PassType" },
                new APIInputInfo() { InputName = "PassData" },
                new APIInputInfo() { InputName = "PassReason" },
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUnPass = new APIInfo
        {
            FunctionName = "DoUnPass",
            Description = "Do unPass by id",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "ID"},
                new APIInputInfo(){ InputName = "UnPassReason"}
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPassStation = new APIInfo
        {
            FunctionName = "GetPassStation",
            Description = "Get Pass station",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){InputName = "PassType" },
                new APIInputInfo(){InputName = "PassData"}
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPassInfo = new APIInfo
        {
            FunctionName = "GetPassInfo",
            Description = "Get pass info by wo or sn or lot",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "Type"},
                new APIInputInfo(){ InputName = "Data"},
                new APIInputInfo(){ InputName = "Status"}
            },
            Permissions = new List<MESPermission>()
        };

        //獲取所有的處於Locked中的數據
        protected APIInfo FGetPassedAllInfo = new APIInfo
        {
            FunctionName = "GetPassedAllInfo",
            Description = "Get All Passed Info",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
        };

        public PassStationConfig()
        {
            this.Apis.Add(FPass.FunctionName, FPass);
            this.Apis.Add(FUnPass.FunctionName, FUnPass);
            this.Apis.Add(FGetPassStation.FunctionName, FGetPassStation);
            this.Apis.Add(FGetPassInfo.FunctionName, FGetPassInfo);
            this.Apis.Add(FGetPassedAllInfo.FunctionName, FGetPassedAllInfo);
        }

        //GetPassedAllInfo顯示所有的正處於Passed的數據
        public void GetPassedAllInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_SN_PASS>().Where(rsp=>rsp.STATUS=="1")
                    .OrderBy(rsl => rsl.CREATE_TIME, SqlSugar.OrderByType.Desc)
                    .OrderBy(rsl => rsl.SN, SqlSugar.OrderByType.Asc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetPassInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {


            string type = Data["Type"].ToString().ToUpper();
            string status = Data["Status"].ToString();
            List<string> snList = new List<string>();
            OleExec sfcdb = null;
            List<R_SN_PASS> passList = new List<R_SN_PASS>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                // T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DBTYPE);
                if (status.ToUpper() == "ALL")
                {
                    status = "";
                }
                if (type == "BYWO")
                {

                    passList = GetPassList("", "", "", Data["Data"].ToString(), status, sfcdb);
                }
                else if (type == "BYLOT")
                {
                    passList = GetPassList("", Data["Data"].ToString(), "", "", status, sfcdb);
                }
                else if (type == "BYSN")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];

                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        passList.AddRange(GetPassList("", "", arraySN[i].ToString(), "", status, sfcdb));
                    }

                }
                else if (type == "BYID")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        passList.AddRange(GetPassList(arraySN[i].ToString(), "", "", "", status, sfcdb));
                    }
                }
                else if (type == "BYSKU")
                {
                    string sku = Data["Data"].ToString().Trim().ToUpper();
                    passList = sfcdb.ORM.Queryable<R_SN_PASS>().Where(r => r.TYPE == "SKU" && r.WORKORDERNO == sku).WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(status), r => r.STATUS == status).ToList();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = passList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetPassStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string passType = Data["PassType"].ToString().ToUpper().Trim();
            string passData = Data["PassData"].ToString().ToUpper().Trim();
            DataTable routeTable = new DataTable();
            List<string> stationList = new List<string>();
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DBTYPE);
                if (passType == "PASSBYWO")
                {
                    T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                    Row_R_WO_BASE rowWo = t_r_wo_base.GetWo(passData, sfcdb);
                    if (rowWo == null)
                    {
                        throw new Exception(passData + " Not Exists!");
                    }
                    R_WO_BASE r_wo_base = rowWo.GetDataObject();
                    stationList = t_c_route_detail.GetByRouteIdOrderBySEQASC(r_wo_base.ROUTE_ID, sfcdb).Select(route => route.STATION_NAME).ToList();
                }
                else if (passType == "PASSBYSKU")
                {
                    T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBTYPE);
                    C_SKU c_sku = t_c_sku.GetSku(passData, sfcdb);
                    if (c_sku == null)
                    {
                        throw new Exception(passData + " Not Exists!");
                    }
                    R_SKU_ROUTE skuRoute = sfcdb.ORM.Queryable<R_SKU_ROUTE>().Where(r => r.SKU_ID == c_sku.ID).ToList().FirstOrDefault();
                    stationList = t_c_route_detail.GetByRouteIdOrderBySEQASC(skuRoute.ROUTE_ID, sfcdb).Select(route => route.STATION_NAME).ToList();
                }
                else if (passType == "PASSBYLOT")
                {
                    T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(sfcdb, DBTYPE);
                    Row_R_LOT_STATUS rowLotStatus = t_r_lot_status.GetByLotNo(passData, sfcdb);
                    if (rowLotStatus.ID == null)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000161", new string[] { }));
                    }
                    R_LOT_STATUS r_lot_status = rowLotStatus.GetDataObject();
                    T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBTYPE);
                    C_SKU c_sku = t_c_sku.GetSku(r_lot_status.SKUNO, sfcdb);
                    T_R_SKU_ROUTE t_r_sku_route = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    List<R_SKU_ROUTE> r_sku_route_list = t_r_sku_route.GetMappingBySkuId(c_sku.ID, sfcdb);
                    if (r_sku_route_list.Count > 0)
                    {
                        //t_c_route_detail.GetByRouteIdOrderBySEQASC(r_sku_route_list[0].ROUTE_ID, sfcdb);
                        stationList = t_c_route_detail.GetByRouteIdOrderBySEQASC(r_sku_route_list[0].ROUTE_ID, sfcdb).Select(route => route.STATION_NAME).ToList();
                    }
                    else
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000179", new string[] { }));
                    }

                }
                else
                {
                    routeTable = t_c_route_detail.GetALLStation(sfcdb);
                    foreach (DataRow row in routeTable.Rows)
                    {
                        stationList.Add(row["station_name"].ToString());
                    }
                    stationList.Sort();
                }
                //2019/04/22 patty added: "ALL" station
                stationList.Add("ALL");

                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = stationList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DoPass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string passType = Data["PassType"].ToString().ToUpper().Trim();
            string passReason = Data["PassReason"].ToString().Trim();
            string passStation = Data["PassStation"].ToString().ToUpper().Trim();
            OleExec sfcdb = null;
            T_R_SN_PASS trsp = null;
            Row_R_SN_PASS rowSNPass = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                trsp = new T_R_SN_PASS(sfcdb, DBTYPE);
                if (passType == "PASSBYWO")
                {
                    if (IsUnPass("", "", Data["PassData"].ToString().Trim(), passStation, sfcdb))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["PassData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNPass = (Row_R_SN_PASS)trsp.NewRow();
                    rowSNPass.ID = trsp.GetNewID(this.BU, sfcdb);
                    rowSNPass.WORKORDERNO = Data["PassData"].ToString().Trim();
                    rowSNPass.TYPE = "WO";
                    rowSNPass.PASS_STATION = passStation;
                    rowSNPass.REASON = passReason;
                    rowSNPass.STATUS = "1";
                    rowSNPass.CREATE_EMP = this.LoginUser.EMP_NO;
                    rowSNPass.CREATE_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNPass.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (passType == "PASSBYLOT")
                {
                    if (IsUnPass(Data["PassData"].ToString().Trim(), "", "", passStation, sfcdb))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["PassData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNPass = (Row_R_SN_PASS)trsp.NewRow();
                    rowSNPass.ID = trsp.GetNewID(this.BU, sfcdb);
                    rowSNPass.LOTNO = Data["PassData"].ToString().Trim();
                    rowSNPass.TYPE = "LOT";
                    rowSNPass.PASS_STATION = passStation;
                    rowSNPass.REASON = passReason;
                    rowSNPass.STATUS = "1";
                    rowSNPass.CREATE_EMP = this.LoginUser.EMP_NO;
                    rowSNPass.CREATE_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNPass.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (passType == "PASSBYSN")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["PassData"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        if (!IsUnPass("", arraySN[i].ToString(),"", passStation, sfcdb))
                        {
                            rowSNPass = (Row_R_SN_PASS)trsp.NewRow();
                            rowSNPass.ID = trsp.GetNewID(this.BU, sfcdb);
                            rowSNPass.SN = arraySN[i].ToString();
                            rowSNPass.TYPE = "SN";
                            rowSNPass.PASS_STATION = passStation;
                            rowSNPass.REASON = passReason;
                            rowSNPass.STATUS = "1";
                            rowSNPass.CREATE_EMP = this.LoginUser.EMP_NO;
                            rowSNPass.CREATE_TIME = GetDBDateTime();
                            sfcdb.ThrowSqlExeception = true;
                            sfcdb.ExecSQL(rowSNPass.GetInsertString(DBTYPE));
                        }
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (passType == "BYPASSORT")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["PassData"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        string updatero = $@"update r_ort set sn='BYPASS'|| sn where sn='{arraySN[i].ToString()}' ";
                        string updateroa = $@"update R_ORT_ALERT set sn='BYPASS'|| sn where sn='{arraySN[i].ToString()}' ";
                        sfcdb.ExecSQL(updatero);
                        sfcdb.ExecSQL(updateroa);
                        if (!IsUnPass("",arraySN[i].ToString(), "", passStation, sfcdb))
                        {
                            rowSNPass = (Row_R_SN_PASS)trsp.NewRow();
                            rowSNPass.ID = trsp.GetNewID(this.BU, sfcdb);
                            rowSNPass.SN = arraySN[i].ToString();
                            rowSNPass.TYPE = "BYPASSORT";
                            rowSNPass.PASS_STATION = passStation;
                            rowSNPass.REASON = passReason;
                            rowSNPass.STATUS = "1";
                            rowSNPass.CREATE_EMP = this.LoginUser.EMP_NO;
                            rowSNPass.CREATE_TIME = GetDBDateTime();
                            sfcdb.ThrowSqlExeception = true;
                            sfcdb.ExecSQL(rowSNPass.GetInsertString(DBTYPE));
                        }
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (passType == "PASSBYSKU")
                {
                    string skuno = Data["PassData"].ToString().Trim();
                    R_SN_PASS passObj = sfcdb.ORM.Queryable<R_SN_PASS>().Where(r => r.TYPE == "SKU" && r.WORKORDERNO == skuno && r.PASS_STATION == passStation && r.STATUS == "1").ToList().FirstOrDefault();
                    if (passObj != null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["PassData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNPass = (Row_R_SN_PASS)trsp.NewRow();
                    rowSNPass.ID = trsp.GetNewID(this.BU, sfcdb);
                    rowSNPass.WORKORDERNO = skuno;
                    rowSNPass.TYPE = "SKU";
                    rowSNPass.PASS_STATION = passStation;
                    rowSNPass.REASON = passReason;
                    rowSNPass.STATUS = "1";
                    rowSNPass.CREATE_EMP = this.LoginUser.EMP_NO;
                    rowSNPass.CREATE_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNPass.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "PassType" }));
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DoUnPass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            string unlockReason = Data["UnPassReason"].ToString();
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_SN_PASS trsp = new T_R_SN_PASS(sfcdb, DBTYPE);

                for (int i = 0; i < arraySN.Count; i++)
                {
                    Row_R_SN_PASS rowPass = (Row_R_SN_PASS)trsp.GetObjByID(arraySN[i].ToString(), sfcdb);
                    if (rowPass.STATUS == "1")
                    {
                        rowPass.STATUS = "0";
                        rowPass.CANCEL_REASON = unlockReason;
                        rowPass.CANCEL_EMP = this.LoginUser.EMP_NO;
                        rowPass.CANCEL_TIME = GetDBDateTime();
                        sfcdb.ExecSQL(rowPass.GetUpdateString(DBTYPE));
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = "";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public List<R_SN_PASS> GetPassList(string PASS_ID, string LOTNO, string SN, string WORKORDERNO, string STATUS, OleExec DB)
        {
            List<R_SN_PASS> Seq = new List<R_SN_PASS>();
            T_R_SN_PASS trsp = null;
            //OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            trsp = new T_R_SN_PASS(DB, DBTYPE);
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_R_SN_PASS SeqRow = (Row_R_SN_PASS)trsp.NewRow();
            if (DBTYPE.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT ID,LOTNO,SN,TYPE,WORKORDERNO,PASS_STATION,CASE WHEN STATUS =1 THEN 'YES' ELSE 'NO' END STATUS ,REASON,CANCEL_REASON,CREATE_EMP,CREATE_TIME,CANCEL_EMP,CANCEL_TIME FROM R_SN_PASS WHERE  1=1 AND STATUS =1  ";

                if (PASS_ID != "")
                {
                    sql += $@" and ID='{PASS_ID}' ";
                }

                if (LOTNO != "")
                {
                    sql += $@" and LOTNO='{LOTNO}' ";
                }

                if (SN != "")
                {
                    sql += $@" and SN='{SN}' ";
                }

                if (WORKORDERNO != "")
                {
                    sql += $@" and WORKORDERNO='{WORKORDERNO}' ";
                }
                if (STATUS != "")
                {
                    sql += $@" and STATUS='{STATUS}'";
                }
                if (LOTNO == "" && SN == "" && WORKORDERNO == "")
                {
                    sql += $@" and  rownum<21  order by CREATE_TIME ";
                }
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SeqRow.loadData(dr);
                    Seq.Add(SeqRow.GetDataObject());
                }
            }
            else
            {
                //if (sfcdb != null)
                //{
                //    DBPools["SFCDB"].Return(sfcdb);
                //}
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBTYPE.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return Seq;
        }

        public bool IsUnPass(string LOTNO, string SN, string WORKORDERNO, string station, OleExec DB)
        {
            List<R_SN_PASS> Seq = new List<R_SN_PASS>();
            string sql = string.Empty;
            bool isPass = true;

            if (this.DBTYPE.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select * from R_SN_PASS where 1=1 AND STATUS =1 and pass_station='{station}'  ";
                if (LOTNO != "")
                {
                    sql += $@" and LOTNO='{LOTNO}' ";
                }
                if (SN != "")
                {
                    sql += $@" and SN='{SN}' ";
                }
                if (WORKORDERNO != "")
                {
                    sql += $@" and WORKORDERNO='{WORKORDERNO}' ";
                }
                if (DB.ExecSelect(sql, null).Tables[0].Rows.Count == 0)
                {
                    isPass = false;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBTYPE.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return isPass;
        }
    }
}
