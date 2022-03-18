using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_DETAIL : DataObjectTable
    {
        public T_C_SKU_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_DETAIL);
            TableName = "C_SKU_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// add by hgb 2019.06.20
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckExists(string SKUNO, string CATEGORY, string CATEGORY_NAME, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            string StrsqlCATEGORY = string.Empty;
            string StrsqlCATEGORY_NAME = string.Empty;
            if (CATEGORY.Length!=0)
            {
                StrsqlCATEGORY = $@" AND Category='{CATEGORY}'";
            }
            if (CATEGORY_NAME.Length != 0)
            {
                StrsqlCATEGORY_NAME = $@" AND CATEGORY_NAME='{CATEGORY_NAME}'";
            }
            StrSql = $@"SELECT * FROM C_SKU_DETAIL WHERE SKUNO = '{SKUNO}' {StrsqlCATEGORY} {StrsqlCATEGORY_NAME}   ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool CheckNameExists(string SKUNO,string NAME, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM C_SKU_DETAIL WHERE SKUNO = '{SKUNO}' and CATEGORY_NAME='{NAME}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }
        public bool CheckReworkMapping(string NAME, string RwSkuno, string RwWo, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM  R_FUNCTION_CONTROL WHERE  FUNCTIONNAME='{NAME}' 
                                and VALUE='{RwSkuno}' and EXTVAL='{RwWo}' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool NOCHECK_WW(string tempsn, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT *FROM C_SKU_DETAIL A, R_SN_STATION_DETAIL B, R_WO_BASE C WHERE B.WORKORDERNO = C.WORKORDERNO
                           AND C.SKUNO = A.SKUNO
                           AND A.CATEGORY = 'NOCHECK_WW'
                           AND B.SN = '{tempsn}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool LockPreAsyy(string tempsn, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT *FROM C_SKU_DETAIL A, R_SN_STATION_DETAIL B, R_WO_BASE C WHERE B.WORKORDERNO = C.WORKORDERNO
                           AND C.SKUNO = A.SKUNO
                           AND A.CATEGORY IN('LOCK_PREASSY','LOCK_VI')
                           AND B.SN = '{tempsn}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool NOCHECK_MM(string tempsn, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT *FROM C_SKU_DETAIL A, R_SN_STATION_DETAIL B, R_WO_BASE C WHERE B.WORKORDERNO = C.WORKORDERNO
                           AND C.SKUNO = A.SKUNO
                           AND A.CATEGORY = 'NOCHECK_MM'
                           AND B.SN = '{tempsn}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool NOCHECK_YYYY(string tempsn, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT *FROM C_SKU_DETAIL A, R_SN_STATION_DETAIL B, R_WO_BASE C WHERE B.WORKORDERNO = C.WORKORDERNO
                           AND C.SKUNO = A.SKUNO
                           AND A.CATEGORY = 'NOCHECK_YYYY'
                           AND B.SN = '{tempsn}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool CheckExistsFatherSN(string SKUNO, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM C_SKU_DETAIL WHERE SKUNO = '{SKUNO}' AND VALUE='2'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// add by hgb 2019.06.20
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SKU_DETAIL LoadData(string SKUNO, string CATEGORY, string CATEGORY_NAME, OleExec DB)
        { 
            return DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO).WhereIF(!string.IsNullOrEmpty(CATEGORY), t => t.CATEGORY == CATEGORY).WhereIF(!string.IsNullOrEmpty(CATEGORY_NAME), t => t.CATEGORY_NAME == CATEGORY_NAME).ToList().FirstOrDefault();
        }

        /// <summary>
        /// add by hgb 2019.08.27
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU_DETAIL> LoadListData(string SKUNO, string CATEGORY, string CATEGORY_NAME, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO).WhereIF(!string.IsNullOrEmpty(CATEGORY), t => t.CATEGORY == CATEGORY).WhereIF(!string.IsNullOrEmpty(CATEGORY_NAME), t => t.CATEGORY_NAME == CATEGORY_NAME).ToList();
        }

        /// <summary>
        /// 獲取流水號
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="CATEGORY"></param>
        /// <param name="CATEGORY_NAME"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetSerialno( OleExec DB)
        {
            string StrSql = ""; 
            DataTable Dt = new DataTable();
            StrSql = $@" select sfc.SEQ_R_SN.NEXTVAL as LS from dual";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
              string LS=  Dt.Rows[0]["LS"].ToString();
              return LS;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210812161846", new string[] { });
                throw new MESReturnMessage(errMsg);
                //throw new MESReturnMessage($@"獲取流水號失敗");
            }

        }
       

        public C_SKU_DETAIL getC_SKU_DETAILbyID(string id, OleExec DB)
        {
            List<C_SKU_DETAIL> Sds = DB.ORM.Queryable<C_SKU_DETAIL>().Where(sd => sd.ID == id).ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 獲取所有的 C_SKU_DETAIL 記錄
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU_DETAIL> GetAllSkuDetail(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_DETAIL>().ToList();
        }


        public int CheckNewConfigCount(string SKUNO, string CATEGORY,string EDIT_EMP, OleExec DB)
        {
            int ExistCount =DB.ORM.Queryable<C_SKU_DETAIL>()
                .Where(It=>It.SKUNO==SKUNO&&It.CATEGORY==CATEGORY)
                 .With(SqlSugar.SqlWith.NoLock)
                .Count();

            double? MaxEnableConfig = DB.ORM.Queryable<C_SKU_DETAIL>()
                .Where(It =>It.CATEGORY == CATEGORY && It.EDIT_EMP == EDIT_EMP)
                .Select(It => It.SNLENGTH)
                .With(SqlSugar.SqlWith.NoLock)
                .First();
            int IntMaxEnableConfig = 0;
            if (MaxEnableConfig != null || MaxEnableConfig != 0)
            {
                IntMaxEnableConfig=Convert.ToInt32(MaxEnableConfig);
            }

            if (ExistCount >= IntMaxEnableConfig) { return 0; }
            else  { return 1; }

        }
        public List<C_SKU_DETAIL> GetSkuDetailBySkuno(string skuno, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_DETAIL>().WhereIF(!skuno.Equals(""),sd => sd.SKUNO == skuno).OrderBy(sd => sd.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<C_SKU_DETAIL> GetListbyCategorySkuno(string CATEGORY, string SKUNO, OleExec SFCDB)
        {
            return SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == CATEGORY && t.SKUNO == SKUNO).ToList();     
        }
      
        /// <summary>
        /// 根據料號，類別，類別具體項目來獲得設置在 C_SKU_DETAIL 裡面的值
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="CategoryName"></param>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SKU_DETAIL GetSkuDetail(string Category, string CategoryName, string Skuno, OleExec DB)
        {
            List<C_SKU_DETAIL> Sds = DB.ORM.Queryable<C_SKU_DETAIL>().Where(sd => sd.SKUNO == Skuno && sd.CATEGORY == Category && sd.CATEGORY_NAME == CategoryName)
                .ToList();
            if (Sds.Count > 0)
            {
                return Sds.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加或者更新一個 C_SKU_DETAIL 記錄
        /// 需要傳遞一個完完整整的 C_SKU_DETAIL 對象，包括 ID
        /// </summary>
        /// <param name="SkuDetail"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddOrUpdateSkuDetail(string Operation,C_SKU_DETAIL SkuDetail,OleExec DB)
        {
            int result = 0;
            switch (Operation.Trim().ToUpper())
            {
                case "ADD":
                    result= DB.ORM.Insertable<C_SKU_DETAIL>(SkuDetail).ExecuteCommand();
                    break;
                case "UPDATE":
                    result= DB.ORM.Updateable<C_SKU_DETAIL>(SkuDetail).Where(sd=>sd.ID==SkuDetail.ID).ExecuteCommand();
                    break;
            }
            return result;
        }

        /// <summary>
        /// 刪除一個 C_SKU_DETAIL 記錄
        /// </summary>
        /// <param name="SkuDetailId"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteSkuDetail(string SkuDetailId, OleExec DB)
        {
            return DB.ORM.Deleteable<C_SKU_DETAIL>().Where(sd => sd.ID == SkuDetailId).ExecuteCommand();
        }


        /// <summary>
        /// 查詢該變更工號的配置信息
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU_DETAIL> GetbyEditDetail(string Editby, OleExec DB)
        {
            List<C_SKU_DETAIL> SkuDetails = new List<C_SKU_DETAIL>();
            string sql = string.Empty;
            DataTable dt = new DataTable("GetbyEditDel");
            Row_C_SKU_DETAIL SkuDetailRow = (Row_C_SKU_DETAIL)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT * FROM C_SKU_DETAIL where EDIT_EMP='{Editby}' ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SkuDetailRow.loadData(dr);
                    SkuDetails.Add(SkuDetailRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return SkuDetails;
        }


        /// <summary>
        /// Update變更權限所屬工號,A工號配置的sku信息轉移到B工號
        /// </summary>
        /// <param name="Editby"></param>
        /// <param name="Editchage"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpbyEdit(string Editby, string Editchage, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (Editby.Length > 0)
                {
                    sql = $@" update C_SKU_DETAIL set EDIT_EMP='{Editchage}' where EDIT_EMP='{Editby}'";
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

        public C_SKU_DETAIL GetBySKU(string SKUNO, string Name, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY_NAME == Name).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 通過料號與category查詢配置,並且返回成橫向數據模式
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="Category"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable GetBySKUandCate(string SKUNO, string Category, OleExec DB)
        {
            //List < C_SKU_DETAIL >
            List<C_SKU_DETAIL> list = new List<C_SKU_DETAIL>();
            List<string> skunolist = new List<string>();
            DataTable dt = new DataTable();
            string detailskuno = string.Empty;


            //如果不傳料號,就區搜索所有料號
            if (SKUNO.Equals(string.Empty))
            {
                skunolist = GetSkuNoByCategory(Category, DB);
            }
            else
            {
                skunolist.Add(SKUNO);
            }

            //所有料號逐個處理
            for (int i = 0; i < skunolist.Count; i++)
            {

                detailskuno = skunolist[i].ToString();

                #region 將同一料號轉換成一行
                list = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == detailskuno && t.CATEGORY == Category).ToList();

                if (list.Count > 0)
                {

                    dt.Rows.Add();
                    if (!dt.Columns.Contains("SKUNO"))
                    {
                        dt.Columns.Add("SKUNO", Type.GetType("System.String"));
                    }
                    dt.Rows[i]["SKUNO"] = list[0].SKUNO.ToString();

                    foreach (var item in list)
                    {
                        if (!dt.Columns.Contains(item.CATEGORY_NAME.ToString()))
                        {
                            dt.Columns.Add(item.CATEGORY_NAME.ToString(), Type.GetType("System.String"));
                        }
                        dt.Rows[i][item.CATEGORY_NAME.ToString()] = item.VALUE == null ? "" : item.VALUE.ToString();
                    }
                }
                #endregion

            }
            return dt;
        }

        /// <summary>
        /// 增加模板配置方法
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="dic"></param>
        /// <param name="BU"></param>
        /// <param name="EMP"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertKPConfig(string SKUNO, KeyValuePair<string, string> dic, string BU, string EMP, OleExec DB)
        {
            string SqlStr = string.Empty;
            string ID = string.Empty;
            int res;

            ID = MesDbBase.GetNewID(DB.ORM, BU, "C_SKU_DETAIL");

            SqlStr += $@"
                    insert into C_SKU_DETAIL values('{ID}','{SKUNO}','PRINT_KP_LABEL','{dic.Key.ToString()}','{dic.Value.ToString()}','','','','{EMP}',SYSDATE)";

            try
            {
                res = DB.ExecuteNonQuery(SqlStr, CommandType.Text, null);
            }
            catch (Exception e)
            {
                throw e;
            }

            return res;


        }

        /// <summary>
        /// 修改更新模板配置
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="dic"></param>
        /// <param name="BU"></param>
        /// <param name="EMP"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateKPConfig(string SKUNO, KeyValuePair<string, string> dic, string BU, string EMP, OleExec DB)
        {
            string SqlStr = string.Empty;
            int res;
            string ID = string.Empty;
            //ID = MesDbBase.GetNewID(DB.ORM, BU, "C_SKU_DETAIL");              
            SqlStr += $@" UPDATE  C_SKU_DETAIL SET VALUE='{dic.Value.ToString()}',EDIT_EMP='{EMP}',EDIT_TIME=SYSDATE WHERE SKUNO='{SKUNO}' AND CATEGORY='PRINT_KP_LABEL' AND CATEGORY_NAME='{dic.Key.ToString()}'
            ";
            //string result = DB.ExecSQL(SqlStr);

            try
            {
                res = DB.ExecuteNonQuery(SqlStr, CommandType.Text, null);
            }
            catch (Exception)
            {
                //result = e.Message.ToString();
                res = 0;
            }

            return res;
        }

        /// <summary>
        /// 刪除當前料號的模板配置
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string DeleteKPConfig(string SKUNO, OleExec DB)
        {
            string SqlStr = string.Empty;
            SqlStr = $@"DELETE C_SKU_DETAIL WHERE SKUNO='{SKUNO}' AND CATEGORY='PRINT_KP_LABEL'";

            string result = string.Empty;
            //string result = DB.ExecSQL(SqlStr);
            DB.BeginTrain();
            try
            {
                result = DB.ExecSQL(SqlStr);
                DB.CommitTrain();
            }
            catch (Exception e)
            {
                result = e.Message.ToString();
                DB.RollbackTrain();
            }

            return result;

        }

        public List<string> GetSkuNoByCategory(string Category, OleExec DB)
        {
            string SqlStr = $@"select distinct SKUNO from C_SKU_DETAIL where CATEGORY='{Category}'";
            DataSet ResultDs = DB.RunSelect(SqlStr);
            List<string> SkuNoList = new List<string>();

            for (int i = 0; i < ResultDs.Tables[0].Rows.Count; i++)
            {
                SkuNoList.Add(ResultDs.Tables[0].Rows[i][0].ToString().Trim().ToUpper());
            }

            return SkuNoList;

        }

        public C_SKU_DETAIL GetWoHaveInSkuDetai(string SKUNO,  OleExec DB)
        {
            C_SKU_DETAIL CSKUDETAIL = null;

            return CSKUDETAIL = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY == "FAI_CONFIG").ToList().FirstOrDefault();
            
        }

        public C_SKU_DETAIL GetBySKUList(string SKUNO, string Name, OleExec DB)
        {
            C_SKU_DETAIL CSKUDETAIL = null;
            if (DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY_NAME == Name).ToList().FirstOrDefault() != null)
            {
                CSKUDETAIL = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY_NAME == Name).ToList().FirstOrDefault();
            }
            return CSKUDETAIL;
        }
        public List<C_SKU_DETAIL> GetSKUCATEGORYNAMEBYVALUE(string SKUNO, string CATEGORY_NAME, string CATEGORY, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY_NAME == CATEGORY_NAME && t.CATEGORY == CATEGORY).ToList();
        }

        public C_SKU_DETAIL GetDetailBySkuAndCategory(OleExec sfcdb, string skuno, string category)
        {
            return sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(r => r.SKUNO == skuno && r.CATEGORY == category)
                .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        public string SNPreprocessor(OleExec SFCDB,string SKUNO,string SerialNO,string Station_name)
        {
            var categorylist = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY == "SN-PREPROCESSOR" &&t.STATION_NAME== Station_name).ToList();
            for (int i = 0; i < categorylist.Count; i++)
            {
                switch (categorylist[i].CATEGORY_NAME)
                {
                    case "REPLACE":
                        SerialNO = SerialNO.Replace(categorylist[i].VALUE, categorylist[i].EXTEND);
                        break;
                    case "ADD-SUFFIX":
                        SerialNO = SerialNO.Insert(SerialNO.Length, categorylist[i].VALUE);
                        break;
                    case "ADD-PREFIX":
                        SerialNO = SerialNO.Insert(0, categorylist[i].VALUE);
                        break;
                    case "REMOVE-SUFFIX":
                        if (SerialNO.EndsWith(categorylist[i].VALUE))
                        {
                            SerialNO = SerialNO.Remove(SerialNO.Length - categorylist[i].VALUE.Length).Trim();
                        }
                        break;
                    case "REMOVE-PREFIX":
                        if (SerialNO.StartsWith(categorylist[i].VALUE))
                        {
                            SerialNO = SerialNO.Remove(0, categorylist[i].VALUE.Length);
                        }
                        break;
                    default:
                        break;
                }
            }
            return SerialNO;
        }
    }
    public class Row_C_SKU_DETAIL : DataObjectBase
    {
        public Row_C_SKU_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_DETAIL GetDataObject()
        {
            C_SKU_DETAIL DataObject = new C_SKU_DETAIL();
            DataObject.SKUNO = this.SKUNO;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.CATEGORY_NAME = this.CATEGORY_NAME;
            DataObject.VALUE = this.VALUE;
            DataObject.EXTEND = this.EXTEND;
            DataObject.VERSION = this.VERSION;
            DataObject.BASETEMPLATE = this.BASETEMPLATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ID = this.ID;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.SNLENGTH = this.SNLENGTH;
            return DataObject;
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
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string CATEGORY_NAME
        {
            get
            {
                return (string)this["CATEGORY_NAME"];
            }
            set
            {
                this["CATEGORY_NAME"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string EXTEND
        {
            get
            {
                return (string)this["EXTEND"];
            }
            set
            {
                this["EXTEND"] = value;
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
        public string BASETEMPLATE
        {
            get
            {
                return (string)this["BASETEMPLATE"];
            }
            set
            {
                this["BASETEMPLATE"] = value;
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

        public string SEQ_NO
        {
            get
            {
                return (string)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }

        public double? SNLENGTH
        {
            get
            {
                return (double?)this["SNLENGTH"];
            }
            set
            {
                this["SNLENGTH"] = value;
            }
        }
    }
    public class C_SKU_DETAIL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SKUNO{get;set;}
        public string CATEGORY{get;set;}
        public string CATEGORY_NAME{get;set;}
        public string VALUE{get;set;}
        public string EXTEND{get;set;}
        public string VERSION{get;set;}
        public string BASETEMPLATE{get;set;}
        public string STATION_NAME { get; set; }
        public double? SNLENGTH { get; set; }
        public string SEQ_NO { get; set; }
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}