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
    public class T_C_REWORK : DataObjectTable
    {
        public T_C_REWORK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REWORK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REWORK);
            TableName = "C_REWORK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_REWORK> GetRWSN(string SkuNo, string RWSkuNo, string RWWO, string SN, OleExec DB)
        {
            return DB.ORM.Queryable<C_REWORK>().Where(t => t.SKUNO == SkuNo && t.RW_SKUNO == RWSkuNo && RWWO.Contains(t.RW_WO) && SN.Contains(t.SN)).ToList();
        }
        public List<C_REWORK> GetRWSKUNOSKUNOREWO(string SkuNo, string RWSkuNo, string RWWO, OleExec DB)
        {
            return DB.ORM.Queryable<C_REWORK>().Where(t => t.SKUNO == SkuNo && t.RW_SKUNO == RWSkuNo && RWWO.Contains(t.RW_WO)).ToList();
        }
        /// <summary>
        /// 通過ID獲取單條信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_REWORK> GetBySn(string RW_WO, OleExec DB)
        {
            return DB.ORM.Queryable<C_REWORK>().Where(t => t.RW_WO ==RW_WO).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        /// <summary>
        /// 通過ID獲取信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_REWORK> GetById(string ID, OleExec DB)
        {
            return DB.ORM.Queryable<C_REWORK>().Where(t => t.ID == ID).ToList();
        }

        /// <summary>
        ///check 工單是否是重工工單
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_WO_BASE> CheckWOIsRework(string WO, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).ToList();
        }

        //public List<C_REWORK> AddReworkWO(string ID, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_REWORK>().Where(t => t.ID == ID).ToList();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_REWORK> GetAllRework(OleExec DB)
        {
            string strSql = $@"select *from C_REWORK ORDER BY EDIT_TIME DESC";
            List<C_REWORK> result = new List<C_REWORK>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_REWORK ret = (Row_C_REWORK)NewRow();
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

        /// <summary>
        /// 添加新的信息
        /// </summary>
        /// <param name="NewActionCode"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddReworkWO(C_REWORK NewCODE, OleExec DB)
        {
            Row_C_REWORK NewReworkWo = (Row_C_REWORK)NewRow();
            NewReworkWo.ID = NewCODE.ID;
            NewReworkWo.SN = NewCODE.SN;
            NewReworkWo.R_SN_ID = NewCODE.R_SN_ID;
            NewReworkWo.SKUNO = NewCODE.SKUNO;
            NewReworkWo.RW_SKUNO = NewCODE.RW_SKUNO;
            NewReworkWo.RW_WO = NewCODE.RW_WO;
            NewReworkWo.EDIT_TIME = NewCODE.EDIT_TIME;
            NewReworkWo.EDIT_EMP = NewCODE.EDIT_EMP;
            int result = DB.ExecuteNonQuery(NewReworkWo.GetInsertString(DBType), CommandType.Text);
            return result;
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="NewCODE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateById(C_REWORK NewCODE, OleExec DB)
        {
            Row_C_REWORK NewReworkWo = (Row_C_REWORK)NewRow();
            NewReworkWo.ID = NewCODE.ID;
            NewReworkWo.SN= NewCODE.SN;
            NewReworkWo.R_SN_ID = NewCODE.R_SN_ID;
            NewReworkWo.SKUNO = NewCODE.SKUNO;
            NewReworkWo.RW_SKUNO = NewCODE.RW_SKUNO;
            NewReworkWo.RW_WO = NewCODE.RW_WO;
            NewReworkWo.EDIT_TIME = NewCODE.EDIT_TIME;
            NewReworkWo.EDIT_EMP = NewCODE.EDIT_EMP;          
            int result = DB.ExecuteNonQuery(NewReworkWo.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
            return result;
        }

        /// <summary>
        ///刪除
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
    public class Row_C_REWORK : DataObjectBase
    {
        public Row_C_REWORK(DataObjectInfo info) : base(info)
        {

        }
        public C_REWORK GetDataObject()
        {
            C_REWORK DataObject = new C_REWORK();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.RW_SKUNO = this.RW_SKUNO;
            DataObject.RW_WO = this.RW_WO;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
        public string RW_SKUNO
        {
            get
            {
                return (string)this["RW_SKUNO"];
            }
            set
            {
                this["RW_SKUNO"] = value;
            }
        }
        public string RW_WO
        {
            get
            {
                return (string)this["RW_WO"];
            }
            set
            {
                this["RW_WO"] = value;
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
    public class C_REWORK
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string R_SN_ID { get; set; }
        public string SKUNO { get; set; }
        public string RW_SKUNO { get; set; }
        public string RW_WO { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}