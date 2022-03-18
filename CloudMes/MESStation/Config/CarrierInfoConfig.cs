using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESPubLab;
using MESStation.Interface.SAPRFC;
using SqlSugar;

namespace MESStation.Config
{
    public class CarrierInfoConfig : MesAPIBase
    {
        protected APIInfo FGetCCARRIERListData = new APIInfo()
        {
            FunctionName = "GetCCARRIERListData",
            Description = "Get CARRIER List",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetCCSIListData = new APIInfo()
        {
            FunctionName = "GetCCSIListData",
            Description = "Get CARRIER SKUNO LIST",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetRCSLListData = new APIInfo()
        {
            FunctionName = "GetRCSLListData",
            Description = "Get r_carrier_skuno_link ",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDelRCSLData = new APIInfo()
        {
            FunctionName = "DelRCSLData",
            Description = "Delete R_CARRIER_SKUNO_LINK ",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadExcelCarrierSkunoInfo = new APIInfo()
        {
            FunctionName = "UploadExcelCarrierSkunoInfo",
            Description = "上傳CarrierSkunoInfo的Excel ",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadExcelCarrier = new APIInfo()
        {
            FunctionName = "UploadExcelCarrier",
            Description = "上傳Carrier的Excel ",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteDataCsku = new APIInfo()
        {
            FunctionName = "DeleteDataCsku",
            Description = "將C_CARRIER_SKUNO_INFO 的valid_flag改為0 ",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteDataCarrier = new APIInfo()
        {
            FunctionName = "DeleteDataCarrier",
            Description = "將C_CARRIER 的valid_flag改為0 ",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FResetCarrierUsetimes = new APIInfo()
        {
            FunctionName = "ResetCarrierUsetimes",
            Description = "重置使用次數，C_CARRIER 的Usertimes",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FShowFailRecord = new APIInfo()
        {
            FunctionName = "ShowFailRecord",
            Description = "ShowFailRecord",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetCarrierSnLinkData = new APIInfo()
        {
            FunctionName = "GetCarrierSnLinkData",
            Description = "GetCarrierSnLinkData",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCheckCarrierNo = new APIInfo()
        {
            FunctionName = "CheckCarrierNo",
            Description = "CheckCarrierNo",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCheckSNForLink = new APIInfo()
        {
            FunctionName = "CheckSNForLink",
            Description = "CheckSNForLink",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FInsertCarrierSnLink = new APIInfo()
        {
            FunctionName = "InsertCarrierSnLink",
            Description = "InsertCarrierSnLink",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        public CarrierInfoConfig()
        {
            this.Apis.Add(FGetCCARRIERListData.FunctionName, FGetCCARRIERListData);
            this.Apis.Add(FGetCCSIListData.FunctionName, FGetCCSIListData);
            this.Apis.Add(FGetRCSLListData.FunctionName, FGetRCSLListData);
            this.Apis.Add(FDelRCSLData.FunctionName, FDelRCSLData);
            this.Apis.Add(FUploadExcelCarrierSkunoInfo.FunctionName, FUploadExcelCarrierSkunoInfo);
            this.Apis.Add(FDeleteDataCsku.FunctionName, FDeleteDataCsku);
            this.Apis.Add(FDeleteDataCarrier.FunctionName, FDeleteDataCarrier);
            this.Apis.Add(FUploadExcelCarrier.FunctionName, FUploadExcelCarrier);
            this.Apis.Add(FResetCarrierUsetimes.FunctionName, FResetCarrierUsetimes);
            this.Apis.Add(FShowFailRecord.FunctionName, FShowFailRecord);
            this.Apis.Add(FGetCarrierSnLinkData.FunctionName, FGetCarrierSnLinkData);
            this.Apis.Add(FCheckCarrierNo.FunctionName, FCheckCarrierNo);
            this.Apis.Add(FCheckSNForLink.FunctionName, FCheckSNForLink);
            this.Apis.Add(FInsertCarrierSnLink.FunctionName, FInsertCarrierSnLink);

        }

        public void GetCCARRIERListData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_C_CARRIER cc = new T_C_CARRIER(DB, DB_TYPE_ENUM.Oracle);
                List<C_CARRIER_MENU_LIST> Lccml= cc.GetCCarrierMenu(DB);
                foreach (var item in Lccml)
                {
                   item.enableTime = GetDBDateTime().AddDays(-2);
                   if (item.enableTime >= item.EDITTIME) item.timeLimitFlag = 0;
                   else item.timeLimitFlag = 1;
                }
                StationReturn.Data = Lccml;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void GetCCSIListData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string SEARCHTEXT = Data["SEARCHTEXT"].ToString().Trim();
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_C_CARRIER_SKUNO_INFO Ccsi = new T_C_CARRIER_SKUNO_INFO(DB, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = SEARCHTEXT==""?Ccsi.GetValidData(DB): Ccsi.GetValidData(SEARCHTEXT,DB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void ShowFailRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string FUNCTION_NAME = Data["FUNCTION_NAME"].ToString();
            string SearchTime = GetDBDateTime().AddSeconds(-12).ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TRML.GetMESLog("CloudMES", "MESStation.Config.CarrierInfoConfig", FUNCTION_NAME, SearchTime, "", LoginUser.EMP_NO.ToString(), DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void GetRCSLListData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string CARRIER_SKUNO = Data["CARRIER_SKUNO"].ToString();
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_CARRIER_SKUNO_LINK RCSL = new T_R_CARRIER_SKUNO_LINK(DB, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = RCSL.GetListByCarrierSkuno(CARRIER_SKUNO, DB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void DelRCSLData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string ID = Data["ID"].ToString();
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_CARRIER_SKUNO_LINK RCSL = new T_R_CARRIER_SKUNO_LINK(DB, DB_TYPE_ENUM.Oracle);
                int ret = RCSL.DeleteById(ID, DB);
                //StationReturn.Data = RCSL.DeleteById(ID, DB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                if (ret != 0)
                {
                    StationReturn.Message = "Successfully";
                }
                else
                {
                    StationReturn.Message = "Fail";
                }


            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void UploadExcelCarrierSkunoInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string data = Data["DataList"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                Newtonsoft.Json.Linq.JToken dataList = Data["DataList"];
                var successCount0 = 0;
                var successCount1 = 0;
                var failCount0 = 0;
                var failCount1 = 0;

                string CARRIER_SKUNO = string.Empty;
                string SKUNO = string.Empty;
                bool checkCCSI = true;
                bool checkRCSL = true;
                bool checkSKU = false;

                for (int i = 0; i < array.Count; i++)
                {
                    T_C_CARRIER_SKUNO_INFO TCCSI = new T_C_CARRIER_SKUNO_INFO(SFCDB, DBTYPE);
                    T_R_CARRIER_SKUNO_LINK TRCSL = new T_R_CARRIER_SKUNO_LINK(SFCDB, DBTYPE);
                    T_C_SKU TCSKU = new T_C_SKU(SFCDB, DBTYPE);
                    CARRIER_SKUNO = array[i]["CARRIER_SKUNO"].ToString().Trim();
                    SKUNO = array[i]["SKUNO"].ToString().Trim();
                    checkCCSI = TCCSI.checkExistByCarrierskuno(CARRIER_SKUNO, SFCDB);
                    checkRCSL = TRCSL.checkExist(CARRIER_SKUNO, SKUNO, SFCDB);
                    checkSKU = TCSKU.SkuNoIsExist(SKUNO,SFCDB);
                    if (CARRIER_SKUNO == "") { checkCCSI = true; }


                    if (!checkCCSI&& checkSKU)
                    {
                        try
                        {
                            Row_C_CARRIER_SKUNO_INFO rowCCSI = null;
                            rowCCSI = (Row_C_CARRIER_SKUNO_INFO)TCCSI.NewRow();
                            rowCCSI.ID = TCCSI.GetNewID(BU, SFCDB);
                            rowCCSI.CARRIER_SKUNO = array[i]["CARRIER_SKUNO"].ToString().Trim();
                            rowCCSI.CARRIER_TYPE = array[i]["CARRIER_TYPE"].ToString().Trim();
                            rowCCSI.CARRIER_NAME = array[i]["CARRIER_NAME"].ToString().Trim();
                            rowCCSI.CUSTOMER_NAME = array[i]["CUSTOMER_NAME"].ToString().Trim();
                            rowCCSI.CARRIER_MFR = array[i]["CARRIER_MFR"].ToString().Trim();
                            rowCCSI.LOCATION = array[i]["LOCATION"].ToString().Trim();
                            rowCCSI.SUSELIMIT = Convert.ToInt32(array[i]["SUSELIMIT"].ToString().Trim());
                            rowCCSI.MAXUSELIMIT = Convert.ToInt32(array[i]["MAXUSELIMIT"].ToString().Trim());
                            rowCCSI.LINKQTY = Convert.ToInt32(array[i]["LINKQTY"].ToString().Trim());
                            rowCCSI.VALID_FLAG = "1";
                            rowCCSI.EDITBY = LoginUser.EMP_NO;
                            rowCCSI.EDITTIME = TCCSI.GetDBDateTime(SFCDB);
                            SFCDB.ExecSQL(rowCCSI.GetInsertString(DBTYPE));
                            successCount0 += 1;

                        }
                        catch (Exception ex)
                        {
                            InsertFailLog(ex.Message, CARRIER_SKUNO, SKUNO, "UploadExcelCSN", SFCDB);
                            failCount0 += 1;
                            continue;
                        }
                    }
                    else if (checkRCSL && checkSKU)
                    {
                        InsertFailLog("配置已存在", CARRIER_SKUNO, SKUNO, "UploadExcelCSN", SFCDB);
                        failCount0 += 1;
                    }
                    else
                    {
                        InsertFailLog("產品機種不存在", CARRIER_SKUNO, SKUNO, "UploadExcelCSN", SFCDB);
                        failCount0 += 1;
                    }

                    if (!checkRCSL&& checkSKU)
                    {
                        try
                        {
                            Row_R_CARRIER_SKUNO_LINK rowRCSL = null;
                            rowRCSL = (Row_R_CARRIER_SKUNO_LINK)TRCSL.NewRow();
                            rowRCSL.ID = TRCSL.GetNewID(BU, SFCDB);
                            rowRCSL.CARRIER_SKUNO = array[i]["CARRIER_SKUNO"].ToString().Trim();
                            rowRCSL.SKUNO = array[i]["SKUNO"].ToString().Trim();
                            SFCDB.ExecSQL(rowRCSL.GetInsertString(DBTYPE));
                            successCount1 += 1;

                        }
                        catch (Exception ex)
                        {
                            InsertFailLog(ex.Message, CARRIER_SKUNO, SKUNO, "UploadExcelCSN", SFCDB);
                            failCount1 += 1;
                            continue;
                        }
                    }
                    else if(checkRCSL&& checkSKU)
                    {
                        InsertFailLog("綁定關係已存在", CARRIER_SKUNO, SKUNO, "UploadExcelCSN", SFCDB);
                        failCount1 += 1;
                    }
                    else
                    {
                        InsertFailLog("產品機種不存在", CARRIER_SKUNO, SKUNO, "UploadExcelCSN", SFCDB);
                        failCount1 += 1;
                    }

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = failCount0+ failCount1;
                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814172244", new string[] { successCount0.ToString(), failCount0.ToString(), successCount1.ToString(),failCount1.ToString() });
                StationReturn.Message = errMessage;
                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        public void UploadExcelCarrier(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string data = Data["DataList"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                Newtonsoft.Json.Linq.JToken dataList = Data["DataList"];
                var successCount = 0;
                var failCount = 0;
                string CARRIERNO = string.Empty;
                string CARRIER_SKUNO = string.Empty;
                bool checkTCC = true;
                bool checkTCCSI = true;


                for (int i = 0; i < array.Count; i++)
                {
                    T_C_CARRIER_SKUNO_INFO TCCSI = new T_C_CARRIER_SKUNO_INFO(SFCDB, DBTYPE);
                    T_C_CARRIER TCC = new T_C_CARRIER(SFCDB, DBTYPE);

                    CARRIERNO = array[i]["CARRIERNO"].ToString().Trim();
                    CARRIER_SKUNO = array[i]["CARRIER_SKUNO"].ToString().Trim();
                    checkTCC = TCC.checkExistByCARRIERNO(CARRIERNO, SFCDB);
                    checkTCCSI = TCCSI.checkExistByCarrierskuno(CARRIER_SKUNO, SFCDB);


                    if (!checkTCC && checkTCCSI)
                    {
                        try
                        {
                            Row_C_CARRIER rowRCC = null;
                            rowRCC = (Row_C_CARRIER)TCC.NewRow();
                            rowRCC.ID = TCC.GetNewID(BU, SFCDB);
                            rowRCC.CARRIERNO = array[i]["CARRIERNO"].ToString().Trim();
                            rowRCC.CARRIER_SKUNO = array[i]["CARRIER_SKUNO"].ToString().Trim();
                            rowRCC.USETIMES = 0;
                            rowRCC.VALID_FLAG = "1";
                            rowRCC.EDITBY = LoginUser.EMP_NO;
                            rowRCC.EDITTIME = TCC.GetDBDateTime(SFCDB);
                            SFCDB.ExecSQL(rowRCC.GetInsertString(DBTYPE));
                            successCount += 1;

                        }
                        catch (Exception ex)
                        {
                            InsertFailLog(ex.Message, CARRIERNO, CARRIER_SKUNO, "UploadExcelCarrier", SFCDB);
                            failCount += 1;
                            continue;
                        }
                    }
                    else if (checkTCC)
                    {
                        InsertFailLog("配置已存在", CARRIERNO, CARRIER_SKUNO, "UploadExcelCarrier", SFCDB);
                        failCount += 1;
                    }
                    else if (!checkTCCSI)
                    {
                        InsertFailLog("載具機種不存在", CARRIERNO, CARRIER_SKUNO, "UploadExcelCarrier", SFCDB);
                        failCount += 1;
                    }

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = failCount;
                string errMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814171820",new string[] { successCount.ToString(), failCount.ToString() });
                StationReturn.Message = errMessage;
                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        public void DeleteDataCsku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string ID = Data["ID"].ToString();
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_CARRIER_SKUNO_LINK TRCSL = new T_R_CARRIER_SKUNO_LINK(DB, DB_TYPE_ENUM.Oracle);
                T_C_CARRIER_SKUNO_INFO TCCSI = new T_C_CARRIER_SKUNO_INFO(DB, DB_TYPE_ENUM.Oracle);
                C_CARRIER_SKUNO_INFO CCSIList = TCCSI.GET_byID(ID, DB);
                int ret0 = TCCSI.setValidFlag0byID(ID, LoginUser.EMP_NO, TCCSI.GetDBDateTime(DB), DB);
                int ret1 = TRCSL.DeleteByCarrierSkuno(CCSIList.CARRIER_SKUNO, DB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                if (ret0 != 0)
                {
                    StationReturn.Message = "Successfully";
                }
                else
                {
                    StationReturn.Message = "Fail";
                }


            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void DeleteDataCarrier(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string ID = Data["ID"].ToString();
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_C_CARRIER TCC = new T_C_CARRIER(DB, DB_TYPE_ENUM.Oracle);

                int ret0 = TCC.setValidFlag0byID(ID, LoginUser.EMP_NO, TCC.GetDBDateTime(DB), DB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                if (ret0 != 0)
                {
                    StationReturn.Message = "Successfully";
                }
                else
                {
                    StationReturn.Message = "Fail";
                }


            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void ResetCarrierUsetimes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string ID = Data["ID"].ToString();
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_C_CARRIER TCC = new T_C_CARRIER(DB, DB_TYPE_ENUM.Oracle);

                int ret0 = TCC.resetCarrierUsetimes(ID, LoginUser.EMP_NO, TCC.GetDBDateTime(DB), DB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                if (ret0 != 0)
                {
                    StationReturn.Message = "Successfully";
                }
                else
                {
                    StationReturn.Message = "Fail";
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void InsertFailLog(string LOG_MESSAGE, string Data1, string Data2, string FUNCTION_NAME, MESDBHelper.OleExec DB)
        {
            T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DBTYPE);
            R_MES_LOG LOG = new R_MES_LOG
            {
                ID = TRML.GetNewID(BU, DB),
                DATA1 = Data1,
                DATA2 = Data2,
                FUNCTION_NAME = FUNCTION_NAME,
                CLASS_NAME = "MESStation.Config.CarrierInfoConfig",
                PROGRAM_NAME = "CloudMES",
                EDIT_TIME = GetDBDateTime(),
                EDIT_EMP = LoginUser.EMP_NO,
                LOG_MESSAGE = LOG_MESSAGE
            };
            TRML.InsertMESLog(LOG, DB);
        }
        public void GetCarrierSnLinkData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            

            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_CARRIER_LINK TRCL = new T_R_CARRIER_LINK(DB,DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TRCL.GetDataList(DB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void CheckCarrierNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string  Message = string.Empty;
            string CARRIERNO = Data["CARRIERNO"].ToString().Trim();
            int linkQTY = 1;
            int CarrierUseTimes = 0;
            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_CARRIER_LINK TRCL = new T_R_CARRIER_LINK(DB,DB_TYPE_ENUM.Oracle);
                T_C_CARRIER TCC = new T_C_CARRIER(DB, DB_TYPE_ENUM.Oracle);
                T_C_CARRIER_SKUNO_INFO TCCSI = new T_C_CARRIER_SKUNO_INFO(DB, DB_TYPE_ENUM.Oracle);
                C_CARRIER cc = TCC.GET_byCarrierNo(CARRIERNO,DB);
                CarrierUseTimes = TRCL.GetCarrierCount(CARRIERNO, DB);
                if (cc != null)
                {
                    C_CARRIER_SKUNO_INFO si = TCCSI.GET_byCarrierSkuno(cc.CARRIER_SKUNO, DB);
                    linkQTY = si.LINKQTY;
                    DateTime LastEditTime = Convert.ToDateTime(cc.EDITTIME);
                    if (si.SUSELIMIT<=cc.USETIMES)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507093423", new string[] { CARRIERNO });
                        StationReturn.Message = Message;
                    }
                    else if (LastEditTime.AddDays(2) < GetDBDateTime())
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507093904", new string[] { CARRIERNO,"2" });
                        StationReturn.Message = Message;

                    }
                    else if (CarrierUseTimes>=si.MAXUSELIMIT)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507094226", new string[] { CARRIERNO });
                        StationReturn.Message = Message;
                    }
                    else
                    {
                        StationReturn.Data = linkQTY;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507095102", new string[] { CARRIERNO ,cc.USETIMES.ToString(),si.SUSELIMIT.ToString(),cc.EDITTIME.ToString()});
                        StationReturn.Message = Message;
                    }
                    
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    Message=MESReturnMessage.GetMESReturnMessage("MSGCODE20200507092635", new string[] { CARRIERNO });
                    StationReturn.Message = Message;
                }

           
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void CheckSNForLink(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string Message = string.Empty;
            string SN = Data["SN"].ToString().Trim();
            string CARRIERNO = Data["CARRIERNO"].ToString().Trim();
            bool checkSkuno = true;


            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_SN TRSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
                T_R_CARRIER_LINK TRCL = new T_R_CARRIER_LINK(DB, DB_TYPE_ENUM.Oracle);
                T_C_CARRIER TCC = new T_C_CARRIER(DB, DB_TYPE_ENUM.Oracle);
                T_C_CARRIER_SKUNO_INFO TCCSI = new T_C_CARRIER_SKUNO_INFO(DB, DB_TYPE_ENUM.Oracle);
                R_SN rsn = TRSN.LoadData(SN, DB);
                List<C_CARRIER_MENU_LIST> cc = TCC.GetSnSkunoByCarrierNo(CARRIERNO, DB);
                if (rsn != null)
                {
                    if (cc != null)
                    {   
                        for (int i=0;i<cc.Count;i++)
                        {
                            if (rsn.SKUNO == cc[i].SKUNO) {
                                checkSkuno = false;
                                break;
                            }
                        }
                        if(checkSkuno)
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507152955", new string[] { rsn.SKUNO });
                            StationReturn.Message = Message;
                        }
                        else
                        {
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            Message = MESReturnMessage.GetMESReturnMessage("MES00000109", new string[] {SN});
                            StationReturn.Message = Message;

                        }
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        Message = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { CARRIERNO });
                        StationReturn.Message = Message;
                    }

                }
                else {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    Message = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SN });
                    StationReturn.Message = Message;
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
        public void InsertCarrierSnLink(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec DB = null;
            string Message = string.Empty;
            string CARRIERNO = Data["CARRIERNO"].ToString().Trim();
            string LinkQTY = Data["LinkQTY"].ToString().Trim();
            string SN = string.Empty;


            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_CARRIER_LINK TRCL = new T_R_CARRIER_LINK(DB, DBTYPE);
                T_C_CARRIER TCC = new T_C_CARRIER(DB, DBTYPE);
                if (LinkQTY == "1")
                {
                    SN = Data["SN"].ToString().Trim();
                    Row_R_CARRIER_LINK rowRCL = null;
                    rowRCL = (Row_R_CARRIER_LINK)TRCL.NewRow();
                    rowRCL.ID = TRCL.GetNewID(BU, DB);
                    rowRCL.SN = SN;
                    rowRCL.CARRIERNO = CARRIERNO;
                    rowRCL.EDITBY = LoginUser.EMP_NO;
                    rowRCL.EDITTIME = TRCL.GetDBDateTime(DB);
                    DB.ExecSQL(rowRCL.GetInsertString(DBTYPE));
                    TCC.AddUsetimes(CARRIERNO, DB);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507155100");
                    StationReturn.Message = Message;
                }
                else
                {
                    try { 
                    int intLinkQTY = Convert.ToInt32(LinkQTY);
                    for (int i=0;i< intLinkQTY;i++) {
                        string DataName = "SN"+i.ToString();
                        SN = Data[DataName].ToString().Trim();
                        Row_R_CARRIER_LINK rowRCL = null;
                        rowRCL = (Row_R_CARRIER_LINK)TRCL.NewRow();
                        rowRCL.ID = TRCL.GetNewID(BU, DB);
                        rowRCL.SN = SN;
                        rowRCL.CARRIERNO = CARRIERNO;
                        rowRCL.EDITBY = LoginUser.EMP_NO;
                        rowRCL.EDITTIME = TRCL.GetDBDateTime(DB);
                        DB.ExecSQL(rowRCL.GetInsertString(DBTYPE));
                    }
                    TCC.AddUsetimes(CARRIERNO, DB);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200507155100");
                    StationReturn.Message = Message;
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
                
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }
    }
}
