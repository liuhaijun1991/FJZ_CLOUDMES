using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MATERIAL_STAGE_DETAIL : DataObjectTable
    {
        public T_R_MATERIAL_STAGE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MATERIAL_STAGE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MATERIAL_STAGE_DETAIL);
            TableName = "R_MATERIAL_STAGE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MATERIAL_STAGE_DETAIL : DataObjectBase
    {
        public Row_R_MATERIAL_STAGE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_MATERIAL_STAGE_DETAIL GetDataObject()
        {
            R_MATERIAL_STAGE_DETAIL DataObject = new R_MATERIAL_STAGE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.EVENTPOINT = this.EVENTPOINT;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.CUSTPNDESC = this.CUSTPNDESC;
            DataObject.QTY = this.QTY;
            DataObject.SOPPAGE = this.SOPPAGE;
            DataObject.SOPEVENTPN = this.SOPEVENTPN;
            DataObject.NOTE = this.NOTE;
            DataObject.TOTALQTY = this.TOTALQTY;
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.VAILD = this.VAILD;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string EVENTPOINT
        {
            get
            {
                return (string)this["EVENTPOINT"];
            }
            set
            {
                this["EVENTPOINT"] = value;
            }
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
            }
        }
        public string CUSTPNDESC
        {
            get
            {
                return (string)this["CUSTPNDESC"];
            }
            set
            {
                this["CUSTPNDESC"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string SOPPAGE
        {
            get
            {
                return (string)this["SOPPAGE"];
            }
            set
            {
                this["SOPPAGE"] = value;
            }
        }
        public string SOPEVENTPN
        {
            get
            {
                return (string)this["SOPEVENTPN"];
            }
            set
            {
                this["SOPEVENTPN"] = value;
            }
        }
        public string NOTE
        {
            get
            {
                return (string)this["NOTE"];
            }
            set
            {
                this["NOTE"] = value;
            }
        }
        public string TOTALQTY
        {
            get
            {
                return (string)this["TOTALQTY"];
            }
            set
            {
                this["TOTALQTY"] = value;
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
        public string VAILD
        {
            get
            {
                return (string)this["VAILD"];
            }
            set
            {
                this["VAILD"] = value;
            }
        }
    }
    public class R_MATERIAL_STAGE_DETAIL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string EVENTPOINT { get; set; }
        public string CUSTPARTNO { get; set; }
        public string CUSTPNDESC { get; set; }
        public double? QTY { get; set; }
        public string SOPPAGE { get; set; }
        public string SOPEVENTPN { get; set; }
        public string NOTE { get; set; }
        public string TOTALQTY { get; set; }
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string VAILD { get; set; }
    }
}