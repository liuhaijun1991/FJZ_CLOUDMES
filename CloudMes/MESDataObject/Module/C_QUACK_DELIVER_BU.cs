using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_QUACK_DELIVER_BU : DataObjectTable
    {
        public T_C_QUACK_DELIVER_BU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_QUACK_DELIVER_BU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_QUACK_DELIVER_BU);
            TableName = "C_QUACK_DELIVER_BU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 查詢LISTNAME
        /// </summary>
        /// <param name="kpID"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<string> GetListByBU(OleExec DB)
        {
            return DB.ORM.Queryable<C_QUACK_DELIVER_BU>().Select(t => t.BU).ToList();
            //string sql = string.Empty;
            //DataTable dt = new DataTable();
            //List<C_QUACK_DELIVER_BU> CQUACKDELIVERBU = new List<C_QUACK_DELIVER_BU>();
            //sql = $@"Select * from c_quack_deliver_bu";
            //dt = DB.ExecSelect(sql).Tables[0];
            //foreach (DataRow item in dt.Rows)
            //{
            //    Row_C_QUACK_DELIVER_BU ret = (Row_C_QUACK_DELIVER_BU)NewRow();
            //    ret.loadData(item);
            //    CQUACKDELIVERBU.Add(ret.GetDataObject());
            //}
            //return CQUACKDELIVERBU;
        }
    }
    public class Row_C_QUACK_DELIVER_BU : DataObjectBase
    {
        public Row_C_QUACK_DELIVER_BU(DataObjectInfo info) : base(info)
        {

        }
        public C_QUACK_DELIVER_BU GetDataObject()
        {
            C_QUACK_DELIVER_BU DataObject = new C_QUACK_DELIVER_BU();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.FLOOR = this.FLOOR;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string FLOOR
        {
            get
            {
                return (string)this["FLOOR"];
            }
            set
            {
                this["FLOOR"] = value;
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
    public class C_QUACK_DELIVER_BU
    {
        public string ID;
        public string BU;
        public string FLOOR;
        public string VALID_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}