using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PACKING_FIFO : DataObjectTable
    {
        public T_R_PACKING_FIFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PACKING_FIFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PACKING_FIFO);
            TableName = "R_PACKING_FIFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PACKING_FIFO : DataObjectBase
    {
        public Row_R_PACKING_FIFO(DataObjectInfo info) : base(info)
        {

        }
        public R_PACKING_FIFO GetDataObject()
        {
            R_PACKING_FIFO DataObject = new R_PACKING_FIFO();
            DataObject.ID = this.ID;
            DataObject.PACKNO = this.PACKNO;
            DataObject.STATUS = this.STATUS;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string PACKNO
        {
            get
            {
                return (string)this["PACKNO"];
            }
            set
            {
                this["PACKNO"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
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
    public class R_PACKING_FIFO
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string PACKNO { get; set; }
        public string STATUS { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        public string REASON { get; set; }        
    }
}