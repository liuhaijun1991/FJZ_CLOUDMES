using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ORDER_SN : DataObjectTable
    {
        public T_R_ORDER_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORDER_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORDER_SN);
            TableName = "R_ORDER_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORDER_SN : DataObjectBase
    {
        public Row_R_ORDER_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_ORDER_SN GetDataObject()
        {
            R_ORDER_SN DataObject = new R_ORDER_SN();
            DataObject.ID = this.ID;
            DataObject.DNID = this.DNID;
            DataObject.GROUPTYPE = this.GROUPTYPE;
            DataObject.SN = this.SN;
            DataObject.ORDERNO = this.ORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PRINTFLAG = this.PRINTFLAG;
            DataObject.OVERPACKTIME = this.OVERPACKTIME;
            DataObject.LASTEDITBY = this.LASTEDITBY;
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
        public string DNID
        {
            get
            {
                return (string)this["DNID"];
            }
            set
            {
                this["DNID"] = value;
            }
        }
        public string GROUPTYPE
        {
            get
            {
                return (string)this["GROUPTYPE"];
            }
            set
            {
                this["GROUPTYPE"] = value;
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
        public string ORDERNO
        {
            get
            {
                return (string)this["ORDERNO"];
            }
            set
            {
                this["ORDERNO"] = value;
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
        public string PRINTFLAG
        {
            get
            {
                return (string)this["PRINTFLAG"];
            }
            set
            {
                this["PRINTFLAG"] = value;
            }
        }
        public DateTime? OVERPACKTIME
        {
            get
            {
                return (DateTime?)this["OVERPACKTIME"];
            }
            set
            {
                this["OVERPACKTIME"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
    }
    public class R_ORDER_SN
    {
        public string ID { get; set; }
        public string DNID { get; set; }
        public string GROUPTYPE { get; set; }
        public string SN { get; set; }
        public string ORDERNO { get; set; }
        public string SKUNO { get; set; }
        public string PRINTFLAG { get; set; }
        public DateTime? OVERPACKTIME { get; set; }
        public string LASTEDITBY { get; set; }
    }
}