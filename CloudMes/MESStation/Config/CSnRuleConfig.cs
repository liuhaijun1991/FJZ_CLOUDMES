using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class CSnRuleConfig : MesAPIBase
    {
        protected APIInfo FGetAllSnRule = new APIInfo()
        {
            FunctionName = "GetAllSnRule",
            Description = "獲取所有的SN規則",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetSkuSnRule = new APIInfo()
        {
            FunctionName = "GetSkuSnRule",
            Description = "根據機種獲取對應的SN規則",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SkuId" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetSNRuleDetailById = new APIInfo()
        {
            FunctionName = "GetSNRuleDetailById",
            Description = "根據SN Rule 的 ID 獲取對應的SN規則詳細信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleId" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddSkuSnRule = new APIInfo()
        {
            FunctionName = "AddSkuSnRule",
            Description = "更新選定機種的 SN 編碼規則",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SkuId" },
                    new APIInputInfo(){ InputName="RuleId" } 
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddRule = new APIInfo()
        {
            FunctionName = "AddRule",
            Description = "添加一條編碼規則記錄到 C_SN_RULE",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleName" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpdateRule = new APIInfo()
        {
            FunctionName = "UpdateRule",
            Description = "更新 C_SN_RULE 的一條編碼規則",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName="RuleId" }, new APIInputInfo() { InputName = "RuleName" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteRule = new APIInfo()
        {
            FunctionName = "DeleteRule",
            Description = "刪除 C_SN_RULE 的一條編碼規則",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleId" }, new APIInputInfo() { InputName = "RuleName" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetRuleById = new APIInfo()
        {
            FunctionName = "GetRuleById",
            Description = "通過 RuleId 獲取到 C_SN_RULE 的一條編碼規則",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleId" }, new APIInputInfo() { InputName = "RuleName" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddRuleDetail = new APIInfo()
        {
            FunctionName = "AddRuleDetail",
            Description = "新增一条 C_SN_RULE_DETAIL 数据",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName="RuleDetail"} },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpdateRuleDetail = new APIInfo()
        {
            FunctionName = "UpdateRuleDetail",
            Description = "通过 RuleDetail 对象更新一条 C_SN_RULE_DETAIL 数据",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleDetail" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteRuleDetailById = new APIInfo()
        {
            FunctionName= "DeleteRuleDetail",
            Description="通过 RuleDetailId 删除 C_SN_RULE_DETAIL 的一条数据",
            Parameters=new List<APIInputInfo>() { new APIInputInfo() { InputName="RuleDetailId"} },
            Permissions=new List<MESPermission>()
        };

        protected APIInfo FGetInputType = new APIInfo()
        {
            FunctionName = "GetInputType",
            Description = "獲取所有的輸入類型",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetCodeType = new APIInfo()
        {
            FunctionName = "GetCodeType",
            Description = "獲取所有的編碼類型",
            Parameters = new List<APIInputInfo>() ,
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetRuleDetailByDetailId = new APIInfo()
        {
            FunctionName= "GetRuleDetailByDetailId",
            Description="根據 DetailId 獲取 C_SN_RULE_DETAIL 數據",
            Parameters=new List<APIInputInfo>() { new APIInputInfo() { InputName="RuleDetailID"} },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddRuleByCopySku = new APIInfo()
        {
            FunctionName = "AddRuleByCopySku",
            Description = "根據參考機種和前綴添加SN規則",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "CRuleName" }, new APIInputInfo() { InputName = "CPrefix" }, new APIInputInfo() { InputName = "CSkuNo" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetSelectValue = new APIInfo()
        {
            FunctionName = "GetSelectValue",
            Description = "Get Select Value",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetSNRuleDetailByIdNew = new APIInfo()
        {
            FunctionName = "GetSNRuleDetailByIdNew",
            Description = "根據SN Rule 的 ID 獲取對應的SN規則詳細信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleId" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllCodeType = new APIInfo()
        {
            FunctionName = "GetAllCodeType",
            Description = "GetAllCodeType",
            Parameters = new List<APIInputInfo>() ,
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllSNRuleAndSkuno = new APIInfo()
        {
            FunctionName = "GetAllSNRuleAndSkuno",
            Description = "Get All SN Rule And Skuno",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FSaveSNRuleDetail = new APIInfo()
        {
            FunctionName = "SaveSNRuleDetail",
            Description = "Save SN Rule Detail",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SnRuleDetail" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetSNRuleIdByName = new APIInfo() 
        {
            FunctionName = "GetSNRuleIdByName",
            Description = "GetSNRuleIdByName",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RuleName" } },
            Permissions = new List<MESPermission>()
        };



        public CSnRuleConfig()
        {
            this.Apis.Add(FGetAllSnRule.FunctionName, FGetAllSnRule);
            this.Apis.Add(FGetSkuSnRule.FunctionName, FGetSkuSnRule);
            this.Apis.Add(FGetSNRuleDetailById.FunctionName, FGetSNRuleDetailById);
            this.Apis.Add(FAddSkuSnRule.FunctionName, FAddSkuSnRule);
            this.Apis.Add(FAddRule.FunctionName, FAddRule);
            this.Apis.Add(FUpdateRule.FunctionName, FUpdateRule);
            this.Apis.Add(FDeleteRule.FunctionName, FDeleteRule);
            this.Apis.Add(FGetRuleById.FunctionName, FGetRuleById);
            this.Apis.Add(FAddRuleDetail.FunctionName, FAddRuleDetail);
            this.Apis.Add(FUpdateRuleDetail.FunctionName, FUpdateRuleDetail);
            this.Apis.Add(FDeleteRuleDetailById.FunctionName, FDeleteRuleDetailById);
            this.Apis.Add(FGetRuleDetailByDetailId.FunctionName, FGetRuleDetailByDetailId);
            this.Apis.Add(FAddRuleByCopySku.FunctionName, FAddRuleByCopySku);
            this.Apis.Add(FGetSelectValue.FunctionName, FGetSelectValue);
            this.Apis.Add(FGetSNRuleDetailByIdNew.FunctionName, FGetSNRuleDetailByIdNew);
            this.Apis.Add(FGetAllCodeType.FunctionName, FGetAllCodeType);
            this.Apis.Add(FGetAllSNRuleAndSkuno.FunctionName, FGetAllSNRuleAndSkuno);
            this.Apis.Add(FGetSNRuleIdByName.FunctionName, FGetSNRuleIdByName);
        }

        public void GetAllSnRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SN_RULE TCSR= new T_C_SN_RULE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_SN_RULE> SRs = new List<C_SN_RULE>();
                SRs = TCSR.GetAllData(sfcdb);
                if (SRs != null)
                {
                    StationReturn.Data = SRs;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetSkuSnRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SN_RULE TCSR = new T_C_SN_RULE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_SN_RULE SR = new C_SN_RULE();
                string SkuId = Data["SkuId"].ToString();
                SR = TCSR.GetSnRuleBySku(SkuId,sfcdb);
                if (SR != null)
                {
                    StationReturn.Data = SR;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara = new List<object>() { SkuId };
                }

                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetSNRuleDetailById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_SN_RULE_DETAIL> SRDs = new List<C_SN_RULE_DETAIL>();
                string RuleId = Data["RuleId"].ToString();
                SRDs = TCSRD.GetDataByRuleID(RuleId, sfcdb);
                if (SRDs.Count>0)
                {
                    StationReturn.Data = SRDs;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara = new List<object>() {SRDs.Count};
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void AddSkuSnRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE TCSR = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCSR = new T_C_SN_RULE(sfcdb, DB_TYPE_ENUM.Oracle);
                string SkuId = Data["SkuId"].ToString();
                string RuleId= Data["RuleId"].ToString();
                int result = TCSR.UpdateSkuSnRule(SkuId, RuleId, sfcdb);
                if (result > 0)
                {
                    StationReturn.Data = SkuId;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void AddRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE TCSR = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCSR = new T_C_SN_RULE(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleName = Data["RuleName"].ToString();
                if(sfcdb.ORM.Queryable<C_SN_RULE>().Any(r=>r.NAME==RuleName))
                {
                    throw new Exception($@"{RuleName} already exist.");
                }
                int result = TCSR.AddARule(BU,LoginUser.EMP_NO, RuleName, sfcdb);

                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                StationReturn.Data = result;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void UpdateRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE TCSR = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCSR = new T_C_SN_RULE(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleId = Data["RuleId"].ToString();
                string RuleName = Data["RuleName"].ToString();
                int result = TCSR.UpdateARule( LoginUser.EMP_NO,RuleId, RuleName, sfcdb);

                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }

                StationReturn.Data = result;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void DeleteRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE TCSR = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCSR = new T_C_SN_RULE(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleId = Data["RuleId"].ToString();
                int result = TCSR.DeleteRule(RuleId,sfcdb);

                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }

                StationReturn.Data = result;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetRuleById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE TCSR = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCSR = new T_C_SN_RULE(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleId = Data["RuleId"].ToString();
                C_SN_RULE rule= TCSR.GetRuleById(RuleId, sfcdb);
                if (rule!=null)
                {
                    StationReturn.Data = rule;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";

                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void AddRuleDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE_DETAIL TRuleDetail = null;

            try
            {
                sfcdb= this.DBPools["SFCDB"].Borrow();
                TRuleDetail = new T_C_SN_RULE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleDetail = Data["RuleDetail"].ToString();
                C_SN_RULE_DETAIL rd = Newtonsoft.Json.JsonConvert.DeserializeObject<C_SN_RULE_DETAIL>(RuleDetail);
                rd.EDIT_EMP = LoginUser.EMP_NO;
                int result = TRuleDetail.AddRuleDetail(rd, BU,LoginUser.EMP_NO, sfcdb);
                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                StationReturn.Data = result;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void UpdateRuleDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE_DETAIL TRuleDetail = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TRuleDetail = new T_C_SN_RULE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleDetail = Data["RuleDetail"].ToString();
                C_SN_RULE_DETAIL rd = Newtonsoft.Json.JsonConvert.DeserializeObject<C_SN_RULE_DETAIL>(RuleDetail);
                int result = TRuleDetail.UpdateRuleDetail(rd,LoginUser.EMP_NO, sfcdb);
                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                StationReturn.Data = result;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void DeleteRuleDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE_DETAIL TRuleDetail = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TRuleDetail = new T_C_SN_RULE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleDetailId = Data["RuleDetailId"].ToString();
                int result = TRuleDetail.DeleteRuleDetail(RuleDetailId,LoginUser.EMP_NO, sfcdb);
                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                StationReturn.Data = result;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetInputType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE_DETAIL RuleDetail = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RuleDetail = new T_C_SN_RULE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                List<string> InputTypes = RuleDetail.GetAllInputType(sfcdb);
                if (InputTypes.Count > 0)
                {
                    StationReturn.Data = InputTypes;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara = new List<object>() { InputTypes.Count };
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetCodeType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE_DETAIL RuleDetail = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RuleDetail = new T_C_SN_RULE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                List<string> CodeTypes = RuleDetail.GetAllCodeType(sfcdb);
                if (CodeTypes.Count > 0)
                {
                    StationReturn.Data = CodeTypes;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara = new List<object>() { CodeTypes.Count };
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void GetRuleDetailByDetailId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                string RuleDetailId = Data["RuleDetailId"].ToString();
                C_SN_RULE_DETAIL SRD = TCSRD.GetRuleDetailByDetailId(RuleDetailId, sfcdb);
                if (SRD!=null)
                {
                    StationReturn.Data = SRD;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara = new List<object>() {0 };
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void AddRuleByCopySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SN_RULE TCSR = null;
            T_C_SN_RULE_DETAIL TCSRD = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCSR = new T_C_SN_RULE(sfcdb, DB_TYPE_ENUM.Oracle);
                TCSRD = new T_C_SN_RULE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                string RuleName = Data["CRuleName"].ToString();
                string Prefix = Data["CPrefix"].ToString();
                string SkuNo = Data["CSkuNo"].ToString();

                #region 檢查檢查
                //檢查輸入的RuleName是否存在
                var _RULE = TCSR.GetDataByName(RuleName, sfcdb);
                if (_RULE != null)
                {
                    throw new Exception($@"RuleName:{RuleName} is already exits!");
                }
                //檢查輸入的前綴是否存在
                var _DETAIL = sfcdb.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.CURVALUE == Prefix).First();
                if (_DETAIL != null)
                {
                    throw new Exception($@"Prefix:{Prefix} has been config on Rule:{_DETAIL.C_SN_RULE_ID}!");
                }
                //檢查機種是否已配置SN規則
                var _SKU = sfcdb.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SkuNo).First();
                if (_SKU == null)
                {
                    throw new Exception($@"SkuNo:{SkuNo} is not exits!");
                }
                if (string.IsNullOrEmpty(_SKU.SN_RULE))
                {
                    throw new Exception($@"SkuNo:{SkuNo} has not config sn rule![C_SKU.SN_RULE]");
                }
                var _SNRules = sfcdb.ORM.Queryable<C_SN_RULE, C_SN_RULE_DETAIL>((t1, t2) => t1.ID == t2.C_SN_RULE_ID)
                    .Where((t1, t2) => t1.NAME == _SKU.SN_RULE).OrderBy((t1, t2) => t2.SEQ).Select((t1, t2) => t2).ToList();
                if (_SNRules == null)
                {
                    throw new Exception($@"SkuNo:{SkuNo} has not config sn rule![C_SN_RULE or DETAIL]");
                }
                #endregion
                
                //int result = TCSR.AddARule(BU, LoginUser.EMP_NO, RuleName, sfcdb);
                C_SN_RULE rule = new C_SN_RULE();
                rule.ID = RuleName;
                rule.NAME = RuleName;
                rule.EDIT_TIME = DateTime.Now;
                rule.EDIT_EMP = LoginUser.EMP_NO;
                int result = sfcdb.ORM.Insertable<C_SN_RULE>(rule).ExecuteCommand();
                if (result <= 0)
                {
                    throw new Exception($@"Add sn rule :{RuleName} fail!");
                }
                //寫入C_SN_RULE成功後立馬加載對象，用來取ID
                _RULE = TCSR.GetDataByName(RuleName, sfcdb);
                //先把前綴改了，一般都是第一行，如果不是，那是奇葩
                _SNRules[0].CURVALUE = Prefix;  
                for (int i = 0; i < _SNRules.Count; i++)
                {
                    _SNRules[i].C_SN_RULE_ID = _RULE.ID;
                    _SNRules[i].EDIT_EMP = LoginUser.EMP_NO;
                    
                    result = TCSRD.AddRuleDetailByCopy(_SNRules[i], BU, sfcdb);
                    if (result <= 0)
                    {
                        throw new Exception($@"Add sn rule detail :{RuleName} fail!");
                    }
                }

                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000035";
                StationReturn.MessagePara = new List<object> { result };
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    
        public void GetSelectValue(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                List<string> codeType = new List<string>() { "PREFIX" };
                codeType.AddRange(SFCDB.ORM.Queryable<C_CODE_MAPPING>().Select(r => r.CODETYPE).Distinct().ToList());

                List<string> inputType = new List<string>() { "PREFIX", "YYYY", "MM", "DD", "WW", "SN" };
                List<object> resetSnFlag = new List<object>() {
                    new { value="0", text= "NO"},
                    new { value="1", text= "YES"}
                };
                StationReturn.Data = new { InputType = inputType, CodeType = codeType, ResetSnFlag = resetSnFlag };
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
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void GetSNRuleDetailByIdNew(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ruleId = Data["RuleId"] == null ? "" : Data["RuleId"].ToString();
                var list = SFCDB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(c => c.C_SN_RULE_ID == ruleId)
                    .OrderBy( c => c.SEQ)
                    .Select(c => new
                    {
                        ID = c.ID,
                        C_SN_RULE_ID = c.C_SN_RULE_ID,
                        SEQ = c.SEQ,
                        INPUTTYPE = c.INPUTTYPE,
                        CODETYPE = c.CODETYPE,
                        CURVALUE = c.CURVALUE,
                        RESETSN_FLAG = c.RESETSN_FLAG,// SqlSugar.SqlFunc.IF(c.RESETSN_FLAG==1).Return("YES").End("NO"),
                        RESETVALUE = c.RESETVALUE,
                        CHECK_FLAG = c.CHECK_FLAG,
                        EDIT_TIME = c.EDIT_TIME,
                        EDIT_EMP = c.EDIT_EMP,
                        VALUE10 = c.VALUE10
                    }).ToList();


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
    
        public void GetAllCodeType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string codeType = Data["CodeType"] == null ? "" : Data["CodeType"].ToString();
                List<C_CODE_MAPPING> list = new List<C_CODE_MAPPING>();
                if (codeType.ToUpper().Equals("ALL") || string.IsNullOrWhiteSpace(codeType))
                {
                    list = SFCDB.ORM.Queryable<C_CODE_MAPPING>().OrderBy(c => c.CODETYPE).OrderBy(c => c.SEQ).ToList();
                }
                else
                {
                    list = SFCDB.ORM.Queryable<C_CODE_MAPPING>().Where(c => c.CODETYPE == codeType).OrderBy(c => c.SEQ).ToList();
                }
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
    
        public void GetAllSNRuleAndSkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                
                StationReturn.Data = SFCDB.ORM.Ado.GetDataTable(
                    $@"select distinct r.id,r.name,r.curvalue,r.edit_time,r.edit_emp, to_char(wm_concat (s.skuno)) as skuno from c_sn_rule r left join c_sku s on r.id=s.sn_rule
                        group by r.id,r.name,r.curvalue,r.edit_time,r.edit_emp order by r.edit_time desc");
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
        public void SaveSNRuleDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                //List<string> list1 = new List<string>();
                //T_R_REPLACE_SN ttt = new T_R_REPLACE_SN(SFCDB, DB_TYPE_ENUM.Oracle);
                //ttt.GetOldSnList(SFCDB, list1, "2102130992218C0G003C");                
                //var nowSN = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN("teste", SFCDB);
                var snRuleDetail = Data["SnRuleDetail"];
                List<C_SN_RULE_DETAIL> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<C_SN_RULE_DETAIL>>(snRuleDetail.ToString());

                //检查PREFIX和seq一致且规则长度一样的则做弹框提示
                List<C_SN_RULE_DETAIL> prefixList = list.FindAll(r => r.INPUTTYPE == "PREFIX").OrderBy(r => r.SEQ).ToList();
                List<C_SN_RULE_DETAIL> otherPrefixList = SFCDB.ORM.Queryable<C_SN_RULE_DETAIL>()
                    .Where(r => r.INPUTTYPE == "PREFIX" && r.C_SN_RULE_ID!= prefixList.FirstOrDefault().C_SN_RULE_ID)                    
                    .ToList();
                bool showCheck = false;
                List<string> ruleIdList = otherPrefixList.Select(r => r.C_SN_RULE_ID).ToList();
                foreach (var ruleId in ruleIdList)
                {
                    var tempPrefixList = otherPrefixList.FindAll(r => r.C_SN_RULE_ID == ruleId).OrderBy(r => r.SEQ).ToList();
                    if (tempPrefixList.Count == prefixList.Count)
                    {
                        int sameCount = 0;
                        for (int i = 0; i < tempPrefixList.Count; i++)
                        {
                            if(tempPrefixList[i].CURVALUE==prefixList[i].CURVALUE)
                            {
                                sameCount++;
                            }
                        }
                        if(sameCount==prefixList.Count)
                        {
                            showCheck = true;
                            break;
                            
                        }
                    }
                }                
                if (showCheck)
                {
                    UIInputData O = new UIInputData()
                    {
                        Timeout = 100000,
                        UIArea = new string[] { "50%", "40%" },
                        IconType = IconType.None,
                        Message = "Continue",
                        Tittle = "Tip",
                        Type = UIInputType.Confirm,
                        Name = "",
                        ErrMessage = "Please check.",
                    };
                    
                    O.OutInputs.Add(new DisplayOutPut() { Name = "Tip", DisplayType = UIOutputType.TextArea.ToString(), Value = "The same[PREFIX] rules already exist.Do you want to continue?" });
                    var inputObj = O.GetUiInput(this, UIInput.Normal);
                }

                T_C_SN_RULE_DETAIL t_c_sn_rule_detail = new T_C_SN_RULE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                DateTime sysdate = SFCDB.ORM.GetDate();
                foreach (var item in list)
                {
                    if (item.SEQ == null)
                    {
                        throw new Exception("SEQ is null");
                    }
                    if(string.IsNullOrWhiteSpace(item.INPUTTYPE))
                    {
                        throw new Exception("INPUTTYPE is null");
                    }
                    if (string.IsNullOrWhiteSpace(item.CODETYPE))
                    {
                        throw new Exception("CODETYPE is null");
                    }
                    if (string.IsNullOrWhiteSpace(item.CURVALUE))
                    {
                        throw new Exception("CURVALUE is null");
                    }
                    if (item.RESETSN_FLAG == null)
                    {
                        throw new Exception("RESETSN_FLAG is null");
                    }
                    if(item.RESETSN_FLAG==1)
                    {
                        var resetItem = list.Find(r => r.INPUTTYPE == "SN");
                        if(resetItem!=null&&string.IsNullOrWhiteSpace(resetItem.RESETVALUE))
                        {
                            throw new Exception("RESETVALUE is null");
                        }                        
                    }
                    item.EDIT_TIME = sysdate;
                    item.EDIT_EMP = LoginUser.EMP_NO;
                    int result = t_c_sn_rule_detail.SaveSnRuleDetail(item, BU, LoginUser.EMP_NO, SFCDB);
                    if (result <= 0)
                    {
                        throw new Exception("Save fail.");
                    }
                }
                SFCDB.CommitTrain();

                string complicateId = t_c_sn_rule_detail.GetComplicateRuleId(list.FirstOrDefault(), SFCDB);
                if (complicateId.Length > 0)
                {                   
                    OleExec deleteDB = this.DBPools["SFCDB"].Borrow(); 
                    C_SN_RULE Rule = deleteDB.ORM.Queryable<C_SN_RULE>().Where(t => t.ID == complicateId).ToList().FirstOrDefault();
                    deleteDB.ORM.Deleteable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == list.FirstOrDefault().C_SN_RULE_ID).ExecuteCommand();
                    deleteDB.ORM.Updateable<C_SN_RULE>().SetColumns(r => new C_SN_RULE { CURVALUE = "" }).Where(r => r.ID == list.FirstOrDefault().C_SN_RULE_ID).ExecuteCommand();
                    this.DBPools["SFCDB"].Return(deleteDB);
                    //throw new Exception(string.Format("存在相同的編碼規則，規則名為 {0},您之前添加的規則細項都已經被刪除！", Rule.NAME));
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816100333", new string[] { Rule.NAME }));
                }                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
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

        public void GetSNRuleIdByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ruleName = Data["RuleName"] == null ? "" : Data["RuleName"].ToString();
                var ruleObj = SFCDB.ORM.Queryable<C_SN_RULE>().Where(c => c.NAME == ruleName).OrderBy(c=>c.EDIT_TIME,SqlSugar.OrderByType.Desc)
                    .ToList().FirstOrDefault();
                if (ruleObj == null)
                    throw new Exception($@"{ruleName} not exist.");
                StationReturn.Data = ruleObj.ID;
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
    }
}
