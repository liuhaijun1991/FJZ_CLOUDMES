using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_REPLACEPN : DataObjectTable
    {
        public T_R_JNP_REPLACEPN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_REPLACEPN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_REPLACEPN);
            TableName = "R_JNP_REPLACEPN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_REPLACEPN : DataObjectBase
    {
        public Row_R_JNP_REPLACEPN(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_REPLACEPN GetDataObject()
        {
            R_JNP_REPLACEPN DataObject = new R_JNP_REPLACEPN();
            DataObject.ID = this.ID;
            DataObject.UPOID = this.UPOID;
            DataObject.POID = this.POID;
            DataObject.PARTNO = this.PARTNO;
            DataObject.REPLACEPN = this.REPLACEPN;
            DataObject.VALIDFLAG = this.VALIDFLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.REPLACEPNREV = this.REPLACEPNREV;
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
        public string UPOID
        {
            get
            {
                return (string)this["UPOID"];
            }
            set
            {
                this["UPOID"] = value;
            }
        }
        public string POID
        {
            get
            {
                return (string)this["POID"];
            }
            set
            {
                this["POID"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string REPLACEPN
        {
            get
            {
                return (string)this["REPLACEPN"];
            }
            set
            {
                this["REPLACEPN"] = value;
            }
        }
        public string VALIDFLAG
        {
            get
            {
                return (string)this["VALIDFLAG"];
            }
            set
            {
                this["VALIDFLAG"] = value;
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
        public string REPLACEPNREV
        {
            get
            {
                return (string)this["REPLACEPNREV"];
            }
            set
            {
                this["REPLACEPNREV"] = value;
            }
        }
    }
    public class R_JNP_REPLACEPN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string UPOID { get; set; }
        public string POID { get; set; }
        public string PARTNO { get; set; }
        public string REPLACEPN { get; set; }
        public string VALIDFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public string REPLACEPNREV { get; set; }
    }
}