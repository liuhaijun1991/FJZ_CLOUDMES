using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_I285_MAIN : DataObjectTable
    {
        public T_R_I285_MAIN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I285_MAIN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I285_MAIN);
            TableName = "R_I285_MAIN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I285_MAIN : DataObjectBase
    {
        public Row_R_I285_MAIN(DataObjectInfo info) : base(info)
        {

        }
        public R_I285_MAIN GetDataObject()
        {
            R_I285_MAIN DataObject = new R_I285_MAIN();
            DataObject.ID = this.ID;
            DataObject.FILENAME = this.FILENAME;
            DataObject.VALID = this.VALID;
            DataObject.TRANID = this.TRANID;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
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
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
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
    }
    public class R_I285_MAIN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string FILENAME { get; set; }
        public string VALID { get; set; }
        public string TRANID { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? STOCKINGTIME { get; set; }
    }
}