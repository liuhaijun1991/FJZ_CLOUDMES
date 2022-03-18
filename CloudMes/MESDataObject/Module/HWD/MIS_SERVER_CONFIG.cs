using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_MIS_SERVER_CONFIG : DataObjectTable
    {
        public T_MIS_SERVER_CONFIG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_MIS_SERVER_CONFIG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_MIS_SERVER_CONFIG);
            TableName = "MIS_SERVER_CONFIG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_MIS_SERVER_CONFIG : DataObjectBase
    {
        public Row_MIS_SERVER_CONFIG(DataObjectInfo info) : base(info)
        {

        }
        public MIS_SERVER_CONFIG GetDataObject()
        {
            MIS_SERVER_CONFIG DataObject = new MIS_SERVER_CONFIG();
            DataObject.ID = this.ID;
            DataObject.SERVERNO = this.SERVERNO;
            DataObject.IP = this.IP;
            DataObject.SERVERUSE = this.SERVERUSE;
            DataObject.OS = this.OS;
            DataObject.OS_LEGAL = this.OS_LEGAL;
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
        public string SERVERNO
        {
            get
            {
                return (string)this["SERVERNO"];
            }
            set
            {
                this["SERVERNO"] = value;
            }
        }
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
            }
        }
        public string SERVERUSE
        {
            get
            {
                return (string)this["SERVERUSE"];
            }
            set
            {
                this["SERVERUSE"] = value;
            }
        }
        public string OS
        {
            get
            {
                return (string)this["OS"];
            }
            set
            {
                this["OS"] = value;
            }
        }
        public string OS_LEGAL
        {
            get
            {
                return (string)this["OS_LEGAL"];
            }
            set
            {
                this["OS_LEGAL"] = value;
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
    public class MIS_SERVER_CONFIG
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string SERVERNO { get; set; }
        public string IP { get; set; }
        public string SERVERUSE { get; set; }
        public string OS { get; set; }
        public string OS_LEGAL { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}