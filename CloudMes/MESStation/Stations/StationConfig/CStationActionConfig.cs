using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using System.Reflection;

namespace MESStation.Stations.StationConfig
{
    public  class CStationActionConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addcstationaction = new APIInfo()
        {
            FunctionName = "AddCStationAcation",
            Description = "添加動作函數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ActionType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ActionName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DllName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ClassName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="FunName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo updatecstationacation = new APIInfo()
        {
            FunctionName = "UpdateCStationAcation",
            Description = "修改動作函數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ActionType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ActionName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DllName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ClassName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="FunName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo deletecstationAcation = new APIInfo()
        {
            FunctionName = "DeleteCStationAcation",
            Description = "刪除動作函數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo querycstationAcation = new APIInfo()
        {
            FunctionName = "QueryCStationAcation",
            Description = "查詢動作函數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ActionType",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo getclass = new APIInfo()
        {
            FunctionName = "GetClassList",
            Description = "根據DLL名稱取所有類",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="DllName",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo getfunction = new APIInfo()
        {
            FunctionName = "GetFunctionsList",
            Description = "根據類名取所有函數",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="DllName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ClassName",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };
        public CStationActionConfig()
        {
            this.Apis.Add(addcstationaction.FunctionName, addcstationaction);
            this.Apis.Add(updatecstationacation.FunctionName, updatecstationacation);
            this.Apis.Add(deletecstationAcation.FunctionName, deletecstationAcation);
            this.Apis.Add(querycstationAcation.FunctionName, querycstationAcation);
            this.Apis.Add(getclass.FunctionName, getclass);
            this.Apis.Add(getfunction.FunctionName, getfunction);
        }
        public void AddCStationAcation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_c_station_action stationaction;
            string ActionType = Data["ActionType"].ToString().Trim();
            string ActionName = Data["ActionName"].ToString().Trim();
            string DllName = Data["DllName"].ToString().Trim();
            string ClassName = Data["ClassName"].ToString();
            string FunctionName = Data["FunName"].ToString();
            string Desc = Data["Desc"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                stationaction = new T_c_station_action(sfcdb, DBTYPE);
                if (stationaction.CheckDataExist(FunctionName, sfcdb))
                {
                    Row_c_station_action row = (Row_c_station_action)stationaction.NewRow();
                    row.ID = stationaction.GetNewID(BU, sfcdb);
                    row.ACTION_TYPE = ActionType;
                    row.ACTION_NAME = ActionName;
                    row.DLL_NAME = DllName;
                    row.CLASS_NAME = ClassName;
                    row.FUNCTION_NAME = FunctionName;
                    row.DESCRIPTION = Desc;
                    row.EDIT_EMP = LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    InsertSql = row.GetInsertString(DBTYPE);
                    sfcdb.ExecSQL(InsertSql);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = row.ID;
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

        public void DeleteCStationAcation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_c_station_action stationaction;
            T_R_Station_Action saction;
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                stationaction = new T_c_station_action(sfcdb, DBTYPE);
                saction = new T_R_Station_Action(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();

                    if (saction.CheckExistByCSActionID(StrID, sfcdb))
                    {
                        Row_c_station_action row = (Row_c_station_action)stationaction.GetObjByID(StrID, sfcdb);
                        DeleteSql = row.GetDeleteString(DBTYPE);
                        sfcdb.ExecSQL(DeleteSql);
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000113";
                        sfcdb.RollbackTrain();
                        this.DBPools["SFCDB"].Return(sfcdb);
                        return ;
                    }
                   
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
        /// <summary>
        /// 更新標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateCStationAcation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_c_station_action stationaction;
            string ID = Data["ID"].ToString().Trim();
            string ActionType = Data["ActionType"].ToString().Trim();
            string ActionName = Data["ActionName"].ToString().Trim();
            string DllName = Data["DllName"].ToString().Trim();
            string ClassName = Data["ClassName"].ToString();
            string FunctionName = Data["FunName"].ToString();
            string Desc = Data["Desc"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                stationaction = new T_c_station_action(sfcdb, DBTYPE);
                Row_c_station_action row = (Row_c_station_action)stationaction.GetObjByID(Data["ID"].ToString().Trim(), sfcdb);
                row.ACTION_TYPE = ActionType;
                row.ACTION_NAME = ActionName;
                row.DLL_NAME = DllName;
                row.CLASS_NAME = ClassName;
                row.FUNCTION_NAME = FunctionName;
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
        public void QueryCStationAcation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_c_station_action stationaction;
            List<c_station_action> stationactionList;
            string ID = Data["ID"].ToString().Trim();
            string ActionType = Data["ActionType"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                stationaction = new T_c_station_action(sfcdb, DBTYPE);
                stationactionList = stationaction.Querycstationaction(ID, ActionType, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = stationactionList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }


        public void GetClassList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string DllName = Data["DllName"].ToString().Trim();
            MESPubLab.MESStation.MESReturnView.Public.GetApiClassListReturncs ret = new MESPubLab.MESStation.MESReturnView.Public.GetApiClassListReturncs();
            Assembly assemby = Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\" + DllName);
            Type tagType = typeof(MesAPIBase);
            Type[] t = assemby.GetTypes();
            for (int i = 0; i < t.Length; i++)
            {
                TypeInfo ti = t[i].GetTypeInfo();
                Type baseType = ti.BaseType;
                if (baseType != tagType)
                {
                    ret.ClassName.Add(ti.FullName);
                }
            }
            StationReturn.Data = ret;
            StationReturn.MessageCode = "MES00000001";
            StationReturn.Status = "Pass";
        }

        public void GetFunctionsList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
          //  Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\" + DLL);
            string DllName = Data["DllName"].ToString().Trim();
            string ClassName = Data["ClassName"].ToString();
            List<string> FunctionList = new List<string>();
             Assembly assemby = Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\" + DllName);
          //  Assembly assemby = Assembly.Load(DllName);
            Type t = assemby.GetType(ClassName);
            object obj = assemby.CreateInstance(ClassName);
           // MesAPIBase API = (MesAPIBase)obj;
         //   Dictionary<string, MESPubLab.MESStation.APIInfo> APIS;
            //   MESReturnView.Public.GetApiFunctionsListReturn ret = new MESReturnView.Public.GetApiFunctionsListReturn();
            //APIS = API.Apis;
            foreach (var item in obj.GetType().GetMethods())
            {
                if (item.Name == "ToString")
                {
                    continue;
                }

                if (item.Name == "Equals")
                {
                    continue;
                }

                if (item.Name == "GetHashCode")
                {
                    continue;
                }

                if (item.Name == "GetType")
                {
                    continue;
                }

                FunctionList.Add(item.Name);

            }
            StationReturn.Data = FunctionList;
            StationReturn.MessageCode = "MES00000001";
            StationReturn.Status = "Pass";

        }

    }
}
