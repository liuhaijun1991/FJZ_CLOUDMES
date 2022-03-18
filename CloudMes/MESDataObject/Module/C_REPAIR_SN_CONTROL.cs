using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_REPAIR_SN_CONTROL : DataObjectTable
    {
        public T_C_REPAIR_SN_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REPAIR_SN_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REPAIR_SN_CONTROL);
            TableName = "C_REPAIR_SN_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_REPAIR_SN_CONTROL : DataObjectBase
    {
        public Row_C_REPAIR_SN_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public C_REPAIR_SN_CONTROL GetDataObject()
        {
            C_REPAIR_SN_CONTROL DataObject = new C_REPAIR_SN_CONTROL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.STATION = this.STATION;
            DataObject.REPAIRCOUNT = this.REPAIRCOUNT;
            DataObject.REASON = this.REASON;
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
        public double? REPAIRCOUNT
        {
            get
            {
                return (double?)this["REPAIRCOUNT"];
            }
            set
            {
                this["REPAIRCOUNT"] = value;
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
    public class C_REPAIR_SN_CONTROL
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string STATION { get; set; }
        public double? REPAIRCOUNT { get; set; }
        public string REASON { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}