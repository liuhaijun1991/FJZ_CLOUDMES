using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PTM_TACONFIG : DataObjectTable
    {
        public T_R_PTM_TACONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PTM_TACONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PTM_TACONFIG);
            TableName = "R_PTM_TACONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PTM_TACONFIG : DataObjectBase
    {
        public Row_R_PTM_TACONFIG(DataObjectInfo info) : base(info)
        {

        }
        public R_PTM_TACONFIG GetDataObject()
        {
            R_PTM_TACONFIG DataObject = new R_PTM_TACONFIG();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PREXID = this.PREXID;
            DataObject.TA_NUMBER = this.TA_NUMBER;
            DataObject.VALUE1 = this.VALUE1;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string PREXID
        {
            get
            {
                return (string)this["PREXID"];
            }
            set
            {
                this["PREXID"] = value;
            }
        }
        public string TA_NUMBER
        {
            get
            {
                return (string)this["TA_NUMBER"];
            }
            set
            {
                this["TA_NUMBER"] = value;
            }
        }
        public string VALUE1
        {
            get
            {
                return (string)this["VALUE1"];
            }
            set
            {
                this["VALUE1"] = value;
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
    }
    public class R_PTM_TACONFIG
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string SKUNO { get; set; }
        public string PREXID { get; set; }
        public string TA_NUMBER { get; set; }
        public string VALUE1 { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}