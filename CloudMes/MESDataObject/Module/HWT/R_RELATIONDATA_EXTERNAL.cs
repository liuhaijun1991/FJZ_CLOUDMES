using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_RELATIONDATA_EXTERNAL : DataObjectTable
    {
        public T_R_RELATIONDATA_EXTERNAL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RELATIONDATA_EXTERNAL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RELATIONDATA_EXTERNAL);
            TableName = "R_RELATIONDATA_EXTERNAL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// add by hgb 2019.08.25
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExists(string SN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM R_RELATIONDATA_EXTERNAL WHERE SN = '{SN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// add by hgb 2019.08.25
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_RELATIONDATA_EXTERNAL LoadData(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_RELATIONDATA_EXTERNAL>().Where(t => t.SN == SN).ToList().FirstOrDefault();
        }

        public int Insert(R_RELATIONDATA_EXTERNAL R_RELATIONDATA_EXTERNAL, OleExec DB)
        {
            return DB.ORM.Insertable<R_RELATIONDATA_EXTERNAL>(R_RELATIONDATA_EXTERNAL).ExecuteCommand();
        }
    }
    public class Row_R_RELATIONDATA_EXTERNAL : DataObjectBase
    {
        public Row_R_RELATIONDATA_EXTERNAL(DataObjectInfo info) : base(info)
        {

        }
        public R_RELATIONDATA_EXTERNAL GetDataObject()
        {
            R_RELATIONDATA_EXTERNAL DataObject = new R_RELATIONDATA_EXTERNAL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.PARENT = this.PARENT;
            DataObject.RECEIVE_FLAG = this.RECEIVE_FLAG;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
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
        public string PARENT
        {
            get
            {
                return (string)this["PARENT"];
            }
            set
            {
                this["PARENT"] = value;
            }
        }
        public string RECEIVE_FLAG
        {
            get
            {
                return (string)this["RECEIVE_FLAG"];
            }
            set
            {
                this["RECEIVE_FLAG"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
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
    public class R_RELATIONDATA_EXTERNAL
    {
        public string ID{ get; set; }
        public string SN{ get; set; }
        public string PARENT{ get; set; }
        public string RECEIVE_FLAG{ get; set; }
        public string DATA1{ get; set; }
        public string DATA2{ get; set; }
        public string DATA3{ get; set; }
        public string EDIT_EMP{ get; set; }
        public DateTime? EDIT_TIME{ get; set; }
    }
}