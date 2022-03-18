using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace MESStation.GlobalConfig
{
  public  class LanguagePageConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo getPagelanguagejson = new APIInfo()
        {
            FunctionName = "GetPagelanguagejson",
            Description = "GetPagelanguagejson",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo pagelanguage = new APIInfo()
        {
            FunctionName = "GetPageLanguage",
            Description = "獲取網頁標簽語言",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="LanguageValue",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo addpagelanguage = new APIInfo()
        {
            FunctionName = "AddPageLanguage",
            Description = "添加頁面標簽語言",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="LabelName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Chinese",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ChineseWT",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="English",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo deletepagelanguage = new APIInfo()
        {
            FunctionName = "DeletePageLanguage",
            Description = "刪除頁面標簽語言",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo updatepagelanguage = new APIInfo()
        {
            FunctionName = "UpdatePageLanguage",
            Description = "更新頁面標簽語言",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="LabelName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Chinese",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ChineseWT",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="English",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo querypagelanguage = new APIInfo()
        {
            FunctionName = "QueryPageLanguage",
            Description = "查詢頁面標簽語言",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="LabelName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Chinese_TW",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo excelimportpagelanguage = new APIInfo()
        {
            FunctionName = "ExcelImport",
            Description = "Excel導入多語言數據",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>()
            { }

        };

        public LanguagePageConfig()
        {
            _MastLogin = false;
            this.Apis.Add(getPagelanguagejson.FunctionName, getPagelanguagejson);
            this.Apis.Add(pagelanguage.FunctionName, pagelanguage);
            this.Apis.Add(addpagelanguage.FunctionName, addpagelanguage);
            this.Apis.Add(deletepagelanguage.FunctionName, deletepagelanguage);
            this.Apis.Add(updatepagelanguage.FunctionName, updatepagelanguage);
            this.Apis.Add(querypagelanguage.FunctionName, querypagelanguage);
            this.Apis.Add(excelimportpagelanguage.FunctionName, excelimportpagelanguage);
        }

        public void GetPagelanguagejson(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<LanguagePage> ret = new List<LanguagePage>();
            


            //string PageName = Data["PageName"].ToString().Trim();
            //string LanguageValue = Data["LanguageValue"] == null || Data["LanguageValue"].ToString().ToUpper() == "ES" ? "ENGLISH" : Data["LanguageValue"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var ls = sfcdb.ORM.Queryable<C_LANGUAGE_PAGE>().ToList();

                for (int i = 0; i < ls.Count; i++)
                {
                    
                    try
                    {
                        LanguagePage l = new LanguagePage()
                        { labelName = ls[i].LABEL_NAME, pagename = ls[i].PAGE_NAME };
                        ret.Add(l);
                        l.Txt.Add("en", ls[i].ENGLISH);
                        l.Txt.Add("chs", ls[i].CHINESE);
                        l.Txt.Add("ch-tw", ls[i].CHINESE_TW);

                    }
                    catch
                    { 
                    
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }


        }

        /// <summary>
        /// 獲取標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetPageLanguage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            Dictionary<String, String> LanguageList = new Dictionary<String, String>();
            T_C_LANGUAGE_PAGE language;
            string PageName = Data["PageName"].ToString().Trim();
            string LanguageValue = Data["LanguageValue"]==null || Data["LanguageValue"].ToString().ToUpper() == "ES" ? "ENGLISH": Data["LanguageValue"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                language = new T_C_LANGUAGE_PAGE(sfcdb, DBTYPE);
                LanguageList = language.GetPageLanguage(PageName, LanguageValue, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode ="MES00000001";
                StationReturn.Data = LanguageList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
           

        }
        /// <summary>
        /// 添加標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddPageLanguage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_C_LANGUAGE_PAGE language;
            string PageName = Data["PageName"].ToString().Trim();
            string LabelName = Data["LabelName"].ToString().Trim();
            string Chinese = Data["Chinese"].ToString().Trim();
            string ChineseWT = Data["ChineseWT"].ToString().Trim();
            string English = Data["English"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                language = new T_C_LANGUAGE_PAGE(sfcdb, DBTYPE);
                if (language.CheckDataExist(PageName, LabelName, sfcdb))
                {
                    Row_C_LANGUAGE_PAGE row = (Row_C_LANGUAGE_PAGE)language.NewRow();
                    row.ID = language.GetNewID(BU, sfcdb);
                    row.PAGE_NAME = PageName;
                    row.LABEL_NAME = LabelName;
                    row.CHINESE = Chinese;
                    row.CHINESE_TW = ChineseWT;
                    row.ENGLISH = English;
                    row.EDIT_EMP = LoginUser.EMP_NO;
                    row.SYSTEM_NAME = SystemName;
                    row.EDIT_TIME = GetDBDateTime();
                    InsertSql = row.GetInsertString(DBTYPE);
                    sfcdb.ExecSQL(InsertSql);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    this.DBPools["SFCDB"].Return(sfcdb);
                    //    StationReturn.MessagePara.Add("46545645674");
                }
                else
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }
        /// <summary>
        /// 刪除標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeletePageLanguage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_C_LANGUAGE_PAGE language;
         //   string[] ID = Newtonsoft.Json.Linq.JArray(Data["ID"].);
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {
               
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                language = new T_C_LANGUAGE_PAGE(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    Row_C_LANGUAGE_PAGE row = (Row_C_LANGUAGE_PAGE)language.GetObjByID(StrID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode ="MES00000004";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
           
        }
        /// <summary>
        /// 更新標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdatePageLanguage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_C_LANGUAGE_PAGE language;
            string ID = Data["ID"].ToString().Trim();
            string PageName = Data["PageName"].ToString().Trim();
            string LabelName = Data["LabelName"].ToString().Trim();
            string Chinese = Data["Chinese"].ToString().Trim();
            string ChineseWT = Data["ChineseWT"].ToString().Trim();
            string English = Data["English"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                language = new T_C_LANGUAGE_PAGE(sfcdb, DBTYPE);
                Row_C_LANGUAGE_PAGE row = (Row_C_LANGUAGE_PAGE)language.GetObjByID(Data["ID"].ToString().Trim(), sfcdb);
                row.ID = ID;
                row.PAGE_NAME = PageName;
                row.LABEL_NAME = LabelName;
                row.CHINESE = Chinese;
                row.CHINESE_TW = ChineseWT;
                row.ENGLISH = English;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.SYSTEM_NAME = SystemName;
                row.EDIT_TIME = GetDBDateTime();

                UpdateSql = row.GetUpdateString(DBTYPE);
                sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
           
        }
        /// <summary>
        /// 查詢標簽語言
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void QueryPageLanguage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_LANGUAGE_PAGE language;
            List<C_LANGUAGE_PAGE> LanguageList;
            string PageName = Data["PageName"].ToString().Trim();
            string LabelName = Data["LabelName"].ToString().Trim();
            string Chinese_TW = Data["Chinese_TW"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                language = new T_C_LANGUAGE_PAGE(sfcdb, DBTYPE);
                LanguageList = language.QueryPageLanguage(PageName, LabelName, Chinese_TW, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = LanguageList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
            
        }
        /// <summary>
        ///将JSON字符串转换成对象 Transform The JSON String Into an Instance
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="str">JSON String</param>
        /// <returns></returns>
        private T ParsesJSON<T>(string str)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Excel導入多語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void ExcelImport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_LANGUAGE_PAGE language;
            string InsertSql="";
            string repeat = "";
            string Fail = "";
            string IsNull = "";
            string Messaeg = "";
            string LabelName = "";
            string ChineseTW = "";
            string Chinese = "";
            string English = "";
            string PageName = "";
            List<C_LanguagePage> LanguagePage = JsonConvert.DeserializeObject<List<C_LanguagePage>>(Data["jsond"].ToString());
            
            sfcdb = this.DBPools["SFCDB"].Borrow();
            for (int i=0;i< LanguagePage.Count;i++)
            {
                try {
                     PageName = LanguagePage[i].PageName.ToString().Trim();
                     LabelName = LanguagePage[i].labelName.ToString().Trim();
                     ChineseTW = LanguagePage[i].ChineseTW.ToString().Trim();
                     Chinese = LanguagePage[i].Chinese.ToString().Trim();
                     English   = LanguagePage[i].English.ToString().Trim();
                }
                catch(Exception) {
                    IsNull = IsNull + LabelName + ",";
                }
               
                try
                {
                    language = new T_C_LANGUAGE_PAGE(sfcdb, DBTYPE);
                    if (language.CheckDataExist(PageName, LabelName, sfcdb))
                    {
                        Row_C_LANGUAGE_PAGE row = (Row_C_LANGUAGE_PAGE)language.NewRow();
                        row.ID = language.GetNewID(BU, sfcdb);
                        row.PAGE_NAME = PageName;
                        row.LABEL_NAME = LabelName;
                        row.CHINESE = Chinese;
                        row.CHINESE_TW = ChineseTW;
                        row.ENGLISH = English;
                        row.EDIT_EMP = LoginUser.EMP_NO;
                        row.SYSTEM_NAME = SystemName;
                        row.EDIT_TIME = GetDBDateTime();
                        InsertSql = row.GetInsertString(DBTYPE);
                        sfcdb.ExecSQL(InsertSql);
                        //StationReturn.Status = StationReturnStatusValue.Pass;
                        //StationReturn.MessageCode = "MES00000002";
                        

                        //    StationReturn.MessagePara.Add("46545645674");
                    }
                    else
                    {
                        repeat = repeat  + LabelName + ",";
                        //StationReturn.Status = StationReturnStatusValue.Fail;
                        //StationReturn.MessageCode = "MES00000005";
                    }
                }
                catch (Exception)
                {
                    Fail = Fail + LabelName + ",";
                    //this.DBPools["SFCDB"].Return(sfcdb);
                    //throw e;

                }
            }
            if (repeat != "")
            {
                Messaeg = Messaeg +"Lab Name"+ repeat + "existed!  ";
            }
            if (Fail != "")
            {
                Messaeg = Messaeg + "Lab Name" + Fail + "Import failed  ";
            }
            if (IsNull != "")
            {
                Messaeg = Messaeg + "Lab Name" + Fail + "Data is empty!  ";
            }
            if (Messaeg == "")
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = Messaeg;
            }
            this.DBPools["SFCDB"].Return(sfcdb);
        }
      

    }

    public class LanguagePage
    { 
        public string pagename { get; set; }
        public string labelName { get; set; }
        public Dictionary<string, string> Txt { get; set; } = new Dictionary<string, string>();
}

    public class C_LanguagePage
    {
        public string PageName { get; set; }
        public string labelName { get; set; }
        public string Chinese { get; set; }
        public string ChineseTW { get; set; }
        public string English { get; set; }
    }
}
