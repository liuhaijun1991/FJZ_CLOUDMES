using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_C_SERVICE_CONTROL : DataObjectTable
    {
        public T_C_SERVICE_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SERVICE_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SERVICE_CONTROL);
            TableName = "C_SERVICE_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SERVICE_CONTROL : DataObjectBase
    {
        public Row_C_SERVICE_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public C_SERVICE_CONTROL GetDataObject()
        {
            C_SERVICE_CONTROL DataObject = new C_SERVICE_CONTROL();
            DataObject.ID = this.ID;
            DataObject.SERVERNAME = this.SERVERNAME;
            DataObject.TIMESPAN = this.TIMESPAN;
            DataObject.RUNSTATUS = this.RUNSTATUS;
            DataObject.SERVERFUNCTION = this.SERVERFUNCTION;
            DataObject.FUNCTIONNAME = this.FUNCTIONNAME;
            DataObject.CLASSNAME = this.CLASSNAME;
            DataObject.DESCRIPTIONS = this.DESCRIPTIONS;
            DataObject.CURRENTIP = this.CURRENTIP;
            DataObject.RUNTIME = this.RUNTIME;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
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
        public string SERVERNAME
        {
            get
            {
                return (string)this["SERVERNAME"];
            }
            set
            {
                this["SERVERNAME"] = value;
            }
        }
        public string TIMESPAN
        {
            get
            {
                return (string)this["TIMESPAN"];
            }
            set
            {
                this["TIMESPAN"] = value;
            }
        }
        public string RUNSTATUS
        {
            get
            {
                return (string)this["RUNSTATUS"];
            }
            set
            {
                this["RUNSTATUS"] = value;
            }
        }
        public string SERVERFUNCTION
        {
            get
            {
                return (string)this["SERVERFUNCTION"];
            }
            set
            {
                this["SERVERFUNCTION"] = value;
            }
        }
        public string FUNCTIONNAME
        {
            get
            {
                return (string)this["FUNCTIONNAME"];
            }
            set
            {
                this["FUNCTIONNAME"] = value;
            }
        }
        public string CLASSNAME
        {
            get
            {
                return (string)this["CLASSNAME"];
            }
            set
            {
                this["CLASSNAME"] = value;
            }
        }
        public string DESCRIPTIONS
        {
            get
            {
                return (string)this["DESCRIPTIONS"];
            }
            set
            {
                this["DESCRIPTIONS"] = value;
            }
        }
        public string CURRENTIP
        {
            get
            {
                return (string)this["CURRENTIP"];
            }
            set
            {
                this["CURRENTIP"] = value;
            }
        }
        public DateTime? RUNTIME
        {
            get
            {
                return (DateTime?)this["RUNTIME"];
            }
            set
            {
                this["RUNTIME"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DATA4
        {
            get
            {
                return (string)this["DATA4"];
            }
            set
            {
                this["DATA4"] = value;
            }
        }
        public string DATA5
        {
            get
            {
                return (string)this["DATA5"];
            }
            set
            {
                this["DATA5"] = value;
            }
        }
    }
    public class C_SERVICE_CONTROL
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public string ID { get; set; }
        public string SERVERNAME { get; set; }
        public string TIMESPAN { get; set; }
        public string RUNSTATUS { get; set; }
        public string SERVERFUNCTION { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string CLASSNAME { get; set; }
        public string DESCRIPTIONS { get; set; }
        public string CURRENTIP { get; set; }
        public DateTime? RUNTIME { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
    }
}