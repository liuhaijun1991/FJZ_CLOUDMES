using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.Config
{
    public class SAPStationMapConfig : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        private APIInfo _GetAllSapStationMaps = new APIInfo()
        {
            FunctionName = "GetAllSapStationMaps",
            Description = "獲取所有機種站位與 SAP 拋賬點的映射關係",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetAllSapStationMapBySku = new APIInfo()
        {
            FunctionName = "GetAllSapStationMapBySku",
            Description = "根據機種、站位獲取其與 SAP 拋賬點的映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="SkuNo",InputType="string",DefaultValue="" },
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetAllSapStationMapBySkuAndStation = new APIInfo()
        {
            FunctionName = "GetAllSapStationMapBySkuAndStation",
            Description = "根據機種、站位獲取其與 SAP 拋賬點的映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="SkuNo",InputType="string",DefaultValue="" },
                new APIInputInfo() {InputName="Station",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _UpdateSapStationMap = new APIInfo()
        {
            FunctionName = "UpdateSapStationMap",
            Description = "根據機種、站位獲取其與 SAP 拋賬點的映射關係",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="Operation",InputType="string",DefaultValue="" },
                new APIInputInfo() {InputName="MapObject",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSapStationMap = new APIInfo() {
            FunctionName = "DeleteSapStationMap",
            Description = "删除抛帐点",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="IDList",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        public SAPStationMapConfig()
        {
            this.Apis.Add(_GetAllSapStationMaps.FunctionName, _GetAllSapStationMaps);
            this.Apis.Add(_GetAllSapStationMapBySku.FunctionName, _GetAllSapStationMapBySku);
            this.Apis.Add(_GetAllSapStationMapBySkuAndStation.FunctionName, _GetAllSapStationMapBySkuAndStation);
            this.Apis.Add(_UpdateSapStationMap.FunctionName, _UpdateSapStationMap);
        }

        public void GetAllSapStationMaps(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SAP_STATION_MAP> MapList = new List<C_SAP_STATION_MAP>();
            T_C_SAP_STATION_MAP Table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SAP_STATION_MAP(sfcdb, DBTYPE);
                MapList = Table.GetAllSAPStationMaps(sfcdb);
                if (MapList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(MapList.Count().ToString());
                    StationReturn.Data = MapList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetAllSapStationMapBySkuAndStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SAP_STATION_MAP> MapList = new List<C_SAP_STATION_MAP>();
            T_C_SAP_STATION_MAP Table = null;
            string SkuNo = string.Empty;
            string Station = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SAP_STATION_MAP(sfcdb, DBTYPE);
                SkuNo = Data["SkuNo"].ToString().Trim();
                Station = Data["Station"].ToString().Trim();
                if (string.IsNullOrEmpty(SkuNo) || string.IsNullOrEmpty(Station))
                {
                    GetAllSapStationMaps(requestValue, Data, StationReturn);
                }
                else
                {
                    MapList = Table.GetSAPStationMapBySkuAndStation(SkuNo, Station, sfcdb);
                    if (MapList.Count() == 0)
                    {
                        //沒有獲取到數據
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        //獲取成功
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(MapList.Count().ToString());
                        StationReturn.Data = MapList;

                    }
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetAllSapStationMapBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SAP_STATION_MAP> MapList = new List<C_SAP_STATION_MAP>();
            T_C_SAP_STATION_MAP Table = null;
            string SkuNo = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SAP_STATION_MAP(sfcdb, DBTYPE);
                SkuNo = Data["SkuNo"].ToString().Trim();
                if (string.IsNullOrEmpty(SkuNo))
                {
                    GetAllSapStationMaps(requestValue, Data, StationReturn);
                }
                else
                {
                    MapList = Table.GetSAPStationMapBySku(SkuNo, sfcdb);
                    if (MapList.Count() == 0)
                    {
                        //沒有獲取到數據
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        //獲取成功
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(MapList.Count().ToString());
                        StationReturn.Data = MapList;

                    }
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void UpdateSapStationMap(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Operation = string.Empty;
            string MapObject = string.Empty;
            OleExec sfcdb = null;
            T_C_SAP_STATION_MAP Table = null;
            C_SAP_STATION_MAP Map = null;
            string result = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SAP_STATION_MAP(sfcdb, DBTYPE);
                MapObject = Data["MapObject"].ToString();
                Operation = Data["Operation"].ToString();
                Map = (C_SAP_STATION_MAP)JsonConvert.Deserialize(MapObject, typeof(C_SAP_STATION_MAP));
                Map.EDIT_EMP = LoginUser.EMP_NO;
                Map.EDIT_TIME = GetDBDateTime();
                result = Table.UpdateSAPStationMap(Map, Operation, BU,this.LoginUser.EMP_NO, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //更新成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Clear();
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = result;
                }
                else
                {
                    //更新失敗
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                //不是最新的數據，返回字符串無法被 Int32.Parse 方法轉換成 int,所以出現異常
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000032";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    //數據庫執行異常
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DeleteSapStationMap(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SAP_STATION_MAP Table = null;
            try
            {
                JToken data = Data["IDList"];
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SAP_STATION_MAP(sfcdb, DBTYPE);
                int n = 0;
                for (int i = 0; i < data.Count(); i++)
                {
                    var s = Table.DeleteSAPStationMap(data[i].ToString(),BU, this.LoginUser.EMP_NO, sfcdb);
                    if (int.Parse(s) > 0)
                    {
                        n++;
                    }
                }
                if (n > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Clear();
                    StationReturn.MessagePara.Add(n);
                    StationReturn.Data = n;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = n;
                }
            }
            catch (Exception e)
            {
                //數據庫執行異常
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
