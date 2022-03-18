using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{ 
    public class BonepileConfig : MesAPIBase
    {
        protected APIInfo FUploadPartnoPrice = new APIInfo()
        {
            FunctionName = "UploadPartnoPrice",
            Description = "Upload Partno Price",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetUploadPriceRecord = new APIInfo()
        {
            FunctionName = "GetUploadPriceRecord",
            Description = "Get Upload Price Record",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadCustomerCriticalSN = new APIInfo()
        {
            FunctionName = "UploadCustomerCriticalSN",
            Description = "Upload Customer Critical SN",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetUploadCustomerCriticalSNRecord = new APIInfo()
        {
            FunctionName = "GetUploadCustomerCriticalSNRecord",
            Description = "Get Upload Customer Critical SN Record",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadCriticalDesc = new APIInfo()
        {
            FunctionName = "UploadCriticalDesc",
            Description = "Upload Critical Desc",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetUploadCriticalDescRecord = new APIInfo()
        {
            FunctionName = "GetUploadCriticalDescRecord",
            Description = "Get Upload Critical Desc Record",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadFoxconnCriticalSN = new APIInfo()
        {
            FunctionName = "UploadFoxconnCriticalSN",
            Description = "Upload Foxconn Critical SN",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetUploadFoxconnCriticalSNRecord = new APIInfo()
        {
            FunctionName = "GetUploadFoxconnCriticalSNRecord",
            Description = "Get Upload Foxconn Critical SN Record",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetBonepileSummaryReportData = new APIInfo()
        {
            FunctionName = "GetBonepileSummaryReportData",
            Description = "Get Bonepile Summary Report Data",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "DataClass", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "Date", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCheckCollectPrivilege = new APIInfo()
        {
            FunctionName = "CheckCollectPrivilege",
            Description = "Check Collect Privilege",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName = "DataClass", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "Date", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FClosedBonepileData = new APIInfo()
        {
            FunctionName = "ClosedBonepileData",
            Description = "Closed Bonepile Data",
            Parameters = new List<APIInputInfo>()
            {                
                 new APIInputInfo() { InputName = "Type", InputType = "string", DefaultValue = "" },
                  new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "Remark", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public BonepileConfig()
        {
            this.Apis.Add(FUploadPartnoPrice.FunctionName, FUploadPartnoPrice);
            this.Apis.Add(FGetUploadPriceRecord.FunctionName, FGetUploadPriceRecord);
            this.Apis.Add(FUploadCustomerCriticalSN.FunctionName, FUploadCustomerCriticalSN);
            this.Apis.Add(FGetUploadCustomerCriticalSNRecord.FunctionName, FGetUploadCustomerCriticalSNRecord);
            this.Apis.Add(FUploadCriticalDesc.FunctionName, FUploadCriticalDesc);
            this.Apis.Add(FGetUploadCriticalDescRecord.FunctionName, FGetUploadCriticalDescRecord);
            this.Apis.Add(FUploadFoxconnCriticalSN.FunctionName, FUploadFoxconnCriticalSN);
            this.Apis.Add(FGetUploadFoxconnCriticalSNRecord.FunctionName, FGetUploadFoxconnCriticalSNRecord);
            this.Apis.Add(FGetBonepileSummaryReportData.FunctionName, FGetBonepileSummaryReportData);
            this.Apis.Add(FCheckCollectPrivilege.FunctionName, FCheckCollectPrivilege);
            this.Apis.Add(FClosedBonepileData.FunctionName, FClosedBonepileData);
        }

        public void UploadPartnoPrice(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["ExcelData"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113501");
                    throw new Exception(errMessage);
                }
                if (Data["FileName"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113918");
                    throw new Exception(errMessage);
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114056");
                    throw new Exception(errMessage);
                }
                List<string> listColumns = new List<string>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    listColumns.Add(dt.Columns[j].ToString());
                }
                //定義上傳Excel的列名
                List<string> inputTitle = new List<string> { "P/N", "Price" };
                string errTitle = "";
                string pn = "", price = "";                     
                bool hasErr = CheckInputExcelTitle(listColumns, inputTitle, out errTitle);
                if (!hasErr)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114415", new string[] { errTitle });
                    throw new Exception(errMessage);
                }
                T_R_PN_MASTER_DATA t_r_pn_master_data = new T_R_PN_MASTER_DATA(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_SKU t_c_sku = new T_C_SKU(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_PN_MASTER_DATA master = null;

                int dotnum;
                int result;
                bool bPNExist = false;
                int failCount = 0;
                int passCount = 0;
                DateTime sysdt = t_r_pn_master_data.GetDBDateTime(SFCDB);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    pn = dt.Rows[i]["P/N"].ToString();
                    price = dt.Rows[i]["Price"].ToString();
                    dotnum = price.IndexOf(".");
                    if (dotnum > 0)
                    {
                        price = price + "00";
                        price = price.Substring(0, dotnum + 3);
                    }
                    master = t_r_pn_master_data.GetMasterObj(SFCDB, pn);
                    if (master != null)
                    {
                        master.PRICE1 = Convert.ToDouble(price);
                        master.LAST_EDIT_BY = LoginUser.EMP_NO;
                        master.LAST_EDIT_DT = sysdt;
                        result = t_r_pn_master_data.Update(SFCDB, master);
                        passCount++;
                    }
                    else
                    {
                        bPNExist = t_c_sku.IsExists(pn, SFCDB);
                        if (bPNExist)
                        {
                            master = new R_PN_MASTER_DATA();
                            master.ID = t_r_pn_master_data.GetNewID(BU, SFCDB);
                            master.PN = pn;
                            master.PRICE1 = Convert.ToDouble(price);
                            master.LAST_EDIT_BY = LoginUser.EMP_NO;
                            master.LAST_EDIT_DT = sysdt;
                            result = t_r_pn_master_data.Save(SFCDB, master);
                            passCount++;
                        }
                        else
                        {
                            //記錄FAIL 信息
                            t_r_mes_log.InsertMESLog(SFCDB, BU, "BonepileUploadPartnoPrice", "MESStation.Config.BonepileConfig", "UploadPartnoPrice", "該料號在系統中不存在，請找PE維護",
                                pn, LoginUser.EMP_NO);
                            failCount++;
                        }
                    }
                }
                if (passCount == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115039");
                    StationReturn.Message = errMessage;
                }
                else if (failCount == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115248"); 
                    StationReturn.Message = errMessage;
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814120033", new string[] { Convert.ToString (passCount), Convert.ToString(failCount)});
                    
                    StationReturn.Message = errMessage;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetUploadPriceRecord(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["Type"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218");
                    throw new Exception(errMessage);
                }
                string type = Data["Type"].ToString();
                string sql = "";
                DataTable dt = new DataTable();
                if (type.ToUpper() == "SUCCESS")
                {
                    sql = $@"select a.pn,c.route_name,b.sku_name,b.sku_type,b.cust_sku_code,b.description,a.last_edit_by,a.last_edit_dt from r_pn_master_data a left join c_sku b on a.pn=b.skuno
                            left join(select m.route_name, n.sku_id from r_sku_route n, c_route m where n.route_id= m.id) c on c.sku_id = b.id
                            where a.last_edit_by = '{LoginUser.EMP_NO}' and a.last_edit_dt > sysdate - 1";
                }
                else if (type.ToUpper() == "FAIL")
                {
                    sql = $@"select data1 as pn,log_message,edit_emp,edit_time from r_mes_log where program_name='BonepileUploadPartnoPrice' and class_name='MESStation.Config.BonepileConfig' 
                                and function_name='UploadPartnoPrice' and edit_emp = '{LoginUser.EMP_NO}' and edit_time> sysdate - 1 ";
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142342");
                    throw new Exception(errMessage);
                }
                dt = SFCDB.ExecSelect(sql, null).Tables[0];
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104911");
                    throw new Exception(errMessage);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(dt.Rows.Count);
                StationReturn.Data = dt;
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }
        
        public void UploadCustomerCriticalSN(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["ExcelData"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113501"); 
                    throw new Exception(errMessage);
                }
                if (Data["FileName"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113918");
                    throw new Exception(errMessage);
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114056");
                    throw new Exception(errMessage);
                }
                List<string> listColumns = new List<string>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    listColumns.Add(dt.Columns[j].ToString());
                }
                
                //定義上傳Excel的列名
                List<string> inputTitle = new List<string> { "S/N", "BRCStartDate" , "FailStation", "FailureSymptom", "BonepileDescription", "BonepileStation", "BonepileCategory", "Remark1" };               

                string errTitle = "";
                string sn = "", startdate = "", failstation ="", failsymptom ="", description ="", station = "", bonepileCategory ="", remark ="";
               
                bool hasErr = CheckInputExcelTitle(listColumns, inputTitle, out errTitle);
                if (!hasErr)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114415", new string[] { errTitle });
                    throw new Exception(errMessage);
                }
                T_R_PN_MASTER_DATA t_r_pn_master_data = new T_R_PN_MASTER_DATA(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_SKU t_c_sku = new T_C_SKU(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_SN t_r_sn = new T_R_SN(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_SERIES t_c_series = new T_C_SERIES(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                MESDataObject.Module.Vertiv.T_R_RMA_BONEPILE t_r_rma_bonepile = new MESDataObject.Module.Vertiv.T_R_RMA_BONEPILE(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_Station t_r_station = new T_R_Station(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_CRITICAL_BONEPILE t_r_critical_bonepile = new T_R_CRITICAL_BONEPILE(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_MRB t_r_mrb = new T_R_MRB(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);               
                T_C_CUSTOMER t_c_customer = new T_C_CUSTOMER(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);


                R_SN snObj = null;
                C_SKU skuObj = null;
                C_SERIES series = null;
                MESDataObject.Module.Vertiv.R_RMA_BONEPILE rmaBonepile = null;
                R_CRITICAL_BONEPILE critical = null;
                R_PN_MASTER_DATA masterData = null;

                int result;
                bool bNozw = true;
                bool bRMA = false;
                DateTime sysdt = t_r_pn_master_data.GetDBDateTime(SFCDB);
                int failCount = 0;
                int passCount = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                { 
                    sn = dt.Rows[i]["S/N"].ToString().Trim();                   
                    if (sn != "")
                    {                        
                        startdate = dt.Rows[i]["BRCStartDate"].ToString().Trim();
                        failstation = dt.Rows[i]["FailStation"].ToString().Trim();
                        failsymptom = dt.Rows[i]["FailureSymptom"].ToString().Trim();
                        description = dt.Rows[i]["BonepileDescription"].ToString().Trim();
                        station = dt.Rows[i]["BonepileStation"].ToString().Trim();
                        bonepileCategory = dt.Rows[i]["BonepileCategory"].ToString().Trim();
                        remark = dt.Rows[i]["Remark1"].ToString().Trim();
                        try
                        {
                            bNozw = CheckChinese(dt.Rows[i], inputTitle);
                            if (!bNozw)
                            {

                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145531"); 
                                throw new Exception(errMessage);
                            }
                            if (failsymptom.Length > 150)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150436"); 
                                throw new Exception(errMessage);
                            }
                            if (description.Length > 300)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150741");
                                throw new Exception(errMessage);
                            }
                            if (remark.Length == 0)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151006");
                                throw new Exception(errMessage);
                            }
                            if (remark.Length > 100)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151117");
                                throw new Exception(errMessage);
                            }
                            snObj = t_r_sn.GetSN(sn, SFCDB);
                            if (snObj == null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151201", new string[] { sn }); 
                                throw new Exception(errMessage);
                            }
                            if (snObj.SCRAPED_FLAG == "1")
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151355", new string[] { sn }); 
                                throw new Exception(errMessage);
                            }
                            skuObj = t_c_sku.GetSku(snObj.SKUNO, SFCDB);
                            series = t_c_series.GetDetailById(SFCDB, skuObj.C_SERIES_ID);
                            masterData = t_r_pn_master_data.GetMasterObj(SFCDB, snObj.SKUNO);
                            if (series == null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102404"); 
                                throw new Exception(errMessage);
                            }
                            if (snObj.SHIPPED_FLAG == "1" || snObj.CURRENT_STATION == "SHIPOUT")
                            {
                                if (!t_r_rma_bonepile.RmaBonepileIsOpen(SFCDB, snObj.SN))
                                {
                                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151640"); 
                                    throw new Exception(errMessage);
                                }
                                else
                                {
                                    bRMA = true;
                                }
                            }
                            else
                            {
                                if (t_r_rma_bonepile.IsInRmaBonepile(SFCDB, snObj.SN))
                                {
                                    if (!t_r_rma_bonepile.RmaBonepileIsOpen(SFCDB, snObj.SN))
                                    {
                                        string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151640");
                                        throw new Exception(errMessage);
                                    }
                                    else
                                    {
                                        bRMA = true;
                                    }
                                }
                            }

                            if (!t_r_station.StationIsExist(SFCDB, station))
                            {                                
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152219");
                                throw new Exception(errMessage);
                            }
                            if (bonepileCategory != "Cosmetic" && bonepileCategory != "Function")
                            {  
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152821");
                                throw new Exception(errMessage);
                            }
                            List<R_CRITICAL_BONEPILE> listCriticla = t_r_critical_bonepile.GetRecordBySn(SFCDB, snObj.SN);
                            R_CRITICAL_BONEPILE openCritical = listCriticla.Find(r => r.CLOSED_FLAG == "0");
                            if (openCritical != null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152839");
                                throw new Exception(errMessage);
                            }
                            DateTime dtStart = Convert.ToDateTime(startdate);
                            R_CRITICAL_BONEPILE closeCritical = listCriticla.Find(r => r.CLOSED_FLAG == "1" && r.BRC_START_DATE >= dtStart);
                            if (closeCritical != null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153054");
                                throw new Exception(errMessage);
                            }
                            if (snObj.COMPLETED_FLAG == "1" && (snObj.CURRENT_STATION == "MRB" || snObj.NEXT_STATION == "REWORK"))
                            {
                                R_MRB mrb = t_r_mrb.GetRecerdBySNAndWO(SFCDB, snObj.SN, snObj.WORKORDERNO);
                                string mrbSation = mrb == null ? "" : mrb.NEXT_STATION;
                                C_ROUTE_DETAIL mrbRoute = t_c_route_detail.GetStationRoute(snObj.ROUTE_ID, mrbSation, SFCDB);
                                double? mrbSeq = mrbRoute == null ? 0 : mrbRoute.SEQ_NO;
                                C_ROUTE_DETAIL bonepileRoute = t_c_route_detail.GetStationRoute(snObj.ROUTE_ID, station, SFCDB);
                                double? bonepileSeq = bonepileRoute == null ? 0 : bonepileRoute.SEQ_NO;
                                if (mrbSeq > bonepileSeq)
                                {
                                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153301");
                                    throw new Exception(errMessage);
                                }
                            }
                            else
                            {
                                C_ROUTE_DETAIL currentStationRoute = t_c_route_detail.GetStationRoute(snObj.ROUTE_ID, snObj.CURRENT_STATION, SFCDB);
                                double? currentSeq = currentStationRoute == null ? 0 : currentStationRoute.SEQ_NO;
                                C_ROUTE_DETAIL bonepileRoute = t_c_route_detail.GetStationRoute(snObj.ROUTE_ID, station, SFCDB);
                                double? bonepileSeq = bonepileRoute == null ? 0 : bonepileRoute.SEQ_NO;
                                if (currentSeq > bonepileSeq)
                                {
                                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153902");
                                    throw new Exception(errMessage);
                                }
                            }

                            if (bRMA)
                            {
                                station = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == snObj.ROUTE_ID && SqlSugar.SqlFunc.EndsWith(r.STATION_NAME, "PACKOUT"))
                                    .ToList().FirstOrDefault().STATION_NAME;
                                rmaBonepile = t_r_rma_bonepile.GetOpenRecord(SFCDB, snObj.SN);
                                if (rmaBonepile != null)
                                {
                                    //update sfc_rmabonepile_report set CriticalBonepile=1 where sysserialno=@SN and closed=0
                                    //待寫
                                    //rmaBonepile.c
                                }
                            }
                            SFCDB.ORM.Updateable<R_NORMAL_BONEPILE>().UpdateColumns(r => new R_NORMAL_BONEPILE
                            {
                                CLOSED_FLAG = "1",
                                CLOSED_BY = "System",
                                CLOSED_REASON = "CriticalBonepile Insert",
                                LASTEDIT_BY = "System",
                                CLOSED_DATE = sysdt,
                                LASTEDIT_DATE = sysdt
                            })
                            .Where(r => r.SN == snObj.SN && r.CLOSED_FLAG == "0").ExecuteCommand();

                            DateTime? loadDate = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.SN == snObj.SN && SqlSugar.SqlFunc.Contains(r.STATION_NAME, "LOAD"))
                                .ToList().FirstOrDefault().EDIT_TIME;

                            string current_station = snObj.REPAIR_FAILED_FLAG == "1" ? snObj.NEXT_STATION : snObj.CURRENT_STATION;

                            critical = new R_CRITICAL_BONEPILE();
                            critical.ID = t_r_critical_bonepile.GetNewID(BU, SFCDB);
                            critical.SN = snObj.SN;
                            critical.BRC_START_DATE = dtStart;
                            critical.FIRST_LOAD_DATE = loadDate;
                            critical.WORKORDERNO = snObj.WORKORDERNO;
                            critical.SKUNO = snObj.SKUNO;
                            critical.PRODUCT_NAME = skuObj.SKU_NAME;// "產品名稱";     
                            critical.SUB_SERIES = series.SERIES_NAME;// "產品系列";
                            critical.PRODUCT_SERIES = t_c_customer.GetCustomerName(SFCDB, series.CUSTOMER_ID);//"客戶名稱";
                            critical.FAIL_STATION = failstation;
                            critical.FAILURE_SYMPTOM = failsymptom;
                            critical.BONEPILE_DESCRIPTION = description;
                            critical.BONEPILE_CATEGORY = bonepileCategory;
                            critical.OWNER = "BRC";
                            critical.BRC_REMARK = remark;
                            critical.CLOSED_FLAG = "0";
                            critical.RMA_FLAG = bRMA ? "1" : "0";
                            critical.CRITICAL_BONEPILE_FLAG = "1";
                            critical.HARDCORE_BOARD = "";//
                            if (bRMA)
                            {
                                critical.CURRENT_STATION = "Receiving";
                                critical.CLOSED_REASON = "[noted:RMA Bonepile Open]";
                            }
                            else
                            {
                                critical.CURRENT_STATION = current_station;
                            }
                            critical.CURRENT_STATUS = snObj.REPAIR_FAILED_FLAG;
                            critical.SCRAPPED_FLAG = snObj.SCRAPED_FLAG;
                            critical.SHIPPED_FLAG = snObj.SHIPPED_FLAG;
                            critical.PRICE = masterData == null ? 0 : masterData.PRICE1;
                            critical.UPLOAD_BY = LoginUser.EMP_NO;
                            critical.UPLOAD_DATE = sysdt;
                            critical.LASTEDIT_BY = LoginUser.EMP_NO;
                            critical.LASTEDIT_DATE = sysdt;

                            result = t_r_critical_bonepile.Save(SFCDB, critical);
                            if (result == 0)
                            {
                                throw new Exception($@"{snObj.SN} Save R_CRITICAL_BONEPILE Fail!");
                            }
                            passCount++;
                        }
                        catch (Exception ex)
                        {
                            t_r_mes_log.InsertMESLog(SFCDB, BU, "BonepileCustomerCriticalSN", "MESStation.Config.BonepileConfig", "UploadCustomerCriticalSN", ex.Message,
                                        sn, LoginUser.EMP_NO);
                            failCount++;
                        }
                    }
                }
                if (passCount == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115039");
                    StationReturn.Message = errMessage;
                }
                if (failCount == 0)
                {

                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115248"); 
                    StationReturn.Message = errMessage;
                }
                else
                {

                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814120033", new string[] { Convert.ToString(passCount), Convert.ToString(failCount) }); 
                    StationReturn.Message = errMessage;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetUploadCustomerCriticalSNRecord(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["Type"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218");
                    throw new Exception(errMessage);
                }
                string type = Data["Type"].ToString();
                string sql = "";
                DataTable dt = new DataTable();
                if (type.ToUpper() == "SUCCESS")
                {
                    sql = $@"select * from r_critical_bonepile a  where a.lastedit_by = '{LoginUser.EMP_NO}' and a.lastedit_date > sysdate - 1 order by lastedit_date desc";
                }
                else if (type.ToUpper() == "FAIL")
                {
                    sql = $@"select data1 as sn,log_message,edit_emp,edit_time from r_mes_log where program_name='BonepileCustomerCriticalSN' and class_name='MESStation.Config.BonepileConfig' 
                                and function_name='UploadCustomerCriticalSN' and edit_emp = '{LoginUser.EMP_NO}' and edit_time> sysdate - 1 order by edit_time desc ";
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142342"); 
                    throw new Exception(errMessage);
                }
                dt = SFCDB.ExecSelect(sql, null).Tables[0];
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104911"); 
                    throw new Exception(errMessage);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(dt.Rows.Count);
                StationReturn.Data = dt;
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void UploadCriticalDesc(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["ExcelData"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113501");
                    throw new Exception(errMessage);
                }
                if (Data["FileName"] == null)
                {

                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113918");
                    throw new Exception(errMessage);
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {

                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114056"); 
                    throw new Exception(errMessage);
                }
                List<string> listColumns = new List<string>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    listColumns.Add(dt.Columns[j].ToString());
                }
                //定義上傳Excel的列名
                List<string> inputTitle = new List<string> { "Sysserialno", "FailureSymptom", "BonepileDescription", "BRCRemark" };

                string errTitle = "";
                string sn = "", failsymptom = "", description = "", remark = "";
               
                bool hasErr = CheckInputExcelTitle(listColumns, inputTitle, out errTitle);
                if (!hasErr)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114415",new string[] { errTitle }); 
                    throw new Exception(errMessage);
                }
                int result;
                bool bNozw = true;                
                int failCount = 0;
                int passCount = 0;

                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_CRITICAL_BONEPILE t_r_critical_bonepile = new T_R_CRITICAL_BONEPILE(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_CRITICAL_BONEPILE critical = null;
                DateTime sysdate = t_r_critical_bonepile.GetDBDateTime(SFCDB);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {        
                        sn = dt.Rows[i]["Sysserialno"].ToString().Trim();
                        failsymptom = dt.Rows[i]["FailureSymptom"].ToString().Trim();
                        description = dt.Rows[i]["BonepileDescription"].ToString().Trim();
                        remark = dt.Rows[i]["BRCRemark"].ToString().Trim();
                        if (sn != "")
                        {
                            bNozw = CheckChinese(dt.Rows[i], inputTitle);
                            if (!bNozw)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145531"); 
                                throw new Exception(errMessage);
                            }
                            if (remark.Length == 0)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151006"); 
                                throw new Exception(errMessage);
                            }
                            critical = t_r_critical_bonepile.GetOpenRecord(SFCDB, sn);
                            if (critical == null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155124"); 
                                throw new Exception(errMessage);
                            }
                            critical.FAILURE_SYMPTOM = failsymptom;
                            critical.BONEPILE_DESCRIPTION = description;
                            critical.BRC_REMARK = critical.BRC_REMARK + "|" + remark;
                            critical.LASTEDIT_BY = LoginUser.EMP_NO;
                            critical.LASTEDIT_DATE = sysdate;
                            result = t_r_critical_bonepile.Update(SFCDB, critical);
                            if (result == 0)
                            {
                                throw new Exception($@"{sn},Update R_CRITICAL_BONEPILE Fail!");
                            }
                            passCount++;
                        }                       
                    }
                    catch (Exception ex)
                    {
                        t_r_mes_log.InsertMESLog(SFCDB, BU, "UploadCriticalDesc", "MESStation.Config.BonepileConfig", "UploadCriticalDesc", ex.Message,
                                    sn, LoginUser.EMP_NO);
                        failCount++;
                    }

                }

                if (passCount == 0)
                {

                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115039"); 
                    StationReturn.Message = errMessage;
                }
                if (failCount == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115248"); 
                    StationReturn.Message = errMessage;
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115248",new string[] {Convert.ToString(passCount), Convert.ToString(failCount) });
                    StationReturn.Message = errMessage;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetUploadCriticalDescRecord(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["Type"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218"); 
                    throw new Exception(errMessage);
                }
                string type = Data["Type"].ToString();
                string sql = "";
                DataTable dt = new DataTable();
                if (type.ToUpper() == "SUCCESS")
                {
                    sql = $@"select * from r_critical_bonepile a  where a.lastedit_by = '{LoginUser.EMP_NO}' and a.lastedit_date > sysdate - 1 order by lastedit_date desc";
                }
                else if (type.ToUpper() == "FAIL")
                {
                    sql = $@"select data1 as sn,log_message,edit_emp,edit_time from r_mes_log where program_name='UploadCriticalDesc' and class_name='MESStation.Config.BonepileConfig' 
                                and function_name='UploadCriticalDesc' and edit_emp = '{LoginUser.EMP_NO}' and edit_time> sysdate - 1 order by edit_time desc ";
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142342"); 
                    throw new Exception(errMessage);
                }
                dt = SFCDB.ExecSelect(sql, null).Tables[0];
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104911"); 
                    throw new Exception(errMessage);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(dt.Rows.Count);
                StationReturn.Data = dt;
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void UploadFoxconnCriticalSN(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["ExcelData"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113501"); 
                    throw new Exception(errMessage);
                }
                if (Data["FileName"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814113918"); 
                    throw new Exception(errMessage);
                }               
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160849"); 
                    throw new Exception(errMessage);
                }
                List<string> listColumns = new List<string>();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    listColumns.Add(dt.Columns[j].ToString());
                }
                //定義上傳Excel的列名 S/N	FLH Start Date	Owner (BRC/FLH)	Remark2
                List<string> inputTitle = new List<string> { "S/N", "FLHStartDate", "Owner(BRC/FLH)", "Remark2" };
                string errTitle = "";
                string sn = "", startDate = "", owner = "", remark = "";
                bool hasErr = CheckInputExcelTitle(listColumns, inputTitle, out errTitle);
                if (!hasErr)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114415",new string[] { errTitle }); 
                    throw new Exception(errMessage);
                }
                int result;
                bool bNozw = true;
                int failCount = 0;
                int passCount = 0;

                T_R_PN_MASTER_DATA t_r_pn_master_data = new T_R_PN_MASTER_DATA(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_SKU t_c_sku = new T_C_SKU(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_SN t_r_sn = new T_R_SN(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_SERIES t_c_series = new T_C_SERIES(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);           
                T_R_CRITICAL_BONEPILE t_r_critical_bonepile = new T_R_CRITICAL_BONEPILE(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_CUSTOMER t_c_customer = new T_C_CUSTOMER(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);


                R_SN snObj = null;
                C_SKU skuObj = null;
                C_SERIES series = null;
                R_CRITICAL_BONEPILE critical = null;
                R_PN_MASTER_DATA masterData = null;

                DateTime sysdate = t_r_critical_bonepile.GetDBDateTime(SFCDB);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {                        
                        sn = dt.Rows[i]["S/N"].ToString().Trim();                        
                        if (sn != "")
                        {
                            startDate = dt.Rows[i]["FLHStartDate"].ToString().Trim();
                            owner = dt.Rows[i]["Owner(BRC/FLH)"].ToString().Trim();
                            remark = dt.Rows[i]["Remark2"].ToString().Trim();
                            DateTime? dtStart = Convert.ToDateTime(startDate);
                            bNozw = CheckChinese(dt.Rows[i], inputTitle);
                            if (!bNozw)
                            {
                                
                             string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145531");
                                throw new Exception(errMessage);
                            }
                            if (remark.Length == 0)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161550");
                                throw new Exception(errMessage);
                            }
                            if (remark.Length > 100)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162512");
                                throw new Exception(errMessage);
                            }
                            if (owner.Length == 0)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162557");
                                throw new Exception(errMessage);
                            }
                            snObj = t_r_sn.GetSN(sn, SFCDB);
                            if (snObj == null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151201", new string[] { sn });
                                throw new Exception(errMessage);
                            }
                            if (snObj.SCRAPED_FLAG == "1")
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151355", new string[] { sn });
                                throw new Exception(errMessage);
                            }
                            skuObj = t_c_sku.GetSku(snObj.SKUNO, SFCDB);
                            series = t_c_series.GetDetailById(SFCDB, skuObj.C_SERIES_ID);
                            masterData = t_r_pn_master_data.GetMasterObj(SFCDB, snObj.SKUNO);
                            if (snObj == null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102404"); 
                                throw new Exception(errMessage);
                            }
                            critical = t_r_critical_bonepile.GetOpenRecord(SFCDB, sn);
                            if (critical == null)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155124"); 
                                throw new Exception(errMessage);
                            }
                            string owner2 = critical.OWNER;
                            DateTime? startDate2 = critical.FLH_START_DATE;
                            if (owner == owner2)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163016"); 
                                throw new Exception(errMessage);
                            }                            
                            if (owner == "FLH" && (startDate2 >= dtStart))
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163025");
                                throw new Exception(errMessage);
                            }
                            if (owner == "BRC")
                            {
                                dtStart = null;
                            }

                            critical.OWNER = owner;
                            critical.FLH_START_DATE = dtStart;
                            critical.FLH_REMARK = (critical.FLH_REMARK == "") ? remark : critical.FLH_REMARK + "|" + remark;
                            critical.LASTEDIT_BY = LoginUser.EMP_NO;
                            critical.LASTEDIT_DATE = sysdate;
                            critical.UPLOADLH_BY = LoginUser.EMP_NO;
                            critical.UPLOADLH_DATE = sysdate;
                            result = t_r_critical_bonepile.Update(SFCDB, critical);
                            if (result == 0)
                            {
                                string errMessage = MESReturnMessage.GetMESReturnMessage("MES00000025",new string[] { sn}); 
                                throw new Exception(errMessage);
                            }
                            passCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        t_r_mes_log.InsertMESLog(SFCDB, BU, "UploadFoxconnCriticalSN", "MESStation.Config.BonepileConfig", "UploadFoxconnCriticalSN", ex.Message,
                                    sn, LoginUser.EMP_NO);
                        failCount++;
                    }

                }

                if (passCount == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115039");
                    StationReturn.Message = errMessage;
                }
                if (failCount == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115248");
                    StationReturn.Message = errMessage;
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814120033", new string[] { Convert.ToString(passCount), Convert.ToString(failCount) });
                    StationReturn.Message = errMessage;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetUploadFoxconnCriticalSNRecord(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["Type"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218");
                    throw new Exception(errMessage);
                }
                string type = Data["Type"].ToString();
                string sql = "";
                DataTable dt = new DataTable();
                if (type.ToUpper() == "SUCCESS")
                {
                    sql = $@"select * from r_critical_bonepile a  where a.lastedit_by = '{LoginUser.EMP_NO}' and a.lastedit_date > sysdate - 1 order by lastedit_date desc";
                }
                else if (type.ToUpper() == "FAIL")
                {
                    sql = $@"select data1 as sn,log_message,edit_emp,edit_time from r_mes_log where program_name='UploadFoxconnCriticalSN' and class_name='MESStation.Config.BonepileConfig' 
                                and function_name='UploadFoxconnCriticalSN' and edit_emp = '{LoginUser.EMP_NO}' and edit_time> sysdate - 1 order by edit_time desc ";
                }
                else
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142342");
                    throw new Exception(errMessage);
                }
                dt = SFCDB.ExecSelect(sql, null).Tables[0];
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                if (dt.Rows.Count == 0)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104911"); 
                    throw new Exception(errMessage);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(dt.Rows.Count);
                StationReturn.Data = dt;
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetBonepileSummaryReportData(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["DataClass"] == null)
                {
                    throw new Exception($@"DataClass Is Null!");
                }
                if (Data["Type"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218");
                    throw new Exception(errMessage);
                }
                if (Data["Date"] == null)
                {
                    throw new Exception($@"Date Is Null!");
                }
                string data_class = Data["DataClass"].ToString();
                string type = Data["Type"].ToString();
                string date = Data["Date"].ToString();
                date = string.IsNullOrEmpty(date) ? SFCDB.ORM.GetDate().ToString("yyyy/MM/dd HH:mm:ss") : date;
                DateTime current_date = Convert.ToDateTime(date);
                BonepileSummaryReport(SFCDB, BU, data_class, type, current_date);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";            
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

        public void CheckCollectPrivilege(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_USER_PRIVILEGE t_c_user_privilege = new T_C_USER_PRIVILEGE(SFCDB, this.DBTYPE); 
                bool bPrivilege = t_c_user_privilege.CheckpPivilegeByName(SFCDB, "CollectBonepileData", LoginUser.EMP_NO);
                if (LoginUser.EMP_LEVEL == "9" || bPrivilege)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "No Privilege{CollectBonepileData}!";
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
        public void ClosedBonepileData(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {                
                if (Data["Type"] == null)
                {
                    string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218");
                    throw new Exception(errMessage);
                }
                if (Data["SN"] == null)
                {
                    throw new Exception($@"SN Is Null!");
                }
                if (Data["Remark"] == null)
                {
                    throw new Exception($@"Remark Is Null!");
                }
                
                string type = Data["Type"].ToString();
                string input_sn = Data["SN"].ToString();
                string remark = Data["Remark"].ToString();
                string[] snList = input_sn.Split('\n');
                string pass_sn = "";
                string error_sn = "";
                string sn = "";
                int result = 0;
                DateTime sysdate = SFCDB.ORM.GetDate();
                for (int i = 0; i < snList.Length; i++)
                {
                    sn = snList[i].Trim().ToUpper();
                    if (string.IsNullOrEmpty(sn))
                    {
                        continue;
                    }
                    switch (type)
                    {
                        case "Normal":
                            result = SFCDB.ORM.Updateable<R_NORMAL_BONEPILE>()
                                .UpdateColumns(r =>
                                new R_NORMAL_BONEPILE
                                {
                                    CLOSED_FLAG = "1",
                                    CLOSED_BY = LoginUser.EMP_NO,
                                    CLOSED_REASON = remark,
                                    CLOSED_DATE = sysdate
                                }).Where(r => r.SN == sn && r.CLOSED_FLAG == "0").ExecuteCommand();
                            break;
                        case "RMA":
                            result = SFCDB.ORM.Updateable<MESDataObject.Module.Vertiv.R_RMA_BONEPILE>()
                                .UpdateColumns(r =>
                                new MESDataObject.Module.Vertiv.R_RMA_BONEPILE
                                {
                                    CLOSED_FLAG = "1",
                                    EDIT_EMP = LoginUser.EMP_NO,
                                    EDIT_TIME = sysdate,
                                    CLOSED_DATE = sysdate
                                }).Where(r => r.SN == sn && r.CLOSED_FLAG == "0").ExecuteCommand();
                            T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, DBTYPE);
                            R_MES_LOG LOG = new R_MES_LOG
                            {
                                ID = TRML.GetNewID(BU, SFCDB),
                                DATA1 = sn,
                                FUNCTION_NAME = "ClosedBonepileData",
                                CLASS_NAME = "MESStation.Config.BonepileConfig",
                                PROGRAM_NAME = "CloudMES",
                                EDIT_TIME = sysdate,
                                EDIT_EMP = LoginUser.EMP_NO,
                                LOG_MESSAGE = remark,
                            };
                            TRML.InsertMESLog(LOG, SFCDB);
                            break;
                        default:
                            result = 0;
                            break;
                    }
                    
                    if (result == 0)
                    {
                        error_sn += "," + snList[i];
                    }
                    else
                    {
                        pass_sn += "," + snList[i];
                    }
                }
                if (string.IsNullOrEmpty(error_sn))
                {
                    StationReturn.Message = "All SN Cloesed OK!";
                }
                else
                {
                    StationReturn.Message = error_sn.Substring(1) + ",Cloesed Fail!Other Closed OK!";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;

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
        /// <summary>
        /// 檢查上傳的Excle是否包含模板中的列
        /// </summary>
        /// <param name="inputExcelColumn"></param>
        /// <param name="listTitle"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool CheckInputExcelTitle(JObject inputExcelColumn, List<string> listTitle, out string title)
        {
            bool bColumnExists = true;
            string out_title = "";
            foreach (string t in listTitle)
            {
                bColumnExists = inputExcelColumn.Properties().Any(p => p.Name == t);
                if (!bColumnExists)
                {
                    out_title = t;
                    break;
                }
            }
            title = out_title;
            return bColumnExists;
        }
        private bool CheckInputExcelTitle(List<string> inputExcelColumn, List<string> listTitle, out string title)
        {
            bool bColumnExists = true;
            string out_title = "";
            foreach (string t in listTitle)
            {
                bColumnExists = inputExcelColumn.Any(p => p == t);
                if (!bColumnExists)
                {
                    out_title = t;
                    break;
                }
            }
            title = out_title;
            return bColumnExists;
        }
        private bool CheckChinese( JObject uploadData, List<string> listTitle)
        {
            foreach (string t in listTitle)
            {
                if (uploadData[t].ToString().Length == 0)
                {
                    continue;
                }
                bool isChinese = System.Text.RegularExpressions.Regex.IsMatch(uploadData[t].ToString(), @"^[\u4e00-\u9fa5]{0,}$");
                if (isChinese)
                {
                    return false;
                }
            }
            return true;
        }
        private bool CheckChinese(DataRow row, List<string> listTitle)
        {
            foreach (string t in row.ItemArray)
            {
                if (t.Length == 0)
                {
                    continue;
                }
                bool isChinese = System.Text.RegularExpressions.Regex.IsMatch(t, @"^[\u4e00-\u9fa5]{0,}$");
                if (isChinese)
                {
                    return false;
                }
            }
            return true;
        }

        public void BonepileSummaryReport(OleExec SFCDB, string bu,string data_class, string type, DateTime? input_current_date)
        {
            DataTable dt = new DataTable();
            string sql = "";
            string firstDateOfWork = "";
            int workYear = 0;
            int workWeek = 0;
            int workMoth = 0;
            DateTime dateInLastWeekOrMonth;
            DateTime current_date;

            string lastDateOfWork = "";
            string firstDateOfWork2 = "";
            string year2;
            string month2;
            int workTime = 0;
            int workTime2 = 0;
            if (string.IsNullOrEmpty(type))
            {
                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142218");
                throw new Exception(errMessage);
            }
            current_date = input_current_date == null ? SFCDB.ORM.GetDate() : (DateTime)input_current_date;
            T_R_BONEPILE_BASIC t_r_bonepile_base = new T_R_BONEPILE_BASIC(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            switch (type)
            {
                case "WK":
                    //當前運行時間點的前一個星期
                    dateInLastWeekOrMonth = current_date.AddDays(-7);
                    //當前運行時間點的前一個星期的第一天
                    t_r_bonepile_base.GetWorkYearAndWeek(SFCDB, dateInLastWeekOrMonth, 0, 0, ref workYear, ref workWeek, ref firstDateOfWork);
                    //當前運行時間點的前一個星期的下個星期的第一天
                    lastDateOfWork = Convert.ToDateTime(firstDateOfWork).AddDays(7).ToString("yyyy/MM/dd");
                    workTime = workWeek;
                    //當前運行時間點的前一個星期的前一個星期以及那個星期的第一天日期
                    dateInLastWeekOrMonth = current_date.AddDays(-1);
                    t_r_bonepile_base.GetWorkYearAndWeek(SFCDB, dateInLastWeekOrMonth, 0, 0, ref workYear, ref workWeek, ref firstDateOfWork);
                    workTime2 = workWeek;
                    break;
                case "MO":
                    //當前運行時間點的前一個月
                    dateInLastWeekOrMonth = current_date.AddDays(-15);
                    sql = $@"select to_char(to_date('{dateInLastWeekOrMonth.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd'), 'YYYY') as yy,to_char(to_date('{dateInLastWeekOrMonth.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd'), 'mm') as mm from dual";
                    dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                    workYear = Convert.ToInt32(dt.Rows[0]["YY"].ToString());
                    workMoth = Convert.ToInt32(dt.Rows[0]["MM"].ToString());

                    //當前運行時間點的前一個月的第一天
                    firstDateOfWork = workYear + "/" + workMoth + "/01";
                    //當前運行時間點的前一個月的下個月的第一天
                    DateTime firstDateOfNextWork = Convert.ToDateTime(firstDateOfWork).AddMonths(1);
                    year2 = firstDateOfNextWork.ToString("yyyy");
                    month2 = firstDateOfNextWork.ToString("MM");

                    lastDateOfWork = year2 + "/" + month2 + "/01";
                    workTime = workMoth;
                    //當前運行時間點的前一個月的前一個月以及那個月的第一天日期
                    dateInLastWeekOrMonth = Convert.ToDateTime(firstDateOfWork).AddDays(-1);
                    year2 = dateInLastWeekOrMonth.ToString("yyyy");
                    month2 = dateInLastWeekOrMonth.ToString("MM");
                    workTime2 = Convert.ToInt32(month2);
                    firstDateOfWork2 = year2 + "/" + month2 + "/01";
                    break;
                default:
                    throw new Exception("BonepileSummaryReport Type Error!");
            }

            //清空當前工作週期數據: Basic
            //delete from sfc_bonepile_basic with(rowlock) where TranType=@TranType and YR=@WorkYear and WkOrMo=@WorkTime
            SFCDB.ORM.Deleteable<R_BONEPILE_BASIC>().Where(r => r.TRAN_TYPE == type && r.YEAR == workYear.ToString() && r.WEEK_OR_MONTH == workTime.ToString())
                .ExecuteCommand();
            //清空當前工作週期數據: Basic2
            //delete from sfc_bonepile_basic2 with(rowlock) where TranType=@TranType and YR=@WorkYear and WkOrMo=@WorkTime
            SFCDB.ORM.Deleteable<R_BONEPILE_BASIC2>().Where(r => r.TRAN_TYPE == type && r.YEAR == workYear.ToString() && r.WEEK_OR_MONTH == workTime.ToString())
               .ExecuteCommand();
            //清空當前工作週期數據: Overall
            //delete from sfc_bonepile_Overall with(rowlock) where TranType=@TranType and YR=@WorkYear and WkOrMo=@WorkTime
            SFCDB.ORM.Deleteable<R_BONEPILE_OVERALL>().Where(r => r.TRAN_TYPE == type && r.YEAR == workYear.ToString() && r.WEEK_OR_MONTH == workTime.ToString())
              .ExecuteCommand();

            switch (data_class.ToUpper())
            {
                case "ALL":
                    NormalBonepileReport(SFCDB, bu, type, firstDateOfWork, lastDateOfWork, workYear.ToString(), workTime.ToString());
                    RMABonepileReport(SFCDB, bu, type, firstDateOfWork, lastDateOfWork, workYear.ToString(), workTime.ToString());
                    break;
                case "NORMAL":
                    NormalBonepileReport(SFCDB, bu, type, firstDateOfWork, lastDateOfWork, workYear.ToString(), workTime.ToString());                    
                    break;
                case "RMA":                   
                    RMABonepileReport(SFCDB, bu, type, firstDateOfWork, lastDateOfWork, workYear.ToString(), workTime.ToString());
                    break;
                default :
                    NormalBonepileReport(SFCDB, bu, type, firstDateOfWork, lastDateOfWork, workYear.ToString(), workTime.ToString());
                    RMABonepileReport(SFCDB, bu, type, firstDateOfWork, lastDateOfWork, workYear.ToString(), workTime.ToString());
                    break;
            }          
        }

        private void NormalBonepileReport(OleExec SFCDB, string bu, string type, string firstDateOfWork, string lastDateOfWork, string work_year, string work_time)
        {
            string sql = "";
            DataTable dt = new DataTable();
            T_R_BONEPILE_BASIC t_r_bonepile_basic = new T_R_BONEPILE_BASIC(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_BONEPILE_BASIC2 t_r_bonepile_basic2 = new T_R_BONEPILE_BASIC2(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_BONEPILE_OVERALL t_r_bonepile_overall = new T_R_BONEPILE_OVERALL(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_R_BONEPILE_BASIC rowBasic = null;
            Row_R_BONEPILE_BASIC2 rowBasic2 = null;
            Row_R_BONEPILE_OVERALL rowOverall = null;
            #region 1 Basic data analysis table:r_bonepile_basic,r_bonepile_basic2
            // insert into r_bonepile_basic(id,tran_type,year,WEEK_OR_MONTH,data_class,brcd,flh,normal,rma,category,status,hardcore,series,sub_series,product_name,
            //bonepile_description,Aging,Avgdays,Qty,Amount,LASTEDIT_BY,LASTEDIT_DATE)
            sql = $@" select 
                '{type}' AS tran_type,'{work_year}' AS year,'{work_time}' AS WEEK_OR_MONTH,'Normal' AS Data_Class,BRCD,FLH,Normal,
                 RMA,Category,Status,Hardcore,Series,Sub_series,Product_Name,bonepile_description,Aging,trunc(avg(Days),2) AS AvgDays,count(*) AS Qty ,SUM(Price) AS Amount,
                'SYSTEM' as LASTEDIT_BY,SYSDATE as LASTEDIT_DATE
                from (
                /*New Add*/
                select sn,'N' as BRCD,'N' as FLH,'Y' as Normal,'N' as RMA,
                       Bonepile_Category as Category,'New' as Status,
                       case 
                         when Hardcore_Board is not null and Hardcore_Board<>'' 
                           then 'Yes' 
                         else 'No' 
                       end as Hardcore,
                       Product_Series as Series,Sub_Series,Product_Name,'' as bonepile_description,
                        Price, to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') as Days,
                        case 
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')) <=30 then 'A'          
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') >30 and 
                          to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')<=60) then 'B'          
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd') -to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>60  and 
                         to_date('{lastDateOfWork}','yyyy/mm/dd')- to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')<=90) then 'C'         
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>90  and 
                              to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') <=120) then 'D'          
                        when  to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>120 then 'E'
                        end as Aging
                      from r_normal_bonepile
                      where (
                      to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>= to_date('{firstDateOfWork}','yyyy/mm/dd') 
                      and to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') < to_date('{lastDateOfWork}','yyyy/mm/dd')
                      )       
                      and CRITICAL_BONEPILE_FLAG = 0      
                /*Closed*/
                union
                select sn,'N' as BRCD,'N' as FLH,'Y' as Normal,'N' as RMA,
                       Bonepile_Category as Category,'Closed' as Status,
                       case 
                         when Hardcore_Board is not null and Hardcore_Board<>'' 
                           then 'Yes' 
                         else 'No' 
                       end as Hardcore,
                       Product_Series as Series,Sub_Series,Product_Name,'' as bonepile_description,
                        Price, to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') as Days,
                        case 
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')) <=30 then 'A'          
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') >30 and 
                          to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')<=60) then 'B'          
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd') -to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>60  and 
                         to_date('{lastDateOfWork}','yyyy/mm/dd')- to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')<=90) then 'C'         
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>90  and 
                              to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') <=120) then 'D'          
                        when  to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>120 then 'E'
                        end as Aging
                      from r_normal_bonepile
                      where (
                      to_date(to_char(CLOSED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>= to_date('{firstDateOfWork}','yyyy/mm/dd') 
                      and to_date(to_char(CLOSED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') < to_date('{lastDateOfWork}','yyyy/mm/dd')
                      )       
                      and CRITICAL_BONEPILE_FLAG = 0    
                /*Open*/
                union
                select sn,'N' as BRCD,'N' as FLH,'Y' as Normal,'N' as RMA,
                       Bonepile_Category as Category,'Open' as Status,
                       case 
                         when Hardcore_Board is not null and Hardcore_Board<>'' 
                           then 'Yes' 
                         else 'No' 
                       end as Hardcore,
                       Product_Series as Series,Sub_Series,Product_Name,'' as bonepile_description,
                        Price, to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') as Days,
                        case 
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')) <=30 then 'A'          
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') >30 and 
                          to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')<=60) then 'B'          
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd') -to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>60  and 
                         to_date('{lastDateOfWork}','yyyy/mm/dd')- to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')<=90) then 'C'         
                        when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>90  and 
                              to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') <=120) then 'D'          
                        when  to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd')>120 then 'E'
                        end as Aging
                      from r_normal_bonepile
                      where 
                      to_date(to_char(fail_date,'yyyy/mm/dd'),'yyyy/mm/dd') < to_date('{lastDateOfWork}','yyyy/mm/dd')         
                      and (CLOSED_DATE is null or CLOSED_DATE = '' or  to_date(to_char(CLOSED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>= to_date('{lastDateOfWork}','yyyy/mm/dd') )    
                      and CRITICAL_BONEPILE_FLAG = 0       
                      ) A   group by BRCD,FLH,Normal,RMA,Category,Status,Hardcore,Series,Sub_series,Product_Name,Bonepile_Description,Aging";
            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    rowBasic = (Row_R_BONEPILE_BASIC)t_r_bonepile_basic.NewRow();
                    rowBasic.loadData(row);
                    rowBasic.ID = t_r_bonepile_basic.GetNewID(bu, SFCDB);
                    t_r_bonepile_basic.Insert(SFCDB, rowBasic.GetDataObject());
                }
            }
            //insert into r_bonepile_basic2(id,tran_type,year,week_or_month,data_class,brcd,flh,normal,rma,category,status,hardcore,series,sub_series,product_name,bonepile_description,
            //a_qty,a_amount,b_qty,b_amount,c_qty,c_amount,d_qty,d_amount,e_qty,e_amount,LASTEDIT_BY,LASTEDIT_DATE)
            sql = $@"select  H.Tran_Type,H.year,H.WEEK_OR_MONTH,H.Data_Class,H.BRCD,H.FLH,H.Normal,H.RMA,H.Category,H.Status,H.Hardcore,H.Series,H.Sub_series,H.Product_Name,H.bonepile_description,
                          decode(A.Qty,null,0,a.qty) as A_Qty ,decode(A.Amount,null,0,a.Amount) as A_Amount,
                          decode(B.Qty,null,0,B.Qty) as B_Qty, decode(B.Amount,null,0,B.Amount) as B_Amount,
                          decode(C.Qty,null,0,C.Qty) as C_Qty, decode(C.Amount,null,0,C.Amount) as C_Amount,
                          decode(D.Qty,null,0,D.Qty) as D_Qty, decode(D.Amount,null,0,E.Amount) as D_Amount,
                          decode(E.Qty,null,0,E.Qty) as E_Qty, decode(E.Amount,null,0,E.Amount) as E_Amount,
                          'SYSTEM' as LASTEDIT_BY,SYSDATE as LASTEDIT_DATE
                        from (select distinct tran_type,YEAR,WEEK_OR_MONTH,Data_Class,BRCD,FLH,Normal,RMA,Category,Status,Hardcore,Series,sub_series,Product_Name,bonepile_description
                            from r_bonepile_basic where Tran_Type='{type}' and YEAR='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='Normal') H
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='Normal' and Aging='A') A
                            on H.BRCD=A.BRCD and H.FLH=A.FLH and H.Normal=A.Normal and H.RMA=A.RMA and H.Category=A.Category and H.Status=A.Status and H.Hardcore=A.Hardcore 
                            and H.Series=A.Series and H.Sub_series=A.Sub_series and H.Product_Name=A.Product_Name and H.bonepile_description=A.bonepile_description        
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='Normal' and Aging='B') B
                            on H.BRCD=B.BRCD and H.FLH=B.FLH and H.Normal=B.Normal and H.RMA=B.RMA and H.Category=B.Category and H.Status=B.Status and H.Hardcore=B.Hardcore 
                            and H.Series=B.Series and H.Sub_series=B.Sub_series and H.Product_Name=B.Product_Name and H.bonepile_description=B.bonepile_description
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='Normal' and Aging='C') C
                            on H.BRCD=C.BRCD and H.FLH=C.FLH and H.Normal=C.Normal and H.RMA=C.RMA and H.Category=C.Category and H.Status=C.Status and H.Hardcore=C.Hardcore 
                            and H.Series=C.Series and H.Sub_series=C.Sub_series and H.Product_Name=C.Product_Name and H.bonepile_description=C.bonepile_description
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='Normal' and Aging='D') D
                            on H.BRCD=D.BRCD and H.FLH=D.FLH and H.Normal=D.Normal and H.RMA=D.RMA and H.Category=D.Category and H.Status=D.Status and H.Hardcore=D.Hardcore 
                            and H.Series=D.Series and H.Sub_series=D.Sub_series and H.Product_Name=D.Product_Name and H.bonepile_description=D.bonepile_description
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='Normal' and Aging='E') E
                            on H.BRCD=E.BRCD and H.FLH=E.FLH and H.Normal=E.Normal and H.RMA=E.RMA and H.Category=E.Category and H.Status=E.Status and H.Hardcore=E.Hardcore 
                            and H.Series=E.Series and H.Sub_series=E.Sub_series and H.Product_Name=E.Product_Name and H.bonepile_description=E.bonepile_description ";
            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    rowBasic2 = (Row_R_BONEPILE_BASIC2)t_r_bonepile_basic2.NewRow();
                    rowBasic2.loadData(row);
                    rowBasic2.ID = t_r_bonepile_basic2.GetNewID(bu, SFCDB);
                    t_r_bonepile_basic2.Insert(SFCDB, rowBasic2.GetDataObject());
                }
            }
            #endregion

            #region 2 Overall Weekly Report
            //對應前面寫入r_bonepile_basic的Aging           
            List<string> listAging = new List<string>() { "ALL", "A", "B", "C", "D", "E" };
            //對應前面寫入r_bonepile_basic的狀態
            List<string> listStatus = new List<string>() { "New", "Closed", "Open" };
            Dictionary<string, string> dictionaryQty = new Dictionary<string, string>();
            foreach (var s in listStatus)
            {
                foreach (var a in listAging)
                {
                    dictionaryQty.Add(s.ToLower() + "_" + a.ToLower() + "_qty", "0");
                    if (s == "Open")
                    {
                        dictionaryQty.Add(s.ToLower() + "_" + a.ToLower() + "_amount", "0");
                    }
                }
            }
            foreach (var s in listStatus)
            {
                sql = $@"select * from (
                        select 'ALL' AS aging,decode(sum(Qty),null,0,sum(qty)) Qty,decode(sum(Amount),null,0,sum(Amount)) as Amount
                            from r_bonepile_basic
                            where Tran_Type='{type}' and YEAR='{work_year}' and week_or_month='{work_time}' 
                              and Data_Class='Normal'
                              and Status='{s}'
                        union
                        select aging,decode(sum(Qty),null,0,sum(qty)) Qty,decode(sum(Amount),null,0,sum(Amount)) as Amount
                            from r_bonepile_basic
                            where Tran_Type='{type}' and YEAR='{work_year}' and week_or_month='{work_time}' 
                              and Data_Class='Normal'
                              and Status='{s}'   
                              group by aging)
                              order by aging ";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dictionaryQty[s.ToLower() + "_" + row["AGING"].ToString().ToLower() + "_qty"] = row["QTY"].ToString();
                        if (s == "Open")
                        {
                            dictionaryQty[s.ToLower() + "_" + row["AGING"].ToString().ToLower() + "_amount"] = row["QTY"].ToString();
                        }
                    }
                }

            }
            //原有DCN 邏輯有OEM,ODM,Broadcom,Extreme,RUCKUS等Data_Type,現先默認為VERTIV，後面有區分在說
            sql = $@"select '{type}' as Tran_Type,'{work_year}' as year,'{work_time}' as week_or_month,'VERTIV' as Data_Type,'Normal' as Data_Class,'N' as BRCD,'N' as FLH";
            foreach (var d in dictionaryQty)
            {
                switch (d.Key)
                {
                    case "new_all_qty":
                        sql = sql + $@",{d.Value} as new_add_qty";
                        break;
                    case "closed_all_qty":
                        sql = sql + $@",{d.Value} as closed_qty";
                        break;
                    case "open_all_qty":
                        sql = sql + $@",{d.Value} as open_qty";
                        break;
                    case "open_all_amount":
                        sql = sql + $@",{d.Value} as Open_Amount";
                        break;
                    default:

                        sql = sql + (d.Key.Contains("amount") ? $@",{d.Value}/1000000.0 as {d.Key}" : $@",{d.Value} as {d.Key}");
                        break;
                }
            }
            sql = sql + $@",'SYSTEM' as LASTEDIT_BY,SYSDATE as LASTEDIT_DATE from dual";

            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    rowOverall = (Row_R_BONEPILE_OVERALL)t_r_bonepile_overall.NewRow();
                    rowOverall.loadData(row);
                    rowOverall.ID = t_r_bonepile_overall.GetNewID(bu, SFCDB);
                    t_r_bonepile_overall.Insert(SFCDB, rowOverall.GetDataObject());
                }
            }
            #endregion
        }

        private void RMABonepileReport(OleExec SFCDB, string bu, string type, string firstDateOfWork, string lastDateOfWork, string work_year, string work_time)
        {
            string sql = "";
            DataTable dt = new DataTable();
            T_R_BONEPILE_BASIC t_r_bonepile_basic = new T_R_BONEPILE_BASIC(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_BONEPILE_BASIC2 t_r_bonepile_basic2 = new T_R_BONEPILE_BASIC2(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_BONEPILE_OVERALL t_r_bonepile_overall = new T_R_BONEPILE_OVERALL(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_R_BONEPILE_BASIC rowBasic = null;
            Row_R_BONEPILE_BASIC2 rowBasic2 = null;
            Row_R_BONEPILE_OVERALL rowOverall = null;
            #region 1 Basic data analysis table:r_bonepile_basic,r_bonepile_basic2
            sql = $@"select  '{type}' AS tran_type,'{work_year}' AS year,'{work_time}' AS WEEK_OR_MONTH,'RMA' AS Data_Class,BRCD,FLH,Normal,
                       RMA,Category,Status,Hardcore,Series,Sub_series,Product_Name,bonepile_description,Aging,trunc(avg(Days),2) AS AvgDays,count(*) AS Qty ,SUM(Price) AS Amount,
                        'SYSTEM' as LASTEDIT_BY,SYSDATE as LASTEDIT_DATE
                        from (
                          --NewAdd: 
                          select rma.SN,'N' as BRCD,'N' as FLH,'N' as Normal,'Y' as RMA,
                            '' as Category,'New' as Status, 'No' as Hardcore ,sc.customer_name as Series,
                            sc.series_name as Sub_series,sc.sku_name as Product_Name,rma.FAILURE_SYMPTOM as bonepile_description,0 as Price,
                            to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd') as Days,
                           case
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')) <=30 then 'A'
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd') >30 and 
                               to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')<=60) then 'B'          
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd') -to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')>60  and 
                                to_date('{lastDateOfWork}','yyyy/mm/dd')- to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')<=90) then 'C'         
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')>90  and 
                                to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd') <=120) then 'D'          
                             when  to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')>120 then 'E'
                           end as Aging
                          from r_rma_bonepile rma left join
                                  (select distinct s.skuno, s.sku_name, c.series_name,cc.customer_name from c_sku s 
                                  left join c_series c on s.c_series_id= c.id left join C_CUSTOMER cc on cc.id=c.customer_id) sc
                                   on rma.skuno = sc.skuno 
                          where to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd')>= to_date('{firstDateOfWork}','yyyy/mm/dd') 
                            and to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd') < to_date('{lastDateOfWork}','yyyy/mm/dd')
                          union all
                          --Closed:
                          select  rma.SN,'N' as BRCD,'N' as FLH,'N' as Normal,'Y' as RMA,
                            '' as Category,'Closed' as Status, 'No' as Hardcore,sc.customer_name as Series,
                            sc.series_name as Sub_series,sc.sku_name as Product_Name,rma.FAILURE_SYMPTOM as bonepile_description,0 as Price,
                           to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') as Days,
                           case
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')) <=30 then 'A'
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') >30 and 
                               to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')<=60) then 'B'          
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd') -to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>60  and 
                                to_date('{lastDateOfWork}','yyyy/mm/dd')- to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')<=90) then 'C'         
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>90  and 
                                to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') <=120) then 'D'          
                             when  to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>120 then 'E'
                           end as Aging            
                          from r_rma_bonepile rma left join
                                  (select distinct s.skuno, s.sku_name, c.series_name,cc.customer_name from c_sku s 
                                  left join c_series c on s.c_series_id= c.id left join C_CUSTOMER cc on cc.id=c.customer_id) sc
                                   on rma.skuno = sc.skuno 
                          where to_date(to_char(CLOSED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>= to_date('{firstDateOfWork}','yyyy/mm/dd') 
                            and to_date(to_char(CLOSED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') < to_date('{lastDateOfWork}','yyyy/mm/dd')
                          union all
                          --Open:
                          select rma.SN,'N' as BRCD,'N' as FLH,'N' as Normal,'Y' as RMA,
                            '' as Category,'Open' as Status, 'No' as Hardcore,sc.customer_name as Series,
                            sc.series_name as Sub_series,sc.sku_name as Product_Name,rma.FAILURE_SYMPTOM as bonepile_description,0 as Price,
                           to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') as Days,
                           case
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')) <=30 then 'A'
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') >30 and 
                               to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')<=60) then 'B'          
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd') -to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>60  and 
                                to_date('{lastDateOfWork}','yyyy/mm/dd')- to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')<=90) then 'C'         
                             when (to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>90  and 
                                to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd') <=120) then 'D'          
                             when  to_date('{lastDateOfWork}','yyyy/mm/dd')-to_date(to_char(RECEIVED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>120 then 'E'
                           end as Aging     
                          from r_rma_bonepile rma left join
                                  (select distinct s.skuno, s.sku_name, c.series_name,cc.customer_name from c_sku s 
                                  left join c_series c on s.c_series_id= c.id left join C_CUSTOMER cc on cc.id=c.customer_id) sc
                                   on rma.skuno = sc.skuno 
                          where to_date(to_char(UPLOAD_TIME,'yyyy/mm/dd'),'yyyy/mm/dd') < to_date('{lastDateOfWork}','yyyy/mm/dd')
                            and (CLOSED_DATE is null or CLOSED_DATE = '' or  to_date(to_char(CLOSED_DATE,'yyyy/mm/dd'),'yyyy/mm/dd')>= to_date('{firstDateOfWork}','yyyy/mm/dd'))      
                        ) a
                        group by BRCD,FLH,Normal,RMA,Category,Status,Hardcore,Series,Sub_series,Product_Name,bonepile_description,Aging";
            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    rowBasic = (Row_R_BONEPILE_BASIC)t_r_bonepile_basic.NewRow();
                    rowBasic.loadData(row);
                    rowBasic.ID = t_r_bonepile_basic.GetNewID(bu, SFCDB);
                    t_r_bonepile_basic.Insert(SFCDB, rowBasic.GetDataObject());
                }
            }

            sql = $@"select  H.Tran_Type,H.year,H.WEEK_OR_MONTH,H.Data_Class,H.BRCD,H.FLH,H.Normal,H.RMA,H.Category,H.Status,H.Hardcore,H.Series,H.Sub_series,H.Product_Name,H.bonepile_description,
                          decode(A.Qty,null,0,a.qty) as A_Qty ,decode(A.Amount,null,0,a.Amount) as A_Amount,
                          decode(B.Qty,null,0,B.Qty) as B_Qty, decode(B.Amount,null,0,B.Amount) as B_Amount,
                          decode(C.Qty,null,0,C.Qty) as C_Qty, decode(C.Amount,null,0,C.Amount) as C_Amount,
                          decode(D.Qty,null,0,D.Qty) as D_Qty, decode(D.Amount,null,0,E.Amount) as D_Amount,
                          decode(E.Qty,null,0,E.Qty) as E_Qty, decode(E.Amount,null,0,E.Amount) as E_Amount,
                          'SYSTEM' as LASTEDIT_BY,SYSDATE as LASTEDIT_DATE
                        from (select distinct tran_type,YEAR,WEEK_OR_MONTH,Data_Class,BRCD,FLH,Normal,RMA,Category,Status,Hardcore,Series,sub_series,Product_Name,bonepile_description
                            from r_bonepile_basic where Tran_Type='{type}' and YEAR='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='RMA') H
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='RMA' and Aging='A') A
                            on H.BRCD=A.BRCD and H.FLH=A.FLH and H.Normal=A.Normal and H.RMA=A.RMA and H.Category=A.Category and H.Status=A.Status and H.Hardcore=A.Hardcore 
                            and H.Series=A.Series and H.Sub_series=A.Sub_series and H.Product_Name=A.Product_Name and H.bonepile_description=A.bonepile_description        
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='RMA' and Aging='B') B
                            on H.BRCD=B.BRCD and H.FLH=B.FLH and H.Normal=B.Normal and H.RMA=B.RMA and H.Category=B.Category and H.Status=B.Status and H.Hardcore=B.Hardcore 
                            and H.Series=B.Series and H.Sub_series=B.Sub_series and H.Product_Name=B.Product_Name and H.bonepile_description=B.bonepile_description
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='RMA' and Aging='C') C
                            on H.BRCD=C.BRCD and H.FLH=C.FLH and H.Normal=C.Normal and H.RMA=C.RMA and H.Category=C.Category and H.Status=C.Status and H.Hardcore=C.Hardcore 
                            and H.Series=C.Series and H.Sub_series=C.Sub_series and H.Product_Name=C.Product_Name and H.bonepile_description=C.bonepile_description
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='RMA' and Aging='D') D
                            on H.BRCD=D.BRCD and H.FLH=D.FLH and H.Normal=D.Normal and H.RMA=D.RMA and H.Category=D.Category and H.Status=D.Status and H.Hardcore=D.Hardcore 
                            and H.Series=D.Series and H.Sub_series=D.Sub_series and H.Product_Name=D.Product_Name and H.bonepile_description=D.bonepile_description
                          left join (select * from r_bonepile_basic where Tran_Type='{type}' and year='{work_year}' and WEEK_OR_MONTH='{work_time}' and Data_Class='RMA' and Aging='E') E
                            on H.BRCD=E.BRCD and H.FLH=E.FLH and H.Normal=E.Normal and H.RMA=E.RMA and H.Category=E.Category and H.Status=E.Status and H.Hardcore=E.Hardcore 
                            and H.Series=E.Series and H.Sub_series=E.Sub_series and H.Product_Name=E.Product_Name and H.bonepile_description=E.bonepile_description ";
            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    rowBasic2 = (Row_R_BONEPILE_BASIC2)t_r_bonepile_basic2.NewRow();
                    rowBasic2.loadData(row);
                    rowBasic2.ID = t_r_bonepile_basic2.GetNewID(bu, SFCDB);
                    t_r_bonepile_basic2.Insert(SFCDB, rowBasic2.GetDataObject());
                }
            }
            #endregion

            #region 2 Overall Weekly Report
            //對應前面寫入r_bonepile_basic的Aging           
            List<string> listAging = new List<string>() { "ALL", "A", "B", "C", "D", "E" };
            //對應前面寫入r_bonepile_basic的狀態
            List<string> listStatus = new List<string>() { "New", "Closed", "Open" };
            Dictionary<string, string> dictionaryQty = new Dictionary<string, string>();
            foreach (var s in listStatus)
            {
                foreach (var a in listAging)
                {
                    dictionaryQty.Add(s.ToLower() + "_" + a.ToLower() + "_qty", "0");
                    if (s == "Open")
                    {
                        dictionaryQty.Add(s.ToLower() + "_" + a.ToLower() + "_amount", "0");
                    }
                }
            }
            foreach (var s in listStatus)
            {
                sql = $@"select * from (
                        select 'ALL' AS aging,decode(sum(Qty),null,0,sum(qty)) Qty,decode(sum(Amount),null,0,sum(Amount)) as Amount
                            from r_bonepile_basic
                            where Tran_Type='{type}' and YEAR='{work_year}' and week_or_month='{work_time}' 
                              and Data_Class='RMA'
                              and Status='{s}'
                        union
                        select aging,decode(sum(Qty),null,0,sum(qty)) Qty,decode(sum(Amount),null,0,sum(Amount)) as Amount
                            from r_bonepile_basic
                            where Tran_Type='{type}' and YEAR='{work_year}' and week_or_month='{work_time}' 
                              and Data_Class='RMA'
                              and Status='{s}'   
                              group by aging)
                              order by aging ";
                dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dictionaryQty[s.ToLower() + "_" + row["AGING"].ToString().ToLower() + "_qty"] = row["QTY"].ToString();
                        if (s == "Open")
                        {
                            dictionaryQty[s.ToLower() + "_" + row["AGING"].ToString().ToLower() + "_amount"] = row["QTY"].ToString();
                        }
                    }
                }

            }
            //原有DCN 邏輯有OEM,ODM,Broadcom,Extreme,RUCKUS等Data_Type,現先默認為VERTIV，後面有區分在說
            sql = $@"select '{type}' as Tran_Type,'{work_year}' as year,'{work_time}' as week_or_month,'VERTIV' as Data_Type,'RMA' as Data_Class,'N' as BRCD,'N' as FLH";
            foreach (var d in dictionaryQty)
            {
                switch (d.Key)
                {
                    case "new_all_qty":
                        sql = sql + $@",{d.Value} as new_add_qty";
                        break;
                    case "closed_all_qty":
                        sql = sql + $@",{d.Value} as closed_qty";
                        break;
                    case "open_all_qty":
                        sql = sql + $@",{d.Value} as open_qty";
                        break;
                    case "open_all_amount":
                        sql = sql + $@",{d.Value} as Open_Amount";
                        break;
                    default:

                        sql = sql + (d.Key.Contains("amount") ? $@",{d.Value}/1000000.0 as {d.Key}" : $@",{d.Value} as {d.Key}");
                        break;
                }
            }
            sql = sql + $@",'SYSTEM' as LASTEDIT_BY,SYSDATE as LASTEDIT_DATE from dual";

            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    rowOverall = (Row_R_BONEPILE_OVERALL)t_r_bonepile_overall.NewRow();
                    rowOverall.loadData(row);
                    rowOverall.ID = t_r_bonepile_overall.GetNewID(bu, SFCDB);
                    t_r_bonepile_overall.Insert(SFCDB, rowOverall.GetDataObject());
                }
            }
            #endregion
        }


    }
}
