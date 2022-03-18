using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.MESStation;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESStation.Config.HWD
{
    /// <summary>
    /// 用於HWD終端新增的出貨方式：出貨軟銀
    /// </summary>
    public class TGMESApi : MesAPIBase
    {
        protected APIInfo FSelectTGMESList = new APIInfo
        {
            FunctionName = "SelectTGMESList",
            Description = "Select",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName="Barcode", InputType="string" },
                new APIInputInfo { InputName="Pallet", InputType="string" },
                new APIInputInfo { InputName="SkuNo", InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteTGMESInfo = new APIInfo
        {
            FunctionName = "DeleteTGMESInfo",
            Description = "Delete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName="Pallet", InputType="string" },
                new APIInputInfo { InputName="Remark", InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadTGMESInfo = new APIInfo()
        {
            FunctionName = "UploadTGMESInfo",
            Description = "Upload",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DataList", InputType = "string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDownLoadTGMESCSV = new APIInfo
        {
            FunctionName = "DownLoadTGMESCSV",
            Description = "DownLoad",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName="PO", InputType="string" },
                new APIInputInfo { InputName="TO", InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadTGMESMessage = new APIInfo
        {
            FunctionName = "UploadTGMESMessage",
            Description = "Select",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FShowTGMESCSV = new APIInfo
        {
            FunctionName = "ShowTGMESCSV",
            Description = "ShowCsvTable",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName="PO", InputType="string" },
                new APIInputInfo { InputName="TO", InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetShippingListByType = new APIInfo()
        {
            FunctionName = "GetShippingListByType",
            Description = "Get Shipping List By Type",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Type", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Value", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DNLine", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCancelShipping = new APIInfo()
        {
            FunctionName = "CancelShipping",
            Description = "Cancel Shipping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Type", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Value", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DNLine", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Remark", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public TGMESApi()
        {
            this.Apis.Add(FSelectTGMESList.FunctionName, FSelectTGMESList);
            this.Apis.Add(FDeleteTGMESInfo.FunctionName, FDeleteTGMESInfo);
            this.Apis.Add(FUploadTGMESInfo.FunctionName, FUploadTGMESInfo);
            this.Apis.Add(FDownLoadTGMESCSV.FunctionName, FDownLoadTGMESCSV);
            this.Apis.Add(FShowTGMESCSV.FunctionName, FShowTGMESCSV);
            this.Apis.Add(FGetShippingListByType.FunctionName, FGetShippingListByType);
            this.Apis.Add(FCancelShipping.FunctionName, FCancelShipping);
        }

        /// <summary>
        /// 獲取用戶上傳的R_SN_TGMES_INFO表信息
        /// </summary>
        public void SelectTGMESList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            var list = new List<R_SN_TGMES_INFO>();
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string barcode = Data["Barcode"].ToString();
                string pallet = Data["Pallet"].ToString();
                string skuNo = Data["SkuNo"].ToString();

                if (!string.IsNullOrEmpty(barcode) || !string.IsNullOrEmpty(pallet) || !string.IsNullOrEmpty(skuNo))
                {
                    list = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>()
                    .Where(t => t.VALID_FLAG == "1")
                    .WhereIF(!string.IsNullOrEmpty(barcode), t => t.PCBA_BARCODE== barcode)
                    .WhereIF(!string.IsNullOrEmpty(pallet), t => t.PACKING3 == pallet)
                    .WhereIF(!string.IsNullOrEmpty(skuNo), t => SqlFunc.Contains(t.ITEM_SALES, skuNo))                    
                    .OrderBy(t => t.PACKING3, OrderByType.Asc)
                    .OrderBy(t => t.PCBA_BARCODE, OrderByType.Asc)
                    .OrderBy(t => t.DATALOAD_TIME, OrderByType.Asc)
                    .ToList();
                }
                else
                {
                    list = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.VALID_FLAG == "1").OrderBy(t => t.DATALOAD_TIME, OrderByType.Asc).Take(1000).ToList();
                }
                
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// 根據ID刪除R_SN_TGMES_INFO表信息
        /// </summary>
        public void DeleteTGMESInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string strPallet = Data["Pallet"].ToString();
                string strRemark = Data["Remark"].ToString();

                List<R_SN_TGMES_INFO> TGMESList = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == strPallet && t.VALID_FLAG == "1").ToList();
                if (TGMESList == null || TGMESList.Count == 0)
                {
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(strPallet);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else
                {
                    if (TGMESList.FindAll(t => t.SHIPPED_FLAG == "1").Count > 0)
                    {
                        StationReturn.Message = "已經掃描出貨無法刪除！";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                    else
                    {
                        SFCDB.ORM.Updateable<R_SN_TGMES_INFO>()
                        .SetColumns(t => new R_SN_TGMES_INFO() { VALID_FLAG = "0", REMARK = strRemark, EDIT_EMP = LoginUser.EMP_NO, EDIT_TIME = GetDBDateTime() })
                        .Where(t => t.PACKING3 == strPallet && t.VALID_FLAG == "1").ExecuteCommand();

                        MESPubLab.WriteLog.WriteIntoMESLog(SFCDB, BU, "CloudMES", "MESStation.Config.HWD.TGMESApi", "DeleteTGMESInfo", strPallet + ", Remark:" + strRemark, "", LoginUser.EMP_NO);
                        StationReturn.Message = "刪除成功！";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                }                
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// 上傳EXCEL內容到R_SN_TGMES_INFO表信息
        /// </summary>
        public void UploadTGMESInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            DateTime SYSDATE = DateTime.Now;
            SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string dataList = Data["DataList"].ToString();
                T_R_SN_TGMES_INFO T_TGMES = new T_R_SN_TGMES_INFO(SFCDB, DBTYPE);
                Row_R_SN_TGMES_INFO R_TGMES = (Row_R_SN_TGMES_INFO)T_TGMES.NewRow();
                Newtonsoft.Json.Linq.JArray dataArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(dataList);
                for (int i = 0; i < dataArray.Count; i++)
                {
                    string pcbaBarcode = dataArray[i]["IMEI"].ToString();
                    string itemSales = dataArray[i]["ITEM_SALES"].ToString();

                    //新增校驗是否解密上傳 BY ZXY 20220125
                    string wifikey = dataArray[i]["WIFI_KEY"].ToString();
                    string ssid = dataArray[i]["SSID"].ToString();
                    string ssid3 = dataArray[i]["SSID3"].ToString();

                    if (wifikey.IndexOf('^') >= 0 || ssid.IndexOf('^') >= 0 || ssid3.IndexOf('^') >= 0)
                    {
                        throw new Exception("附件中WIFI_KEY/SSID/SSID3特殊字段內容未解密,無法上傳文件！");
                    }

                    R_SN_TGMES_INFO TGMES = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PCBA_BARCODE == pcbaBarcode && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                    if (TGMES != null)
                    {
                        throw new Exception("附件中PCBA_BARCODE[" + pcbaBarcode + "]已存在R_SN_TGMES_INFO表中！");
                    }
                    C_SKU_DETAIL DETAIL = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "TGMES_UPLOAD" && t.CATEGORY_NAME == "ITEM_SALES" && t.VALUE == itemSales).ToList().FirstOrDefault();
                    if (DETAIL == null)
                    {
                        throw new Exception("附件中ITEM_SALES["+ itemSales + "]未在MES系統中配置Mapping編碼！");
                    }

                    #region 寫入R_SN_TGMES_INFO表
                    SYSDATE = T_TGMES.GetDBDateTime(SFCDB);
                    R_TGMES = (Row_R_SN_TGMES_INFO)T_TGMES.NewRow();
                    R_TGMES.ID = T_TGMES.GetNewID(BU, SFCDB);
                    R_TGMES.WORKORDERNO = "";
                    R_TGMES.QTY_M = 0;
                    R_TGMES.PCBA_BARCODE = dataArray[i]["IMEI"].ToString();
                    R_TGMES.PRODUCT_BARCODE = dataArray[i]["PRODUCT_BARCODE"].ToString();
                    R_TGMES.COMPLETED_FLAG = "0";
                    R_TGMES.SHIPPED_FLAG = "0";
                    R_TGMES.CURRENT_STATION = "TGMES_LOADING";
                    R_TGMES.NEXT_STATION = "TGMES_CBS";
                    R_TGMES.IMEI = dataArray[i]["IMEI"].ToString();
                    R_TGMES.PHYSICSNO = dataArray[i]["IMEI"].ToString();
                    R_TGMES.MAC_1 = dataArray[i]["MAC_1"].ToString();
                    R_TGMES.SPECIAL_SN_ID = "";
                    R_TGMES.WIFI_KEY = dataArray[i]["WIFI_KEY"].ToString();
                    R_TGMES.PACKING2 = dataArray[i]["Packing2"].ToString();
                    R_TGMES.PACKINGWEIGHT2 = "";
                    R_TGMES.PACKING3 = dataArray[i]["Packing3"].ToString();
                    R_TGMES.EMS_PCBA_PART_NO = "";
                    R_TGMES.HW_PCBA_PART_NO = "";
                    R_TGMES.ITEM_SALES = DETAIL.SKUNO;
                    R_TGMES.ITEM_BOM = "";
                    R_TGMES.LOTNO = "";
                    R_TGMES.MODEL = dataArray[i]["MODEL"].ToString();
                    R_TGMES.ORIGIN_CN = "中国制造";
                    R_TGMES.ORIGIN_EN = "MADE IN CHINA";
                    R_TGMES.PRODUCT_DATE = "";
                    R_TGMES.VALID_FLAG = "1";
                    R_TGMES.DATALOAD_EMP = LoginUser.EMP_NO;
                    R_TGMES.DATALOAD_TIME = SYSDATE;
                    R_TGMES.PLANT = "WDN1";
                    R_TGMES.PACKING_LIST_NO = dataArray[i]["PACKING_LIST_NO"].ToString();
                    R_TGMES.SSID = dataArray[i]["SSID"].ToString();
                    R_TGMES.SSID3 = dataArray[i]["SSID3"].ToString();
                    SFCDB.ExecSQL(R_TGMES.GetInsertString(DBTYPE));
                    #endregion
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "上傳成功！";
                SFCDB.CommitTrain();
            }
            catch (Exception e)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {                
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// TO完成出貨後生成下載CSV文件
        /// </summary>
        public void DownLoadTGMESCSV(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            string sql1 = string.Empty, sql2 = string.Empty;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string strPO = Data["PO"].ToString();
                string strTO = Data["TO"].ToString();

                //曾攀要求可輸入多個TO生成CSV文件 2021-05-10
                bool toEnd = true;//默認TO已完成出貨
                string message = "", strTOs = "", strDNs = "";
                string[] arrTO = strTO.Split(',');
                for (int i = 0; i < arrTO.Length; i++)
                {
                    strTO = arrTO[i];
                    //檢查TO是否正確
                    bool toExists = SFCDB.ORM.Queryable<R_TO_DETAIL>().Where(t => t.TO_NO == strTO).Any();
                    if (!toExists)
                    {
                        message = "TO:" + strTO + " 不正確,請確認!";
                        toEnd = false;
                        break;
                    }
                    //檢查TO是否完成出貨
                    DataTable dtWaitShip = SFCDB.ORM.Queryable<R_TO_DETAIL, R_DN_STATUS>((t, d) => t.DN_NO == d.DN_NO && d.DN_FLAG == "0")
                        .Where((t, d) => t.TO_NO == strTO).Select((t, d) => new { t.TO_NO, d.DN_NO }).ToDataTable();
                    if (dtWaitShip.Rows.Count > 0)
                    {
                        message = "TO:" + strTO + " 未完成出貨不允許生成CSV文件!";
                        toEnd = false;
                        break;
                    }
                    strTOs = strTOs + "'" + strTO + "',";

                    //取TO對應DN備註進文件名中 20200527
                    List<R_TO_DETAIL> dnList = SFCDB.ORM.Queryable<R_TO_DETAIL>().Where(t => t.TO_NO == strTO).OrderBy(t => t.DN_NO, OrderByType.Asc).ToList();
                    for (int j = 0; j < dnList.Count; j++)
                    {
                        strDNs += "_" + dnList[j].DN_NO.ToString();
                    }                    
                }
                
                if (!toEnd)
                {
                    StationReturn.Message = message;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else
                {
                    strTOs = strTOs.Substring(0, strTOs.Length - 1);

                    sql1 = $@"
                    select distinct c.imei ""IMEI"",
                                    c.ssid ""SSID"",
                                    c.ssid3 ""SSID3"",
                                    c.wifi_key ""WIFI_KEY""
                      from r_to_detail a, r_ship_detail b, r_sn_tgmes_info c
                     where a.dn_no = b.dn_no
                       and b.sn = c.pcba_barcode
                       and c.valid_flag = '1'
                       /*and a.to_no = '{strTO}'*/
                       and a.to_no in ({strTOs})
                     order by c.imei";

                    sql2 = $@"
                    select distinct '{strPO}' ""PO"",
                                    'P210003' ""SBB_PN"",
                                    'N1A000' ""LOGISTIC_CODE"",
                                    c.imei ""SBB_SN"",
                                    c.mac_1 ""WAN_MAC_ID"",
                                    '' ""FOX_SN"",
                                    '1' ""STATUS""
                      from r_to_detail a, r_ship_detail b, r_sn_tgmes_info c
                     where a.dn_no = b.dn_no
                       and b.sn = c.pcba_barcode
                       and c.valid_flag = '1'
                       /*and a.to_no = '{strTO}'*/
                       and a.to_no in ({strTOs})
                     order by c.imei";

                    DataTable dt1 = SFCDB.ExecuteDataTable(sql1, CommandType.Text, null);
                    DataTable dt2 = SFCDB.ExecuteDataTable(sql2, CommandType.Text, null);
                    if (dt1.Rows.Count == 0 || dt2.Rows.Count == 0)
                    {
                        StationReturn.Message = "NoData！";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                    else
                    {
                        IList list1 = ConverDatatableToList(dt1);
                        string content1 = ExcelHelp.ExportCsvToBase64String(list1);

                        var entity = new { PO = string.Empty, SBB_PN = string.Empty, LOGISTIC_CODE = string.Empty, SBB_SN = string.Empty, WAN_MAC_ID = string.Empty, FOX_SN = string.Empty, STATUS = string.Empty };
                        IList list2 = ObjectDataHelper.ConverDatatableToList(entity.GetType(), dt2);
                        string content2 = ExcelHelp.ExportCsvToBase64String(list2);
                        
                        var x = new List<CsvFile>();
                        var y = new CsvFile
                        {
                            FileName = "AirTerminalEnt_" + DateTime.Now.ToString("yyyyMMdd") + strDNs + ".csv",
                            Content = content1
                        };
                        x.Add(y);
                        y = new CsvFile
                        {
                            FileName = "DeliEqipmntBfrhndEnt_" + DateTime.Now.ToString("yyMMdd") + strDNs + ".csv",
                            Content = content2
                        };
                        x.Add(y);

                        StationReturn.Message = "Success！";
                        StationReturn.Data = x;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                }                
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// 獲取用戶上傳的R_MES_LOG表報錯信息
        /// </summary>
        public void UploadTGMESMessage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(t => t.FUNCTION_NAME == "UploadTGMESInfo")
                    .Select(t => new { t.CLASS_NAME, t.FUNCTION_NAME, t.LOG_MESSAGE, t.EDIT_EMP, t.EDIT_TIME })
                    .OrderBy(t => t.EDIT_TIME, OrderByType.Desc)
                    .ToList();
                if (list.Count > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// TO完成出貨後顯示CSV Table以方便下載
        /// </summary>
        public void ShowTGMESCSV(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            string sql1 = string.Empty, sql2 = string.Empty;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string strPO = Data["PO"].ToString();
                string strTO = Data["TO"].ToString();

                //檢查TO是否完成出貨
                DataTable dtWaitShip = SFCDB.ORM.Queryable<R_TO_DETAIL, R_DN_STATUS>((t, d) => t.DN_NO == d.DN_NO && d.DN_FLAG == "0")
                    .Where((t, d) => t.TO_NO == strTO).Select((t, d) => new { t.TO_NO, d.DN_NO }).ToDataTable();
                if (dtWaitShip.Rows.Count > 0)
                {
                    StationReturn.Message = "TO:" + strTO + " 未完成出貨不允許生成CSV文件！";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else
                {
                    sql1 = $@"
                    select distinct /*c.special_sn_id ""MakerSERIAL"",
                                    c.physicsno ""SERIAL"",
                                    c.physicsno ""IMEI"",*/
                                    c.imei ""IMEI"",
                                    c.ssid ""Wi-Fi(2.4G)"",
                                    c.ssid3 ""Wi-Fi(5G)"",
                                    c.wifi_key ""Initial Password""
                      from r_to_detail a, r_ship_detail b, r_sn_tgmes_info c
                     where a.dn_no = b.dn_no
                       and b.sn = c.pcba_barcode
                       and c.valid_flag = '1'
                       and a.to_no = '{strTO}'
                     order by /*c.special_sn_id, c.physicsno*/ c.imei";

                    sql2 = $@"
                    select distinct '{strPO}' ""PO"",
                                    /*'P200025' ""SBB_PN"",*/
                                    'P210003' ""SBB_PN"",
                                    'N1A000' ""LOGISTIC_CODE"",
                                    /*c.physicsno ""SBB_SN"",*/
                                    c.imei ""SBB_SN"",
                                    c.mac_1 ""WAN_MAC_ID"",
                                    '' ""FOX_SN"",
                                    '1' ""STATUS""
                      from r_to_detail a, r_ship_detail b, r_sn_tgmes_info c
                     where a.dn_no = b.dn_no
                       and b.sn = c.pcba_barcode
                       and c.valid_flag = '1'
                       and a.to_no = '{strTO}'
                     order by /*c.physicsno*/ c.imei";

                    DataTable dt1 = SFCDB.ExecuteDataTable(sql1, CommandType.Text, null);
                    DataTable dt2 = SFCDB.ExecuteDataTable(sql2, CommandType.Text, null);

                    List<DataTable> dtList = new List<DataTable>();
                    dtList.Add(dt1);
                    dtList.Add(dt2);

                    StationReturn.Message = "Success！";
                    StationReturn.Data = dtList;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// 取消出貨時根據輸入獲取出貨信息
        /// </summary>
        public void GetShippingListByType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string type = Data["Type"] == null ? "" : Data["Type"].ToString().Trim();
                string value = Data["Value"] == null ? "" : Data["Value"].ToString().Trim();
                string dnLine = Data["DNLine"] == null ? "" : Data["DNLine"].ToString().Trim();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(SFCDB, DBTYPE);
                if (type == "DN")
                {
                    StationReturn.Data = TRSD.GetShipDetailByDN(SFCDB, value, dnLine);
                }
                else if (type == "PALLET")
                {
                    StationReturn.Data = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN_TGMES_INFO>((rsd, rs) => rsd.SN==rs.PCBA_BARCODE)
                        .Where((rsd, rs) => rs.PACKING3 == value).Select((rsd, rs) => rsd).ToList();
                }
                else
                {
                    StationReturn.Data = "";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";

                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = "";
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
        /// 根據輸入取消出貨記錄,更新SN/DN狀態
        /// </summary>
        public void CancelShipping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string type = Data["Type"] == null ? "" : Data["Type"].ToString().Trim();
                string inputValue = Data["Value"] == null ? "" : Data["Value"].ToString().Trim();
                string dnLine = Data["DNLine"] == null ? "" : Data["DNLine"].ToString().Trim();
                string remark = Data["Remark"] == null ? "" : Data["Remark"].ToString().Trim();

                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.BeginTrain();
                T_R_SN_TGMES_INFO T_TGMES = new T_R_SN_TGMES_INFO(SFCDB, this.DBTYPE);
                T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(SFCDB, DBTYPE);
                T_R_DN_STATUS TRDS = new T_R_DN_STATUS(SFCDB, DBTYPE);
                T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(SFCDB, DBTYPE);
                T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBTYPE);

                int result = 0;
                string dnNo = "";
                R_SHIP_DETAIL SHIPDetail;
                R_DN_STATUS DNStatus;
                DateTime SYSDATE = T_TGMES.GetDBDateTime(SFCDB);
                switch (type)
                {
                    case "PALLET":
                        #region Cancel Ship Out By Pallet                        
                        List<R_SN_TGMES_INFO> TGMESList = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == inputValue && t.VALID_FLAG == "1").ToList();
                        if (TGMESList.Count == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { inputValue }));
                        }
                        if (TGMESList.FindAll(t => t.COMPLETED_FLAG != "1").Count > 0)
                        {
                            throw new MESReturnMessage(inputValue + "中SN未完工!");
                        }
                        if (TGMESList.FindAll(t => t.SHIPPED_FLAG != "1").Count > 0)
                        {
                            throw new MESReturnMessage(inputValue + "中SN未出貨!");
                        }
                        if (TGMESList.FindAll(t => t.CURRENT_STATION != "TGMES_SHIPOUT").Count > 0)
                        {
                            throw new MESReturnMessage(inputValue + "中SN當前站錯誤!");
                        }
                        if (TGMESList.FindAll(t => t.NEXT_STATION != "TGMES_SHIPFINISH").Count > 0)
                        {
                            throw new MESReturnMessage(inputValue + "中SN下一站錯誤!");
                        }
                        
                        foreach (R_SN_TGMES_INFO TGMES in TGMESList)
                        {
                            T_TGMES.ChangeSnStatus(TGMES, "RETURN_SHIPPING", LoginUser.EMP_NO, SFCDB);

                            R_SN_STATION_DETAIL STATIONDetail = TRSSD.GetDetailBySnAndStation(TGMES.PCBA_BARCODE, "TGMES_SHIPOUT", SFCDB);
                            STATIONDetail.SN = "RS_" + STATIONDetail.SN;
                            result = TRSSD.Update(STATIONDetail, SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + TGMES.PCBA_BARCODE }));
                            }

                            SHIPDetail = TRSD.GetShipDetailBySN(SFCDB, TGMES.PCBA_BARCODE);
                            if (SHIPDetail == null)
                            {
                                throw new MESReturnMessage(TGMES.PCBA_BARCODE + " No Shipping Record!");
                            }
                            dnNo = SHIPDetail.DN_NO;
                            dnLine = SHIPDetail.DN_LINE;
                            
                            string newID = "RS" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + SHIPDetail.ID;
                            string newSN = "RS_" + SHIPDetail.SN;
                            string newSku = "RS_" + SHIPDetail.SKUNO;
                            string newDN = "RS_" + SHIPDetail.DN_NO;
                            string newDNLine = "RS_" + SHIPDetail.DN_LINE;
                            result = SFCDB.ORM.Updateable<R_SHIP_DETAIL>()
                                .SetColumns(r => new R_SHIP_DETAIL { ID = newID, SN = newSN, SKUNO = newSku, DN_NO = newDN, DN_LINE = newDNLine })
                                .Where(r => r.ID == SHIPDetail.ID).ExecuteCommand();
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + TGMES.PCBA_BARCODE }));
                            }
                        }

                        DNStatus = TRDS.GetStatusByNOAndLine(SFCDB, dnNo, dnLine);
                        if (DNStatus.DN_FLAG == "1")
                        {
                            DNStatus.DN_FLAG = "0";
                            DNStatus.EDITTIME = SYSDATE;
                            result = TRDS.Update(SFCDB, DNStatus);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                            }
                        }                        
                        #endregion
                        break;
                    case "DN":
                        #region Cancel Ship Out By DN 
                        dnNo = inputValue;
                        DNStatus = TRDS.GetStatusByNOAndLine(SFCDB, dnNo, dnLine);
                        if (DNStatus.DN_FLAG != "1")
                        {
                            throw new Exception("This " + dnNo + "," + dnLine + " Has Not Ship Finish!");
                        }
                        List<R_SHIP_DETAIL> SHIPDetailList = TRSD.GetShipDetailByDN(SFCDB, dnNo, dnLine);
                        if (SHIPDetailList.Count == 0)
                        {
                            throw new MESReturnMessage(dnNo + " No Shipping Record!");
                        }
                        foreach (R_SHIP_DETAIL SHIP in SHIPDetailList)
                        {
                            R_SN_TGMES_INFO TGMES = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(r => r.PCBA_BARCODE == SHIP.SN && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                            if (TGMES.NEXT_STATION != "TGMES_SHIPFINISH" && TGMES.CURRENT_STATION != "TGMES_SHIPOUT")
                            {
                                throw new MESReturnMessage(TGMES.PCBA_BARCODE + " Hasn't Been Shipped Yet!");
                            }
                            T_TGMES.ChangeSnStatus(TGMES, "RETURN_SHIPPING", LoginUser.EMP_NO, SFCDB);

                            R_SN_STATION_DETAIL STATIONDetail = TRSSD.GetDetailBySnAndStation(TGMES.PCBA_BARCODE, "TGMES_SHIPOUT", SFCDB);
                            STATIONDetail.SN = "RS_" + STATIONDetail.SN;
                            result = TRSSD.Update(STATIONDetail, SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + TGMES.PCBA_BARCODE }));
                            }
                            
                            string newID = "RS" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + SHIP.ID;
                            string newSN = "RS_" + SHIP.SN;
                            string newSku = "RS_" + SHIP.SKUNO;
                            string newDN = "RS_" + SHIP.DN_NO;
                            string newDNLine = "RS_" + SHIP.DN_LINE;
                            result = SFCDB.ORM.Updateable<R_SHIP_DETAIL>()
                                .SetColumns(r => new R_SHIP_DETAIL { ID = newID, SN = newSN, SKUNO = newSku, DN_NO = newDN, DN_LINE = newDNLine })
                                .Where(r => r.ID == SHIP.ID).ExecuteCommand();
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + TGMES.PCBA_BARCODE }));
                            }
                        }
                        DNStatus.DN_FLAG = "0";
                        DNStatus.EDITTIME = SYSDATE;
                        result = TRDS.Update(SFCDB, DNStatus);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                        }
                        #endregion
                        break;
                    default:
                        throw new MESReturnMessage(type + ",Input Type Error!");
                }
                MESPubLab.WriteLog.WriteIntoMESLog(SFCDB, BU, "RETURN_SHIPPING", "MESStation.Config.HWD.TGMESApi", "CancelShipping", remark, "", LoginUser.EMP_NO, inputValue, "");

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Message = "OK";
                SFCDB.CommitTrain();

                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = "";
                SFCDB.RollbackTrain();
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        private IList ConverDatatableToList(DataTable dataTable)
        {            
            var csvObjList = new List<CsvObject>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var csvObj = new CsvObject();
                csvObj.IMEI = dataTable.Rows[i]["IMEI"].ToString();
                csvObj.SSID = dataTable.Rows[i]["SSID"].ToString();
                csvObj.SSID3 = dataTable.Rows[i]["SSID3"].ToString();
                csvObj.WIFI_KEY = dataTable.Rows[i]["WIFI_KEY"].ToString();
                csvObjList.Add(csvObj);
            }
            return csvObjList;
        }

        public class CsvFile
        {
            public string FileName { get; set; }
            public string Content { get; set; }
        }

        public class CsvObject
        {
            public string IMEI { get; set; }

            [CsvHelper.Configuration.Attributes.Name("Wi-Fi(2.4G)")]
            public string SSID { get; set; }

            [CsvHelper.Configuration.Attributes.Name("Wi-Fi(5G)")]
            public string SSID3 { get; set; }

            [CsvHelper.Configuration.Attributes.Name("Initial Password")]
            public string WIFI_KEY { get; set; }
        }
    }
}
