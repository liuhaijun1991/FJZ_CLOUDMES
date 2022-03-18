using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESPubLab;

namespace MESStation.Config
{
    public class SkuKeyPartConfig : MesAPIBase
    {

        protected APIInfo FQueryMpnBySku = new APIInfo()
        {
            FunctionName = "QueryMpnBySku",
            Description = "Query Mpn By Sku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "AddReplaceKpWithSku",
            Description = "AddReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REPLACEPARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };

        protected APIInfo FUpdateReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "UpdateReplaceKpWithSku",
            Description = "UpdateReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REPLACEPARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>(){}
        };

        protected APIInfo FQueryReplaceKpBySku = new APIInfo()
        {
            FunctionName = "QueryReplaceKpBySku",
            Description = "QueryReplaceKpBySku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddMpnWithSku = new APIInfo()
        {
            FunctionName = "AddMpnWithSku",
            Description = "Query Mpn With Sku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "MFRCODE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteMpnWithSku = new APIInfo()
        {
            FunctionName = "DeleteMpnWithSku",
            Description = "DeleteMpnWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CSKUMPNIDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateMpnBySku = new APIInfo()
        {
            FunctionName = "UpdateMpnWithSku",
            Description = "Update Mpn With Sku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MFRCODE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSetWoReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "SetWoReplaceKpWithSku",
            Description = "SetWoReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetWoReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "GetWoReplaceKpWithSku",
            Description = "GetWoReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "REPLACEPARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteWoReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "DeleteWoReplaceKpWithSku",
            Description = "DeleteWoReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RWOKPREPLACEIDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddCKpCheck = new APIInfo()
        {
            FunctionName = "AddCKpCheck",
            Description = "AddCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DLL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FUNCTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FQueryCKpCheck = new APIInfo()
        {
            FunctionName = "QueryCKpCheck",
            Description = "QueryCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPENAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateCKpCheck = new APIInfo()
        {
            FunctionName = "UpdateCKpCheck",
            Description = "UpdateCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DLL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FUNCTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCKpCheck = new APIInfo()
        {
            FunctionName = "DeleteCKpCheck",
            Description = "DeleteCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FQueryCKpRule = new APIInfo()
        {
            FunctionName = "QueryCKpRule",
            Description = "QueryCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddCKpRule = new APIInfo()
        {
            FunctionName = "AddCKpRule",
            Description = "AddCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SCANTYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REGEX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FModifyCKpRule = new APIInfo()
        {
            FunctionName = "ModifyCKpRule",
            Description = "ModifyCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REGEX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCKpRule = new APIInfo()
        {
            FunctionName = "DeleteCKpRule",
            Description = "DeleteCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetWOMPNList = new APIInfo()
        {
            FunctionName = "GetWOMPNList",
            Description = "Get WO MPN List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEditWOMPN = new APIInfo()
        {
            FunctionName = "EditWOMPN",
            Description = "Edit WO MPN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },                
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MFRCODE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteWOMPN = new APIInfo()
        {
            FunctionName = "DeleteWOMPN",
            Description = "Delete WO MPN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public SkuKeyPartConfig()
        {
            this.Apis.Add(FQueryMpnBySku.FunctionName, FQueryMpnBySku);
            this.Apis.Add(FAddMpnWithSku.FunctionName, FAddMpnWithSku);
            this.Apis.Add(FDeleteMpnWithSku.FunctionName, FDeleteMpnWithSku);
            this.Apis.Add(FUpdateMpnBySku.FunctionName, FUpdateMpnBySku);
            this.Apis.Add(FQueryReplaceKpBySku.FunctionName, FQueryReplaceKpBySku);
            this.Apis.Add(FAddReplaceKpWithSku.FunctionName, FAddReplaceKpWithSku);
            this.Apis.Add(FSetWoReplaceKpWithSku.FunctionName, FSetWoReplaceKpWithSku);
            this.Apis.Add(FGetWoReplaceKpWithSku.FunctionName, FGetWoReplaceKpWithSku);
            this.Apis.Add(FAddCKpCheck.FunctionName, FAddCKpCheck);
            this.Apis.Add(FQueryCKpCheck.FunctionName, FQueryCKpCheck);
            this.Apis.Add(FUpdateCKpCheck.FunctionName, FUpdateCKpCheck);

            this.Apis.Add(FQueryCKpRule.FunctionName, FQueryCKpRule);
            this.Apis.Add(FAddCKpRule.FunctionName, FAddCKpRule);
            this.Apis.Add(FModifyCKpRule.FunctionName, FModifyCKpRule);
            this.Apis.Add(FDeleteCKpRule.FunctionName, FDeleteCKpRule);
            this.Apis.Add(FGetWOMPNList.FunctionName, FGetWOMPNList);
            this.Apis.Add(FEditWOMPN.FunctionName, FEditWOMPN);
            this.Apis.Add(FDeleteWOMPN.FunctionName, FDeleteWOMPN);
            
        }

        public void QueryMpnBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            List<C_SKU_MPN> cSkuMpnList = new List<C_SKU_MPN>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                cSkuMpnList = cSkuMpn.GetMpnBySku(oleDB, Sku);
                if(cSkuMpnList.Count>0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cSkuMpnList.Count);
                    StationReturn.Data = cSkuMpnList;
                }else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryMpnruleByListNane(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Listname = Data["ListName"].ToString().Trim();
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var MpnRule = oleDB.ORM.Queryable<C_KP_LIST, C_KP_List_Item, C_KP_List_Item_Detail, C_SKU_MPN, C_KP_Rule>
                    ((KL, KIT, KITD, MPN, RL) =>new object[] { 
                        SqlSugar.JoinType.Left,(KL.ID==KIT.LIST_ID),
                        SqlSugar.JoinType.Left,(KIT.ID==KITD.ITEM_ID),
                        SqlSugar.JoinType.Left,(KIT.KP_PARTNO==MPN.PARTNO && KL.SKUNO==MPN.SKUNO),
                        SqlSugar.JoinType.Left,(MPN.PARTNO==RL.PARTNO && MPN.MPN==RL.MPN && KITD.SCANTYPE==RL.SCANTYPE)
                    })
                    .Where((KL, KIT, KITD, MPN, RL) => KL.LISTNAME == Listname)
                    .OrderBy((KL, KIT, KITD, MPN, RL) =>KIT.SEQ,SqlSugar.OrderByType.Asc)
                    .OrderBy((KL, KIT, KITD, MPN, RL) => KITD.SEQ, SqlSugar.OrderByType.Asc)
                    .Select((KL, KIT, KITD, MPN, RL) => new {
                        SEQ = KIT.SEQ,
                        SCANSEQ = KITD.SEQ,
                        SKUNO = KL.SKUNO,
                        LISTNAME = KL.LISTNAME,
                        PARTNO = MPN.PARTNO,
                        MPN = MPN.MPN,
                        STATION = KIT.STATION,
                        MFRCODE = MPN.MFRCODE,
                        SCANTYPE = KITD.SCANTYPE,
                        REGEX = RL.REGEX
                    }).ToList();

                if (MpnRule.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(MpnRule.Count);
                    StationReturn.Data = MpnRule;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryReplaceKpBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Replace cKpReplace = null;
            List<C_KP_Replace> cKpReplaceList = new List<C_KP_Replace>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpReplace = new T_C_KP_Replace(oleDB, DBTYPE);
                cKpReplaceList = cKpReplace.GetReplaceKpBySku(oleDB, Sku);
                if (cKpReplaceList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cKpReplaceList.Count);
                    StationReturn.Data = cKpReplaceList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddMpnWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //2019/04/09 patty modified to show MFRCODE
            string Sku = Data["SKUNO"].ToString().Trim(), PartNo = Data["PARTNO"].ToString().Trim(), Mpn = Data["MPN"].ToString().Trim(), Mfrcode = Data["MFRCODE"].ToString().Trim();
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            T_C_SKU cSku = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                cSku = new T_C_SKU(oleDB, DBTYPE);

                
                //if (!cSku.SkuNoIsExist(Sku,oleDB))
                //{
                //    StationReturn.Status = StationReturnStatusValue.Fail;
                //    StationReturn.MessageCode = "MES00000245";
                //    StationReturn.MessagePara = new List<object>() { Sku} ;
                //}
                //else 
                if (cSkuMpn.IsExists(oleDB,Sku, PartNo, Mpn, Mfrcode))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.NewRow();
                    rowCSkuMpn.ID = cSkuMpn.GetNewID(this.BU, oleDB, DBTYPE);
                    rowCSkuMpn.SKUNO = Sku;
                    rowCSkuMpn.PARTNO = PartNo;
                    rowCSkuMpn.MPN = Mpn;
                    rowCSkuMpn.MFRCODE = Mfrcode; //2019/04/09 patty modified to show MFRCODE
                    rowCSkuMpn.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCSkuMpn.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCSkuMpn.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetMPNRuleDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //2019/04/09 patty modified to show MFRCODE
            string Sku = Data["SKUNO"].ToString().Trim();
            string Partno = Data["PARTNO"].ToString().Trim();
            string MPN = Data["MPN"].ToString().Trim();
            string Scantype = Data["SCANTYPE"].ToString().Trim();
            string REGEX = Data["REGEX"].ToString().Trim();
            string Listname = Data["LISTNAME"].ToString().Trim();
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();

                var MpnRule = oleDB.ORM.Queryable<C_KP_LIST, C_SKU_MPN, C_KP_Rule>((KL , CSM, CKP) => KL.SKUNO==CSM.SKUNO && CSM.PARTNO == CKP.PARTNO && CSM.MPN == CKP.MPN)
                    .Where((KL , CSM, CKP) => KL.LISTNAME== Listname && CSM.PARTNO == Partno && CSM.MPN == MPN && CKP.SCANTYPE == Scantype )
                    .Select((KL,CSM, CKP) => new
                    {
                        MPNID = CSM.ID,
                        RLID = CKP.ID,
                        KL.LISTNAME,
                        SKUNO = CSM.SKUNO,
                        PARTNO = CSM.PARTNO,
                        MPN = CSM.MPN,
                        MFRCODE = CSM.MFRCODE,
                        SCANTYPE = CKP.SCANTYPE,
                        REGEX = CKP.REGEX
                    })
                    .ToList();

                if (MpnRule.Count()>0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(MpnRule.Count());
                    StationReturn.Data = MpnRule;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim(), PartNo = Data["PARTNO"].ToString().Trim(), ReplacePartno = Data["REPLACEPARTNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Replace cKpReplace = null;
            T_C_SKU cSku = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpReplace = new T_C_KP_Replace(oleDB, DBTYPE);
                cSku = new T_C_SKU(oleDB, DBTYPE);
                if (!cSku.SkuIsExist(Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000244";
                    StationReturn.MessagePara = new List<object>() { Sku };
                }
                else if (cKpReplace.IsExists(oleDB, Sku, PartNo, ReplacePartno))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_KP_Replace rowCKpReplace = (Row_C_KP_Replace)cKpReplace.NewRow();
                    rowCKpReplace.ID = cKpReplace.GetNewID(this.BU, oleDB, DBTYPE);
                    rowCKpReplace.SKUNO = Sku;
                    rowCKpReplace.PARTNO = PartNo;
                    rowCKpReplace.REPLACEPARTNO = ReplacePartno;
                    rowCKpReplace.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpReplace.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpReplace.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void SetWoReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Wo = Data["WO"].ToString().Trim(), PartNo = Data["PARTNO"].ToString().Trim(), ReplacePartno = Data["REPLACEPARTNO"].ToString().Trim(),Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_R_WO_KP_Repalce rWoKpReplace = null;
            T_C_SKU cSku = null;
            T_R_WO_BASE rWoBase = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                rWoKpReplace = new T_R_WO_KP_Repalce(oleDB, DBTYPE);
                cSku = new T_C_SKU(oleDB, DBTYPE);
                rWoBase = new T_R_WO_BASE(oleDB, DBTYPE);
                if (!cSku.SkuIsExist(Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000245";
                    StationReturn.MessagePara = new List<object>() { Sku };
                }
                else if (rWoBase.CheckDataExist(Wo,Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000245";
                    StationReturn.Data = "";
                }
                else if (rWoKpReplace.CheckDataExist(Wo, PartNo, ReplacePartno, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_R_WO_KP_Repalce rowRWoKpReplace = (Row_R_WO_KP_Repalce)rWoKpReplace.NewRow();
                    rowRWoKpReplace.ID = rWoKpReplace.GetNewID(this.BU, oleDB, DBTYPE);
                    rowRWoKpReplace.WO = Wo;
                    rowRWoKpReplace.PARTNO = PartNo;
                    rowRWoKpReplace.REPALCEPARTNO = ReplacePartno;
                    rowRWoKpReplace.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowRWoKpReplace.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowRWoKpReplace.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetWoReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string PartNo = Data["PARTNO"].ToString().Trim(), ReplacePartno = Data["REPLACEPARTNO"].ToString().Trim(), Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = oleDB = this.DBPools["SFCDB"].Borrow(); ;
            T_R_WO_KP_Repalce rWoKpReplace = null;
            //T_R_WO_BASE rWoBase = null;
            try
            {
                rWoKpReplace = new T_R_WO_KP_Repalce(oleDB, DBTYPE);
                List<R_WO_KP_Repalce> rWoKpRepalceList = rWoKpReplace.GetWoRepalceKpBySkuPartno(Sku, PartNo, ReplacePartno, oleDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Data = rWoKpRepalceList;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteMpnWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string CSKUMPNIDS = Data["CSKUMPNIDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(CSKUMPNIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(rowCSkuMpn.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateMpnWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   sku = Data["SKUNO"].ToString().Trim(),
                   partNo = Data["PARTNO"].ToString().Trim(),
                   mpn = Data["MPN"].ToString().Trim(),
                   mfrcode = Data["MFRCODE"].ToString().Trim(); //2019/04/09 patty modified to show MFRCODE
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.GetObjByID(id, oleDB, DBTYPE);
                if (cSkuMpn.IsExists(oleDB, sku, partNo, mpn,mfrcode))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else if (rowCSkuMpn!=null)
                {
                    rowCSkuMpn.SKUNO = sku;
                    rowCSkuMpn.PARTNO = partNo;
                    rowCSkuMpn.MPN = mpn;
                    rowCSkuMpn.MFRCODE = mfrcode; //2019/04/09 patty modified to show MFRCODE
                    rowCSkuMpn.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCSkuMpn.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCSkuMpn.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateMpnRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string mpnid= Data["MPNID"].ToString().Trim(),
                   rlid= Data["RLID"].ToString().Trim(),
                   sku = Data["SKUNO"].ToString().Trim(),
                   partNo = Data["PARTNO"].ToString().Trim(),
                   mpn = Data["MPN"].ToString().Trim(),
                   mfrcode = Data["MFRCODE"].ToString().Trim(), //2019/04/09 patty modified to show MFRCODE
                   scantype = Data["SCANTYPE"].ToString().Trim(),
                   regex = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
           
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.GetObjByID(mpnid, oleDB, DBTYPE);
                bool IsChangMpn = oleDB.ORM.Queryable<C_SKU_MPN>().Where(c => c.SKUNO == sku && c.PARTNO == partNo && c.MPN == mpn && c.MFRCODE==mfrcode).ToList().Any();
                bool IsChangRule = oleDB.ORM.Queryable<C_KP_Rule>().Where(c => c.PARTNO == partNo && c.MPN == mpn && c.SCANTYPE == scantype && c.REGEX == regex).ToList().Any();
                if (!IsChangMpn) 
                {
                    
                    WriteLog.WriteIntoMESLog(oleDB, BU, "CloudMES", "MESStation.Config.SkuKeyPartConfig", "SkuKeyPartConfig", "OldPARTNO: " + rowCSkuMpn.PARTNO + ",MPN: " + rowCSkuMpn.MPN + ",MFRCODE :" + rowCSkuMpn.MFRCODE + ";"+" NewpartNo: " + partNo + ", MPN: " + mpn + ", MFRCODE: " + mfrcode, "UpdateKeyPart", "C_SKU_MPN.ID: " + rowCSkuMpn.ID , "", "", this.LoginUser.EMP_NO, "N");
                    oleDB.ORM.Updateable<C_SKU_MPN>().SetColumns(t => new C_SKU_MPN
                    {
                        PARTNO = partNo,
                        MPN = mpn,
                        MFRCODE = mfrcode,
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now
                    }).Where(t=>t.ID==mpnid).ExecuteCommand();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                }
                if (!IsChangRule)
                {
                    oleDB.ORM.Updateable<C_KP_Rule>().SetColumns(t => new C_KP_Rule
                    {
                        PARTNO = partNo,
                        MPN = mpn,
                        REGEX = regex
                    }).Where(t => t.ID == rlid).ExecuteCommand();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   sku = Data["SKUNO"].ToString().Trim(),
                   partNo = Data["PARTNO"].ToString().Trim(),
                   replacePartno = Data["REPLACEPARTNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Replace cKpReplace = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpReplace = new T_C_KP_Replace(oleDB, DBTYPE);
                Row_C_KP_Replace rowCKpReplace = (Row_C_KP_Replace)cKpReplace.GetObjByID(id, oleDB, DBTYPE);
                if (cKpReplace.IsExists(oleDB, sku, partNo, replacePartno))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else if (rowCKpReplace != null)
                {
                    rowCKpReplace.SKUNO = sku;
                    rowCKpReplace.PARTNO = partNo;
                    rowCKpReplace.REPLACEPARTNO = replacePartno;
                    rowCKpReplace.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpReplace.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpReplace.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteWoReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["RWOKPREPLACEIDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_R_WO_KP_Repalce rWoKpRepalce = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                rWoKpRepalce = new T_R_WO_KP_Repalce(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_R_WO_KP_Repalce rowCSkuMpn = (Row_R_WO_KP_Repalce)rWoKpRepalce.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(rowCSkuMpn.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string TypeName = Data["TYPENAME"].ToString().Trim(), Dll = Data["DLL"].ToString().Trim(), Class = Data["CLASS"].ToString().Trim(), Function = Data["FUNCTION"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                if (cKpCheck.IsExists(TypeName, Dll, Class, Function, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_KP_Check rowCKpCheck = (Row_C_KP_Check)cKpCheck.NewRow();
                    rowCKpCheck.ID = cKpCheck.GetNewID(this.BU, oleDB, DBTYPE);
                    rowCKpCheck.TYPENAME = TypeName;
                    rowCKpCheck.DLL = Dll;
                    rowCKpCheck.CLASS = Class;
                    rowCKpCheck.FUNCTION = Function;
                    rowCKpCheck.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpCheck.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpCheck.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string TypeName = Data["TYPENAME"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                List<C_KP_Check> cKpCheckList = new List<C_KP_Check>();
                cKpCheckList = cKpCheck.GetCKpCheckByType(TypeName, oleDB);
                if (cKpCheckList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cKpCheckList.Count);
                    StationReturn.Data = cKpCheckList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   Dll = Data["DLL"].ToString().Trim(),
                   Class = Data["CLASS"].ToString().Trim(),
                   Function = Data["FUNCTION"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                Row_C_KP_Check rowCKpCheck = (Row_C_KP_Check)cKpCheck.GetObjByID(id, oleDB, DBTYPE);
                if (rowCKpCheck == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else 
                {
                    rowCKpCheck.DLL = Dll;
                    rowCKpCheck.CLASS = Class;
                    rowCKpCheck.FUNCTION = Function;
                    rowCKpCheck.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpCheck.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpCheck.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["IDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_C_KP_Check row = (Row_C_KP_Check)cKpCheck.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(row.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Partno = Data["PARTNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Rule T = null;
            List<C_KP_Rule> CList = new List<C_KP_Rule>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                T = new T_C_KP_Rule(oleDB, DBTYPE);
                CList = T.GetCKpRule(oleDB, Partno);
                if (CList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(CList.Count);
                    StationReturn.Data = CList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Partno = Data["PARTNO"].ToString().Trim(), Mpn = Data["MPN"].ToString().Trim(), ScanType = Data["SCANTYPE"].ToString().Trim(), REGEX = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Rule cKpRule = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpRule = new T_C_KP_Rule(oleDB, DBTYPE);
                if (LoginUser.FACTORY == "FTX") //FTX can add different REGEX even if same scantype, pm and MPN.
                {
                    if (cKpRule.IsExistsFTX(oleDB, Partno, Mpn, ScanType, REGEX))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000005";
                        StationReturn.Data = "";
                    }
                    else
                    {
                        Row_C_KP_Rule row = (Row_C_KP_Rule)cKpRule.NewRow();
                        row.ID = cKpRule.GetNewID(this.BU, oleDB, DBTYPE);
                        row.PARTNO = Partno;
                        row.MPN = Mpn;
                        row.SCANTYPE = ScanType;
                        row.REGEX = REGEX;
                        row.EDIT_EMP = this.LoginUser.EMP_NO;
                        row.EDIT_TIME = GetDBDateTime(); ;
                        oleDB.ThrowSqlExeception = true;
                        oleDB.ExecSQL(row.GetInsertString(DBTYPE));
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000002";
                        StationReturn.Data = "";
                    }
                }
                else
                {
                    if (cKpRule.IsExists(oleDB, Partno, Mpn, ScanType))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000005";
                        StationReturn.Data = "";
                    }
                    else
                    {
                        Row_C_KP_Rule row = (Row_C_KP_Rule)cKpRule.NewRow();
                        row.ID = cKpRule.GetNewID(this.BU, oleDB, DBTYPE);
                        row.PARTNO = Partno;
                        row.MPN = Mpn;
                        row.SCANTYPE = ScanType;
                        row.REGEX = REGEX;
                        row.EDIT_EMP = this.LoginUser.EMP_NO;
                        row.EDIT_TIME = GetDBDateTime(); ;
                        oleDB.ThrowSqlExeception = true;
                        oleDB.ExecSQL(row.GetInsertString(DBTYPE));
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000002";
                        StationReturn.Data = "";
                    }
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void ModifyCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   REGEX = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Rule cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Rule(oleDB, DBTYPE);
                Row_C_KP_Rule row = (Row_C_KP_Rule)cKpCheck.GetObjByID(id, oleDB, DBTYPE);
                if (row == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    row.REGEX = REGEX;
                    row.EDIT_EMP = this.LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(row.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["IDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_KP_Rule cKpRule = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpRule = new T_C_KP_Rule(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_C_KP_Rule row = (Row_C_KP_Rule)cKpRule.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(row.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetWOMPNList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {  
            OleExec SFCDB = null;           
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string wo = Data["WO"].ToString().Trim();
                string partno = Data["PARTNO"].ToString().Trim();
                var list = SFCDB.ORM.Queryable<C_WO_MPN>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(wo), r => r.WO.Contains(wo))
                    .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(partno), r => r.PARTNO.Contains(partno)).ToList();
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
                StationReturn.Data = "";                
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void EidtWOMPNL(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"].ToString().Trim();
                string wo = Data["WO"].ToString().Trim();
                string partno = Data["PARTNO"].ToString().Trim();
                string mpn = Data["MPN"].ToString().Trim();
                string mfrcode = Data["MFRCODE"].ToString().Trim();
                int result = 0;
                var mpnObj = new C_WO_MPN()
                {
                    ID = id,
                    WO = wo,
                    PARTNO = partno,
                    MPN = mpn,
                    MFRCODE = mfrcode,
                    EDIT_EMP = LoginUser.EMP_NO,
                    EDIT_TIME = SFCDB.ORM.GetDate()
                };
                if (string.IsNullOrEmpty(mpnObj.ID))
                {
                    T_C_WO_MPN t_c_wo_mpn = new T_C_WO_MPN(SFCDB, DB_TYPE_ENUM.Oracle);
                    mpnObj.ID = t_c_wo_mpn.GetNewID(BU, SFCDB);
                    result = SFCDB.ORM.Insertable<C_WO_MPN>(mpnObj).ExecuteCommand();                    
                }
                else
                {
                    bool bExist = SFCDB.ORM.Queryable<C_WO_MPN>().Where(r => r.WO == mpnObj.WO && r.PARTNO == mpnObj.PARTNO
                    && !SqlSugar.SqlFunc.Equals(r.ID, mpnObj.ID)).Any();
                    if (bExist)
                    {
                        //throw new Exception($@"WO:{mpnObj.WO},PARTNO:{mpnObj.PARTNO},already exist!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160358", new string[] { "WO:" + mpnObj.WO + ",Partno:" + mpnObj.PARTNO }));
                    }                   
                    result = SFCDB.ORM.Updateable<C_WO_MPN>(mpnObj).Where(r => r.ID == mpnObj.ID).ExecuteCommand();                    
                }
                if (result == 0)
                {
                    throw new Exception("Save wo mpn fail!");
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void DeleteWOMPNL(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"].ToString().Trim();
                var result = SFCDB.ORM.Deleteable<C_WO_MPN>().Where(r => r.ID == id).ExecuteCommand();
                if (result == 0)
                {
                    throw new Exception("Delete result is 0!");
                }
                StationReturn.Data = result;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
