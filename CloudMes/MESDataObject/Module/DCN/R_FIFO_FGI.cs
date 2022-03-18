using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_FIFO_FGI : DataObjectTable
    {
        public T_R_FIFO_FGI(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FIFO_FGI(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FIFO_FGI);
            TableName = "R_FIFO_FGI".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_FIFO_FGI : DataObjectBase
    {
        public Row_R_FIFO_FGI(DataObjectInfo info) : base(info)
        {

        }
        public R_FIFO_FGI GetDataObject()
        {
            R_FIFO_FGI DataObject = new R_FIFO_FGI();
            DataObject.ID = this.ID;
            DataObject.PALLETNO = this.PALLETNO;
            DataObject.REASON = this.REASON;
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
        public string PALLETNO
        {
            get
            {
                return (string)this["PALLETNO"];
            }
            set
            {
                this["PALLETNO"] = value;
            }
        }
        public string REASON
        {
            get
            {
                return (string)this["REASON"];
            }
            set
            {
                this["REASON"] = value;
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
    public class R_FIFO_FGI
    {
        public string ID { get; set; }
        public string PALLETNO { get; set; }
        public string REASON { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}