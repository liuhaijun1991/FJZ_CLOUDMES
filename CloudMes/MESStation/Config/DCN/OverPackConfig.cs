using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class OverPackConfig
    {
        public string ID;
        public string Skuno;
        public int PackQTY;
        public string PrintType;
        public string EDIT_EMP;
        public DateTime EDIT_TIME;

        public static List<OverPackConfig> GetList(OleExec sfcdb)
        {
            var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", sfcdb);
            return config;
        }

        public static List<OverPackConfig> GetListBySku(string skuno,OleExec sfcdb)
        {
            var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", sfcdb);
            return config.FindAll(t=>t.Skuno == skuno);
        }
    }



    public class OverPackConfigAPI : MesAPIBase
    {
        static Random rnd = new Random();
        string GetNewID()
        {
            var t = DateTime.Now.ToString("yyyyMMddHHmmss");
            t+=rnd.Next(100, 999).ToString();
            return t;
        }
        private APIInfo FGetOverPackConfigList = new APIInfo()
        {
            FunctionName = "GetOverPackConfigList",
            Description = "獲取所有OverPackConfig",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo FSaveOverPackConfig = new APIInfo()
        {
            FunctionName = "SaveOverPackConfig",
            Description = "保存配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName ="JSONDATA", InputType = "STRING", DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo FDeleteOverPackConfig = new APIInfo()
        {
            FunctionName = "DeleteOverPackConfig",
            Description = "删除配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName ="JSONDATA", InputType = "STRING", DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };


        private APIInfo FGetPrintTypeList = new APIInfo()
        {
            FunctionName = "GetPrintTypeList",
            Description = "获取tPrintType定义",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>()
            { }
        };


        public OverPackConfigAPI()
        {
            this.Apis.Add(FGetOverPackConfigList.FunctionName, FGetOverPackConfigList);
            this.Apis.Add(FSaveOverPackConfig.FunctionName, FSaveOverPackConfig);
            this.Apis.Add(FGetPrintTypeList.FunctionName, FGetPrintTypeList);
            this.Apis.Add(FDeleteOverPackConfig.FunctionName, FDeleteOverPackConfig);
        }
        public void DeleteOverPackConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {

                var Edit = Newtonsoft.Json.JsonConvert.DeserializeObject<OverPackConfig>(Data["JSONDATA"].ToString());
                if (!sfcdb.ORM.Queryable<R_JSON>().Where(t => t.TYPE == "SKUCONFIG" && t.NAME == "OVERPACKCONFIG").Any())
                {
                    List<OverPackConfig> l = new List<OverPackConfig>();
                    OverPackConfig c1 = new OverPackConfig() { Skuno = "TEST", PackQTY = 10, PrintType = "LIST" , ID = GetNewID()};
                    l.Add(c1);
                    JsonSave.SaveToDB(l, "OVERPACKCONFIG", "SKUCONFIG", "SYSTEM", sfcdb, BU, true);
                }
                var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", sfcdb);
                var edit = config.Find(t => t.ID == Edit.ID);
                if (edit != null)
                {
                    config.Remove(edit);
                }

                //config.Add(Edit);
                Edit.EDIT_EMP = LoginUser.EMP_NO;
                Edit.EDIT_TIME = DateTime.Now;
                JsonSave.SaveToDB(config, "OVERPACKCONFIG", "SKUCONFIG", "SYSTEM", sfcdb, BU, true);
                StationReturn.Status = StationReturnStatusValue.Pass;


            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;


            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetPrintTypeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Data = new List<string>() { "N/A","LIST", "BOX" };
        }

        public void GetOverPackConfigList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (!sfcdb.ORM.Queryable<R_JSON>().Where(t => t.TYPE == "SKUCONFIG" && t.NAME == "OVERPACKCONFIG").Any())
                {
                    List<OverPackConfig> l = new List<OverPackConfig>();
                    OverPackConfig c1 = new OverPackConfig() { Skuno ="TEST", PackQTY=10, PrintType = "LIST" , ID = GetNewID() };
                    l.Add(c1);
                    JsonSave.SaveToDB(l, "OVERPACKCONFIG", "SKUCONFIG", "SYSTEM", sfcdb, BU, true);
                }
                var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", sfcdb);
                //for (int i = 0; i < config.Count; i++)
                //{
                //    if (config[i].ID == null)
                //    {
                //        config[i].ID = GetNewID();
                //    }
                //    JsonSave.SaveToDB(config, "OVERPACKCONFIG", "SKUCONFIG", "SYSTEM", sfcdb, BU, true);
                //}
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = config;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

               
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void SaveOverPackConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                
                var Edit = Newtonsoft.Json.JsonConvert.DeserializeObject<OverPackConfig>(Data["JSONDATA"].ToString());
                if (Edit.ID == null)
                {
                    Edit.ID = Guid.NewGuid().ToString();
                }
                Edit.Skuno = Edit.Skuno.Trim().ToUpper();
                if (!sfcdb.ORM.Queryable<R_JSON>().Where(t => t.TYPE == "SKUCONFIG" && t.NAME == "OVERPACKCONFIG").Any())
                {
                    List<OverPackConfig> l = new List<OverPackConfig>();
                    OverPackConfig c1 = new OverPackConfig() { Skuno = "TEST", PackQTY = 10, PrintType = "LIST", ID = GetNewID() };
                    l.Add(c1);
                    JsonSave.SaveToDB(l, "OVERPACKCONFIG", "SKUCONFIG", "SYSTEM", sfcdb, BU, true);
                }
                var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", sfcdb);
                var edit = config.Find(t => t.ID == Edit.ID);
                if (edit != null)
                {
                    config.Remove(edit);
                }
                edit = config.Find(t => t.Skuno == Edit.Skuno);
                if (edit != null)
                {
                    config.Remove(edit);
                }

                config.Add(Edit);
                Edit.EDIT_EMP = LoginUser.EMP_NO;
                Edit.EDIT_TIME = DateTime.Now;
                JsonSave.SaveToDB(config, "OVERPACKCONFIG", "SKUCONFIG", "SYSTEM", sfcdb, BU, true);
                StationReturn.Status = StationReturnStatusValue.Pass;


            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;


            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

    }


}
