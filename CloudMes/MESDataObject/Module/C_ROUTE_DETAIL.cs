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
    public class T_C_ROUTE_DETAIL : DataObjectTable
    {
        public T_C_ROUTE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_T_C_ROUTE_DETAIL);
            TableName = "c_route_detail".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_C_ROUTE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_T_C_ROUTE_DETAIL);
            TableName = "c_route_detail".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 取Station之前的測試工站,包括當前工站
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <param name="routeId"></param>
        /// <param name="CurrentStation"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetTestStationByNameBefor(OleExec DB,string routeId, string CurrentStation)
        {
            List<C_ROUTE_DETAIL> r = new List<C_ROUTE_DETAIL>();
            string strSql = $@" select a.* from c_route_detail a,c_temes_station_mapping b,c_route_detail c where a.route_id=:id
                            and c.station_name=:station_name and c.route_id =a.route_id and a.seq_no<=c.seq_no and a.station_name=b.mes_station order by A.seq_no asc ";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id", OleDbType.VarChar, 240),
                new OleDbParameter(":station_name", OleDbType.VarChar, 20)
            };
            paramet[0].Value = routeId;
            paramet[1].Value = CurrentStation;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            foreach (DataRow item in res.Rows)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(item);
                r.Add(ret.GetDataObject());
            }
            return r;
        }

        /// <summary>
        /// 取Station之前的工站,不包括當前工站
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <param name="routeId"></param>
        /// <param name="CurrentStation"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetStationByNameBefor(OleExec DB, string routeId, string CurrentStation)
        {
            List<C_ROUTE_DETAIL> r = new List<C_ROUTE_DETAIL>();
            string strSql = $@" select a.* from c_route_detail a,c_route_detail c where a.route_id=:id
                            and c.station_name=:station_name and c.route_id =a.route_id and a.seq_no<c.seq_no  order by A.seq_no asc ";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id", OleDbType.VarChar, 240),
                new OleDbParameter(":station_name", OleDbType.VarChar, 20)
            };
            paramet[0].Value = routeId;
            paramet[1].Value = CurrentStation;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            foreach (DataRow item in res.Rows)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(item);
                r.Add(ret.GetDataObject());
            }
            return r;
        }

        public C_ROUTE_DETAIL getroutename(string skuid, OleExec DB)
        {
            string strSql = $@"select station_name from c_route_detail where route_id in(
                            select id from c_route where route_name=:skuid)and rownum=1 order by seq_no";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":skuid", OleDbType.VarChar, 240) };
            paramet[0].Value = skuid;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }

        }

        public bool CheckPaste(string id, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail where route_id='{id}' and station_name='PASTE'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", OleDbType.VarChar, 240) };
            paramet[0].Value = id;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 通過id獲取
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_ROUTE_DETAIL GetById(string id, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id",OleDbType.VarChar, 240) };
            paramet[0].Value = id;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通過路由ID獲取所有C_ROUTE_DETAIL，並且order by seq_no asc
        /// </summary>
        /// <param name="RouteID">路由ID</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetByRouteIdOrderBySEQASC(string RouteID, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail where route_id=:RouteID order by seq_no asc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RouteID",OleDbType.VarChar, 240) };
            paramet[0].Value = RouteID;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 20190222 Patty added for FTX Oracle: only show available stations (ASSYs) in route detail
        /// </summary>
        /// <param name="RouteID">路由ID</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> ORAGetPreviousByRouteId(string RouteID, String NextStation, OleExec DB)
        {       
            string strSql = $@"select * from C_ROUTE_DETAIl where ROUTE_ID=:RouteID and SEQ_NO <= (select SEQ_NO from C_ROUTE_DETAIL where  ROUTE_ID=:RouteID and STATION_NAME=:CurrentStation) order by seq_no asc ";
            //string strSql = $@"select * from C_ROUTE_DETAIl where ROUTE_ID=:RouteID and STATION_NAME like '%ASSY%' and SEQ_NO <= (select SEQ_NO from C_ROUTE_DETAIL where  ROUTE_ID=:RouteID and STATION_NAME=:CurrentStation) order by seq_no asc ";

            OleDbParameter[] paramet = new OleDbParameter[] 
            {
                new OleDbParameter(":RouteID", OleDbType.VarChar, 240),
                new OleDbParameter(":CurrentStation", OleDbType.VarChar, 240)
            };
            paramet[0].Value = RouteID;
            paramet[1].Value = NextStation;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通過路由ID和當前工站名稱首先獲取當前工站ID,
        /// 再根據此ID獲取DirectLink中跳站ID,再根據跳站ID獲取C_ROUTE_DETAIL內容
        /// </summary>
        /// <param name="RouteID">路由ID</param>
        /// <param name="CurrentStation">R_SN中CurrentStation值</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        /// 
        public List<C_ROUTE_DETAIL> GetRouteDetailByDirectLinkID(string RouteID, string CurrentStation, OleExec DB)
        {
            string strSql = $@"SELECT * FROM SFCBASE.C_ROUTE_DETAIL WHERE ID IN (
                   SELECT DIRECTLINK_ROUTE_DETAIL_ID FROM SFCBASE.C_ROUTE_DETAIL_DIRECTLINK WHERE C_ROUTE_DETAIL_ID IN(
                   SELECT ID FROM SFCBASE.C_ROUTE_DETAIL WHERE ROUTE_ID = :RouteID AND STATION_NAME = :CurrentStation))";
            OleDbParameter[] paramet = new OleDbParameter[]
            {
                new OleDbParameter(":RouteID", OleDbType.VarChar, 240),
                new OleDbParameter(":CurrentStation", OleDbType.VarChar, 240)
            };
            paramet[0].Value = RouteID;
            paramet[1].Value = CurrentStation;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }
        public List<C_ROUTE_DETAIL> GetByRouteIdOrderBySeqDesc(string RouteId, OleExec DB)
        {
            return DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RouteId).OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Desc).ToList();
        }
        /// <summary>
        /// 添加C_ROUTE_DETAIL
        /// </summary>
        /// <param name="newc_route_detail"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int Add(C_ROUTE_DETAIL newc_route_detail, OleExec DB)
        {
            string strSql = $@"insert into c_route_detail(id,seq_no,route_id,station_name,station_type,return_flag)";
            strSql = strSql + $@"values(:id,:seq_no,:route_id,:station_name,:station_type,:return_flag)";
            OleDbParameter[] paramet = new OleDbParameter[6];
            paramet[0] = new OleDbParameter(":id", newc_route_detail.ID);
            paramet[1] = new OleDbParameter(":seq_no", newc_route_detail.SEQ_NO);
            paramet[2] = new OleDbParameter(":route_id", newc_route_detail.ROUTE_ID);
            paramet[3] = new OleDbParameter(":station_name", newc_route_detail.STATION_NAME);
            paramet[4] = new OleDbParameter(":station_type", newc_route_detail.STATION_TYPE);
            paramet[5] = new OleDbParameter(":return_flag", newc_route_detail.RETURN_FLAG);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過ID刪除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteById(string id, OleExec DB)
        {
            string strSql = $@"delete c_route_detail where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":id", id);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過路由ID刪除
        /// </summary>
        /// <param name="routeid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteByRouteId(string routeid, OleExec DB)
        {
            string strSql = $@"delete c_route_detail where route_id=:route_id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":route_id", routeid);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過ID更新
        /// </summary>
        /// <param name="updateitem"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateById(C_ROUTE_DETAIL updateitem, OleExec DB)
        {
            string strSql = $@"update c_route_detail set seq_no=:seq_no,station_name=:station_name,station_type=:station_type,return_flag=:return_flag where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[5];
            paramet[0] = new OleDbParameter(":seq_no", updateitem.SEQ_NO);
            paramet[1] = new OleDbParameter(":station_name", updateitem.STATION_NAME);
            paramet[2] = new OleDbParameter(":station_type", updateitem.STATION_TYPE);
            paramet[3] = new OleDbParameter(":return_flag", updateitem.RETURN_FLAG);
            paramet[4] = new OleDbParameter(":id", updateitem.ID);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

        public Dictionary<string,object> GetNextStations(string RouteId,string CurrentStation,OleExec DB)
        {
            Dictionary<string, object> Routes = new Dictionary<string, object>();
            List<string> NextStations = new List<string>();
            List<string> Returns = new List<string>();
            List<string> DirectLinks = new List<string>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string CurrentStationSeq = string.Empty;
            string CurrentStationId = string.Empty;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND STATION_NAME='{CurrentStation}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    CurrentStationId = dt.Rows[0]["ID"].ToString();
                    CurrentStationSeq = dt.Rows[0]["SEQ_NO"].ToString();
                    sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND SEQ_NO>'{CurrentStationSeq}' ORDER BY SEQ_NO";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        NextStations.Add(dt.Rows[0]["STATION_NAME"].ToString());
                    }
                    sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ID IN (
                        SELECT DIRECTLINK_ROUTE_DETAIL_ID FROM C_ROUTE_DETAIL_DIRECTLINK WHERE C_ROUTE_DETAIL_ID='{CurrentStationId}')";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            DirectLinks.Add(dr["STATION_NAME"].ToString());
                        }
                    }

                    sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ID IN (
                        SELECT RETURN_ROUTE_DETAIL_ID FROM C_ROUTE_DETAIL_RETURN WHERE ROUTE_DETAIL_ID='{CurrentStationId}')";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Returns.Add(dr["STATION_NAME"].ToString());
                        }
                    }

                    Routes.Add("NextStations", NextStations);
                    Routes.Add("Returns", Returns);
                    Routes.Add("DirectLinks", DirectLinks);
                }
                else if (dt.Rows.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000154",new string[] { CurrentStation}));
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000153",new string[] { CurrentStation }));
                }
                

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return Routes;
        }

        public DataTable GetALLStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            string sql = $@"select distinct station_name from c_route_detail order by station_name";
            dt = db.ExecSelect(sql).Tables[0];
            return dt;
        }
        

        public bool StationInRoute(string routeID, string stationName, OleExec sfcdb)
        {
            string sql = $@"select * from c_route_detail where route_id='{routeID}' and station_name='{stationName}'";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public C_ROUTE_DETAIL GetSnStation(string ROUTEID,String STATIONNAME, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"SELECT distinct a.seq_no,a.STATION_NAME FROM C_ROUTE_DETAIL a,C_ROUTE_DETAIL b where a.seq_no<b.seq_no and a.ROUTE_ID='{ROUTEID}'and b.STATION_NAME='{STATIONNAME}' and rownum=1 order by a.seq_no desc";
                DataTable result = DB.ExecuteDataTable(strsql, CommandType.Text);
                if (result.Rows.Count > 0)
                {
                    return CreateLanguageClass(result.Rows[0]);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public C_ROUTE_DETAIL GetSnstationDetail(string routeid, string ReturnStation, string NextStation, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" SELECT *
                        FROM c_route_detail
                        WHERE route_id = '{routeid}'
                            AND station_name IN
                                (SELECT station_name
                                FROM c_route_detail
                                WHERE route_id =  '{routeid}'
                                    AND seq_no >
                                        (SELECT seq_no
                                        FROM c_route_detail
                                        WHERE route_id =  '{routeid}'
                                            AND station_name = '{ReturnStation}')
                                    AND seq_no <=
                                        (SELECT seq_no
                                        FROM c_route_detail
                                        WHERE route_id =  '{routeid}'
                                            AND station_name = '{NextStation}'))
                            AND station_name = '{NextStation}'";
                DataTable result = DB.ExecuteDataTable(strsql, CommandType.Text);
                if (result.Rows.Count > 0)
                {
                    return CreateLanguageClass(result.Rows[0]);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        

        public C_ROUTE_DETAIL CreateLanguageClass(DataRow dr)
        {
            Row_C_ROUTE_DETAIL row = (Row_C_ROUTE_DETAIL)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }
        /// <summary>
        /// 檢查當前工站的上一站是不是某一個工站
        /// 如檢查ASSY的上一個工站是不是ICT
        /// </summary>
        /// <param name="routeID"></param>
        /// <param name="stationName"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool CheckLaststationIs(string routeID, string stationName, string LaststationName, OleExec sfcdb)
        {
            string sql = $@"select *
  from (select *
          from c_route_detail
         where route_id = '{routeID}'
           and SEQ_NO <
               (select SEQ_NO
                  from c_route_detail
                 where route_id = '{routeID}'
                   and station_name = '{stationName}')
         order by SEQ_NO desc)
 where rownum = 1
   and STATION_NAME = '{LaststationName}'";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool IsJobStockin(string routeID, string stationName, OleExec sfcdb)
        {
            string sql = $@"select * from c_route_detail where route_id='{routeID}' and station_name='{stationName}' and station_type='JOBSTOCKIN' ";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<C_ROUTE_DETAIL> GetLastStations(string routeId, string currentStation, OleExec DB)
        {
            return DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(d => d.ROUTE_ID == routeId && SqlSugar.SqlFunc.Subqueryable<C_ROUTE_DETAIL>()
              .Where(dd => dd.ROUTE_ID == d.ROUTE_ID && dd.STATION_NAME == currentStation && d.SEQ_NO < dd.SEQ_NO).Any()).OrderBy(d => d.SEQ_NO).ToList();
        }

        public List<C_ROUTE_DETAIL> GetAllNextStationsByCurrentStation(string routeId, string currentStation, OleExec DB)
        {   
            return DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(d => d.ROUTE_ID == routeId && SqlSugar.SqlFunc.Subqueryable<C_ROUTE_DETAIL>()
              .Where(dd => dd.ROUTE_ID == d.ROUTE_ID && dd.STATION_NAME == currentStation && d.SEQ_NO >= dd.SEQ_NO).Any()).OrderBy(d => d.SEQ_NO).ToList();
        }
        /// <summary>
        /// 獲取當前站位的返修站
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="CurrentStation"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetReturnStation(string RouteId, string CurrentStation, OleExec DB)
        {
            string ReturnStation = string.Empty;
            C_ROUTE_DETAIL ReturnRouteDetail=DB.ORM.Queryable<C_ROUTE_DETAIL, C_ROUTE_DETAIL_RETURN, C_ROUTE_DETAIL>((rd1, rdr, rd2) => rd1.ID == rdr.ROUTE_DETAIL_ID && rdr.RETURN_ROUTE_DETAIL_ID == rd2.ID)
                .Where((rd1, rdr, rd2) => rd1.ROUTE_ID == RouteId && rd1.STATION_NAME == CurrentStation).Select((rd1, rdr, rd2) => rd2).ToList().FirstOrDefault();
            if (ReturnRouteDetail != null)
            {
                ReturnStation = ReturnRouteDetail.STATION_NAME;
            }
            return ReturnStation;

        }

        /// <summary>
        /// 檢查流程裡面是否同時有 3DX 和 ICT 並且 ICT 在 3DX 後面一站
        /// 此處是為了當在 ICT 測試不良，維修清號時，如果不良原因是連接器不良，那麼就回到 3DX 重測，否則就去 ICT 重測
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSpecialReturn(string RouteId, OleExec DB)
        {
            bool result = false;
            List<C_ROUTE_DETAIL> RouteDetails = DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RouteId).OrderBy(t => t.SEQ_NO).ToList();
            C_ROUTE_DETAIL _3DX = RouteDetails.Find(t => t.STATION_NAME == "3DX");
            if (_3DX != null)
            {
                C_ROUTE_DETAIL _ICT = RouteDetails.Find(t => t.SEQ_NO > _3DX.SEQ_NO);
                if (_ICT.STATION_NAME.Equals("ICT"))
                {
                    result = true; 
                }
            }

            return result;
        }

        /// <summary>
        /// 此處是為了當在 ICT 測試不良，維修清號時，如果不良原因是連接器不良，那麼就回到 3DX 重測，否則就去 ICT 重測
        /// 使用 C_CONTROL 中 CONTROL_NAME 是 SPECIAL_REPAIR_RETURN_STATION 的記錄
        /// 配置為 CONTROL_VALUE 為特定的不良代碼，CONTROL_TYPE 為 管控的站位，此處就是對應 ICT，CONTROL_LEVEL 為 返修站位 3DX
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="CurrentStation"></param>
        /// <param name="FailCode"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetSpecialReturnStation(string RouteId, string CurrentStation,string FailCode, OleExec DB)
        {
            string ReturnStation=GetReturnStation(RouteId, CurrentStation, DB);
            if (CheckSpecialReturn(RouteId, DB))
            {
                C_CONTROL control = DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "SPECIAL_REPAIR_RETURN_STATION").ToList().FirstOrDefault();
                if (control != null)
                {
                    if (control.CONTROL_VALUE.Contains(FailCode) && CurrentStation.Equals(control.CONTROL_TYPE))
                    {
                        ReturnStation = GetReturnStation(RouteId, control.CONTROL_LEVEL, DB);
                    }
                   
                }
               
            }
            return ReturnStation;
        }

        //public DataTable GetSNLStation(string SN, OleExec db)
        //{
        //    List<string> station = new List<string>();
        //    DataTable dt = new DataTable();

        //    string sql = $@"select * from (
        //    select b.STATION_NAME,b.SEQ_NO,a.CURRENT_STATION,a.NEXT_STATION,a.workorderno from
        //    R_SN A left  join  C_ROUTE_DETAIL b  on a.ROUTE_ID=b.ROUTE_ID where a.SN in
        //    (select SN from R_SN  where SN in('{SN}') 
        //    and VALID_FLAG='1' ) and STATION_NAME!='ICT'AND A.VALID_FLAG='1' 
        //    union 
        //    select b.STATION_NAME,b.SEQ_NO,G.CURRENT_STATION,G.NEXT_STATION,G.workorderno from 
        //    (select  a.SN,a.CURRENT_STATION,a.NEXT_STATION,a.workorderno ,F.ROUTE_ID from r_sn  a
        //    left join c_kp_list_item d on a.skuno = d.KP_PARTNO left join C_SKU e on e.skuno in (select skuno from c_kp_list where id=d.list_id)  LEFT JOIN R_SKU_ROUTE F ON E.ID = F.SKU_ID where a.SN in(select SN from R_SN
        //    where SN in ('{SN}')and valid_flag=1 ) AND ROWNUM=1 and valid_flag=1) G
        //    left  join  C_ROUTE_DETAIL b on b.route_id=G.ROUTE_ID 
        //    union
        //    select b.STATION_NAME,b.SEQ_NO,a.CURRENT_STATION,a.NEXT_STATION,a.workorderno from
        //    (select  a.SN,a.CURRENT_STATION,a.NEXT_STATION,a.workorderno ,F.ROUTE_ID from r_sn a
        //    left join  C_KEYPART d on a.skuno =d.skuno left join  c_sku e on e.skuno =d.skuno LEFT JOIN R_SKU_ROUTE F ON E.ID=F.SKU_ID  
        //    where a.sn in(select sn from r_sn  where sn in('{SN}')  AND VALID_FLAG=1)AND ROWNUM=1 and A.valid_flag=1) a left  join  
        //    C_ROUTE_DETAIL b on b.route_id=a.ROUTE_ID  ) a    where  STATION_NAME IS NOT NULL order by SEQ_NO asc";

        //    //string sql = $@"  SELECT *FROM (
        //    //select B.*
        //    //from R_SN a  left outer join  C_ROUTE_DETAIL b on a.route_id=b.route_id where
        //    //SN in (   select SN from R_SN where SN in ('{SN}') ) and STATION_NAME<>'ICT'  union
        //    //select B.* from (select  a.* from R_SN a  left join   C_SKU c on a.SKUNO=c.SKUNO  left join C_KP_LIST_ITEM_CHECK d on C.C_SERIES_ID =d.C_KP_LIST_ITEM_ID 
        //    //left join  C_SKU e on e.C_SERIES_ID =d.ID where a.SN in( select SN from R_SN where SN in ('{SN}') )) a left  join  C_ROUTE_DETAIL b  on b.route_id=a.route_id  ) a  where  STATION_NAME IS NOT NULL  order by SEQ_NO asc";
        //    dt = db.ExecSelect(sql).Tables[0];
        //    return dt;
        //}

        public List<C_ROUTE_DETAIL> GetWoLStation(string wo, OleExec DB)
        {
            //string strSql = $@"SELECT *FROM (
            //select B.*
            //from R_SN a  left outer join  C_ROUTE_DETAIL b on a.route_id=b.route_id where
            //SN in (   select SN from R_SN where workorderno=:wo ) and STATION_NAME<>'ICT'  union
            //select B.* from (select  a.* from R_SN a  left join   C_SKU c on a.SKUNO=c.SKUNO  left join C_KP_LIST_ITEM_CHECK d on C.C_SERIES_ID =d.C_KP_LIST_ITEM_ID 
            //left join  C_SKU e on e.C_SERIES_ID =d.ID where a.SN in( select SN from R_SN where workorderno=:wo )) a left  join  C_ROUTE_DETAIL b  on b.route_id=a.route_id  ) a  where  STATION_NAME IS NOT NULL  order by SEQ_NO asc";

            string strSql = $@"select STATION_NAME,SEQ_NO,route_id From ( select * From C_ROUTE_DETAIL where route_id in(
              select ROUTE_ID from r_wo_base where workorderno='{wo}' )
              and STATION_NAME!= 'ICT' union select *From C_ROUTE_DETAIL where route_id in(
              select a.route_id from r_wo_base a, C_SKU b
              where a.workorderno='{wo}'  and a.skuno = b.CUST_PARTNO )) a order by SEQ_NO ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }

        }
        public DataTable GetWoLStation1(string wo, OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            string sql = $@"  SELECT *FROM (
            select B.*
            from R_SN a  left outer join  C_ROUTE_DETAIL b on a.route_id=b.route_id where
            SN in (   select SN from R_SN where workorderno in ('{wo}') ) and STATION_NAME<>'ICT'  union
            select B.* from (select  a.* from R_SN a  left join   C_SKU c on a.SKUNO=c.SKUNO  left join C_KP_LIST_ITEM_CHECK d on C.C_SERIES_ID =d.C_KP_LIST_ITEM_ID 
            left join  C_SKU e on e.C_SERIES_ID =d.ID where a.SN in( select SN from R_SN where workorderno in ('{wo}') )) a left  join  C_ROUTE_DETAIL b  on b.route_id=a.route_id  ) a  where  STATION_NAME IS NOT NULL  order by SEQ_NO asc";
            dt = db.ExecSelect(sql).Tables[0];
            return dt;
        }

        public C_ROUTE_DETAIL GetRowByRouteIDAndNotInStation(string stationName, string routeID, OleExec db)
        {
            string strSql = $@" select * from c_route_detail where route_id = '{routeID}' and station_name <> '{stationName}' order by seq_no desc";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_ROUTE_DETAIL result = new C_ROUTE_DETAIL();
            if (table.Rows.Count > 0)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetBySN(string Sn, string RouteID, OleExec DB)
        {
            bool res = false;
            string strSQL = $@"select * from C_ROUTE_DETAIL where route_id='{RouteID}' and STATION_NAME='ICT' and ROWNUM=1";
            DataTable Dt = DB.ExecSelect(strSQL).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }

            //int Num = DB.ExecSqlNoReturn(strSQL, null);
            //if (Num > 0)
            //{
            //    res = true;
            //}
            return res;
        }

        public C_ROUTE_DETAIL GetStationRoute(string routeID, string stationName, OleExec sfcdb)
        {
            string sql = $@"select * from c_route_detail where route_id='{routeID}' and station_name='{stationName}'";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            C_ROUTE_DETAIL result = null;
            if (dt.Rows.Count > 0)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)this.NewRow();
                ret.loadData(dt.Rows[0]);
                result = ret.GetDataObject();
            }

            return result;
        }

        /// <summary>
        /// 取Rework之前的工站,不包括當前工站
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetStationListBeforeReworkStation(OleExec DB, string routeId, string StationName)
        {
            List<C_ROUTE_DETAIL> r = new List<C_ROUTE_DETAIL>();
            string strSql = $@" select a.* from c_route_detail a,c_route_detail c where a.route_id=:id
                            and c.station_name=:station_name and c.route_id =a.route_id and a.seq_no<c.seq_no order by A.seq_no asc ";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id", OleDbType.VarChar, 240),
                new OleDbParameter(":station_name", OleDbType.VarChar, 20)
            };
            paramet[0].Value = routeId;
            paramet[1].Value = StationName;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            foreach (DataRow item in res.Rows)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(item);
                r.Add(ret.GetDataObject());
            }
            return r;
        }

        /// <summary>
        /// 取Rework之后的工站,包括當前工站
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetStationListAfterReworkStation(OleExec DB, string routeId, string StationName)
        {
            List<C_ROUTE_DETAIL> r = new List<C_ROUTE_DETAIL>();
            string strSql = $@" select a.* from c_route_detail a,c_route_detail c where a.route_id=:id
                            and c.station_name=:station_name and c.route_id =a.route_id and a.seq_no>=c.seq_no order by A.seq_no asc ";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id", OleDbType.VarChar, 240),
                new OleDbParameter(":station_name", OleDbType.VarChar, 20)
            };
            paramet[0].Value = routeId;
            paramet[1].Value = StationName;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            foreach (DataRow item in res.Rows)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(item);
                r.Add(ret.GetDataObject());
            }
            return r;
        }
        /// <summary>
        /// WZW 为满足用户要求输入不定个数工站名查询对应路由（显示内容为该路由的所有工站以指定工站排序后）
        /// </summary>
        /// <param name="StationName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable GetStatinNameByRouteName(string[] StationName, string TYPE, OleExec DB)
        {
            string StationNameSql = "";
            #region
            //            for (int i = 0; i < StationName.Length; i++)
            //            {
            //                if (i == 0)
            //                {
            //                    StationNameSql += StationNameSql = $@" STATION_NAME = '" + StationName[i] + "' ";
            //                }
            //                else
            //                {
            //                    StationNameSql += StationNameSql = $@" OR STATION_NAME = '" + StationName[i] + "' ";
            //                }
            //            }
            //            if (StationNameSql == "")
            //            {
            //                return null;
            //            }
            //            string StrSql = $@"SELECT ROUTE_ID,SEQ_NO,station_name,ROW_NUMBER() OVER(PARTITION BY ROUTE_ID ORDER BY SEQ_NO) AS RN  FROM C_ROUTE_DETAIL 
            //where ROUTE_ID in (
            //select distinct ROUTE_ID from C_ROUTE_DETAIL 
            //where " + StationNameSql + " )";
            #endregion
            for (int i = 0; i < StationName.Length; i++)
            {
                if (i == StationName.Length - 1)
                {
                    StationNameSql += StationNameSql = $@" '" + StationName[i] + "' ";
                }
                else
                {
                    StationNameSql += StationNameSql = $@" '" + StationName[i] + "' , ";
                }
            }
            if (StationNameSql == "")
            {
                return null;
            }
            //           string StrSql = $@"SELECT ROUTE_ID, SEQ_NO, STATION_NAME, ROW_NUMBER() OVER(PARTITION BY ROUTE_ID ORDER BY SEQ_NO) AS RN FROM C_ROUTE_DETAIL WHERE ROUTE_ID IN(
            //SELECT T.ROUTE_ID FROM(
            //SELECT ROUTE_ID, COUNT(1) FROM C_ROUTE_DETAIL WHERE STATION_NAME IN(" + StationNameSql + ")GROUP BY ROUTE_ID HAVING COUNT(1) = "+StationName.Length+") T)";

            string StrSql = $@"SELECT ROUTE_ID, SEQ_NO, STATION_NAME, ROW_NUMBER() OVER(PARTITION BY ROUTE_ID ORDER BY SEQ_NO) AS RN " +
"FROM C_ROUTE_DETAIL WHERE ROUTE_ID IN( SELECT T.ROUTE_ID FROM( SELECT ROUTE_ID, COUNT(1) FROM C_ROUTE_DETAIL " +
"WHERE STATION_NAME IN(" + StationNameSql + ") AND ROUTE_ID IN ( SELECT ID FROM C_ROUTE WHERE ID IN ( SELECT T.ROUTE_ID FROM( " +
"SELECT ROUTE_ID, COUNT(1) FROM C_ROUTE_DETAIL WHERE STATION_NAME IN(" + StationNameSql + ") GROUP BY ROUTE_ID HAVING COUNT(1) = " + StationName.Length + ") T)" +
"AND ROUTE_TYPE = '" + TYPE + "' )GROUP BY ROUTE_ID HAVING COUNT(1) = " + StationName.Length + ") T)";

            DataTable res = DB.ExecSelect(StrSql).Tables[0];
            //List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
            //for (int i = 0; i < res.Rows.Count; i++)
            //{
            //    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
            //    ret.loadData(res.Rows[i]);
            //    retlist.Add(ret.GetDataObject());
            //}
            //return retlist;
            return res;
        }
        public List<C_ROUTE_DETAIL> GetSKUROUTESTATION(string SKU_ID, OleExec DB)
        {
            string StrSql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID IN (
SELECT ID FROM C_ROUTE WHERE ID IN (
SELECT ROUTE_ID FROM R_SKU_ROUTE WHERE SKU_ID = '{SKU_ID}') AND ROUTE_TYPE = 'SFC')  ORDER BY SEQ_NO ";
            DataTable res = DB.ExecSelect(StrSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }
        public List<C_ROUTE_DETAIL> GetRouteBYRerunTestStatus(string SNObj, OleExec DB)
        {
            string strSql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID = (
SELECT ROUTE_ID FROM R_SN WHERE SN='{SNObj}') ORDER BY SEQ_NO ";
            List<C_ROUTE_DETAIL> result = new List<C_ROUTE_DETAIL>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
            }
            return result;
        }

        public bool RouteEXExist(string detailID, string ex_name,string ex_value, OleExec DB)
        {
            //南寧在c_route_detail_ex新加了一個detail_id的欄位，請不要隨意修改這個查詢語句
            string sql = $@"select * from c_route_detail_ex where detail_id='{detailID}' and name='{ex_name}' and value='{ex_value}' order by seq_no ";
            DataTable dt = DB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetJobfinishNextStation(OleExec SFCDB,string routeID)
        {
            //var routeList = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == routeID && r.STATION_TYPE == "JOBFINISH").OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Asc).ToList();
            var routeList = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(d => d.ROUTE_ID == routeID && SqlSugar.SqlFunc.Subqueryable<C_ROUTE_DETAIL>()
            .Where(dd => dd.ROUTE_ID == d.ROUTE_ID && dd.STATION_TYPE == "JOBFINISH" && d.SEQ_NO > dd.SEQ_NO).Any()).OrderBy(d => d.SEQ_NO).ToList();
            if (routeList.Count > 0)
            {
                return routeList[0].STATION_NAME;
            }
            else
            {
                return "";
            }
        }
    }
    public class Row_T_C_ROUTE_DETAIL : DataObjectBase
    {
        public Row_T_C_ROUTE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE_DETAIL GetDataObject()
        {
            C_ROUTE_DETAIL DataObject = new C_ROUTE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.STATION_TYPE = this.STATION_TYPE;
            DataObject.RETURN_FLAG = this.RETURN_FLAG;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string STATION_TYPE
        {
            get
            {
                return (string)this["STATION_TYPE"];
            }
            set
            {
                this["STATION_TYPE"] = value;
            }
        }
        public string RETURN_FLAG
        {
            get
            {
                return (string)this["RETURN_FLAG"];
            }
            set
            {
                this["RETURN_FLAG"] = value;
            }
        }

    }
    public class C_ROUTE_DETAIL
    {
        public string ID{get;set;}
        public double? SEQ_NO{get;set;}
        public string ROUTE_ID{get;set;}
        public string STATION_NAME{get;set;}
        public string STATION_TYPE{get;set;}
        public string RETURN_FLAG{get;set;}
    }

    public class Row_C_ROUTE_DETAIL : DataObjectBase
    {
        public Row_C_ROUTE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE_DETAIL GetDataObject()
        {
            C_ROUTE_DETAIL DataObject = new C_ROUTE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.STATION_TYPE = this.STATION_TYPE;
            DataObject.RETURN_FLAG = this.RETURN_FLAG;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
      
        public string STATION_TYPE
        {
            get
            {
                return (string)this["STATION_TYPE"];
            }
            set
            {
                this["STATION_TYPE"] = value;
            }
        }
        public string RETURN_FLAG
        {
            get
            {
                return (string)this["RETURN_FLAG"];
            }
            set
            {
                this["RETURN_FLAG"] = value;
            }
        }
    }
}