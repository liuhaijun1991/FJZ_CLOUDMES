using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SAP_STATION_MAP : DataObjectTable
    {
        public T_C_SAP_STATION_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SAP_STATION_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SAP_STATION_MAP);
            TableName = "C_SAP_STATION_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 獲取所有的映射關係
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SAP_STATION_MAP> GetAllSAPStationMaps(OleExec DB)
        {
            List<C_SAP_STATION_MAP> mapList = new List<C_SAP_STATION_MAP>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_C_SAP_STATION_MAP row = null;
            C_SAP_STATION_MAP map = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_SAP_STATION_MAP ORDER BY EDIT_TIME";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    row = (Row_C_SAP_STATION_MAP)NewRow();
                    row.loadData(dr);
                    map = row.GetDataObject();
                    mapList.Add(map);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return mapList;
        }

        /// <summary>
        /// 根據 機種 獲取與 拋賬點 的映射關係
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SAP_STATION_MAP> GetSAPStationMapBySku(string SkuNo, OleExec DB)
        {
            List<C_SAP_STATION_MAP> mapList = new List<C_SAP_STATION_MAP>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_C_SAP_STATION_MAP row = null;
            C_SAP_STATION_MAP map = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_SAP_STATION_MAP WHERE SKUNO='{SkuNo}' ORDER BY EDIT_TIME";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    row = (Row_C_SAP_STATION_MAP)NewRow();
                    row.loadData(dr);
                    map = row.GetDataObject();
                    mapList.Add(map);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return mapList;
        }
        public List<C_SAP_STATION_MAP> GetSAPStationMapBySkuOrderBySAPCodeASC(string SkuNo, OleExec DB)
        {
            List<C_SAP_STATION_MAP> mapList = new List<C_SAP_STATION_MAP>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_C_SAP_STATION_MAP row = null;
            C_SAP_STATION_MAP map = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_SAP_STATION_MAP WHERE SKUNO='{SkuNo}' ORDER BY SAP_STATION_CODE ASC";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    row = (Row_C_SAP_STATION_MAP)NewRow();
                    row.loadData(dr);
                    map = row.GetDataObject();
                    mapList.Add(map);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return mapList;
        }

        /// <summary>
        /// 根據 機種和站位獲取與 拋賬點 的映射關係
        /// </summary>
        /// <param name="SkuNo"></param>
        /// <param name="StationName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SAP_STATION_MAP> GetSAPStationMapBySkuAndStation(string SkuNo, string StationName, OleExec DB)
        {
            List<C_SAP_STATION_MAP> mapList = new List<C_SAP_STATION_MAP>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_C_SAP_STATION_MAP row = null;
            C_SAP_STATION_MAP map = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_SAP_STATION_MAP WHERE SKUNO='{SkuNo}' AND STATION_NAME='{StationName}' ORDER BY EDIT_TIME";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    row = (Row_C_SAP_STATION_MAP)NewRow();
                    row.loadData(dr);
                    map = row.GetDataObject();
                    mapList.Add(map);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return mapList;
        }

        /// <summary>
        /// 增、刪、改 SAP 與 站位的映射
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="Operation"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string UpdateSAPStationMap(C_SAP_STATION_MAP Map, string Operation, string Bu, string emp_no, OleExec DB)
        {
            string sql = string.Empty;
            string result = string.Empty;
            Row_C_SAP_STATION_MAP row = (Row_C_SAP_STATION_MAP)NewRow();
            T_C_SAP_STATION_MAP table = new T_C_SAP_STATION_MAP(DB, DBType);
            T_R_MES_LOG TRML = new T_R_MES_LOG(DB, DBType);
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (Map.ID != null && !Map.ID.Equals(""))
                {
                    row = (Row_C_SAP_STATION_MAP)GetObjByID(Map.ID, DB);
                }

                row.SKUNO = Map.SKUNO;
                row.STATION_NAME = Map.STATION_NAME;
                row.WORKORDER_TYPE = Map.WORKORDER_TYPE;
                row.SAP_STATION_CODE = Map.SAP_STATION_CODE;
                row.EDIT_EMP = Map.EDIT_EMP;
                row.EDIT_TIME = Map.EDIT_TIME;

                switch (Operation.ToUpper())
                {
                    case "ADD":
                        row.ID = GetNewID(Bu, DB);
                        sql = row.GetInsertString(DBType);
                        break;
                    case "UPDATE":
                        sql = row.GetUpdateString(DBType);
                        break;
                    case "DELETE":
                        sql = row.GetDeleteString(DBType);
                        break;
                    default:
                        sql = $@"SELECT * FROM C_SAP_STATION_MAP";
                        break;
                }
                result = DB.ExecSQL(sql);
                
                DB.ORM.Insertable<R_MES_LOG>(new R_MES_LOG()
                {
                    ID = TRML.GetNewID(Bu, DB),
                    PROGRAM_NAME = "SKU SETTING",
                    CLASS_NAME = "MESDataObject.Module",
                    FUNCTION_NAME = "UpdateSAPStationMap",
                    LOG_SQL = sql,
                    EDIT_EMP = emp_no,
                    EDIT_TIME = DateTime.Now
                }).ExecuteCommand();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }
        public string DeleteSAPStationMap(string ID, string Bu,string emp_no, OleExec sfcdb)
        {
            string result = string.Empty;
            string DeleteString = string.Empty;
            Row_C_SAP_STATION_MAP row = (Row_C_SAP_STATION_MAP)NewRow();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBType);
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                var point = sfcdb.ORM.Queryable<C_SAP_STATION_MAP>().Where(t => t.ID == ID).First();
                if (point != null)
                {
                    result = sfcdb.ORM.Deleteable(point).ExecuteCommand().ToString();
                    sfcdb.ORM.Insertable<R_MES_LOG>(new R_MES_LOG()
                    {
                        ID = TRML.GetNewID(Bu, sfcdb),
                        PROGRAM_NAME = "SKU SETTING",
                        CLASS_NAME = "MESDataObject.Module",
                        FUNCTION_NAME = "DeleteSAPStationMap",
                        DATA1 = point.SKUNO,
                        DATA2 = point.SAP_STATION_CODE,
                        DATA3 = point.STATION_NAME,
                        EDIT_EMP = emp_no,
                        EDIT_TIME = DateTime.Now
                    }).ExecuteCommand();
                }
                else
                {
                    result = "0";
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public string GetMAXSAPStationCodeBySkuAndWorkorderType(string SkuNo, string WorkorderType, OleExec DB)
        {
            string result = "";
            string sql = string.Empty;
            DataTable dt = new DataTable();
            //Row_C_SAP_STATION_MAP row = null;
            //C_SAP_STATION_MAP map = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT MAX(SAP_STATION_CODE) FROM C_SAP_STATION_MAP WHERE SKUNO='{SkuNo}' AND WORKORDER_TYPE='{WorkorderType}' ORDER BY EDIT_TIME";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0][0].ToString();
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { SkuNo + "," + WorkorderType });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// add by fgg HWD 不看工單類型，MRB只取排序后的最後一個拋賬點
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetMAXSAPStationCodeBySku(string skuno, OleExec DB)
        {

            string result = "";
            string sql = string.Empty;
            DataTable dt = new DataTable();
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"select sap_station_code from c_sap_station_map c where skuno = '{skuno}' order by c.sap_station_code";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[dt.Rows.Count - 1]["sap_station_code"].ToString();
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000224", new string[] { skuno });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
    }
    public class Row_C_SAP_STATION_MAP : DataObjectBase
    {
        public Row_C_SAP_STATION_MAP(DataObjectInfo info) : base(info)
        {

        }
        public C_SAP_STATION_MAP GetDataObject()
        {
            C_SAP_STATION_MAP DataObject = new C_SAP_STATION_MAP();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SAP_STATION_CODE = this.SAP_STATION_CODE;
            DataObject.WORKORDER_TYPE = this.WORKORDER_TYPE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string SAP_STATION_CODE
        {
            get
            {
                return (string)this["SAP_STATION_CODE"];
            }
            set
            {
                this["SAP_STATION_CODE"] = value;
            }
        }
        public string WORKORDER_TYPE
        {
            get
            {
                return (string)this["WORKORDER_TYPE"];
            }
            set
            {
                this["WORKORDER_TYPE"] = value;
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
    }
    public class C_SAP_STATION_MAP
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string STATION_NAME { get; set; }
        public string SAP_STATION_CODE { get; set; }
        public string WORKORDER_TYPE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
