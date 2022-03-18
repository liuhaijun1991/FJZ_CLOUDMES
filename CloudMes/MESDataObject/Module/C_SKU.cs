using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MESDataObject.Module
{
    public class T_C_SKU : DataObjectTable
    {
        #region Constructors
        public T_C_SKU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU);
            TableName = "C_SKU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        #endregion

        #region 業務方法



        /// <summary>
        /// 獲取所有機種信息
        /// Get all c_sku .
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU> GetAllCSku(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO != null).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_SKU> GetSKUList(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>()
                .Where(t => t.SKUNO != null)
                .Select(t => new C_SKU
                {
                    ID = t.ID,
                    BU = t.BU,
                    SKUNO = t.SKUNO,
                    VERSION = t.VERSION,
                    SKU_NAME = t.SKU_NAME,
                    SKU_TYPE = t.SKU_TYPE,
                    CUST_PARTNO = t.CUST_PARTNO,
                    CUST_SKU_CODE = t.CUST_SKU_CODE,
                    C_SERIES_ID = t.C_SERIES_ID,
                    SERIES_NAME = SqlSugar.SqlFunc.Subqueryable<C_SERIES>().Where(tt => tt.ID == t.C_SERIES_ID).Select(tt => tt.SERIES_NAME),
                    SN_RULE = t.SN_RULE,
                    PANEL_RULE = t.PANEL_RULE,
                    DESCRIPTION = t.DESCRIPTION,
                    EDIT_EMP = t.EDIT_EMP,
                    EDIT_TIME = t.EDIT_TIME
                })
                .ToList();
        }

        /// <summary>
        /// 獲取所有機種信息
        /// Get all sku infomations.
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<SkuObject> GetAllSku(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU, C_SERIES>((sku, series) => sku.C_SERIES_ID == series.ID)
                    .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series })
                    .OrderBy(so => so.SkuBase.EDIT_TIME, SqlSugar.OrderByType.Desc)
                    .Take(200).ToList();
        }


        public List<string> GetStationBySku(OleExec DB, string Skuno)
        {
            List<string> list = DB.ORM.Queryable<C_ROUTE_DETAIL, R_SKU_ROUTE, C_SKU>(
                (route_detail, sku_route, sku) => new object[] {
                    SqlSugar.JoinType.Left,route_detail.ROUTE_ID==sku_route.ROUTE_ID,
                    SqlSugar.JoinType.Left,sku_route.SKU_ID==sku.ID}
                ).Where((route_detail, sku_route, sku) => sku.SKUNO == Skuno)
                .OrderBy((route_detail, sku_route, sku) => route_detail.SEQ_NO)
                .Select((route_detail, sku_route, sku) => route_detail.STATION_NAME).ToList();

            //      List<string> list = DB.ORM.Queryable<C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>(
            //(sku, sku_route, route_detail) => new object[] { sku.ID == sku_route.ID && sku_route.ID == route_detail.ID })
            //.Where(sku => sku.SKUNO == Skuno)
            //.GroupBy((sku, sku_route, route_detail) => route_detail.STATION_NAME)
            //.Select((sku, sku_route, route_detail) => route_detail.STATION_NAME)
            //.ToList();
            return list;
        }


        public bool SeriesIsExist(OleExec DB, string RouteId)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select *
                      from c_sku c, c_series s
                     where c.id in
                           (select sku_id
                              from r_sku_route
                             where route_id in
                                   (select id from c_route where route_name = '{RouteId}'))
                       and c.c_series_id = s.id
                       and s.series_name not in ('NETGEAR ODM', 'D-LINK ODM')";

            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RouteId", OleDbType.VarChar, 240) };
            //paramet[0].Value = RouteId;
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }


        public string GetModelTypeBySku(OleExec DB, string Skuno)
        {
            string strModelType = "";
            List<string> ModelType = DB.ORM.Queryable<C_SKU_EX>().Where(t => t.ID == Skuno).Select(t => t.VALUE).ToList();
            if (ModelType.Count > 0)
            {
                strModelType = ModelType.First();
            }

            return strModelType;
        }

        public string GetSeriesBySku(OleExec DB, string Skuno)
        {
            string series = "";
            series = DB.ORM.Queryable<C_SKU, C_SERIES>((csku, cseries) => csku.C_SERIES_ID == cseries.ID && csku.SKUNO == Skuno)
                .Select((csku, cseries) => cseries.SERIES_NAME).First();
            return series;
        }

        /// <summary>
        /// 根據 ID 獲取 C_SKU 實例
        /// Get an instance of type C_SKU according to ID
        /// </summary>
        /// <param name="SkuID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public SkuObject GetSkuByID(string SkuID, OleExec DB)
        {
            List<SkuObject> Sos = null;
            Sos = DB.ORM.Queryable<C_SKU, C_SERIES>((sku, series) => sku.C_SERIES_ID == series.ID).Where((sku, series) => sku.ID == SkuID)
                .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series }).ToList();

            //Sos = DB.ORM.Queryable<C_SKU, C_SERIES>((t1, t2) => new object[] { t1.C_SERIES_ID == t2.ID, t1.BU == t2.CUSTOMER_ID })
            //    .Where((t1, t2) => t1.ID == SkuID && t1.VERSION == "")
            //    .Select((t1, t2) => new SkuObject { SkuBase = t1, SkuSeries = t2 }).ToList();

            if (Sos.Count > 0)
            {
                return Sos.First();
            }
            else
            {
                return new SkuObject();
            }
        }

        //add by LLF 2017-12-08 begin
        /// <summary>
        /// 檢查單個料號是否存在
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSku(string Skuno, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == Skuno).Any();
        }
        /// <summary>
        /// 獲取料號
        /// </summary>
        /// <param name="Table_Name"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public C_SKU GetSku(string SKUNO, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKUNO).ToList().FirstOrDefault();
        }
        //end

        /// <summary>
        /// 獲取機種標籤體現的環保屬性
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="SEQ_NO"></param>
        /// <param name="NAME"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetSkuRoHs(string ID, string SEQ_NO, string NAME, OleExec DB)
        {
            string rohs = "Y";//沒有維護的默認為Y
            List<string> lrohs = DB.ORM.Queryable<C_SKU_EX>().Where(t => t.ID == ID && t.SEQ_NO == SEQ_NO && t.NAME == NAME).Select(t => t.VALUE).ToList();
            if (lrohs.Count > 0)
            {
                rohs = lrohs.First();
            }
            return rohs;
        }

        /// <summary>
        /// 獲取料號
        /// </summary>
        /// <param name="Table_Name"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public C_SKU GetSku(string SKUNO, string VERSION, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKUNO).WhereIF(VERSION.Length > 0, t => t.VERSION == VERSION).WhereIF(VERSION.Length == 0, t => t.VERSION == null).ToList().FirstOrDefault();
            //return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKUNO).ToList().FirstOrDefault();
        }
        //end

        /// <summary>
        /// 根據機種名獲取所有模糊查詢得到的機種信息
        /// Get an C_SKU instance according to SKU name in the way of fuzzy query.
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="DB"></param> 
        /// <returns></returns>
        public List<SkuObject> GetSkuByName(string SkuNo, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU, C_SERIES>((sku, series) => new object[] { sku.C_SERIES_ID == series.ID }).Where((sku, series) => sku.SKUNO == SkuNo)
                .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series }).ToList();
        }

        /// <summary>
        /// 根據機種、版本獲取機種信息
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="version"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public SkuObject GetSkuByNameAndVersion(string SkuNo, string version, OleExec DB)
        {
            var versionObj = (version == null || version.Length > 0) ? version : null;
            List<SkuObject> Sos = DB.ORM.Queryable<C_SKU, C_SERIES>((sku, series) => sku.C_SERIES_ID == series.ID)
                .Where((sku, series) => sku.SKUNO == SkuNo)
                .WhereIF(version != null && version.Length > 0, (sku, series) => sku.VERSION == version)
                .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series }).ToList();

            if (Sos.Count > 0)
            {
                return Sos.First();
            }
            else
            {
                return new SkuObject();
            }
        }

        public SkuObject GetSkuBySkuno(string SkuNo, OleExec DB)
        {

            List<SkuObject> Sos = DB.ORM.Queryable<C_SKU, C_SERIES>((sku, series) => sku.C_SERIES_ID == series.ID)
                .Where((sku, series) => sku.SKUNO == SkuNo)
                .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series }).ToList();
            if (Sos.Count > 0)
            {
                return Sos.First();
            }
            else
            {
                //Check PE setup C_SERIES in SKU Setting?
                bool checkSeries = DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SkuNo && string.IsNullOrEmpty(t.C_SERIES_ID)).Any();
                if (checkSeries)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20211016095011");
                    throw new MESReturnMessage(errMsg);
                }
                return new SkuObject();
            }
        }

        public C_SKU getskuversionname(string RouteId, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU, R_SKU_ROUTE, C_ROUTE>((cs, rs, cr) => rs.ROUTE_ID == cr.ID && rs.SKU_ID == cs.ID)
                .Where((cs, rs, cr) => cr.ROUTE_NAME == RouteId)
                .Select((cs, rs, cr) => cs).ToList().FirstOrDefault();
        }

        public SkuObject GetSkuBySn(string Sn, OleExec DB)
        {
            try
            {
                List<SkuObject> Sos = DB.ORM.Queryable<C_SKU, C_SERIES, R_SN, R_WO_BASE>(
                        (sku, series, sn, wo) =>
                            sku.C_SERIES_ID == series.ID &&
                            sn.WORKORDERNO == wo.WORKORDERNO &&
                            wo.SKUNO == sku.SKUNO &&
                            wo.SKU_VER == sku.VERSION)
                         .Where((sku, series, sn, wo) => sn.SN == Sn)
                         .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series })
                         .ToList();
                if (Sos.Count > 0)
                {
                    return Sos.First();
                }
                else
                {
                    Sos = DB.ORM.Queryable<C_SKU, C_SERIES, R_SN, R_WO_BASE>(
                        (sku, series, sn, wo) =>
                            sku.C_SERIES_ID == series.ID &&
                            sn.WORKORDERNO == wo.WORKORDERNO &&
                            wo.SKUNO == sku.SKUNO)
                         .Where((sku, series, sn, wo) => sn.SN == Sn)
                         .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series })
                         .ToList();
                    if (Sos.Count > 0)
                    {
                        return Sos.First();
                    }
                    throw new Exception("Check VERSION and series config");
                }
            }
            catch (Exception)
            {
                return new SkuObject();
            }


        }

        public SkuObject GetMaxVersionSkuBySn(string Sn, OleExec DB)
        {
            try
            {
                List<SkuObject> Sos = DB.ORM.Queryable<C_SKU, C_SERIES, R_SN>(
                        (sku, series, sn) =>
                            sku.C_SERIES_ID == series.ID &&
                            sn.SKUNO == sku.SKUNO)
                         .Where((sku, series, sn) => sn.SN == Sn && sn.VALID_FLAG == "1").OrderBy((sku, series, sn) => sku.VERSION, SqlSugar.OrderByType.Desc)
                         .Select((sku, series) => new SkuObject { SkuBase = sku, SkuSeries = series })
                         .ToList();
                if (Sos.Count > 0)
                {
                    return Sos.First();
                }
                else
                {
                    return new SkuObject();
                }
            }
            catch (Exception)
            {
                return new SkuObject();
            }


        }

        public C_SKU GetCSKUBySn(string Sn, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU, R_WO_BASE, R_SN>((cs, rwb, rs) => cs.SKUNO == rwb.SKUNO && cs.VERSION == rwb.SKU_VER && rwb.WORKORDERNO == rs.WORKORDERNO)
                .Where((cs, rwb, rs) => rs.SN == Sn).Select((cs, rwb, rs) => cs).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 增、刪、改 機種信息
        /// Update the data in the table through a C_SKU object
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="Sku"></param>
        /// <param name="Operation"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string UpdateSku(string BU, C_SKU Sku, string Operation, DateTime EditTime, out StringBuilder SkuId, OleExec DB)
        {
            string result = string.Empty;
            int i = 0;
            SkuId = new StringBuilder();

            switch (Operation.ToUpper())
            {
                case "ADD":
                    Sku.ID = GetNewID(BU, DB);
                    Sku.EDIT_TIME = EditTime;

                    i = DB.ORM.Insertable<C_SKU>(Sku).ExecuteCommand();
                    if (i > 0)
                    {

                        SkuId.Append(Sku.ID);
                    }
                    result = i.ToString();
                    break;
                case "UPDATE":
                    i = DB.ORM.Updateable<C_SKU>(Sku).Where(t => t.ID == Sku.ID).ExecuteCommand();

                    if (i > 0)
                    {
                        SkuId.Append(Sku.ID);
                    }
                    result = i.ToString();
                    break;
                case "DELETE":
                    //i = DB.ORM.Updateable<C_SKU>().Where(t => t.ID == Sku.ID).ExecuteCommand();
                    i = DB.ORM.Deleteable<C_SKU>().Where(t => t.ID == Sku.ID).ExecuteCommand();
                    if (i > 0)
                    {
                        SkuId.Append(Sku.ID);
                    }
                    result = i.ToString();
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 判斷機種是否已經存在，不Check vesion
        /// Judge the SKU is exist or not by comparing the SKU name and version.
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool SkuNoIsExist(string SkuNo, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SkuNo).ToList().Count > 0;
        }

        /// <summary>
        /// 判斷機種是否已經存在，不Check vesion
        /// Judge the SKU is exist or not by comparing the SKU name and version.
        /// </summary>
        /// <param name="SkuName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool SkuIsExist(string SkuName, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKU_NAME == SkuName).Any();
        }

        /// <summary>
        /// 判斷機種是否已經存在，機種名和版本相同認定為機種已經存在
        /// Judge the SKU is exist or not by comparing the SKU name and version.
        /// </summary>
        /// <param name="SkuName"></param>
        /// <param name="Version"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool SkuIsExist(string SkuName, string Version, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Where(t => t.SKU_NAME == SkuName && t.VERSION == Version).Any();
        }

        public DataTable GetALLSkuno(OleExec db)
        {
            return db.ORM.Queryable<C_SKU, C_SERIES>((SKU, SER) => new object[] { SqlSugar.JoinType.Left, SKU.C_SERIES_ID == SER.ID })
                .OrderBy((SKU, SER) => SER.SERIES_NAME, SqlSugar.OrderByType.Asc)
                .OrderBy((SKU, SER) => SKU.SKUNO, SqlSugar.OrderByType.Asc)
                .Select((SKU, SER) => new
                {
                    SKU.ID,
                    SKU.BU,
                    SER.SERIES_NAME,
                    SKU.C_SERIES_ID,
                    SKU.SKUNO,
                    SKU.VERSION,
                    SKU.SKU_NAME,
                    SKU.SKU_TYPE,
                    SKU.CUST_PARTNO,
                    SKU.CUST_SKU_CODE,
                    SKU.SN_RULE,
                    SKU.PANEL_RULE,
                    SKU.DESCRIPTION,
                    SKU.EDIT_EMP,
                    SKU.EDIT_TIME
                }).ToDataTable();
        }

        public List<string> GetAllSkunoList(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU>().Select(t => t.SKUNO).ToList();
        }
        #endregion

        public List<string> GetAllBu(OleExec DB)
        {
            List<string> bu = new List<string>();
            string sql = "Select DISTINCT BU  from C_SKU WHERE BU IS NOT NULL";
            DataTable dt = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        bu.Add(dr["BU"].ToString());
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return bu;
        }

        public object GetAllCSkuAutoStation(OleExec DB)
        {
            //string sql = "SELECT a.*,b.TEST_STATION,b.LENGTH,b.LABEL_VER,b.MODELCLEI,b.DESCRIPTION2,b.ECI_NO,b.FO6,c.SERIES_NAME FROM (select * from c_sku order by edit_time desc) a,C_SKU_TEST_STATION b,C_SERIES c WHERE a.ID=b.C_SKU_ID and a.C_SERIES_ID=c.ID(+) and rownum<=100";
            string sql = $@"SELECT a.*,b.TEST_STATION,b.LENGTH,b.LABEL_VER,b.MODELCLEI,b.DESCRIPTION2,b.ECI_NO,b.FO6,c.SERIES_NAME
FROM (select * from c_sku order by edit_time desc) a
left join C_SKU_TEST_STATION b on a.ID = b.C_SKU_ID
left join C_SERIES c on a.C_SERIES_ID = c.ID
WHERE rownum<= 100";
            DataTable dt = null;

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(sql).Tables[0];

                var rows = dt.AsEnumerable().Select(entity => new
                {
                    ID = entity["ID"].ToString(),
                    BU = entity["BU"].ToString(),
                    SKUNO = entity["SKUNO"].ToString(),
                    VERSION = entity["VERSION"].ToString(),
                    SKU_NAME = entity["SKU_NAME"].ToString(),
                    //C_SERIES_ID = entity["C_SERIES_ID"].ToString(),
                    SERIES_NAME = entity["SERIES_NAME"].ToString(),
                    CUST_PARTNO = entity["CUST_PARTNO"].ToString(),
                    CUST_SKU_CODE = entity["CUST_SKU_CODE"].ToString(),
                    SN_RULE = entity["SN_RULE"].ToString(),
                    PANEL_RULE = entity["PANEL_RULE"].ToString(),
                    DESCRIPTION = entity["DESCRIPTION"].ToString(),
                    EDIT_EMP = entity["EDIT_EMP"].ToString(),
                    EDIT_TIME = entity["EDIT_TIME"].ToString(),
                    SKU_TYPE = entity["SKU_TYPE"].ToString(),
                    AQLTYPE = entity["AQLTYPE"].ToString(),
                    LENGTH = entity["LENGTH"].ToString(),
                    LABEL_VER = entity["LABEL_VER"].ToString(),
                    MODELCLEI = entity["MODELCLEI"].ToString(),
                    DESCRIPTION2 = entity["DESCRIPTION2"].ToString(),
                    ECI_NO = entity["ECI_NO"].ToString(),
                    FO6 = entity["FO6"].ToString(),
                    AutoStation = entity["TEST_STATION"].ToString(),
                });

                return rows.Select(t => t.ID).ToList().Count == 0 ? null : rows;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 查詢該變更工號的配置信息
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU> GetbyEditCsku(string Editby, OleExec DB)
        {
            List<C_SKU> CSkuDet = new List<C_SKU>();
            string sql = string.Empty;
            DataTable dt = new DataTable("GetbyEditCsku");
            Row_C_SKU CSkuRow = (Row_C_SKU)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT * FROM C_SKU where EDIT_EMP='{Editby}' ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    CSkuRow.loadData(dr);
                    CSkuDet.Add(CSkuRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return CSkuDet;
        }

        /// <summary>
        /// Update變更權限所屬工號,A工號配置的sku信息轉移到B工號
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="Editchage"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpCSKUEdit(string Editby, string Editchage, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (Editby.Length > 0)
                {
                    sql = $@" update C_SKU set EDIT_EMP='{Editchage}' where EDIT_EMP='{Editby}'";
                    result = DB.ExecSqlNoReturn(sql, null);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public string GETSkuPrefix(string skuno)
        {
            skuno = skuno.Trim();
            if (skuno.Contains("-"))
            {
                skuno = skuno.Substring(0, skuno.LastIndexOf('-') - 1);
            }
            return skuno;
        }

        public C_SKU GetSkuBySkunoAndType(string skuno, string skuType, OleExec db)
        {
            string strSql = $@"select * from C_SKU where skuno = '{skuno}' and sku_type = '{skuType}'";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_SKU result = new C_SKU();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU ret = (Row_C_SKU)this.NewRow();
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
        /// add by hgb 2019.08.26
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExists(string SKU, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM C_SKU WHERE skuno = '{SKU}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }
        /// <summary>
        /// WZW 根據料號查詢
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SKU GetBySKU(string SKU, OleExec DB)
        {
            //return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKU).ToList().FirstOrDefault();
            string strSql = $@"select * from C_SKU where skuno = '{SKU}' ";
            DataTable table = DB.ExecSelect(strSql).Tables[0];
            C_SKU result = new C_SKU();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU ret = (Row_C_SKU)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        public List<C_SKU> GetBySKUCodeValue(string SKU, string CodeValue, OleExec DB)
        {
            //return DB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKU && t.CUST_SKU_CODE == CodeValue).ToList();
            List<C_SKU> ListSku = new List<C_SKU>();
            string strSql = $@"select * from C_SKU where skuno = '{SKU}' and CUST_SKU_CODE='{ CodeValue}' ";
            DataTable table = DB.ExecSelect(strSql).Tables[0];
            C_SKU result = new C_SKU();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU ret = (Row_C_SKU)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
                ListSku.Add(result);
            }
            return ListSku;
        }
        /// <summary>
        /// Add By Simon 2019/2/22
        /// check contains "new NPI skus" or not by mainSkuno,
        /// for new NPI skus no need to check LH_NSDI_SILoadingRohsDataCheck
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool IsContainsNPISkus(string mainSkuno, string editTime, OleExec DB)
        {
            string sql = $@"select 1 from C_SKU where skuno='{mainSkuno}' and edit_time>=to_date('{editTime}','yyyy-mm-dd') and rownum=1";
            object res = DB.ExecSelectOneValue(sql);
            return res != null;
        }
        public C_SKU GetSNBYSKU(string SN, OleExec DB)
        {
            string strSql = $@"SELECT * FROM C_SKU A, R_SN B WHERE A.SKUNO=B.SKUNO AND B.SN='{SN}' ";
            DataTable table = DB.ExecSelect(strSql).Tables[0];
            C_SKU result = new C_SKU();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU ret = (Row_C_SKU)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        public List<C_SKU> GetSNRULESKUTYPEBYSKU(string SKU, string SKUTYPE, OleExec DB)
        {
            List<C_SKU> ListSku = new List<C_SKU>();
            string strSql = $@"select * from C_SKU where skuno = '{SKU}' and SKU_TYPE='{ SKUTYPE}' ";
            DataTable table = DB.ExecSelect(strSql).Tables[0];
            C_SKU result = new C_SKU();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU ret = (Row_C_SKU)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
                ListSku.Add(result);
            }
            return ListSku;
        }

        public bool HWDGeneralSNLabelCheck(string skuno, OleExec DB)
        {

            string sql = $@"select * from c_sku a where exists (select * from c_kp_list_item b where b.kp_partno=a.skuno and exists (select * from c_kp_list c where c.id=b.list_id and c.flag='1'))
                            and a.skuno='{skuno}'
                            union
                            select * from c_sku aa where exists (select * from c_keypart bb where bb.seq_no=20 and bb.part_no=aa.skuno ) and aa.skuno='{skuno}'";
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

        public DataTable HWDGetLast100DaySkuList(OleExec DB)
        {
            string strSql = $@"select distinct skuno,version,sku_name,cust_partno,cust_sku_code,sn_rule,panel_rule,description,edit_emp,edit_time,sku_type,aqltype,route_name from (
                                select a.*, d.route_name from c_sku a,(select b.sku_id ,c.route_name from r_sku_route b,c_route c where c.id=b.route_id) d
                                 where a.edit_time>sysdate-100 and a.id=d.sku_id(+)  order by a.edit_time desc)";
            return DB.ExecSelect(strSql).Tables[0];
        }

        public bool CheckSkuNETGEAR(string sn, OleExec DB)
        {
            string sql = $@" SELECT * FROM R_SN WHERE SN='{sn}' AND SKUNO IN (
                             SELECT SKUNO FROM SFCBASE.C_SKU WHERE C_SERIES_ID IN (
                                  SELECT ID FROM SFCBASE.C_SERIES WHERE CUSTOMER_ID IN (
                                   SELECT ID FROM SFCBASE.C_CUSTOMER WHERE CUSTOMER_NAME='NETGEAR')))";
            DataTable dt = DB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
    public class Row_C_SKU : DataObjectBase
    {
        #region 數據庫行級映射機種類
        public Row_C_SKU(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU GetDataObject()
        {
            C_SKU DataObject = new C_SKU();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.C_SERIES_ID = this.C_SERIES_ID;
            DataObject.CUST_PARTNO = this.CUST_PARTNO;
            DataObject.CUST_SKU_CODE = this.CUST_SKU_CODE;
            DataObject.SN_RULE = this.SN_RULE;
            DataObject.PANEL_RULE = this.PANEL_RULE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.SKU_TYPE = this.SKU_TYPE;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string SKU_TYPE
        {
            get
            {
                return (string)this["SKU_TYPE"];
            }
            set
            {
                this["SKU_TYPE"] = value;
            }
        }
        public string SKU_NAME
        {
            get
            {
                return (string)this["SKU_NAME"];
            }
            set
            {
                this["SKU_NAME"] = value;
            }
        }
        public string C_SERIES_ID
        {
            get
            {
                return (string)this["C_SERIES_ID"];
            }
            set
            {
                this["C_SERIES_ID"] = value;
            }
        }
        public string CUST_PARTNO
        {
            get
            {
                return (string)this["CUST_PARTNO"];
            }
            set
            {
                this["CUST_PARTNO"] = value;
            }
        }
        public string CUST_SKU_CODE
        {
            get
            {
                return (string)this["CUST_SKU_CODE"];
            }
            set
            {
                this["CUST_SKU_CODE"] = value;
            }
        }
        public string SN_RULE
        {
            get
            {
                return (string)this["SN_RULE"];
            }
            set
            {
                this["SN_RULE"] = value;
            }
        }

        public string PANEL_RULE
        {
            get
            {
                return (string)this["PANEL_RULE"];
            }
            set
            {
                this["PANEL_RULE"] = value;
            }
        }

        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
        public DateTime EDIT_TIME
        {
            get
            {
                if (this["EDIT_TIME"] == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    return (DateTime)this["EDIT_TIME"];
                }
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }

        #endregion
    }
    public class C_SKU
    {
        #region 機種實體類
        public string ID { get; set; }
        public string BU { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string SKU_NAME { get; set; }
        public string C_SERIES_ID { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string SERIES_NAME { get; set; }
        public string CUST_PARTNO { get; set; }
        public string CUST_SKU_CODE { get; set; }
        public string SN_RULE { get; set; }
        public string PANEL_RULE { get; set; }
        public string DESCRIPTION { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime EDIT_TIME { get; set; }
        public string SKU_TYPE { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
        #endregion
    }

    public class C_SKU_EX
    {
        public string ID { get; set; }
        public string SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }

    public class SkuObject
    {
        #region 屬性
        public string SkuId
        {
            get
            {
                return SkuBase.ID;
            }
            set
            {
                SkuBase.ID = value;
            }
        }
        public string Bu
        {
            get
            {
                return SkuBase.BU;
            }
            set
            {
                SkuBase.BU = value;
            }
        }
        public string SkuNo
        {
            get
            {
                return SkuBase.SKUNO;
            }
            set
            {
                SkuBase.SKUNO = value;
            }
        }
        public string Version
        {
            get
            {
                return SkuBase.VERSION;
            }
            set
            {
                SkuBase.VERSION = value;
            }
        }
        public string SkuName
        {
            get
            {
                return SkuBase.SKU_NAME;
            }
            set
            {
                SkuBase.SKU_NAME = value;
            }
        }

        public string SkuType
        {
            get
            {
                return SkuBase.SKU_TYPE;
            }
            set
            {
                SkuBase.SKU_TYPE = value;
            }
        }
        public string CSeriesId
        {
            get
            {
                return SkuBase.C_SERIES_ID;
            }
            set
            {
                SkuBase.C_SERIES_ID = value;
            }
        }
        public string CustPartNo
        {
            get
            {
                return SkuBase.CUST_PARTNO;
            }
            set
            {
                SkuBase.CUST_PARTNO = value;
            }
        }
        public string CustSkuCode
        {
            get
            {
                return SkuBase.CUST_SKU_CODE;
            }
            set
            {
                SkuBase.CUST_SKU_CODE = value;
            }
        }
        public string SnRule
        {
            get
            {
                return SkuBase.SN_RULE;
            }
            set
            {
                SkuBase.SN_RULE = value;
            }
        }

        public string PanelRule
        {
            get
            {
                return SkuBase.PANEL_RULE;
            }
            set
            {
                SkuBase.PANEL_RULE = value;
            }
        }

        public string Description
        {
            get
            {
                return SkuBase.DESCRIPTION;
            }
            set
            {
                SkuBase.DESCRIPTION = value;
            }
        }
        public string LastEditUser
        {
            get
            {
                return SkuBase.EDIT_EMP;
            }
            set
            {
                SkuBase.EDIT_EMP = value;
            }
        }
        public DateTime LastEditTime
        {
            get
            {
                return SkuBase.EDIT_TIME;
            }
            set
            {
                SkuBase.EDIT_TIME = value;
            }
        }
        public string SeriesId
        {
            get
            {
                return SkuSeries.ID;
            }
            set
            {
                SkuSeries.ID = value;
            }
        }
        public string SeriesCustomerId
        {
            get
            {
                return SkuSeries.CUSTOMER_ID;
            }
            set
            {
                SkuSeries.CUSTOMER_ID = value;
            }
        }
        public string SeriesName
        {
            get
            {
                return SkuSeries.SERIES_NAME;
            }
            set
            {
                SkuSeries.SERIES_NAME = value;
            }
        }
        public string SeriesDescription
        {
            get
            {
                return SkuSeries.DESCRIPTION;
            }
            set
            {
                SkuSeries.DESCRIPTION = value;
            }
        }
        public string SeriesEditEmp
        {
            get
            {
                return SkuSeries.EDIT_EMP;
            }
            set
            {
                SkuSeries.EDIT_EMP = value;
            }
        }
        public DateTime? SeriesEditTime
        {
            get
            {
                return SkuSeries.EDIT_TIME;
            }
            set
            {
                SkuSeries.EDIT_TIME = value;
            }
        }


        public C_SKU SkuBase { get; set; }

        public C_SERIES SkuSeries { get; set; }
        #endregion

        public SkuObject()
        {
            if (SkuBase == null)
            {
                SkuBase = new C_SKU();
            }
            if (SkuSeries == null)
            {
                SkuSeries = new C_SERIES();
            }
        }
    }

    public class T_R_SKU_ROUTE : DataObjectTable
    {
        public T_R_SKU_ROUTE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SKU_ROUTE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SKU_ROUTE);
            TableName = "R_SKU_ROUTE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        #region 業務方法
        /// <summary>
        /// 根據機種名或者路由名返回所有的機種路由對應關係，也可以不加條件而返回全部的對應關係 R_SKU_ROUTE 實例
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="DB_TYPE"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<R_SKU_ROUTE> Get_SKU_ROUTE_Mappings(OleExec sfcdb, params string[] parameters)
        {
            if (parameters.Length > 0)
            {
                return sfcdb.ORM.Queryable<C_SKU, C_ROUTE, R_SKU_ROUTE>((s, r, sr) => s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID)
                    .Where((s, r, sr) => s.SKUNO.Contains(parameters[0]) || r.ROUTE_NAME.Contains(parameters[0]))
                    .Select((s, r, sr) => sr).ToList();
            }
            else
            {
                return sfcdb.ORM.Queryable<C_SKU, C_ROUTE, R_SKU_ROUTE>((s, r, sr) => s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID)
                    .Select((s, r, sr) => sr).ToList();
            }
        }

        /// <summary>
        /// 刪除機種路由對應關係
        /// </summary>
        /// <param name="MappingID"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public string DeleteMapping(string MappingID, OleExec sfcdb)
        {
            return sfcdb.ORM.Deleteable<R_SKU_ROUTE>().Where(sr => sr.ID == MappingID).ExecuteCommand().ToString();
        }

        public string DeleteMapping(R_SKU_ROUTE Mapping, OleExec sfcdb)
        {
            if (Mapping != null)
            {
                return DeleteMapping(Mapping.SKU_ID, Mapping.ROUTE_ID, sfcdb);
            }
            else
            {
                return "0";
            }
        }

        public string DeleteMapping(string SKU_ID, string ROUTE_ID, OleExec sfcdb)
        {
            return sfcdb.ORM.Deleteable<R_SKU_ROUTE>().Where(sr => sr.SKU_ID == SKU_ID && sr.ROUTE_ID == ROUTE_ID).ExecuteCommand().ToString();
        }

        /// <summary>
        /// 檢查 R_SKU_ROUTE 來判斷該路由-機種映射關係是否可以添加到 R_SKU_ROUTE 中
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool CheckMappingExists(R_SKU_ROUTE mapping, OleExec sfcdb)
        {
            string ErrMessage = string.Empty;

            //判斷該映射關係是否已經存在於 R_SKU_ROUTE 中
            if (sfcdb.ORM.Queryable<R_SKU_ROUTE>().Any(sr => sr.SKU_ID == mapping.SKU_ID && sr.ROUTE_ID == mapping.ROUTE_ID)) //如果存在，則不插入到 R_SKU_ROUTE 中
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000177");
                throw new MESReturnMessage(ErrMessage);
            }
            else //不存在，判斷當前需要設置的映射關係是否想設置成默認的
            {
                if (mapping.DEFAULT_FLAG == "Y") //如果用戶想設置成默認的，即設置該機種的默認路由
                {
                    // 判斷該機種是否已經存在默認路由
                    if (sfcdb.ORM.Queryable<R_SKU_ROUTE>().Any(sr => sr.SKU_ID == mapping.SKU_ID && sr.DEFAULT_FLAG == "Y")) //存在則不允許插入到 R_SKU_ROUTE 中
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000178");
                        throw new MESReturnMessage(ErrMessage);
                    }
                    else //不存在則允許插入到 R_SKU_ROUTE 中
                    {
                        return false;
                    }
                }
                else //如果用戶不想設置成默認路由，則直接插入到 R_SKU_ROUTE 中
                {
                    return false;
                }
            }

            //return false;
        }

        /// <summary>
        /// 檢查 C_ROUTE 來確定要加入的路由-機種映射關係是否可以添加 R_SKU_ROUTE 中
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool CheckMappingCanAdd(R_SKU_ROUTE mapping, OleExec sfcdb)
        {
            T_C_SKU table = new T_C_SKU(sfcdb, this.DBType);
            string ErrMessage = string.Empty;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                List<C_ROUTE> Rs = sfcdb.ORM.Queryable<C_ROUTE>().Where(r => r.ID == mapping.ROUTE_ID && !SqlSugar.SqlFunc.IsNullOrEmpty(r.DEFAULT_SKUNO)).ToList();
                if (Rs.Count > 0)
                //如果有配置默認機種
                {
                    //判斷配置的默認機種和當前需要添加的機種-路由映射中的機種是否一致
                    if (Rs.First().DEFAULT_SKUNO.Equals(table.GetSkuByID(mapping.SKU_ID, sfcdb).SkuNo)) //如果一致，則判斷該機種-路由映射關係是否可以添加到 R_SKU_ROUTE 中
                    {
                        return !CheckMappingExists(mapping, sfcdb);
                    }
                    else //如果不一致，不可以添加到 R_SKU_ROUTE 中，因為有默認機種的路由只可以用到一個機種上，也就是說在 R_SKU_ROUTE 中只能存在最多一條數據
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000176");
                        throw new MESReturnMessage(ErrMessage);
                        //return false;
                    }
                }
                else //沒有配置默認機種
                {
                    //判斷該路由-機種映射關係是否可以添加到 R_SKU_ROUTE 中
                    return !CheckMappingExists(mapping, sfcdb);
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        /// <summary>
        /// 添加機種和路由對應關係
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public string AddMapping(R_SKU_ROUTE mapping, string BU, OleExec sfcdb)
        {
            string result = string.Empty;
            if (CheckMappingCanAdd(mapping, sfcdb))
            {
                mapping.ID = GetNewID(BU, sfcdb);
                result = sfcdb.ORM.Insertable<R_SKU_ROUTE>(mapping).ExecuteCommand().ToString();
            }
            else
            {
                result = "0";
            }
            return result;
        }

        /// <summary>
        /// 根據 ID 獲取機種路由對應關係
        /// </summary>
        /// <param name="MappingId"></param>
        /// <param name="sfcdb"></param>
        /// <param name="DB_TYPE"></param>
        /// <returns></returns>
        public R_SKU_ROUTE GetMappingById(string MappingId, OleExec sfcdb)
        {
            List<R_SKU_ROUTE> Srs = sfcdb.ORM.Queryable<R_SKU_ROUTE>().Where(sr => sr.ID == MappingId).ToList();
            if (Srs.Count > 0)
            {
                return Srs.First();
            }
            else
            {
                return new R_SKU_ROUTE();
            }
        }

        /// <summary>
        /// 根據機種 ID 獲取到所有對應的路由
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="skuid"></param>
        /// <returns></returns>
        public List<SKU_ROUTE> GetBySKU(OleExec DB, string skuid)
        {

            return DB.ORM.Queryable<C_SKU, C_ROUTE, R_SKU_ROUTE>((s, r, sr) => s.ID == sr.SKU_ID && r.ID == sr.ROUTE_ID)
                .Where((s, r, sr) => s.ID == skuid)
                .Select((s, r, sr) => new SKU_ROUTE
                {
                    ID = sr.ID,
                    ROUTE_ID = r.ID,
                    SKU_ID = s.ID,
                    DEFAULT_FLAG = sr.DEFAULT_FLAG,
                    ROUTE_NAME = r.ROUTE_NAME,
                    DEFAULT_SKUNO = r.DEFAULT_SKUNO,
                    ROUTE_TYPE = r.ROUTE_TYPE,
                    EDIT_EMP = sr.EDIT_EMP,
                    EDIT_TIME = sr.EDIT_TIME
                }).ToList();
        }

        /// <summary>
        /// 檢查機種路由映射關係是否可以更新到數據庫
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="sfcdb"></param>
        /// <param name="DB_TYPE"></param>
        /// <returns></returns>
        public bool CheckMappingCanUpdate(R_SKU_ROUTE mapping, OleExec sfcdb)
        {
            string format = "yyyy-MM-dd HH:mm:ss";
            R_SKU_ROUTE originalMapping = null;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            T_C_SKU table = new T_C_SKU(sfcdb, DBType);
            string ErrMessage = string.Empty;

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                originalMapping = GetMappingById(mapping.ID, sfcdb);

                if (originalMapping.EDIT_TIME.ToString(format) != mapping.EDIT_TIME.ToString(format).Replace("T", ""))
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000179");
                    throw new MESReturnMessage(ErrMessage);
                }
                else
                {
                    if (mapping.DEFAULT_FLAG == "Y")
                    {
                        if (originalMapping.DEFAULT_FLAG == "N")
                        {

                            List<C_ROUTE> Routes = sfcdb.ORM.Queryable<C_ROUTE>().Where(r => r.ID == mapping.ROUTE_ID && !SqlSugar.SqlFunc.IsNullOrEmpty(r.DEFAULT_SKUNO)).ToList();
                            if (Routes.Count > 0)
                            {
                                if (!Routes.First().DEFAULT_SKUNO.Equals(((Row_C_SKU)table.GetObjByID(mapping.SKU_ID, sfcdb)).SKUNO))
                                {
                                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000176");
                                    throw new MESReturnMessage(ErrMessage);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (sfcdb.ORM.Queryable<R_SKU_ROUTE>().Where(sr => sr.SKU_ID == mapping.SKU_ID && sr.DEFAULT_FLAG == "Y").Any())
                                {
                                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000178");
                                    throw new MESReturnMessage(ErrMessage);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 更新機種和路由的對應關係
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="sfcdb"></param>
        /// <param name="DB_TYPE"></param>
        /// <returns></returns>
        public string UpdateMapping(R_SKU_ROUTE mapping, OleExec sfcdb)
        {
            //string UpdateString = string.Empty;
            string result = string.Empty;
            //Row_R_SKU_ROUTE row = (Row_R_SKU_ROUTE)NewRow();

            //row = (Row_R_SKU_ROUTE)GetObjByID(mapping.ID,sfcdb);
            //row.ROUTE_ID = mapping.ROUTE_ID;
            //row.SKU_ID = mapping.SKU_ID;
            //row.DEFAULT_FLAG = mapping.DEFAULT_FLAG;
            //row.EDIT_EMP = mapping.EDIT_EMP;
            //row.EDIT_TIME = mapping.EDIT_TIME;

            //if(DBType.Equals(DB_TYPE_ENUM.Oracle))
            //{
            if (CheckMappingCanUpdate(mapping, sfcdb))
            {
                //UpdateString = row.GetUpdateString(this.DBType);
                //result = sfcdb.ExecSQL(UpdateString);
                result = sfcdb.ORM.Updateable<R_SKU_ROUTE>(mapping).ExecuteCommand().ToString();
            }
            else
            {
                result = "0";
            }
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}

            return result;
        }

        /// <summary>
        /// 根據路由ID找到對應的所有使用這個路由的機種 C_SKU 實例
        /// </summary>
        /// <param name="MappingRouteId"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<C_SKU> GetSkuListByMappingRouteID(string MappingRouteId, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<C_SKU, R_SKU_ROUTE>((s, sr) => new object[] { SqlSugar.JoinType.Inner, s.ID == sr.SKU_ID })
                .Where((s, sr) => sr.ROUTE_ID == MappingRouteId).Select((s, sr) => s).ToList();
        }

        /// <summary>
        /// 根據機種名和版本唯一確定機種ID之後獲取所有路由映射關係
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="version"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SKU_ROUTE> GetMappingBySkuAndVersion(string SkuNo, string version, OleExec sfcdb)
        {
            string SkuId = string.Empty;
            List<C_SKU> Skus = sfcdb.ORM.Queryable<C_SKU>().Where(s => s.SKUNO == SkuNo && s.VERSION == version).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
            if (Skus.Count > 0)
            {
                SkuId = Skus.First().ID;
                return GetMappingBySkuId(SkuId, sfcdb);
            }
            else
            {
                return new List<R_SKU_ROUTE>();
            }
        }

        /// <summary>
        /// 根據機種 ID 獲取所有的映射關係
        /// </summary>
        /// <param name="SkuId"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SKU_ROUTE> GetMappingBySkuId(string SkuId, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SKU_ROUTE>().Where(sr => sr.SKU_ID == SkuId).ToList();
        }

        /// <summary>
        /// 獲取該機種流程中的第一站
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="SkuVer"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetFirstStationBySku(string Skuno, string SkuVer, OleExec DB)
        {
            List<C_ROUTE_DETAIL> RDs = GetRouteDetailBySku(Skuno, SkuVer, DB);
            if (RDs.Count > 0)
            {
                return RDs.First().STATION_NAME;
            }
            else
            {
                return "ERROR";
            }
        }

        public List<C_ROUTE_DETAIL> GetRouteDetailBySku(string Skuno, string SkuVer, OleExec DB)
        {
            List<C_ROUTE_DETAIL> RouteDetails = new List<C_ROUTE_DETAIL>();
            List<R_SKU_ROUTE> Routes = DB.ORM.Queryable<R_SKU_ROUTE, C_SKU>((rsr, cs) => rsr.SKU_ID == cs.ID)
                .Where((rsr, cs) => cs.SKUNO == Skuno)
                .WhereIF(SkuVer != null && SkuVer.Length > 0, (rsr, cs) => cs.VERSION == SkuVer)
                .Select((rsr, cs) => rsr).OrderBy(rsr => rsr.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
            R_SKU_ROUTE Route = Routes.Find(t => t.DEFAULT_FLAG == "Y");
            if (Route == null)
            {
                Route = Routes.FirstOrDefault();
            }
            if (Route != null)
            {
                RouteDetails = DB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == Route.ROUTE_ID).OrderBy(t => t.SEQ_NO).ToList();
            }
            return RouteDetails;
        }
        #endregion
    }

    public class Row_R_SKU_ROUTE : DataObjectBase
    {
        public Row_R_SKU_ROUTE(DataObjectInfo info) : base(info)
        {

        }
        public R_SKU_ROUTE GetDataObject()
        {
            R_SKU_ROUTE DataObject = new R_SKU_ROUTE();
            DataObject.ID = this.ID;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.SKU_ID = this.SKU_ID;
            DataObject.DEFAULT_FLAG = this.DEFAULT_FLAG;
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
        public string SKU_ID
        {
            get
            {
                return (string)this["SKU_ID"];
            }
            set
            {
                this["SKU_ID"] = value;
            }
        }
        public string DEFAULT_FLAG
        {
            get
            {
                return (string)this["DEFAULT_FLAG"];
            }
            set
            {
                this["DEFAULT_FLAG"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
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
    public class R_SKU_ROUTE
    {
        public string ID { get; set; }
        public string ROUTE_ID { get; set; }
        public string SKU_ID { get; set; }
        public string DEFAULT_FLAG { get; set; }
        public DateTime EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
    public class SKU_ROUTE
    {
        public string ID { get; set; }
        public string ROUTE_ID { get; set; }
        public string SKU_ID { get; set; }
        public string DEFAULT_FLAG { get; set; }
        public string ROUTE_NAME { get; set; }
        public string DEFAULT_SKUNO { get; set; }
        public string ROUTE_TYPE { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}