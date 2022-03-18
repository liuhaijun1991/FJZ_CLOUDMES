using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_SAMPLE : DataObjectTable
    {
        public T_C_SKU_SAMPLE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_SAMPLE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_SAMPLE);
            TableName = "C_SKU_SAMPLE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SKU_SAMPLE> GetSample(string skuno,string station, OleExec DB)
        {
            List<C_SKU_SAMPLE> aqls = new List<C_SKU_SAMPLE>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SKU_SAMPLE");
            Row_C_SKU_SAMPLE aqlsRow = (Row_C_SKU_SAMPLE)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" select * from c_Sku_Sample where 1=1  ";
                if (skuno != "")
                    sql += $@" and skuno='{skuno}' ";
                if (station != "")
                    sql += $@" and STATION_NAME='{station}' ";
                if(skuno == ""&& station == "")
                    sql += $@" and rownum<21  order by EDIT_TIME ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqlsRow.loadData(dr);
                    aqls.Add(aqlsRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }

        public Row_C_SKU_SAMPLE GetBySkuNo(string _SkuNo,string _Station, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select ID from c_sku_sample where skuno='{_SkuNo.Replace("'", "''")}' and station_name='{_Station.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { _SkuNo, _Station });
                    throw new MESReturnMessage(errMsg);
                }
                Row_C_SKU_SAMPLE R = (Row_C_SKU_SAMPLE)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_C_SKU_SAMPLE : DataObjectBase
    {
        public Row_C_SKU_SAMPLE(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_SAMPLE GetDataObject()
        {
            C_SKU_SAMPLE DataObject = new C_SKU_SAMPLE();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.AQL_TYPE = this.AQL_TYPE;
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
        public string AQL_TYPE
        {
            get

            {
                return (string)this["AQL_TYPE"];
            }
            set
            {
                this["AQL_TYPE"] = value;
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
    public class C_SKU_SAMPLE
    {
        public string ID{get;set;}
        public string SKUNO{get;set;}
        public string STATION_NAME{get;set;}
        public string AQL_TYPE{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}