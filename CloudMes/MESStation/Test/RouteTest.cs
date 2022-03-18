using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Test
{
    public class RouteTest: MESPubLab.MESStation.MesAPIBase
    {
       /* private static DB_TYPE_ENUM DB_TYPE = DB_TYPE_ENUM.Oracle;
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        private APIInfo AllRouteDesc = new APIInfo()
        {
            FunctionName = "GetAllRoute",
            Description="獲取所有的路由描述信息",
            Parameters=new List<APIInputInfo>()
            {

            },
            Permissions=new List<MESPermission>()
            {
            }

        };

        private APIInfo CopyRouteDesc = new APIInfo()
        {
            FunctionName = "CopyOneRoute",
            Description = "拷貝一個已存在的路由",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="CopyRouteId",InputType="string",DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
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
                new APIInputInfo() { InputName="ROUTE_OBJECT",InputType="string",DefaultValue=null},
                new APIInputInfo() { InputName="OPERATION",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo RouteDetailByID = new APIInfo()
        {
            FunctionName = "GetRouteDetailByID",
            Description="根據路由ID獲取路由詳細信息",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="CopyRouteId",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="RouteId",InputType="string",DefaultValue=""}
            },
            Permissions=new List<MESPermission>()
            { }

        };

        public RouteTest()
        {
            this.Apis.Add(AllRouteDesc.FunctionName, AllRouteDesc);
            this.Apis.Add(RouteDetailByID.FunctionName, RouteDetailByID);
            this.Apis.Add(CopyRouteDesc.FunctionName, CopyRouteDesc);
            this.Apis.Add(RouteDescBySku.FunctionName, RouteDescBySku);
            this.Apis.Add(RouteDescOfOthers.FunctionName, RouteDescOfOthers);
            this.Apis.Add(ChangeRouteDesc.FunctionName, ChangeRouteDesc);
        }

        public void GetAllRoute(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_ROUTE> routes = null;
            T_C_ROUTE table = null;
            T_C_ROUTE tr = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_ROUTE(sfcdb, DB_TYPE);
                

                tr = new T_C_ROUTE("C_ROUTE", sfcdb, DB_TYPE);
                routes = tr.GetAllRoutes(sfcdb);

                StationReturn.Status = "Success";
                StationReturn.Message = "獲取成功";
                StationReturn.Data = routes;
            }
            catch (Exception e)
            {
                StationReturn.Status = "Fail";
                StationReturn.Message = e.Message;
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetRouteDetailByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ROUTE_DETAIL table = null;
            List<C_ROUTE_DETAIL> details = new List<C_ROUTE_DETAIL>();
            string RouteId = Data["Route_ID"].ToString();


            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE);
                details = table.returnRouteDetailByID(RouteId, sfcdb);
                ConstructReturns(ref StationReturn, "Success", "獲取路由詳細信息成功", details);
            }
            catch (Exception e)
            {
                ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
       

        public void CopyOneRoute(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_ROUTE_DETAIL table = null;
            string CopyRouteId = Data["CopyRouteId"].ToString();
            List<C_ROUTE_DETAIL> details = new List<C_ROUTE_DETAIL>();
            int count = 0;
            string result = string.Empty;
            

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE);
                details = table.returnCopyRouteDetailByID(CopyRouteId, sfcdb);
                count = table.CopyRoute(details, sfcdb);
               

                if (count !=0)
                {
                    ConstructReturns(
                        ref StationReturn, 
                        "Success", 
                        "成功拷貝流程", 
                        string.Format("成功更新 {0} 行數據！", count));
                }
                else
                {
                    ConstructReturns(ref StationReturn, "Fail", "拷貝流程失敗", "拷貝流程失敗");
                }
                

                
            }
            catch (Exception e)
            {
                ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
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
            List<C_ROUTE> routes = null;
            T_C_ROUTE table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_ROUTE(sfcdb, DB_TYPE);
                routes = table.GetRouteByName(RouteName, sfcdb);
                //routes = table.GetByRouteName(RouteName, sfcdb);

                ConstructReturns(
                    ref StationReturn, 
                    "Success", 
                    string.Format("成功獲取 {0} 路由描述", routes.Count()), 
                    routes);
            }
            catch (Exception e)
            {
                ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
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
            List<C_ROUTE> routes = null;
            string OtherCondition = Data["OTHER_CONDITION"].ToString();
            T_C_ROUTE table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_ROUTE(sfcdb, DB_TYPE);
                routes = table.GetRouteByOtherConditions(OtherCondition, sfcdb);

                ConstructReturns(
                    ref StationReturn, 
                    "Success", 
                    string.Format("成功獲取 {0} 路由描述", routes.Count()), 
                    routes);

            }
            catch (Exception e)
            {
                ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
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
            OleExec sfcdb = null;
            string RouteObject = string.Empty;
            C_ROUTE route = null;
            T_C_ROUTE table = null;
            Row_C_ROUTE row = null;
            string Operation = string.Empty;

            string JsonStr = @"
                {
                    'ID':'ROUTE_000000000000000000000000000001',
                    'ROUTE_NAME': 'U81B048T00111',
                    'DEFAULT_SKUNO': 'U81B048T00111',
                    'ROUTE_TYPE': 'TEST0012111',
                    'EDIT_TIME': '2017/11/29 10:00:00',
                    'EDIT_EMP': 'A0225204111'
                }
            ";

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_ROUTE(sfcdb, DB_TYPE);
                //RouteObject = Data["ROUTE_OBJECT"].ToString();
                Operation = Data["OPERATION"].ToString();
                route = (C_ROUTE)JsonConvert.Deserialize(JsonStr, typeof(C_ROUTE));
                row = (Row_C_ROUTE)table.NewRow();
                route.ApplyData(ref row);
                string result = table.UpdateRoute(row, Operation, sfcdb);

                if (!result.Contains("SQL"))
                {
                    ConstructReturns(
                        ref StationReturn, 
                        "Success", 
                        string.Format("成功更新 {0} 條路由信息！", Int32.Parse(result)),
                        result);
                }
                else
                {
                    ConstructReturns(ref StationReturn, "Fail", result, result);
                }
            
            }
            catch (Exception e)
            {
                ConstructReturns(ref StationReturn, "Fail", e.Message, e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        /// <summary>
        /// 構建返回到前端的結果對象
        /// </summary>
        /// <param name="StationReturn">最終返回到前端的結果對象</param>
        /// <param name="Status">狀態</param>
        /// <param name="Message">信息</param>
        /// <param name="Data">數據</param>
        public void ConstructReturns(ref MESStationReturn StationReturn, string Status, string Message, object Data)
        {
            StationReturn.Status = Status;
            StationReturn.Message = Message;
            StationReturn.Data = Data;
        }
        */
    }
}
