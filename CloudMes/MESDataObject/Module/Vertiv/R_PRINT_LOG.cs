using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Vertiv
{
    public class T_R_PRINT_LOG : DataObjectTable
    {
        public T_R_PRINT_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PRINT_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PRINT_LOG);
            TableName = "R_PRINT_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PRINT_LOG : DataObjectBase
    {
        public Row_R_PRINT_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_PRINT_LOG GetDataObject()
        {
            R_PRINT_LOG DataObject = new R_PRINT_LOG();
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.CTYPE = this.CTYPE;
            DataObject.LABTYPE = this.LABTYPE;
            DataObject.STATION = this.STATION;
            DataObject.SN = this.SN;
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
        public string LABTYPE
        {
            get
            {
                return (string)this["LABTYPE"];
            }
            set
            {
                this["LABTYPE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
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
    public class R_PRINT_LOG
    {
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string CTYPE { get; set; }
        public string LABTYPE { get; set; }
        public string STATION { get; set; }
        public string SN { get; set; }
        public string ID { get; set; }
    }
}
