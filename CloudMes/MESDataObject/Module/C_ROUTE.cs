using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ROUTE : DataObjectTable
    {
        public T_C_ROUTE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_C_ROUTE);
            TableName = "c_route".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_C_ROUTE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROUTE);
            TableName = "c_route".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 通過路由ID獲取
        /// </summary>
        /// <param name="id">路由ID</param>
        /// <param name="DB"></param>
        /// <returns>找到就返回一個C_ROUTE對象，找不到就返回null</returns>
        public C_ROUTE GetById(string id, OleExec DB)
        {
            string strSql = $@"select * from c_route where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id",OleDbType.VarChar,240) };
            paramet[0].Value = id;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_ROUTE ret = (Row_C_ROUTE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        //add by LLF 2017-12-21 begin
        /// <summary>
        /// 通過料號獲取路由
        /// </summary>
        /// <param name="SkuID"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public DataObjectBase GetRouteBySkuno(string SkuID, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string StrSql = $@"select * from c_route c where c.id in (
select route_ID from r_sku_route where 
sku_ID = '{SkuID}' ) ";
            DataTable Table = DB.ExecSelect(StrSql).Tables[0];
            if (Table.Rows.Count == 1)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(Table.Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根據料號ID和工單類型獲取路由
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="DB"></param>
        /// <param name="RouteType"></param>
        /// <returns></returns>
        public C_ROUTE GetRouteBySkuIdAndWoType(string SkuId, OleExec DB, string RouteType = "")
        {
            return DB.ORM.Queryable<C_ROUTE, R_SKU_ROUTE>((cr, rsr) => cr.ID == rsr.ROUTE_ID)
                 .Where((cr, rsr) => rsr.SKU_ID == SkuId)
                 .WhereIF(RouteType.Length > 0, (cr, rsr) => cr.ROUTE_TYPE == RouteType)
                 .Select((cr, rsr) => cr).ToList().FirstOrDefault();
        }
        //add by LLF 2017-12-21 end

        /// <summary>
        /// 通過料號獲取默認路由
        /// </summary>
        /// <param name="SkuID"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public DataObjectBase GetDefaultRouteBySkuno(string SkuID, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string StrSql = $@"select * from c_route c 
                               where c.id in (select route_ID from r_sku_route 
                                              where default_flag='Y' and sku_ID = '{SkuID}' ) ";
            DataTable Table = DB.ExecSelect(StrSql).Tables[0];
            if (Table.Rows.Count == 1)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(Table.Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 通過路由名，不區分大小寫，模糊查找，為分頁而寫，當路由名為空時查找全部
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="RouteName">路由名</param>
        /// <param name="CurrentPage">當前頁（要獲取的第幾頁）</param>
        /// <param name="PageSize">每頁數據的行數</param>
        /// <param name="TotalData">總頁數</param>
        /// <returns>找到就返回 List<C_ROUTE>，找不到就返回NULL</returns>
        public List<C_ROUTE> GetByNameForPagination(OleExec DB, string RouteName, int CurrentPage, int PageSize, out int TotalData)
        {
            bool isGetAll = true;
            OleDbParameter[] paramet;
            DataTable res = new DataTable();
            List<C_ROUTE> getC_ROUTE = new List<C_ROUTE>();
            string strSql = $@"select count(*) from c_route a ";
            if (RouteName.Length > 0)
            {
                strSql = strSql + $@"where upper(a.route_name) like'%{RouteName}%'";
                isGetAll = false;
            }           
            TotalData = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text));
            strSql = $@"select * from (select rownum rnumber,a.* from c_route a ";           
            if (isGetAll)
            {               
                strSql = strSql + " order by edit_time desc)  where rnumber>((:CurrentPage-1)*:PageSize) and rnumber<=((:CurrentPage1-1)*:PageSize1+:PageSize2) order by edit_time desc";
                //oldb 的參數只能是按照順序對應，不能復用，
                paramet = new OleDbParameter[] {
                    new OleDbParameter(":CurrentPage", CurrentPage),
                    new OleDbParameter(":PageSize", PageSize),
                    new OleDbParameter(":CurrentPage1", CurrentPage),
                    new OleDbParameter(":PageSize1", PageSize),
                    new OleDbParameter(":PageSize2", PageSize)
                };
                res= DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            }
            else
            {
                
                strSql = strSql + $@" where  upper(a.route_name) like'%{RouteName}%' order by edit_time desc) where rnumber>((:CurrentPage-1)*:PageSize) and rnumber<=((:CurrentPage1-1)*:PageSize1+:PageSize2) order by edit_time desc";
                //oldb 的參數只能是按照順序對應，不能復用，
                paramet = new OleDbParameter[] {                    
                    new OleDbParameter(":CurrentPage", CurrentPage),
                    new OleDbParameter(":PageSize", PageSize),
                    new OleDbParameter(":CurrentPage1", CurrentPage),
                    new OleDbParameter(":PageSize1", PageSize),
                    new OleDbParameter(":PageSize2", PageSize)
                };
                res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            }               
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ROUTE ret = (Row_C_ROUTE)NewRow();
                    ret.loadData(res.Rows[i]);
                    getC_ROUTE.Add(ret.GetDataObject());
                }
                return getC_ROUTE;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通過路由名獲取
        /// </summary>
        /// <param name="RouteName">路由名</param>
        /// <param name="DB"></param>
        /// <returns>查找到就返回C_ROUTE對象，查找不到返回NULL</returns>
        public C_ROUTE GetByRouteName(string RouteName, OleExec DB)
        {
            string strSql = $@"select * from c_route where route_name=:RouteName";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RouteName", RouteName) };
            DataTable table = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (table.Rows.Count > 0)
            {
                Row_C_ROUTE ret = (Row_C_ROUTE)NewRow();
                ret.loadData(table.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 添加新的路由
        /// </summary>
        /// <param name="newRoute">新的路由</param>
        /// <param name="DB"></param>
        /// <returns>成功返回大於零的數，失敗返回0</returns>
        public int Add(C_ROUTE newRoute, OleExec DB)
        {
            string strSql = $@"insert into c_route(id,route_name,default_skuno,route_type,edit_time,edit_emp)";
            strSql = strSql + $@"values(:id,:route_name,:default_skuno,:route_type,:edit_time,:edit_emp)";
            OleDbParameter[] paramet = new OleDbParameter[6];
            paramet[0] = new OleDbParameter(":id", newRoute.ID);
            paramet[1] = new OleDbParameter(":route_name", newRoute.ROUTE_NAME);
            paramet[2] = new OleDbParameter(":default_skuno", newRoute.DEFAULT_SKUNO);
            paramet[3] = new OleDbParameter(":route_type", newRoute.ROUTE_TYPE);
            paramet[4] = new OleDbParameter(":edit_time", newRoute.EDIT_TIME);
            paramet[5] = new OleDbParameter(":edit_emp", newRoute.EDIT_EMP);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過ID更新路由
        /// </summary>
        /// <param name="newRoute"></param>
        /// <param name="DB"></param>
        /// <returns>成功返回大於零的數，失敗返回0</returns>
        public int UpdateById(C_ROUTE newRoute, OleExec DB)
        {
            string strSql = $@"update c_route set route_name=:route_name ,default_skuno=:default_skuno ,route_type=:route_type ,edit_time=:edit_time ,edit_emp=:edit_emp where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[6];
            paramet[0] = new OleDbParameter(":route_name", newRoute.ROUTE_NAME);
            paramet[1] = new OleDbParameter(":default_skuno", newRoute.DEFAULT_SKUNO);
            paramet[2] = new OleDbParameter(":route_type", newRoute.ROUTE_TYPE);
            paramet[3] = new OleDbParameter(":edit_time", newRoute.EDIT_TIME);
            paramet[4] = new OleDbParameter(":edit_emp", newRoute.EDIT_EMP);
            paramet[5] = new OleDbParameter(":id", newRoute.ID);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過ID刪除路由
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteById(string id, OleExec DB)
        {
            string strSql = $@"delete c_route  where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":id", id);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 獲取新的默認路由名稱
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetNewRouteName(OleExec DB)
        {
            string strSql = $@"select :NewRoute||(max(route_name)+1) as NewRouteName from(" +
                               $@"select to_number(substr(route_name,9)) as route_name from c_route where regexp_like(route_name,:Regex))";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":NewRoute", "NewRoute");
            paramet[1] = new OleDbParameter(":Regex", "^NewRoute([0-9]+)$");
            string res = DB.ExecuteScalar(strSql, CommandType.Text,paramet);
            if (res == null || res.Length <= 0)
            {
                res = "NewRoute0";
            }
            return res;
        }
        /// <summary>
        /// 通過機種ID獲取機種可用路由
        /// </summary>
        /// <param name="skuid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_ROUTE> GetAvailableRoutesForSkuBySkuid(string skuid,OleExec DB)
        {
            string strSql = $@"select * from c_route c  where (c.default_skuno is null or c.default_skuno in("+
                            $@"select skuno from c_sku where id=:skuid))and c.id not in(" +
                            $@"select B.ROUTE_ID from c_sku a,r_sku_route b where a.id=B.SKU_ID and A.id=:skuid1)";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":skuid", skuid);
            paramet[1] = new OleDbParameter(":skuid1", skuid);
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text,paramet);
            List<C_ROUTE> getC_ROUTE = new List<C_ROUTE>();
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ROUTE ret = (Row_C_ROUTE)NewRow();
                    ret.loadData(res.Rows[i]);
                    getC_ROUTE.Add(ret.GetDataObject());
                }
                return getC_ROUTE;
            }
            else
            {
                return null;
            }
        }

        public List<string> GetRouteBySkuno(OleExec DB, string skuno)
        {
            DataTable dt = null;
            //Row_C_ROUTE row_route = null;
            //List<C_ROUTE> routes = new List<C_ROUTE>();
            List<string> routes = new List<string>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select r.* from c_sku s 
                    left join r_sku_route sr on s.id=sr.sku_id 
                    left join c_route r on sr.route_id=r.id 
                    where s.skuno='{skuno}' ";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    //row_route = (Row_C_ROUTE) NewRow();
                    //row_route.loadData(dr);
                    //routes.Add(row_route.GetDataObject());
                    routes.Add(dr["route_name"].ToString());
                }
                return routes;
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }
        }

        // <summary>
        /// WZW Check 是否存在C_ROUTE表
        /// </summary>
        /// <param name="RouteName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_ROUTE CheckisExisRoute(string RouteID, OleExec DB)
        {
            string strSql = $@"select * from c_route where ID=:RouteID";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RouteID", RouteID) };
            DataTable table = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (table.Rows.Count > 0)
            {
                Row_C_ROUTE ret = (Row_C_ROUTE)NewRow();
                ret.loadData(table.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据路由类型返回路由名
        /// </summary>
        /// <param name="ParametValue"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_ROUTE> GetByROUTETYPE(string ROUTE_TYPE, string ROUTE_NAEM, OleExec DB)
        {
            string strSql = $@"select *from C_ROUTE where upper(ROUTE_NAME) like'%{ROUTE_NAEM}%'AND  upper(ROUTE_TYPE)='" + ROUTE_TYPE + "'";
            List<C_ROUTE> result = new List<C_ROUTE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ROUTE ret = (Row_C_ROUTE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 通過機種找到測試路由的信息
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public C_ROUTE GetTestRouteBySkuno(string skuno, OleExec db)
        {

            //return db.ORM.Queryable<C_ROUTE, C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>((r, s, sr, rd) => s.SKUNO == skuno && s.ID == sr.SKU_ID && r.ID == sr.TEST_ROUTE_ID && rd.ROUTE_ID == sr.TEST_ROUTE_ID && r.ROUTE_FLAG == "1")
            //.Select((r, s, sr, rd) => r).ToList().FirstOrDefault();
            //return db.ORM.Queryable<C_ROUTE, C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>((r, s, sr, rd) => s.SKUNO == skuno && s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID && rd.ROUTE_ID == sr.TEST_ROUTE_ID && r.ROUTE_TYPE == "TEST")
            //.Select((r, s, sr, rd) => r).ToList().FirstOrDefault();

            return db.ORM.Queryable<C_ROUTE, C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>((r, s, sr, rd) => s.SKUNO == skuno && s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID && rd.ROUTE_ID == sr.ROUTE_ID && r.ROUTE_TYPE == "TEST")
            .Select((r, s, sr, rd) => r).ToList().FirstOrDefault();
        }
        /// <summary>
        /// 通過機種找到路由的信息
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public C_ROUTE GetRouteBySkuno(string skuno, OleExec db)
        {

            //return db.ORM.Queryable<C_ROUTE, C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>((r, s, sr, rd) => s.SKUNO == skuno && s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID && rd.ROUTE_ID == sr.ROUTE_ID && r.ROUTE_FLAG == "0")
            //.Select((r, s, sr, rd) => r).ToList().FirstOrDefault();
            return db.ORM.Queryable<C_ROUTE, C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>((r, s, sr, rd) => s.SKUNO == skuno && s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID && rd.ROUTE_ID == sr.ROUTE_ID && r.ROUTE_TYPE == "SFC")
                .Select((r, s, sr, rd) => r).ToList().FirstOrDefault();
        }
        public int GetSKUROUTETYPECHECK(string SKUK_ID, string TYPE, OleExec sfcdb)
        {
            string SQLROUTE = $@"SELECT * FROM C_ROUTE WHERE ID IN (
SELECT ROUTE_ID FROM R_SKU_ROUTE WHERE SKU_ID='{SKUK_ID}' AND ROUTE_TYPE = '{TYPE}')";
            DataTable dtroute = sfcdb.ExecSelect(SQLROUTE).Tables[0];
            return dtroute.Rows.Count;
        }

    }
    public class Row_C_ROUTE : DataObjectBase
    {
        public Row_C_ROUTE(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE GetDataObject()
        {
            C_ROUTE DataObject = new C_ROUTE();
            DataObject.ID = this.ID;
            DataObject.ROUTE_NAME = this.ROUTE_NAME;
            DataObject.DEFAULT_SKUNO = this.DEFAULT_SKUNO;
            DataObject.ROUTE_TYPE = this.ROUTE_TYPE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string ROUTE_NAME
        {
            get
            {
                return (string)this["ROUTE_NAME"];
            }
            set
            {
                this["ROUTE_NAME"] = value;
            }
        }
        public string DEFAULT_SKUNO
        {
            get
            {
                return (string)this["DEFAULT_SKUNO"];
            }
            set
            {
                this["DEFAULT_SKUNO"] = value;
            }
        }
        public string ROUTE_TYPE
        {
            get
            {
                return (string)this["ROUTE_TYPE"];
            }
            set
            {
                this["ROUTE_TYPE"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class C_ROUTE
    {
        public string ID{get;set;}
        public string ROUTE_NAME{get;set;}
        public string DEFAULT_SKUNO{get;set;}
        public string ROUTE_TYPE{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}