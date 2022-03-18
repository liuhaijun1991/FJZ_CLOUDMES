using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.DCN;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Config
{
    public class CReplaceSN : MesAPIBase
    {
        protected APIInfo FUploadRepalceSN = new APIInfo()
        {
            FunctionName = "UploadRepalceSN",
            Description = "UploadRepalceSN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadWaitRepalceSN = new APIInfo()
        {
            FunctionName = "UploadWaitRepalceSN",
            Description = "UploadWaitRepalceSN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetRepalceSNList = new APIInfo()
        {
            FunctionName = "GetRepalceSNList",
            Description = "Get Repalce SN List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "OLD_SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NEW_SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DATE_FROM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DATE_TO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_SIZE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_NUM", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteRepalceSN = new APIInfo()
        {
            FunctionName = "DeleteRepalceSN",
            Description = "Delete Repalce SN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CReplaceSN() {
            this.Apis.Add(FUploadRepalceSN.FunctionName, FUploadRepalceSN);
            this.Apis.Add(FDeleteRepalceSN.FunctionName, FDeleteRepalceSN);
            this.Apis.Add(FGetRepalceSNList.FunctionName, FGetRepalceSNList);
            this.Apis.Add(FUploadWaitRepalceSN.FunctionName, FUploadWaitRepalceSN);
        }

        /// <summary>
        ///  ReworkToNewSkuSN.html頁面上傳Excel調用API
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UploadWaitRepalceSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
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
                    //throw new Exception($@"上傳的Excel中沒有內容!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));

                }


              
              
                string result = "";
                string old_sn = "",  remark = "";
                #region 写入数据库

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    old_sn = dt.Rows[i]["OLDSN"] == null ? "" : dt.Rows[i]["OLDSN"].ToString().ToUpper().Trim();
                    remark = dt.Rows[i]["REMARK"] == null ? "" : dt.Rows[i]["REMARK"].ToString().ToUpper().Trim();
                    try
                    {
                        if (!string.IsNullOrEmpty(old_sn))
                        {
                            List<R_SN_REPLACE> r_SNs = SFCDB.ORM.Queryable<R_SN_REPLACE>().Where(t => t.OLDSN == old_sn && t.LINKTYPE == "WaitReplace").ToList();
                            if (r_SNs.Count>0)
                            {
                                throw new Exception("Already Exist!");
                            }
                            List<R_SN> CheckSNExists = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == old_sn && t.VALID_FLAG=="1").ToList();
                            if (CheckSNExists.Count==0)
                            {
                                throw new Exception($@"{old_sn} Is Not System SN!");
                            }
                        
                           int re = SFCDB.ORM.Insertable(new R_SN_REPLACE()
                           {
                               ID = MESDataObject.MesDbBase.GetNewID<R_SN_REPLACE>(SFCDB.ORM, BU),
                               LINKTYPE = "WaitReplace",
                               OLDSN = old_sn,
                               FLAG = "0",
                               REMARK = remark,
                               CREATEBY = LoginUser.EMP_NO,
                               CREATETIME = SFCDB.ORM.GetDate()
                           }).ExecuteCommand();
                            if (re == 0)
                            {
                                throw new Exception("Save Fail!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result += old_sn + "," + ex.Message + ";";
                    }

                }


                if (result == "")
                {
                    result = "All Upload OK ! ";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    result = "Upload Fail:" + result;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                #endregion

                StationReturn.Message = result;

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
        public void UploadRepalceSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;          
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();              
              
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    //throw new Exception($@"上傳的文件內容為空!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));

                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                MESDataObject.Module.T_R_SN_REPLACE t_r_sn_replace = new MESDataObject.Module.T_R_SN_REPLACE(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                MESDataObject.Module.T_R_SN t_r_sn = new MESDataObject.Module.T_R_SN(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                MESDataObject.Module.R_SN_REPLACE reObj = null;
                string result = "";
                string old_sn = "", station = "", remark = "";
                #region 写入数据库

                for (int i = 0; i < array.Count; i++)
                {
                    old_sn = array[i]["OLD_SN"] == null ? "" : array[i]["OLD_SN"].ToString().ToUpper().Trim();
                    station = array[i]["STATION"] == null ? "" : array[i]["STATION"].ToString().ToUpper().Trim();
                    remark = array[i]["REMARK"] == null ? "" : array[i]["REMARK"].ToString().ToUpper().Trim();
                    try
                    {                       
                        if (!string.IsNullOrEmpty(old_sn))
                        {
                            if (string.IsNullOrEmpty(station))
                            {
                                throw new Exception("STATION Is Null!");
                            }
                            bool isExist = SFCDB.ORM.Queryable<MESDataObject.Module.R_SN_REPLACE>().Where(r => r.OLDSN == old_sn).Any();
                            if (isExist)
                            {
                                throw new Exception("Already Exist!");
                            }
                            if (!t_r_sn.CheckSNExists(old_sn, SFCDB))
                            {
                                throw new Exception($@"{old_sn} Is Not System SN!");
                            }
                            reObj = new MESDataObject.Module.R_SN_REPLACE();
                            reObj.ID = t_r_sn_replace.GetNewID(BU, SFCDB);
                            reObj.LINKTYPE = "SN";
                            reObj.OLDSN = old_sn;
                            reObj.STATION = station;
                            reObj.FLAG = "0";
                            reObj.REMARK = remark;
                            reObj.CREATEBY = LoginUser.EMP_NO;
                            reObj.CREATETIME = SFCDB.ORM.GetDate();
                            int re = SFCDB.ORM.Insertable(reObj).ExecuteCommand();
                            if (re == 0)
                            {
                                throw new Exception("Save Fail!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result += old_sn + "," + ex.Message + ";";
                    }
                    
                }
                

                if (result == "")
                {
                    result = "All Upload OK ! ";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    result = "Upload Fail:" + result;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                #endregion

                StationReturn.Message = result;
              
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);               
            }
            finally {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void DeleteRepalceSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                int re=SFCDB.ORM.Deleteable<MESDataObject.Module.R_SN_REPLACE>().Where(r => r.ID == id).ExecuteCommand();
                if (re == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Fail";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
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

        public void GetRepalceSNList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();               
                string old_sn = Data["OLD_SN"] == null ? "" : Data["OLD_SN"].ToString().Trim();
                string new_sn = Data["NEW_SN"] == null ? "" : Data["NEW_SN"].ToString().Trim();
                string flag = Data["FLAG"] == null ? "" : Data["FLAG"].ToString().Trim();
                string date_from = Data["DATE_FROM"] == null ? "" : Data["DATE_FROM"].ToString().Trim();
                string date_to = Data["DATE_TO"] == null ? "" : Data["DATE_TO"].ToString().Trim();
                string page_size = Data["PAGE_SIZE"] == null ? "" : Data["PAGE_SIZE"].ToString().Trim();
                string page_num = Data["PAGE_NUM"] == null ? "" : Data["PAGE_NUM"].ToString().Trim();

                flag = flag == "" ? "ALL" : flag;
                page_size = page_size == "" ? "10" : page_size;
                page_num = page_num == "" ? "1" : page_num;
                string total_sql = "select * from r_sn_replace where 1=1 ";
                string old_sql = "";
                string new_sql = "";
                string flag_sql = "";
                string date_sql = "";
                string page_sql = "";
                if (!string.IsNullOrEmpty(old_sn))
                {
                    old_sql = $@" and oldsn='{old_sn}' ";
                }
                if (!string.IsNullOrEmpty(new_sn))
                {
                    new_sql = $@" and newsn='{new_sn}' ";
                }
                if (flag.ToUpper() != "ALL")
                {
                    flag_sql = $@" and flag='{flag}' ";
                }
                if (!string.IsNullOrEmpty(date_from) && !string.IsNullOrEmpty(date_to))
                {
                    date_sql = $@" and createtime between to_date('{date_from} 00:00:00','yyyy/mm/dd HH24:mi:ss') and to_date('{date_to} 00:00:00','yyyy/mm/dd HH24:mi:ss') ";
                }
                else if (string.IsNullOrEmpty(old_sn) && string.IsNullOrEmpty(new_sn))
                {
                    date_sql = $@" and createtime between sysdate-30 and sysdate ";
                }
                total_sql += old_sql + new_sql + flag_sql + date_sql;
                DataTable dt = SFCDB.ExecuteDataTable($@"select count(*) as total from ({total_sql}) temp", CommandType.Text, null);
                string total = dt.Rows[0]["TOTAL"].ToString() == "" ? "0" : dt.Rows[0]["TOTAL"].ToString();
                if (total == "0")
                {
                    throw new Exception("No Data!");
                }
                page_sql = $@"select  RN as NO,ID,LINKTYPE,OLDSN,NEWSN,BOXSN,STATION,decode(flag,'0','Waiting For Rework','1','Reworked','ERROR') AS FLAG,
                        REMARK,CREATETIME,CREATEBY,EDITTIME,EDITBY from (select rownum as rn ,temp.* from ({total_sql} order by createtime asc ) temp )
                        where rn> ({page_num} - 1) * {page_size} and rn<= {page_num} * {page_size}";
                DataTable dtPage = SFCDB.ExecuteDataTable(page_sql, CommandType.Text, null);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { Total = total, Rows = dtPage };
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
    }
}
