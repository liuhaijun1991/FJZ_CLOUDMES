using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Config.ORACLE
{
    public class KPGroupPartNoConfig : MesAPIBase
    {
        protected APIInfo FGetAllKPGroupPN = new APIInfo()
        {
            FunctionName = "GetAllKPGroupPN",
            Description = "Get all mapping data.",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FUpdateKPGroupPN = new APIInfo()
        {
            FunctionName = "UpdateKPGroupPN",
            Description = "Update mapping data.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "PARTNO" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FAddKPGroupPN = new APIInfo()
        {
            FunctionName = "AddKPGroupPN",
            Description = "Add mapping data.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "GROUPNAME", InputType = "string", DefaultValue = "MES00000001" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "PARTNO" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FDeleteKPGroupPNByID = new APIInfo()
        {
            FunctionName = "DeleteKPGroupPNByID",
            Description = "Delete mapping data.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetDetail = new APIInfo()
        {
            FunctionName = "GetDetail",
            Description = "Get details by input",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "GROUPNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };




        public KPGroupPartNoConfig()
        {
            this.Apis.Add(FGetAllKPGroupPN.FunctionName, FGetAllKPGroupPN);
            this.Apis.Add(FUpdateKPGroupPN.FunctionName, FUpdateKPGroupPN);
            this.Apis.Add(FAddKPGroupPN.FunctionName, FAddKPGroupPN);
            this.Apis.Add(FDeleteKPGroupPNByID.FunctionName, FDeleteKPGroupPNByID);
            Apis.Add(_GetDetail.FunctionName, _GetDetail);
        }

        public void GetDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            string GROUPNAME = Data["GROUPNAME"].ToString();
            string PARTNO = Data["PARTNO"].ToString();

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();

                var KPGROUP = Sfcdb.ORM.Queryable<C_KP_GROUP, C_KP_GROUP_PARTNO>((G, P) => new object[]{
                    SqlSugar.JoinType.Inner,G.ID==P.KP_GROUP_ID})
                   .Where((G, P) => G.GROUPNAME == GROUPNAME || P.PARTNO == PARTNO)
                  .OrderBy((G, P) => G.GROUPNAME)
                  .Select((G, P) => new { P.ID, P.KP_GROUP_ID, G.GROUPNAME, P.PARTNO, P.EDIT_EMP, P.EDIT_TIME }).ToList();

                if (GROUPNAME == "ALL GROUP" && PARTNO == "")
                {

                     KPGROUP = Sfcdb.ORM.Queryable<C_KP_GROUP, C_KP_GROUP_PARTNO>((G, P) => new object[]{
                    SqlSugar.JoinType.Inner,G.ID==P.KP_GROUP_ID})
                    .OrderBy((G, P) => G.GROUPNAME)
                    .Select((G, P) => new { P.ID, P.KP_GROUP_ID, G.GROUPNAME, P.PARTNO, P.EDIT_EMP, P.EDIT_TIME }).ToList();
                }

                


                if (KPGROUP == null || KPGROUP.Count == 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                }
                else
                {
                    StationReturn.Data = KPGROUP;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                //
                if (Sfcdb != null)
                {
                    DBPools["SFCDB"].Return(Sfcdb);
                }
            }
            catch (Exception ex)
            {
                if (Sfcdb != null)
                {
                    DBPools["SFCDB"].Return(Sfcdb);
                }
                throw ex;
            }
        }

        public void GetAllGroupName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_KP_GROUP KP_Group = null;
            List<C_KP_GROUP> KP_GroupList = new List<C_KP_GROUP>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                KP_Group = new T_C_KP_GROUP(oleDB, DBTYPE);
                KP_GroupList = KP_Group.GetAllGroupName(oleDB, DBTYPE);
                if (KP_GroupList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(KP_GroupList.Count);
                    StationReturn.Data = KP_GroupList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
        }




        public void GetAllKPGroupPN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_KP_GROUP_PARTNO> Ret = new List<C_KP_GROUP_PARTNO>();
            T_C_KP_GROUP_PARTNO T_C_KP_GROUP_PARTNO = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_KP_GROUP_PARTNO = new T_C_KP_GROUP_PARTNO(Sfcdb, DBTYPE);
               // Ret = T_C_KP_GROUP_PARTNO.GetAllKPGroupPN(Sfcdb, DBTYPE);

                var KPGROUP = Sfcdb.ORM.Queryable<C_KP_GROUP, C_KP_GROUP_PARTNO>((G, P) => new object[]{
                    SqlSugar.JoinType.Inner,G.ID==P.KP_GROUP_ID
                })
                .OrderBy((G, P) => G.GROUPNAME)
                .Select((G, P) => new { P.ID, P.KP_GROUP_ID, G.GROUPNAME, P.PARTNO, P.EDIT_EMP, P.EDIT_TIME }).ToList();

                StationReturn.Data = KPGROUP;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Message = "Get all KP PN data OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }


        public void UpdateKPGroupPN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            string UpdateSql = "";
            T_C_KP_GROUP_PARTNO T_C_KP_GROUP_PARTNO = null;
            Row_C_KP_GROUP_PARTNO Row_C_KP_GROUP_PARTNO = null;
            try
            {
                string ID = Data["ID"].ToString();
                string KP_GROUP_ID = Data["KP_GROUP_ID"].ToString();
                string GROUPNAME = Data["GROUPNAME"].ToString();
                string PARTNO = Data["PARTNO"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_KP_GROUP_PARTNO = new T_C_KP_GROUP_PARTNO(Sfcdb, DBTYPE);
                Row_C_KP_GROUP_PARTNO = (Row_C_KP_GROUP_PARTNO)T_C_KP_GROUP_PARTNO.GetObjByID(ID, Sfcdb);
                if (Row_C_KP_GROUP_PARTNO == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    return;
                }
                UpdateSql = "UPDATE C_KP_GROUP_PARTNO set PARTNO='" + PARTNO + "', EDIT_TIME = TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') , EDIT_EMP = '"+ LoginUser.EMP_NO +"' where ID = '" + ID + "'";
                Sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Message = "Group PN update OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

        public void AddKPGroupPN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            T_C_KP_GROUP_PARTNO c_kp_group_pn = null;
            Row_C_KP_GROUP_PARTNO Row_C_KP_GROUP_PARTNO = null;
            string InsertSql = "";
            string groupID = "";
            try
            {
                string GROUPNAME = Data["GROUPNAME"].ToString();
                string PARTNO = Data["PARTNO"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();

                c_kp_group_pn = new T_C_KP_GROUP_PARTNO(Sfcdb, DBTYPE);

                var KPGROUPID = Sfcdb.ORM.Queryable<C_KP_GROUP>()
                .Where((G) => G.GROUPNAME == GROUPNAME)
                .Select((G) => new { G.ID }).ToList();

                if (KPGROUPID.Count == 0)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                    return;
                }

                var KPGROUP = Sfcdb.ORM.Queryable<C_KP_GROUP, C_KP_GROUP_PARTNO>((G, P) => new object[]{
                    SqlSugar.JoinType.Inner,G.ID==P.KP_GROUP_ID})
                .Where((G, P) => G.GROUPNAME == GROUPNAME && P.PARTNO == PARTNO)
                .Select((G, P) => new { P.ID, P.KP_GROUP_ID, G.GROUPNAME, P.PARTNO, P.EDIT_EMP, P.EDIT_TIME }).ToList();
                if (KPGROUP.Count > 0)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Message = "KP group PN existed!";
                    return;
                }

                groupID = KPGROUPID[0].ID ;
              
              
                Row_C_KP_GROUP_PARTNO = (Row_C_KP_GROUP_PARTNO)c_kp_group_pn.NewRow();
                Row_C_KP_GROUP_PARTNO.ID = c_kp_group_pn.GetNewID(BU, Sfcdb);
                Row_C_KP_GROUP_PARTNO.KP_GROUP_ID = groupID;
                Row_C_KP_GROUP_PARTNO.PARTNO = PARTNO;
                Row_C_KP_GROUP_PARTNO.CREATE_EMP = LoginUser.EMP_NO;
                Row_C_KP_GROUP_PARTNO.CREATE_TIME = GetDBDateTime();
                Row_C_KP_GROUP_PARTNO.EDIT_EMP = LoginUser.EMP_NO;
                Row_C_KP_GROUP_PARTNO.EDIT_TIME = GetDBDateTime();
                InsertSql = Row_C_KP_GROUP_PARTNO.GetInsertString(DBTYPE);
                Sfcdb.ExecSQL(InsertSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Message = "新增MessageCode OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

        public void DeleteKPGroupPNByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                JToken[] IDS = Data["ID"].ToArray();
                var temp = Sfcdb.ORM.Queryable<C_KP_GROUP_PARTNO>().Where(t => IDS.Contains(t.ID)).ToList();
                Sfcdb.ORM.Deleteable<C_KP_GROUP_PARTNO>(temp).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                StationReturn.Message = "By ID delete KP PN data OK!";
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
        }
    }
}
