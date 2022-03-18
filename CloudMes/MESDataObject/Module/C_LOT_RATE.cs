using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using System.Reflection;

namespace MESDataObject.Module
{
    public class T_C_LOT_RATE : DataObjectTable
    {
        public T_C_LOT_RATE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_LOT_RATE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_LOT_RATE);
            TableName = "C_LOT_RATE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LOT_RATE> GetSKUBYTESTMation(string SAMPLE_STATION, string SKU, string StationName, OleExec DB)
        {
            List<C_LOT_RATE> ret = new List<C_LOT_RATE>();
            string strSql = $@"select * from C_LOT_RATE r where SAMPLE_STATION ='{SAMPLE_STATION}' and SKUNO = '{SKU}' and station_name='{StationName}' AND VALID_FLAG='1'";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_LOT_RATE A = (Row_C_LOT_RATE)NewRow();
                A.loadData(res.Tables[0].Rows[i]);
                ret.Add(A.GetDataObject());
            }
            return ret;
        }
        /// <summary>
        /// WZW 檢查該測試工作是否存在
        /// </summary>
        /// <param name="SAMPLE_STATION"></param>
        /// <param name="StationName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LOT_RATE> IsSampleConfig(string SAMPLE_STATION, string StrSku, string StationName, OleExec DB)
        {
            //bool SampleFlag = false;
            List<C_LOT_RATE> ret = new List<C_LOT_RATE>();
            string strSql = $@"SELECT* FROM C_LOT_RATE WHERE SAMPLE_STATION ='{SAMPLE_STATION}' AND SKUNO='{StrSku}' AND STATION_NAME ='{StationName}' AND VALID_FLAG='1'";
            //int Number = DB.ExecSqlNoReturn(strSql, null);
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_LOT_RATE A = (Row_C_LOT_RATE)NewRow();
                A.loadData(res.Tables[0].Rows[i]);
                ret.Add(A.GetDataObject());
            }
            return ret;
            //if (Dt.Rows.Count > 0)
            //{
            //    SampleFlag = true;
            //}
            //return SampleFlag;
        }
        /// <summary>
        /// WZW 查詢測試工作信息
        /// </summary>
        /// <param name="SAMPLE_STATION"></param>
        /// <param name="StationName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LOT_RATE> SelectSampleConfig(string SKU, string StationName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_LOT_RATE> row = new List<C_LOT_RATE>();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM C_LOT_RATE WHERE SKUNO='{SKU}' AND STATION='{StationName}' AND VALID_FLAG='1'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Row_C_LOT_RATE RowRLotStation = (Row_C_LOT_RATE)NewRow();
                    RowRLotStation.loadData(dt.Rows[0]);
                    row.Add(RowRLotStation.GetDataObject());

                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public C_LOT_RATE SelectBySKUNO(string Skuno, OleExec DB)
        {
            string strsql = $@"select* From C_ROUTE_DETAIL where route_id  in (SELECT id fROM C_ROUTE where route_flag = 0 and id in(select route_id from R_SKU_ROUTE where sku_id in(select id from C_SKU where skuno = '" + Skuno + "')))";
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                Row_C_LOT_RATE ret = (Row_C_LOT_RATE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public List<C_LOT_RATE> GetByCODENAME(string BU, string skuno, string station_name, OleExec DB)
        {
            return DB.ORM.Queryable<C_LOT_RATE>().Where(t => t.BU == BU && t.SKUNO == skuno && t.STATION_NAME == station_name).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        /// <summary>
        /// 判斷輸入的工站是否正確
        /// </summary>
        /// <param name="STATION_NAME"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LOT_RATE> CheckStationName(string STATION_NAME, OleExec DB)
        {
            return DB.ORM.Queryable<C_LOT_RATE>().Where(t => t.STATION_NAME == STATION_NAME).ToList();
        }
        /// <summary>
        /// 根據skuno查詢
        /// </summary>
        /// <param name="SKUNO"></param>
        /// <param name="SAMPLE_STATION"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LOT_RATE> GetBySkuno(string SKUNO, string SAMPLE_STATION, OleExec DB)
        {
            return DB.ORM.Queryable<C_LOT_RATE>().Where(t => t.SKUNO == SKUNO && t.SAMPLE_STATION == SAMPLE_STATION && t.VALID_FLAG == "1").ToList();
        }

        /// <summary>
        /// 根據ID查詢
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LOT_RATE> GetByid(string id, OleExec DB)
        {
            return DB.ORM.Queryable<C_LOT_RATE>().Where(t => t.ID == id).ToList();
        }

        /// <summary>
        /// 模糊查詢根據SKUNO獲取SKU_NAME
        /// </summary>
        /// <param name="ParametValue"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable GetByFuzzySearch(string ParametValue, OleExec DB)
        {
            //string strSql = $@"select *from C_LOT_RATE where upper(skuno) like'%{ParametValue}%' or upper(code_name) like '%{ParametValue}%' or upper(station_name) like '%{ParametValue}%'";
            string strSql = $@"select SKUNO,SKU_NAME AS CODE_NAME,BU from c_sku where upper(skuno) like'%{ParametValue}%'";
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            return res;
        }

        public List<C_LOT_RATE> GetAllCodeName(string SAMPLE_STATION, OleExec DB)
        {
            return DB.ORM.Queryable<C_LOT_RATE>().Where(t => t.SAMPLE_STATION == SAMPLE_STATION && t.VALID_FLAG == "1").ToList();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="NewCODE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateById(double qty,string station_name, string valid_flag,string skuno,string code_name, OleExec DB)
        {
            //return DB.ORM.Updateable(NewCODE).Where(t => t.SKUNO == NewCODE.SKUNO&&t.STATION_NAME==NewCODE.STATION_NAME).ExecuteCommand();
            string strSql = $@"UPDATE C_LOT_RATE SET QTY='"+qty+"',STATION_NAME='"+station_name+"',VALID_FLAG='"+valid_flag+"' WHERE SKUNO='"+skuno+"' AND CODE_NAME='"+code_name+"' ";
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text);
            return result;
        }

        /// <summary>
        /// 添加新的信息
        /// </summary>
        /// <param name="NewActionCode"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddCODENAME(C_LOT_RATE NewCODE, OleExec DB)
        {
            return DB.ORM.Insertable(NewCODE).ExecuteCommand();
        }
        //public int UpdateBySKU(C_LOT_RATE NewCODE, OleExec DB)
        //{
        //    Row_C_LOT_RATE NewVALIDFLAG = (Row_C_LOT_RATE)NewRow();
        //    NewVALIDFLAG.ID = NewCODE.ID;
        //    NewVALIDFLAG.BU = NewCODE.BU;
        //    NewVALIDFLAG.SKUNO = NewCODE.SKUNO;
        //    NewVALIDFLAG.RATE = NewCODE.RATE;
        //    NewVALIDFLAG.CODE_NAME = NewCODE.CODE_NAME;
        //    NewVALIDFLAG.STATION_NAME = NewCODE.STATION_NAME;
        //    NewVALIDFLAG.QTY = NewCODE.QTY;
        //    NewVALIDFLAG.EDIT_EMP = NewCODE.EDIT_EMP;
        //    NewVALIDFLAG.EDIT_TIME = NewCODE.EDIT_TIME;
        //    NewVALIDFLAG.SAMPLE_STATION = NewCODE.SAMPLE_STATION;
        //    NewVALIDFLAG.VALID_FLAG = NewCODE.VALID_FLAG;
        //    int result = DB.ExecuteNonQuery(NewVALIDFLAG.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
        //    return result;
        //}

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteById(string ID, OleExec DB)
        {
            string strSql = $@"update C_LOT_RATE SET VALID_FLAG='0' WHERE ID=:ID";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", ID) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
            //return DB.ORM.Deleteable(C_LOT_RATE).Where(t => t.ID == C_LOT_RATE.ID).ExecuteCommand();
        }
        public string Delete2dxBySku(string SkuId, OleExec DB)//add by zc 2019/04/22 for skusetting 
        {
            string strSql = $@"DELETE FROM C_LOT_RATE WHERE  SKUNO IN (SELECT SKUNO FROM C_SKU WHERE ID= '{SkuId}' )AND SAMPLE_STATION='2DX'";
            string result = string.Empty;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                DB.BeginTrain();
                result = DB.ExecSQL(strSql);
                if (Int32.Parse(result) <= 0)
                {
                    DB.RollbackTrain();
                    return result;
                }
                DB.CommitTrain();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }
        /// <summary>
        /// 將Datatable轉為List實體對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> TableToEntity<T>(DataTable dt) where T : class, new()
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties();
                T entity = new T();
                foreach (PropertyInfo p in pArray)
                {
                    if (row[p.Name] is Int64)
                    {
                        p.SetValue(entity, Convert.ToInt32(row[p.Name]), null);
                        continue;
                    }
                    p.SetValue(entity, row[p.Name], null);
                }
                list.Add(entity);
            }
            return list;
        }
    }
    public class Row_C_LOT_RATE : DataObjectBase
    {
        public Row_C_LOT_RATE(DataObjectInfo info) : base(info)
        {

        }
        public C_LOT_RATE GetDataObject()
        {
            C_LOT_RATE DataObject = new C_LOT_RATE();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.SKUNO = this.SKUNO;
            DataObject.RATE = this.RATE;
            DataObject.CODE_NAME = this.CODE_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.QTY = this.QTY;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.SAMPLE_STATION = this.SAMPLE_STATION;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public double? RATE
        {
            get
            {
                return (double?)this["RATE"];
            }
            set
            {
                this["RATE"] = value;
            }
        }
        public string CODE_NAME
        {
            get
            {
                return (string)this["CODE_NAME"];
            }
            set
            {
                this["CODE_NAME"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
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

        public string SAMPLE_STATION
        {
            get
            {
                return (string)this["SAMPLE_STATION"];
            }
            set
            {
                this["SAMPLE_STATION"] = value;
            }
        }

        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
    }
    public class C_LOT_RATE
    {
        public string ID { get; set; }
        public string BU { get; set; }
        public string SKUNO { get; set; }
        public double? RATE { get; set; }
        public string CODE_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public double? QTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string SAMPLE_STATION { get; set; }

        public string VALID_FLAG { get; set; }
    }

    public class StationName
    {
        public string station { get; set; }
    }

    public class GetBUBySKU
    {
        public string SKUNO { get; set; }
        public string BU { get; set; }
    }

    public class GetcodeBySKU
    {
        public string SKUNO { get; set; }
        public string CODE_NAME { get; set; }
    }
}