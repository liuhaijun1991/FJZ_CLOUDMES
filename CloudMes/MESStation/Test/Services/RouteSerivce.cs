using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.MESReturnView.Public;
using MESStation.BaseClass;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using System.Data;

namespace MESStation.Test.Services
{
    public class RouteSerivce : MesAPIBase
    {
        private static DB_TYPE_ENUM DBType = DB_TYPE_ENUM.Oracle;
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConverter = new System.Web.Script.Serialization.JavaScriptSerializer();

        private APIInfo RouteDescForAll = new APIInfo()
        {
            //FunctionName 需要跟後面提供數據的函數名稱一樣，目前沒有自定義映射關係，因此只能通過反射來執行。
            //FunctionName field need to be same with the function name,we have to invoke the method by reflection because 
            //there is no customered mapping relation now
            FunctionName = "GetAllRouteDesc",
            Description = "獲取所有的流程描述信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        private APIInfo RouteDescBySku = new APIInfo()
        {
            FunctionName = "GetRouteDesc",
            Description = "根據流程名來獲取流程描述信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="ROUTE_NAME",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo RouteDescOfOthers = new APIInfo()
        {
            FunctionName = "GetRouteDescByOtherCondition",
            Description = "根據其他條件獲取流程描述信息，例如 默認適用機種，流程類型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="OTHER_CONDITION",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo ChangeRouteDesc = new APIInfo()
        {
            FunctionName = "UpdateRouteDesc",
            Description = "根據前端返回的對象，插入/刪除/更新數據庫",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ROUTE_OBJECT",InputType=(typeof(R_C_ROUTE)).Name,DefaultValue=null},
                new APIInputInfo() { InputName="OPERATION",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo RouteDetailByID = new APIInfo()
        {
            FunctionName = "GetRouteDetailByRouteID",
            Description = "根據流程名返回流程具體信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ROUTE_ID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo ChangeRouteDetail = new APIInfo()
        {
            FunctionName = "UpdateRouteDetail",
            Description = "根據前端返回對象，插入/刪除/更新數據庫",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ROUTE_DETAIL_OBJECT",InputType=typeof(R_C_ROUTE_DETAIL).Name,DefaultValue=null},
                new APIInputInfo() { InputName="OPERATION",InputType="string",DefaultValue=""}
            },
            Permissions=new List<MESPermission>()
            { }
        };

        private APIInfo 

       


        public RouteSerivce()
        {
            this.Apis.Add(RouteDescForAll.FunctionName, RouteDescForAll);
            this.Apis.Add(RouteDescBySku.FunctionName, RouteDescBySku);
            this.Apis.Add(RouteDescOfOthers.FunctionName, RouteDescOfOthers);
            this.Apis.Add(ChangeRouteDesc.FunctionName, ChangeRouteDesc);
            this.Apis.Add(RouteDetailByID.FunctionName, RouteDetailByID);
            this.Apis.Add(ChangeRouteDetail.FunctionName, ChangeRouteDetail);
        }

        /// <summary>
        /// 返回所有的路由主表信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAllRouteDesc(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            DataTable dt = new DataTable();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ROUTE table = new T_C_ROUTE(sfcdb, DBType);
                dt = table.GetAllRoute(sfcdb);
                ConstructReturnValue(StationReturn, dt);

            }
            catch (Exception e)
            {
                ConstructReturnValue(StationReturn, e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        /// <summary>
        /// 根據機種返回路由描述信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRouteDesc(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string RouteName = Data["ROUTE_NAME"].ToString();
            DataTable dt = new DataTable();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ROUTE table = new T_C_ROUTE(sfcdb, DBType);
                dt = table.GetByRouteName(RouteName, sfcdb);
                ConstructReturnValue(StationReturn, dt);
            }
            catch (Exception e)
            {
                ConstructReturnValue(StationReturn, e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

            
        }

        /// <summary>
        /// 根據其他信息返回路由描述信息，可以根據路由的類型以及默認適用的機種
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRouteDescByOtherCondition(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            DataTable dt = new DataTable();
            string OtherCondition = Data["OTHER_CONDITION"].ToString();
            T_C_ROUTE table = null;

            try
            {
                sfcdb = this.DBPools["SFCBD"].Borrow();
                table = new T_C_ROUTE(sfcdb, DBType);
                dt = table.GetByOtherConditions(OtherCondition, sfcdb);
                ConstructReturnValue(StationReturn, dt);

            }
            catch (Exception e)
            {
                ConstructReturnValue(StationReturn, e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        
        }

        /// <summary>
        /// 前端返回一個 JSON 對象，反序列化成 R_C_ROUTE 對象插入/更新/刪除 流程描述表
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateRouteDesc(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_ROUTE Table = null;
            R_C_ROUTE Route = null;
            OleExec sfcdb = null;
            int Result = 0;
            string Operation = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_ROUTE(sfcdb,DBType);
                Operation = Data["OPERATION"].ToString();
                Route = (R_C_ROUTE)JsonConverter.Deserialize(Data["ROUTE_OBJECT"].ToString(), typeof(R_C_ROUTE));
                Result = Table.UpdateRoute(Route, Operation, sfcdb);
                ConstructReturnValue(StationReturn, Result, string.Format("成功更新 {0} 條數據", Result));
            }
            catch (Exception e)
            {
                ConstructReturnValue(StationReturn, e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

            
        }

        /// <summary>
        /// 根據流程 ID 返回流程具體信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRouteDetailByRouteID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            DataTable dt = new DataTable();
            string RouteID = string.Empty;
            T_C_ROUTE_DETAIL table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RouteID = Data["ROUTE_ID"].ToString();
                table = new T_C_ROUTE_DETAIL(sfcdb, DBType);
                dt = table.GetRouteDetailByID(RouteID, sfcdb);
                ConstructReturnValue(StationReturn, dt);
            }
            catch (Exception e)
            {
                ConstructReturnValue(StationReturn, e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        /// <summary>
        /// 前端返回一個 JSON 對象，反序列化成 R_C_ROUTE_DETAIL 對象插入/更新/刪除 流程具體信息表
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateRouteDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_ROUTE_DETAIL Table = null;
            R_C_ROUTE_DETAIL RouteDetail = null;
            OleExec sfcdb = null;
            int Result = 0;
            string Operation = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_ROUTE_DETAIL(sfcdb, DBType);
                RouteDetail = (R_C_ROUTE_DETAIL)JsonConverter.Deserialize(Data["ROUTE_DETAIL_OBJECT"].ToString(), typeof(R_C_ROUTE_DETAIL));
                Operation = Data["OPERATION"].ToString();
                Result = Table.UpdateRouteDetail(RouteDetail, Operation, sfcdb);
                ConstructReturnValue(StationReturn, Result, string.Format("成功更新 {0} 行數據.", Result));
            }
            catch (Exception e)
            {
                ConstructReturnValue(StationReturn, e);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }


        /// <summary>
        /// 構建返回到前端的對象
        /// </summary>
        /// <param name="StationReturn"></param>
        /// <param name="Data"></param>
        private void ConstructReturnValue(MESStationReturn StationReturn, object Data)
        {
            ConstructReturnValue(StationReturn,Data,"");
        }

        private void ConstructReturnValue(MESStationReturn StationReturn,object Data, string returnMessage)
        {
            if (StationReturn == null)
            {
                StationReturn = new MESStationReturn();
            }

            //Status 值後期可以指定為一個枚舉或者類的靜態成員，這樣方便統一修改。
            StationReturn.Status = "Success";
            StationReturn.data = Data;
            if (Data.GetType() == typeof(Exception))
            {
                StationReturn.Status = "Fail";
                StationReturn.Message = ((Exception)Data).Message;
                return;
            }

            if (Data.GetType() == typeof(DataTable))
            {
                DataTable temp_table = (DataTable)Data;
                StationReturn.data = temp_table;
                if (temp_table.Rows.Count > 0)
                {
                    StationReturn.Message = "獲取路由信息成功";
                }
                else
                {
                    StationReturn.Message = "沒有獲取到路由信息";
                }
            }
            if (!returnMessage.Equals(""))
            {
                StationReturn.Message = returnMessage;
            }
            
        }

    }
}
