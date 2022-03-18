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
    public class T_C_SKU_TEST_CONFIG: DataObjectTable
    {
        public T_C_SKU_TEST_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_TEST_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ACTION_CODE);
            TableName = "C_SKU_TEST_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_SKU_TEST_CONFIG GetStationControl(string skuno,string wo,string station, OleExec DB)
        {
            List<C_SKU_TEST_CONFIG> ACs = DB.ORM.Queryable<C_SKU_TEST_CONFIG>().Where(t => t.SKUNO==skuno&&t.WORKORDERNO==wo&&t.STATION==station).ToList();
            if (ACs.Count > 0)
            {
                return ACs.First();
            }
            else
            {
                return null;
            }
        }
        public int AddNewControl(C_SKU_TEST_CONFIG skuno, OleExec DB)
        {
            return DB.ORM.Insertable<C_SKU_TEST_CONFIG>(skuno).ExecuteCommand();
        }
        public int UpdateBySkuno(C_SKU_TEST_CONFIG NewActionCode, OleExec DB)
        {
            return DB.ORM.Updateable<C_SKU_TEST_CONFIG>(NewActionCode).Where(t => t.SKUNO == NewActionCode.SKUNO).ExecuteCommand();
        }
        public C_SKU_TEST_CONFIG GetByskuno(string skuno, OleExec DB)
        {
            List<C_SKU_TEST_CONFIG> ACs = DB.ORM.Queryable<C_SKU_TEST_CONFIG>().Where(t => t.SKUNO == skuno).ToList();
            if (ACs.Count > 0)
            {
                return ACs.First();
            }
            else
            {
                return null;
            }
        }
        //public List<C_SKU_TEST_CONFIG> GetByFuzzySearch(string ParametValue, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_SKU_TEST_CONFIG>()
        //        .Where(t => t.ACTION_CODE.ToUpper().Contains(ParametValue) || t.ENGLISH_DESCRIPTION.ToUpper().Contains(ParametValue) || t.CHINESE_DESCRIPTION.ToUpper().Contains(ParametValue))
        //        .ToList();
        //}
        public List<C_SKU_TEST_CONFIG> GetAllActionCode(OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_TEST_CONFIG>().ToList();
        }
        public int DeleteById(string skuno, OleExec DB)
        {
            return DB.ORM.Deleteable<C_SKU_TEST_CONFIG>().Where(t => t.SKUNO == skuno).ExecuteCommand();
        }
    }
    public class Row_C_SKU_TEST_CONFIG : DataObjectBase
    {
        public Row_C_SKU_TEST_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_TEST_CONFIG GetDataObject()
        {
            C_SKU_TEST_CONFIG DataObject = new C_SKU_TEST_CONFIG();
            DataObject.BU = this.BU;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.STATION = this.STATION;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.FLAG = this.FALG;
            return DataObject;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
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
        public string SKU_SERIES
        {
            get
            {
                return (string)this["SKU_SERIES"];
            }
            set
            {
                this["SKU_SERIES"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
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
        public int FALG
        {
            get
            {
                return (int)this["FALG"];
            }
            set
            {
                this["FALG"] = value;
            }
        }
    }
    public class C_SKU_TEST_CONFIG
    {
        public string BU { get; set; }
        public string SKUNO { get; set; }
        public string SKU_VER { get; set; }
        public string SKU_NAME { get; set; }
        public string SKU_SERIES { get; set; }
        public string WORKORDERNO { get; set; }
        public string STATION { get; set; }
        public string ROUTE_ID { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public int FLAG { get; set; }
    }
}
