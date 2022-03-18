using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_CSD_T : DataObjectTable
    {
        public T_R_JNP_CSD_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_CSD_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_CSD_T);
            TableName = "R_JNP_CSD_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_CSD_T : DataObjectBase
    {
        public Row_R_JNP_CSD_T(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_CSD_T GetDataObject()
        {
            R_JNP_CSD_T DataObject = new R_JNP_CSD_T();
            DataObject.ID = this.ID;
            DataObject.PONO = this.PONO;
            DataObject.POLINE = this.POLINE;
            DataObject.CSD = this.CSD;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.VALIDFLAG = this.VALIDFLAG;
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
        public string PONO
        {
            get
            {
                return (string)this["PONO"];
            }
            set
            {
                this["PONO"] = value;
            }
        }
        public string POLINE
        {
            get
            {
                return (string)this["POLINE"];
            }
            set
            {
                this["POLINE"] = value;
            }
        }
        public DateTime? CSD
        {
            get
            {
                return (DateTime?)this["CSD"];
            }
            set
            {
                this["CSD"] = value;
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
    }
    public class R_JNP_CSD_T
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public DateTime? CSD { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public string VALIDFLAG { get; set; }
    }
}