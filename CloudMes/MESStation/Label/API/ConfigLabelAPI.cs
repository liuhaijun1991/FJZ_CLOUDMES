using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;

namespace MESStation.Label.API
{
    public class ConfigLabelAPI : MesAPIBase
    {
        private APIInfo _GetLabelValueGroup = new APIInfo()
        {
            FunctionName = "GetLabelValueGroup",
            Description = "获取所有值生成函数",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "GroupName", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetLabelValueGroupNameList = new APIInfo()
        {
            FunctionName = "GetLabelValueGroupNameList",
            Description = "获取所有值生成函数组名称列表",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _UpdateConfigLabelType = new APIInfo()
        {
            FunctionName = "UpdateConfigLabelType",
            Description = "增加或修改配置labelType",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TypeName", InputType = "string", DefaultValue = "LabelTypeName" },
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "ConfigableLabelBase" },
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetConfigLabelTypeList = new APIInfo()
        {
            FunctionName = "GetConfigLabelTypeList",
            Description = "获取配置labelType",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetTestConfigLabel = new APIInfo()
        {
            FunctionName = "GetTestConfigLabel",
            Description = "获取测试configLabel",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _TestRunConfigLabel = new APIInfo()
        {
            FunctionName = "TestRunConfigLabel",
            Description = "测试运行ConfigLabel",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "ConfigableLabelBase" },
            },
            Permissions = new List<MESPermission>()
            { }
        };

        public ConfigLabelAPI()
        {
            Apis.Add(_GetLabelValueGroup.FunctionName, _GetLabelValueGroup);
            Apis.Add(_GetTestConfigLabel.FunctionName, _GetTestConfigLabel);
            Apis.Add(_GetLabelValueGroupNameList.FunctionName, _GetLabelValueGroupNameList);
            Apis.Add(_UpdateConfigLabelType.FunctionName, _UpdateConfigLabelType);
            Apis.Add(_GetConfigLabelTypeList.FunctionName, _GetConfigLabelTypeList);
            Apis.Add(_TestRunConfigLabel.FunctionName, _TestRunConfigLabel);
        }
        public void TestRunConfigLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                var JsonData = Data["JsonData"].ToString();
                var labelBase = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigableLabelBase>(JsonData);

                for (int i = 0; i < labelBase.Inputs.Count; i++)
                {
                    labelBase.Inputs[i].Value = labelBase.Inputs[i].StationSessionValue;
                }
                labelBase.MakeLabel(db);
                StationReturn.Data = labelBase;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Message = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void GetConfigLabelTypeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                var ret = db.ORM.Queryable<C_Label_Type>().Where(t => t.DLL == "JSON").OrderBy(t => t.NAME, SqlSugar.OrderByType.Asc).ToList();
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Message = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void UpdateConfigLabelType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                var TypeName = Data["TypeName"].ToString();
                var JsonData = Data["JsonData"];
                var CLT = db.ORM.Queryable<C_Label_Type>().Where(t => t.NAME == TypeName).First();
                if (CLT != null)
                {
                    if (CLT.DLL != "JSON")
                    {
                        throw new Exception($@"LabelType'{TypeName}' is not a ConfigLabel!");
                    }
                    var JsonID = CLT.CLASS;

                    JsonSave.UpdateToDB(JsonData, JsonID, LoginUser.EMP_NO, db, BU);

                }
                else
                {
                    var JsonID = JsonSave.SaveToDB(JsonData, TypeName, "ConfigLabel", LoginUser.EMP_NO, db, BU);
                    var CID = MesDbBase.GetNewID(db.ORM, BU, "C_Label_Type");
                    CLT = new C_Label_Type() { ID = CID, NAME = TypeName, DLL = "JSON", EDIT_EMP = LoginUser.EMP_NO, EDIT_TIME = DateTime.Now, CLASS = JsonID };
                    db.ORM.Insertable(CLT).ExecuteCommand();
                }

                StationReturn.Data = CLT;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Message = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }


        public void GetLabelValueGroupNameList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {

                var ret = ConfigableLabelBase.GetLabelValueGroupNameList();

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Message = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetTestConfigLabel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                var ret = new TestConfigLabel();
                ret.Inputs.Find(t => t.Name == "SN").Value = "DSDSDSD";
                //ret.MakeLabel(db);
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception ee)
            {
                StationReturn.Message = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void GetLabelValueGroup(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                string GroupName = "";
                try
                {
                    GroupName = Data["GroupName"].ToString();
                }
                catch
                { }
                var ret = ConfigableLabelBase.GetLabelValueGroup(GroupName);

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Message = ee.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void Temp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {

                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

    }
}
