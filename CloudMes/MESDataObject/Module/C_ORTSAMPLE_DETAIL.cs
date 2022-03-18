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
    public class T_C_ORTSAMPLE_DETAIL : DataObjectTable
    {
        public T_C_ORTSAMPLE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ORTSAMPLE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ORTSAMPLE_DETAIL);
            TableName = "C_ORTSAMPLE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_ORTSAMPLE_DETAIL GetSampleDetailByRiskAndQTY(string risk, double? woQty, OleExec db)
        {
            string strSql = $@" select * from C_ORTSAMPLE_DETAIL where risk = '{risk}' and {woQty}>=min_qty and max_qty <= {woQty} ";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_ORTSAMPLE_DETAIL result = new C_ORTSAMPLE_DETAIL();
            if (table.Rows.Count > 0)
            {
                Row_C_ORTSAMPLE_DETAIL ret = (Row_C_ORTSAMPLE_DETAIL)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public List<C_ORTSAMPLE_DETAIL> GetAllrisk(OleExec DB)
        {
            //return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).ToList();
            //return DB.ORM.Queryable<C_ORTSAMPLE_DETAIL>().ToList();

            string strSql = $@"select *from C_ORTSAMPLE_DETAIL ORDER BY EDIT_TIME DESC";
            List<C_ORTSAMPLE_DETAIL> result = new List<C_ORTSAMPLE_DETAIL>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ORTSAMPLE_DETAIL ret = (Row_C_ORTSAMPLE_DETAIL)NewRow();
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

        public List<C_ORTSAMPLE_DETAIL> GetByrisk(string risk, OleExec DB)
        {
            string strSql = $@"select *from C_ORTSAMPLE_DETAIL WHERE RISK='{risk}'";
            List<C_ORTSAMPLE_DETAIL> result = new List<C_ORTSAMPLE_DETAIL>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count>0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ORTSAMPLE_DETAIL ret = (Row_C_ORTSAMPLE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="ort"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddNewOrt(C_ORTSAMPLE_DETAIL ort, OleExec DB)
        {
            //return DB.ORM.Insertable(C_ORTSAMPLE_DETAI).ExecuteCommand();

            Row_C_ORTSAMPLE_DETAIL Newort = (Row_C_ORTSAMPLE_DETAIL)NewRow();
            Newort.ID = ort.ID;
            Newort.RISK = ort.RISK;
            Newort.MIN_QTY = ort.MIN_QTY;
            Newort.MAX_QTY = ort.MAX_QTY;
            Newort.SAMPLE_QTY = ort.SAMPLE_QTY;
            Newort.EDIT_TIME = ort.EDIT_TIME;
            Newort.EDIT_EMP = ort.EDIT_EMP;
            int result = DB.ExecuteNonQuery(Newort.GetInsertString(DBType), CommandType.Text);
            return result;
        }

        /// <summary>
        /// 根據ID修改
        /// </summary>
        /// <param name="ort"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateById(C_ORTSAMPLE_DETAIL ort, OleExec DB)
        {
            Row_C_ORTSAMPLE_DETAIL Newort = (Row_C_ORTSAMPLE_DETAIL)NewRow();
            Newort.ID = ort.ID;
            Newort.RISK = ort.RISK;
            Newort.MIN_QTY = ort.MIN_QTY;
            Newort.MAX_QTY = ort.MAX_QTY;
            Newort.SAMPLE_QTY = ort.SAMPLE_QTY;
            Newort.EDIT_TIME = ort.EDIT_TIME;
            Newort.EDIT_EMP = ort.EDIT_EMP;
            int result = DB.ExecuteNonQuery(Newort.GetUpdateString(DBType), CommandType.Text);
            return result;
        }

        /// <summary>
        /// 根據ID刪除
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"delete c_rework where ID=:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

    }
    public class Row_C_ORTSAMPLE_DETAIL : DataObjectBase
    {
        public Row_C_ORTSAMPLE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_ORTSAMPLE_DETAIL GetDataObject()
        {
            C_ORTSAMPLE_DETAIL DataObject = new C_ORTSAMPLE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.RISK = this.RISK;
            DataObject.MIN_QTY = this.MIN_QTY;
            DataObject.MAX_QTY = this.MAX_QTY;
            DataObject.SAMPLE_QTY = this.SAMPLE_QTY;
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
        public double? MIN_QTY
        {
            get
            {
                return (double?)this["MIN_QTY"];
            }
            set
            {
                this["MIN_QTY"] = value;
            }
        }
        public double? MAX_QTY
        {
            get
            {
                return (double?)this["MAX_QTY"];
            }
            set
            {
                this["MAX_QTY"] = value;
            }
        }
        public double? SAMPLE_QTY
        {
            get
            {
                return (double?)this["SAMPLE_QTY"];
            }
            set
            {
                this["SAMPLE_QTY"] = value;
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
    public class C_ORTSAMPLE_DETAIL
    {
        public string ID;
        public string RISK;
        public double? MIN_QTY;
        public double? MAX_QTY;
        public double? SAMPLE_QTY;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;

        public static C_ORTSAMPLE_DETAIL CheckID(string iD, OleExec sfcdb)
        {
            throw new NotImplementedException();
        }
    }
}