using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Stations.StationConfig
{
    public class ActionParaConfig : MESPubLab.MESStation.MesAPIBase
    {

        private APIInfo addactionpara = new APIInfo()
        {
            FunctionName = "AddActionPara",
            Description = "添加動作函數參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Seq",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Name",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo updateactionpara = new APIInfo()
        {
            FunctionName = "UpdateActionPara",
            Description = "修改動作函數參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Seq",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Name",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}

            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo deleteactionpara = new APIInfo()
        {
            FunctionName = "DeleteActionPara",
            Description = "刪除動作函數參數",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo queryactionpara = new APIInfo()
        {
            FunctionName = "QueryActionPara",
            Description = "查詢動作函數參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Name",InputType="string",DefaultValue=""},


            },
            Permissions = new List<MESPermission>()
            { }

            
        };
        private APIInfo deletebyactionid = new APIInfo()
        {
            FunctionName = "DeleteByActionID",
            Description = "查詢動作函數參數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},



            },
            Permissions = new List<MESPermission>()
            { }
        };
        public ActionParaConfig()
        {
            this.Apis.Add(addactionpara.FunctionName, addactionpara);
            this.Apis.Add(updateactionpara.FunctionName, updateactionpara);
            this.Apis.Add(deleteactionpara.FunctionName, deleteactionpara);
            this.Apis.Add(queryactionpara.FunctionName, queryactionpara);
            this.Apis.Add(deletebyactionid.FunctionName, deletebyactionid);
        }
        public void AddActionPara(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_C_ACTION_PARA actionpara;
            string StationActionID = Data["StationActionID"].ToString().Trim();
            Double Seq = Convert.ToDouble(Data["Seq"]);
            string Name = Data["Name"].ToString().Trim();
            string Desc = Data["Desc"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                actionpara = new T_C_ACTION_PARA(sfcdb, DBTYPE);
                if (actionpara.CheckDataExist(StationActionID,Name, sfcdb))
                {
                    Row_C_ACTION_PARA row = (Row_C_ACTION_PARA)actionpara.NewRow();
                    row.ID = actionpara.GetNewID(BU, sfcdb);
                    row.C_STATION_ACTION_ID = StationActionID;
                    row.SEQ_NO = Seq;
                    row.NAME = Name;
                    row.DESCRIPTION = Desc;
                    row.EDIT_EMP = LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    InsertSql = row.GetInsertString(DBTYPE);
                    sfcdb.ExecSQL(InsertSql);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void DeleteActionPara(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_C_ACTION_PARA language;
            //   string[] ID = Newtonsoft.Json.Linq.JArray(Data["ID"].);
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                language = new T_C_ACTION_PARA(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    Row_C_ACTION_PARA row = (Row_C_ACTION_PARA)language.GetObjByID(StrID, sfcdb);
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
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void DeleteByActionID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string StrID = "";
            T_C_ACTION_PARA actionpara;
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["StationActionID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                actionpara = new T_C_ACTION_PARA(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    actionpara.DeleteByActionID(StrID,sfcdb);   
                }
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
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
        public void UpdateActionPara(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_C_ACTION_PARA actionpara;
            string ID = Data["ID"].ToString().Trim();
            string StationActionID = Data["StationActionID"].ToString().Trim();
            Double Seq = Convert.ToDouble(Data["Seq"]);
            string Name = Data["Name"].ToString().Trim();
            string Desc = Data["Desc"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                actionpara = new T_C_ACTION_PARA(sfcdb, DBTYPE);
                Row_C_ACTION_PARA row = (Row_C_ACTION_PARA)actionpara.GetObjByID(ID, sfcdb);
                row.ID = ID;
                row.C_STATION_ACTION_ID = StationActionID;
                row.SEQ_NO = Seq;
                row.NAME = Name;
                row.DESCRIPTION = Desc;
                row.EDIT_EMP = LoginUser.EMP_NO;
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
        public void QueryActionPara(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ACTION_PARA ActionPara;
            List<C_ACTION_PARA> InputList;
            string ID = Data["ID"].ToString().Trim();
            string StationActionID = Data["StationActionID"].ToString().Trim();
            string Name = Data["Name"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                ActionPara = new T_C_ACTION_PARA(sfcdb, DBTYPE);
                InputList = ActionPara.QueryActionPara(ID, StationActionID, Name, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = InputList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
    }
}
