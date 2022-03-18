using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.Vertiv
{
    public class PrintOffLineLabelAPI : MesAPIBase
    {
        protected APIInfo FGetAllOffLineLabelType = new APIInfo()
        {
            FunctionName = "GetAllOffLineLabelType",
            Description = "GetAllOffLineLabelType",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetPrintLog = new APIInfo()
        {
            FunctionName = "GetPrintLog",
            Description = "Get Print Log",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="LabelType",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="WO",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="SN",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetReprintLog = new APIInfo()
        {
            FunctionName = "GetReprintLog",
            Description = "Get Rerint Log",
            Parameters = new List<APIInputInfo>() {                
                new APIInputInfo(){ InputName="WO",InputType="string",DefaultValue="" },
                new APIInputInfo(){ InputName="SN",InputType="string",DefaultValue="" } 
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FReprintLabel = new APIInfo()
        {
            FunctionName = "ReprintLabel",
            Description = "ReprintLabel",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName="ID",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetExceptionSn = new APIInfo()
        {
            FunctionName = "GetExceptionSn",
            Description = "GetExceptionSn",
            Parameters = new List<APIInputInfo>() {             
                new APIInputInfo(){ InputName="SN",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCheckUploadPermission = new APIInfo()
        {
            FunctionName = "CheckUploadPermission",
            Description = "CheckUploadPermission",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUploadExceptionSn = new APIInfo()
        {
            FunctionName = "UploadExceptionSn",
            Description = "PE Upload Exception Sn",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public PrintOffLineLabelAPI()
        {
            this.Apis.Add(FGetAllOffLineLabelType.FunctionName, FGetAllOffLineLabelType);
            this.Apis.Add(FGetPrintLog.FunctionName, FGetPrintLog);
            this.Apis.Add(FGetReprintLog.FunctionName, FGetReprintLog);
            this.Apis.Add(FReprintLabel.FunctionName, FReprintLabel);
            this.Apis.Add(FCheckUploadPermission.FunctionName, FCheckUploadPermission);
            this.Apis.Add(FUploadExceptionSn.FunctionName, FUploadExceptionSn);
            this.Apis.Add(FGetExceptionSn.FunctionName, FGetExceptionSn);
        }

        public void GetAllOffLineLabelType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                List<string> list = new List<string>();
                list.Add("All");
                List<string> listLabel = SFCDB.ORM.Queryable<R_JSON>().Where(r => r.TYPE == "PrintOffLineLabel").Select(r => r.INDEX2).Distinct().ToList();
                list.AddRange(listLabel);
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }            
        }

        public void GetPrintLog(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string labelType = Data["LabelType"] == null ? "All" : Data["LabelType"].ToString();
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString();
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString();
                var list = SFCDB.ORM.Queryable<R_JSON>()
                    .Where(r => r.TYPE == "PrintOffLineLabel")
                    .WhereIF(!labelType.ToUpper().Equals("ALL"), r => r.INDEX2 == labelType)
                    .WhereIF(!string.IsNullOrEmpty(wo), r => r.INDEX1 == wo)
                    .WhereIF(!string.IsNullOrEmpty(sn), r => r.NAME == sn)
                    .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Asc)
                    .Select(r => new { ID = r.ID, WO = r.INDEX1, SN = r.NAME, LabelType = r.INDEX2, r.EDIT_TIME, r.EDIT_EMP })
                    .ToList();                
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetReprintLog(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {                
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString();
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString();
                var list = SFCDB.ORM.Queryable<R_SN_LOG>()
                    .Where(r => r.LOGTYPE == "ReprintOffLineLabel")
                    .WhereIF(!string.IsNullOrEmpty(wo), r => r.DATA1 == wo)
                    .WhereIF(!string.IsNullOrEmpty(sn), r => r.SN == sn)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Asc)
                    .Select(r => new { ID = r.ID, WO = r.DATA1, SN = r.SN, ReprintTime = r.CREATETIME, ReprintBy = r.CREATEBY })
                    .ToList();
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void ReprintLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if(Data["ID"] == null)
                {
                    throw new Exception("ID is null");
                }
                string id = Data["ID"].ToString();
                var json = SFCDB.ORM.Queryable<R_JSON>().Where(r => r.ID == id).ToList().FirstOrDefault();
                string str = System.Text.Encoding.Unicode.GetString(json.BLOBDATA);
                var labelBase = Newtonsoft.Json.JsonConvert.DeserializeObject<MESPubLab.MESStation.Label.LabelBase>(str);
                Dictionary<string, List<MESPubLab.MESStation.Label.LabelBase>> LabelPrints = new Dictionary<string, List<MESPubLab.MESStation.Label.LabelBase>>();
                LabelPrints.Add(labelBase.FileName, new List<MESPubLab.MESStation.Label.LabelBase>() { labelBase});

                R_SN_LOG logObj = new R_SN_LOG();
                logObj.ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOG");
                logObj.SNID = id;
                logObj.SN = json.NAME;
                logObj.LOGTYPE = "ReprintOffLineLabel";
                logObj.DATA1 = json.INDEX1;
                logObj.CREATEBY = LoginUser.EMP_NO;
                logObj.CREATETIME = SFCDB.ORM.GetDate();
                SFCDB.ORM.Insertable<R_SN_LOG>(logObj).ExecuteCommand();

                StationReturn.Data = new { LabelPrints = LabelPrints };
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }        
        public void GetExceptionSn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {               
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString();
                var list = SFCDB.ORM.Queryable<R_SN_LOG>()
                    .Where(r => r.LOGTYPE == "OffLineLabel_ExceptionSn")                   
                    .WhereIF(!string.IsNullOrEmpty(sn), r => r.SN == sn)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Asc)
                    .Select(r => new { ID = r.ID, SN = r.SN,r.CREATETIME,r.CREATEBY })
                    .ToList();
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }    
        public void CheckUploadPermission(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_USER_PRIVILEGE TCUP = new T_C_USER_PRIVILEGE(SFCDB, DBTYPE);              
                bool bUpload = TCUP.CheckpPivilegeByName(SFCDB, "UPLOAD_EXCEPTIONSN", this.LoginUser.EMP_NO);
                if(!bUpload)
                {
                    throw new Exception("no permission");
                }
                StationReturn.Data = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    
        public void UploadExceptionSn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                //定義上傳Excel的列名 
                List<string> inputTitle = new List<string> { "SN"};
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
                
                string sn = "";
                string message = "";


                string fail_msg = "";
                int fail_count = 0;
                int pass_count = 0;
              
                SFCDB = this.DBPools["SFCDB"].Borrow();               
              
                for (int i = 0; i < array.Count; i++)
                {
                    sn = "";                   
                    try
                    {
                        sn = array[i]["SN"].ToString().ToUpper().Trim();
                        if(string.IsNullOrWhiteSpace(sn))
                        {
                            continue;
                        }
                        R_SN_LOG logObj = new R_SN_LOG();
                        logObj.ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOG");
                        logObj.SNID = sn;
                        logObj.SN = sn;
                        logObj.LOGTYPE = "OffLineLabel_ExceptionSn";                       
                        logObj.CREATEBY = LoginUser.EMP_NO;
                        logObj.CREATETIME = SFCDB.ORM.GetDate();
                        logObj.DATA9 = "例外SN，不在OffLineLabel工站打印又要進行綁定";
                        SFCDB.ORM.Insertable<R_SN_LOG>(logObj).ExecuteCommand();
                        pass_count++;                        
                    }
                    catch (Exception ex)
                    {
                        fail_count++;
                        fail_msg += $@"{sn}:{ex.Message};";                        
                    }
                }

                if (fail_count != 0)
                {
                    message = $@"{pass_count} SN Successful,The Following SN Uplaod Fail:/n/r {fail_msg}!";
                }
                else
                {
                    message = "All SN Upload Successful!";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = message;
                StationReturn.MessageCode = null;
                StationReturn.Data = null;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
