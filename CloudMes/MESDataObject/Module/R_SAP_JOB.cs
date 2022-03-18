using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_SAP_JOB : DataObjectTable
    {
        public T_R_SAP_JOB(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_JOB(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_JOB);
            TableName = "R_SAP_JOB".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_JOB : DataObjectBase
    {
        public Row_R_SAP_JOB(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_JOB GetDataObject()
        {
            R_SAP_JOB DataObject = new R_SAP_JOB();
            DataObject.ID = this.ID;
            DataObject.JOBKEY = this.JOBKEY;
            DataObject.JOBNAME = this.JOBNAME;
            DataObject.SAPCLIENT = this.SAPCLIENT;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
            DataObject.DATA6 = this.DATA6;
            DataObject.JOBSTATUS = this.JOBSTATUS;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.LASTEDITTIME = this.LASTEDITTIME;
            DataObject.RUNTIME = this.RUNTIME;
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
        public string JOBKEY
        {
            get
            {
                return (string)this["JOBKEY"];
            }
            set
            {
                this["JOBKEY"] = value;
            }
        }
        public string JOBNAME
        {
            get
            {
                return (string)this["JOBNAME"];
            }
            set
            {
                this["JOBNAME"] = value;
            }
        }
        public string SAPCLIENT
        {
            get
            {
                return (string)this["SAPCLIENT"];
            }
            set
            {
                this["SAPCLIENT"] = value;
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
        public string DATA6
        {
            get
            {
                return (string)this["DATA6"];
            }
            set
            {
                this["DATA6"] = value;
            }
        }
        public string JOBSTATUS
        {
            get
            {
                return (string)this["JOBSTATUS"];
            }
            set
            {
                this["JOBSTATUS"] = value;
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
        public string LASTEDITTIME
        {
            get
            {
                return (string)this["LASTEDITTIME"];
            }
            set
            {
                this["LASTEDITTIME"] = value;
            }
        }
        public string RUNTIME
        {
            get
            {
                return (string)this["RUNTIME"];
            }
            set
            {
                this["RUNTIME"] = value;
            }
        }
    }
    public class R_SAP_JOB
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string JOBKEY { get; set; }
        public string JOBNAME { get; set; }
        public string SAPCLIENT { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string DATA6 { get; set; }
        public string JOBSTATUS { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public string LASTEDITTIME { get; set; }
        public string RUNTIME { get; set; }
    }


    public enum ENUM_R_SAP_JOB_FUNCTION
    {
        [EnumValue("ChangeCrsdWithSap")]
        ChangeCrsdWithSap,
        [EnumValue("TecoSapWoWithCancel")]
        TecoSapWoWithCancel,
        [EnumValue("TecoSapWoWithChange")]
        TecoSapWoWithChange,
        [EnumValue("CreateWoJob")]
        CreateWoJob
    }

    public enum ENUM_R_SAP_JOB_JOBSTATUS
    {
        [EnumValue("0")]
        Wait,
        [EnumValue("1")]
        Finish,
        [EnumValue("2")]
        Runing,
        [EnumValue("3")]
        Closed
    }
}