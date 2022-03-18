using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.GlobalConfig
{
    public class InputRuleConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addinputrule = new APIInfo()
        {
            FunctionName = "AddInputRule",
            Description = "添加文本框輸入規則",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="InputName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Expression",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo queryinputrule = new APIInfo()
        {
            FunctionName = "QueryInputRule",
            Description = "查詢文本框輸入規則",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                  new APIInputInfo() { InputName="InputName",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo getinputrulelist = new APIInfo()
        {
            FunctionName = "GetInputRuleList",
            Description = "獲取文本框輸入規則",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="PageName",InputType="string",DefaultValue=""},
                  new APIInputInfo() { InputName="InputName",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo deleteinputrule = new APIInfo()
        {
            FunctionName = "DeleteInputRuleList",
            Description = "獲取文本框輸入規則",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        public InputRuleConfig()
        {
            _MastLogin = false;
            this.Apis.Add(addinputrule.FunctionName, addinputrule);
            this.Apis.Add(queryinputrule.FunctionName, queryinputrule);
            this.Apis.Add(getinputrulelist.FunctionName, getinputrulelist);
            this.Apis.Add(deleteinputrule.FunctionName, deleteinputrule);

        }

        /// <summary>
        /// 添加標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddInputRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_C_INPUT_RULE inputrule;
            string PageName = Data["PageName"].ToString().Trim();
            string InputName = Data["InputName"].ToString().Trim();
            string Expression = Data["Expression"].ToString().Trim();
            string Description = Data["Desc"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputrule = new T_C_INPUT_RULE(sfcdb, DBTYPE);
                if (inputrule.CheckDataExist(PageName, InputName, sfcdb))
                {
                    Row_C_INPUT_RULE row = (Row_C_INPUT_RULE)inputrule.NewRow();
                    row.ID = inputrule.GetNewID(BU, sfcdb);
                    row.PAGE_NAME = PageName;
                    row.INPUT_NAME = InputName;
                    row.EXPRESSION = Expression;
                    row.DESCRIPTION = Description;
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

        public void QueryInputRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_INPUT_RULE> RuleList;
            T_C_INPUT_RULE inputrule;
            string PageName = Data["PageName"].ToString().Trim();
            string InputName = Data["InputName"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputrule = new T_C_INPUT_RULE(sfcdb, DBTYPE);
                RuleList = inputrule.QueryInputRule(PageName, InputName, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = RuleList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
           // Dictionary<String, String> LanguageList = new Dictionary<String, String>();

        }

        public void GetInputRuleList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            Dictionary<String, String> LanguageList = new Dictionary<String, String>();
            T_C_INPUT_RULE inputrule;
            string PageName = Data["PageName"].ToString().Trim();
            string InputName = Data["InputName"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputrule = new T_C_INPUT_RULE(sfcdb, DBTYPE);
                LanguageList = inputrule.GetInputRuleList(PageName, InputName, sfcdb);
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
            // Dictionary<String, String> LanguageList = new Dictionary<String, String>();

        }

        public void DeleteInputRuleList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_C_INPUT_RULE inputrule;
            string[] ID = Data["ID"].ToString().Split(',');
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputrule = new T_C_INPUT_RULE(sfcdb, DBTYPE);
                sfcdb.BeginTrain();
                for (int i = 0; i < ID.Length; i++)
                {
                    StrID = ID[i].ToString();
                    Row_C_INPUT_RULE row = (Row_C_INPUT_RULE)inputrule.GetObjByID(StrID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
    }

}


