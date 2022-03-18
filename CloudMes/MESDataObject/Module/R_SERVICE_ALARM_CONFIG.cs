using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SERVICE_ALARM_CONFIG : DataObjectTable
    {
        public T_R_SERVICE_ALARM_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SERVICE_ALARM_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SERVICE_ALARM_CONFIG);
            TableName = "R_SERVICE_ALARM_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SERVICE_ALARM_CONFIG : DataObjectBase
    {
        public Row_R_SERVICE_ALARM_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public R_SERVICE_ALARM_CONFIG GetDataObject()
        {
            R_SERVICE_ALARM_CONFIG DataObject = new R_SERVICE_ALARM_CONFIG();
            DataObject.CONFIGLEVEL = this.CONFIGLEVEL;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEEMP = this.CREATEEMP;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITEMP = this.EDITEMP;
            DataObject.ID = this.ID;
            DataObject.SERVICENAME = this.SERVICENAME;
            DataObject.TIMESPAN = this.TIMESPAN;
            DataObject.RUNSTATUS = this.RUNSTATUS;
            DataObject.RUNTYPE = this.RUNTYPE;
            DataObject.RUNTYPEDESC = this.RUNTYPEDESC;
            DataObject.CONTROLTYPE = this.CONTROLTYPE;
            DataObject.CONTROLCODE = this.CONTROLCODE;
            DataObject.CONTROLTYPEDESC = this.CONTROLTYPEDESC;
            DataObject.CONTROLLEVEL = this.CONTROLLEVEL;
            DataObject.CONTROLFLAG = this.CONTROLFLAG;
            DataObject.TRIGGERFUNCTION = this.TRIGGERFUNCTION;
            DataObject.SEARCHFUNCTION = this.SEARCHFUNCTION;
            DataObject.RUNFUNCTION = this.RUNFUNCTION;
            DataObject.ALARMTITLE = this.ALARMTITLE;
            DataObject.ALARMCONTENT = this.ALARMCONTENT;
            DataObject.ALARMIP = this.ALARMIP;
            DataObject.REMARK = this.REMARK;
            return DataObject;
        }
        public string CONFIGLEVEL
        {
            get
            {
                return (string)this["CONFIGLEVEL"];
            }
            set
            {
                this["CONFIGLEVEL"] = value;
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
        public string CREATEEMP
        {
            get
            {
                return (string)this["CREATEEMP"];
            }
            set
            {
                this["CREATEEMP"] = value;
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
        public string EDITEMP
        {
            get
            {
                return (string)this["EDITEMP"];
            }
            set
            {
                this["EDITEMP"] = value;
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
        public string SERVICENAME
        {
            get
            {
                return (string)this["SERVICENAME"];
            }
            set
            {
                this["SERVICENAME"] = value;
            }
        }
        public double? TIMESPAN
        {
            get
            {
                return (double?)this["TIMESPAN"];
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
        public string RUNTYPE
        {
            get
            {
                return (string)this["RUNTYPE"];
            }
            set
            {
                this["RUNTYPE"] = value;
            }
        }
        public string RUNTYPEDESC
        {
            get
            {
                return (string)this["RUNTYPEDESC"];
            }
            set
            {
                this["RUNTYPEDESC"] = value;
            }
        }
        public string CONTROLTYPE
        {
            get
            {
                return (string)this["CONTROLTYPE"];
            }
            set
            {
                this["CONTROLTYPE"] = value;
            }
        }
        public string CONTROLCODE
        {
            get
            {
                return (string)this["CONTROLCODE"];
            }
            set
            {
                this["CONTROLCODE"] = value;
            }
        }
        public string CONTROLTYPEDESC
        {
            get
            {
                return (string)this["CONTROLTYPEDESC"];
            }
            set
            {
                this["CONTROLTYPEDESC"] = value;
            }
        }
        public string CONTROLLEVEL
        {
            get
            {
                return (string)this["CONTROLLEVEL"];
            }
            set
            {
                this["CONTROLLEVEL"] = value;
            }
        }
        public string CONTROLFLAG
        {
            get
            {
                return (string)this["CONTROLFLAG"];
            }
            set
            {
                this["CONTROLFLAG"] = value;
            }
        }
        public string TRIGGERFUNCTION
        {
            get
            {
                return (string)this["TRIGGERFUNCTION"];
            }
            set
            {
                this["TRIGGERFUNCTION"] = value;
            }
        }
        public string SEARCHFUNCTION
        {
            get
            {
                return (string)this["SEARCHFUNCTION"];
            }
            set
            {
                this["SEARCHFUNCTION"] = value;
            }
        }
        public string RUNFUNCTION
        {
            get
            {
                return (string)this["RUNFUNCTION"];
            }
            set
            {
                this["RUNFUNCTION"] = value;
            }
        }
        public string ALARMTITLE
        {
            get
            {
                return (string)this["ALARMTITLE"];
            }
            set
            {
                this["ALARMTITLE"] = value;
            }
        }
        public string ALARMCONTENT
        {
            get
            {
                return (string)this["ALARMCONTENT"];
            }
            set
            {
                this["ALARMCONTENT"] = value;
            }
        }
        public string ALARMIP
        {
            get
            {
                return (string)this["ALARMIP"];
            }
            set
            {
                this["ALARMIP"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
    }
    public class R_SERVICE_ALARM_CONFIG
    {
        public string CONFIGLEVEL { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEEMP { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITEMP { get; set; }
        public string ID { get; set; }
        public string SERVICENAME { get; set; }
        public double? TIMESPAN { get; set; }
        public string RUNSTATUS { get; set; }
        public string RUNTYPE { get; set; }
        public string RUNTYPEDESC { get; set; }
        public string CONTROLTYPE { get; set; }
        public string CONTROLCODE { get; set; }
        public string CONTROLTYPEDESC { get; set; }
        public string CONTROLLEVEL { get; set; }
        public string CONTROLFLAG { get; set; }
        public string TRIGGERFUNCTION { get; set; }
        public string SEARCHFUNCTION { get; set; }
        public string RUNFUNCTION { get; set; }
        public string ALARMTITLE { get; set; }
        public string ALARMCONTENT { get; set; }
        public string ALARMIP { get; set; }
        public string REMARK { get; set; }
    }
}