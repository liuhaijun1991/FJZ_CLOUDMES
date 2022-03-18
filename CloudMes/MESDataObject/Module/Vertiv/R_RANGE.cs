using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_RANGE : DataObjectTable
    {
        public T_R_RANGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RANGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RANGE);
            TableName = "R_RANGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_RANGE> GetAllData(OleExec DB, string cType, string skuNo)
        {
            return DB.ORM.Queryable<R_RANGE>().Where(t => t.CTYPE == cType && t.VUL == skuNo && t.VALID == "Y").ToList();
        }
        public bool CheckExist(OleExec DB, string ruleId, string cType, string skuNo)
        {
            return DB.ORM.Queryable<R_RANGE>().Where(t => t.RULEID == ruleId && t.CTYPE == cType && t.VUL == skuNo && t.VALID == "Y").Any();
        }
    }
    public class Row_R_RANGE : DataObjectBase
    {
        public Row_R_RANGE(DataObjectInfo info) : base(info)
        {

        }
        public R_RANGE GetDataObject()
        {
            R_RANGE DataObject = new R_RANGE();
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.VALID = this.VALID;
            DataObject.VUL = this.VUL;
            DataObject.CTYPE = this.CTYPE;
            DataObject.RULEID = this.RULEID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string VALID
        {
            get
            {
                return (string)this["VALID"];
            }
            set
            {
                this["VALID"] = value;
            }
        }
        public string VUL
        {
            get
            {
                return (string)this["VUL"];
            }
            set
            {
                this["VUL"] = value;
            }
        }
        public string CTYPE
        {
            get
            {
                return (string)this["CTYPE"];
            }
            set
            {
                this["CTYPE"] = value;
            }
        }
        public string RULEID
        {
            get
            {
                return (string)this["RULEID"];
            }
            set
            {
                this["RULEID"] = value;
            }
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
    }
    public class R_RANGE
    {
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string VALID { get; set; }
        public string VUL { get; set; }
        public string CTYPE { get; set; }
        public string RULEID { get; set; }
        public string ID { get; set; }
    }
}
