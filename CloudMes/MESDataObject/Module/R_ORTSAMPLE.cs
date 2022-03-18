using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORTSAMPLE : DataObjectTable
    {
        public T_R_ORTSAMPLE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORTSAMPLE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORTSAMPLE);
            TableName = "R_ORTSAMPLE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_ORTSAMPLE GetORTSampleBySN(string sn,ref int count, OleExec db)
        {
            string strSql = $@"select * from r_ortsample where sn = '{sn}'";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            count = table.Rows.Count;
            R_ORTSAMPLE result = new R_ORTSAMPLE();
            if (table.Rows.Count > 0)
            {
                Row_R_ORTSAMPLE ret = (Row_R_ORTSAMPLE)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public R_ORTSAMPLE GetORTSampleByWO(string wo, ref int count, OleExec db)
        {
            string strSql = $@"select * from r_ortsample where wo = '{wo}'";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            count = table.Rows.Count;
            R_ORTSAMPLE result = new R_ORTSAMPLE();
            if (table.Rows.Count > 0)
            {
                Row_R_ORTSAMPLE ret = (Row_R_ORTSAMPLE)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public string InsertOneRowORT(string bu, string skuno, string wo, string sn, string flag, string empNo, OleExec db, DB_TYPE_ENUM dbtype)
        {
            //T_R_ORTSAMPLE R_ORTSAMPLE = new T_R_ORTSAMPLE(db, dbtype);
            Row_R_ORTSAMPLE rowORTSample = (Row_R_ORTSAMPLE)this.NewRow();
            rowORTSample.ID = this.GetNewID(bu,db);
            rowORTSample.SKUNO = skuno;
            rowORTSample.WO = wo;
            rowORTSample.SN = sn;
            rowORTSample.LOCKED_FLAG = flag;
            rowORTSample.EDIT_EMP = empNo;
            rowORTSample.EDIT_TIME = this.GetDBDateTime(db);
            return db.ExecSQL(rowORTSample.GetInsertString(dbtype));
        }
    }
    public class Row_R_ORTSAMPLE : DataObjectBase
    {
        public Row_R_ORTSAMPLE(DataObjectInfo info) : base(info)
        {

        }
        public R_ORTSAMPLE GetDataObject()
        {
            R_ORTSAMPLE DataObject = new R_ORTSAMPLE();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WO = this.WO;
            DataObject.SN = this.SN;
            DataObject.LOCKED_FLAG = this.LOCKED_FLAG;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string LOCKED_FLAG
        {
            get
            {
                return (string)this["LOCKED_FLAG"];
            }
            set
            {
                this["LOCKED_FLAG"] = value;
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
    public class R_ORTSAMPLE
    {
        public string ID;
        public string SKUNO;
        public string WO;
        public string SN;
        public string LOCKED_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}