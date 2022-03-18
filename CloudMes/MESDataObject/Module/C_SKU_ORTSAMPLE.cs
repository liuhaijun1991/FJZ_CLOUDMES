using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_SKU_ORTSAMPLE : DataObjectTable
    {
        public T_C_SKU_ORTSAMPLE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_ORTSAMPLE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_ORTSAMPLE);
            TableName = "C_SKU_ORTSAMPLE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_SKU_ORTSAMPLE GetORTBySkunoPrefix(string skunoPrefix, string skuno, OleExec db)
        {
            string strSql = $@"select * from C_SKU_ORTSAMPLE where skuno like '{skunoPrefix}%' and substr('{skuno}',-3) <> 'TAA'";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_SKU_ORTSAMPLE result = new C_SKU_ORTSAMPLE();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU_ORTSAMPLE ret = (Row_C_SKU_ORTSAMPLE)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public C_SKU_ORTSAMPLE GetORTBySkuno(string skuno, OleExec db)
        {
            string strSql = $@"select * from C_SKU_ORTSAMPLE where skuno in ('{skuno}')";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_SKU_ORTSAMPLE result = new C_SKU_ORTSAMPLE();
            if (table.Rows.Count > 0)
            {
                Row_C_SKU_ORTSAMPLE ret = (Row_C_SKU_ORTSAMPLE)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        public C_SKU_ORTSAMPLE GetByCODENAME(string CODE_NAME, OleExec DB)
        {
            string strSql = $@"select *from C_SKU_ORTSAMPLE where CODE_NAME=:CODE_NAME";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":CODE_NAME", CODE_NAME) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_SKU_ORTSAMPLE ret = (Row_C_SKU_ORTSAMPLE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public List<C_SKU_ORTSAMPLE> GetByFuzzySearch(string ParametValue, OleExec DB)
        {
            string strSql = $@"select *from C_SKU_ORTSAMPLE where upper(skuno) like'%{ParametValue}%' or upper(code_name) like '%{ParametValue}%' or upper(station_name) like '%{ParametValue}%'";
            List<C_SKU_ORTSAMPLE> result = new List<C_SKU_ORTSAMPLE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_SKU_ORTSAMPLE ret = (Row_C_SKU_ORTSAMPLE)NewRow();
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
        public List<C_SKU_ORTSAMPLE> GetByid(string id, OleExec DB)
        {
            string strSql = $@"select * from C_SKU_ORTSAMPLE where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<C_SKU_ORTSAMPLE> result = new List<C_SKU_ORTSAMPLE>(); 
            if (res.Rows.Count > 0)
            {
                Row_C_SKU_ORTSAMPLE ret = (Row_C_SKU_ORTSAMPLE)NewRow();
                ret.loadData(res.Rows[0]);
                result.Add(ret.GetDataObject());
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<C_SKU_ORTSAMPLE> GetAllCodeName(OleExec DB)
        {
            string strSql = $@"select * from C_SKU_ORTSAMPLE ";
            List<C_SKU_ORTSAMPLE> result = new List<C_SKU_ORTSAMPLE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_SKU_ORTSAMPLE ret = (Row_C_SKU_ORTSAMPLE)NewRow();
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

        public string InsertOneRowORT(string bu, string skuno, string risk, string flag, string empNo, OleExec db, DB_TYPE_ENUM dbtype)
        {
            //T_C_SKU_ORTSAMPLE C_SKU_ORTSAMPLE = new T_C_SKU_ORTSAMPLE(db, dbtype);
            Row_C_SKU_ORTSAMPLE rowORTSample = (Row_C_SKU_ORTSAMPLE)this.NewRow();
            rowORTSample.ID = this.GetNewID(bu, db);
            rowORTSample.SKUNO = skuno;
            rowORTSample.RISK = risk;
            rowORTSample.SAMPLE_FLAG = flag;
            rowORTSample.EDIT_EMP = empNo;
            rowORTSample.EDIT_TIME = this.GetDBDateTime(db);
            return db.ExecSQL(rowORTSample.GetInsertString(dbtype));
        }

        /// <summary>
        /// 添加新的信息
        /// </summary>
        /// <param name="NewActionCode"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddCODENAME(C_SKU_ORTSAMPLE NewCODE, OleExec DB)
        {
            Row_C_SKU_ORTSAMPLE NewCODENAME = (Row_C_SKU_ORTSAMPLE)NewRow();
            NewCODENAME.ID = NewCODE.ID;
            NewCODENAME.SKUNO = NewCODE.SKUNO;
            NewCODENAME.RISK = NewCODE.RISK;
            NewCODENAME.EDIT_EMP = NewCODE.EDIT_EMP;
            NewCODENAME.EDIT_TIME = NewCODE.EDIT_TIME;
            NewCODENAME.SAMPLE_FLAG = NewCODE.SAMPLE_FLAG;
            int result = DB.ExecuteNonQuery(NewCODENAME.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        public int UpdateBySKU(C_SKU_ORTSAMPLE NewCODE, OleExec DB)
        {
            Row_C_SKU_ORTSAMPLE NewVALIDFLAG = (Row_C_SKU_ORTSAMPLE)NewRow();
            NewVALIDFLAG.ID = NewCODE.ID;
            NewVALIDFLAG.SKUNO = NewCODE.SKUNO;
            NewVALIDFLAG.RISK = NewCODE.RISK;
            NewVALIDFLAG.EDIT_EMP = NewCODE.EDIT_EMP;
            NewVALIDFLAG.EDIT_TIME = NewCODE.EDIT_TIME;
            NewVALIDFLAG.SAMPLE_FLAG = NewCODE.SAMPLE_FLAG;
            int result = DB.ExecuteNonQuery(NewVALIDFLAG.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
            return result;
        }
        public int UpdateById(C_SKU_ORTSAMPLE NewCODE, OleExec DB)
        {
            Row_C_SKU_ORTSAMPLE NewCODENAMERow = (Row_C_SKU_ORTSAMPLE)NewRow();
            NewCODENAMERow.ID = NewCODE.ID;
            NewCODENAMERow.SKUNO = NewCODE.SKUNO;
            NewCODENAMERow.RISK = NewCODE.RISK;
            NewCODENAMERow.EDIT_EMP = NewCODE.EDIT_EMP;
            NewCODENAMERow.EDIT_TIME = NewCODE.EDIT_TIME;
            NewCODENAMERow.SAMPLE_FLAG = NewCODE.SAMPLE_FLAG;
            int result = DB.ExecuteNonQuery(NewCODENAMERow.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
            return result;
        }
        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"DELETE C_SKU_ORTSAMPLE WHERE ID =:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

    }
    public class Row_C_SKU_ORTSAMPLE : DataObjectBase
    {
        public Row_C_SKU_ORTSAMPLE(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_ORTSAMPLE GetDataObject()
        {
            C_SKU_ORTSAMPLE DataObject = new C_SKU_ORTSAMPLE();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.RISK = this.RISK;
            DataObject.SAMPLE_FLAG = this.SAMPLE_FLAG;
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
        public string RISK
        {
            get
            {
                return (string)this["RISK"];
            }
            set
            {
                this["RISK"] = value;
            }
        }
        public string SAMPLE_FLAG
        {
            get
            {
                return (string)this["SAMPLE_FLAG"];
            }
            set
            {
                this["SAMPLE_FLAG"] = value;
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
    public class C_SKU_ORTSAMPLE
    {
        public string ID;
        public string SKUNO;
        public string RISK;
        public string SAMPLE_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}