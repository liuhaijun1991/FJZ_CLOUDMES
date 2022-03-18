using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_WEIGHT : DataObjectTable
    {
        public T_C_WEIGHT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_WEIGHT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_WEIGHT);
            TableName = "C_WEIGHT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_WEIGHT : DataObjectBase
    {
        public Row_C_WEIGHT(DataObjectInfo info) : base(info)
        {

        }
        public C_WEIGHT GetDataObject()
        {
            C_WEIGHT DataObject = new C_WEIGHT();
            DataObject.ID = this.ID;
            DataObject.TYPE = this.TYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.MPN = this.MPN;
            DataObject.MAXWEIGHT = this.MAXWEIGHT;
            DataObject.AVGWEIGHT = this.AVGWEIGHT;
            DataObject.MINWEIGHT = this.MINWEIGHT;
            DataObject.STATION = this.STATION;
            DataObject.ENABLEFLAG = this.ENABLEFLAG;
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
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
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
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string MAXWEIGHT
        {
            get
            {
                return (string)this["MAXWEIGHT"];
            }
            set
            {
                this["MAXWEIGHT"] = value;
            }
        }
        public string AVGWEIGHT
        {
            get
            {
                return (string)this["AVGWEIGHT"];
            }
            set
            {
                this["AVGWEIGHT"] = value;
            }
        }
        public string MINWEIGHT
        {
            get
            {
                return (string)this["MINWEIGHT"];
            }
            set
            {
                this["MINWEIGHT"] = value;
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
        public string ENABLEFLAG
        {
            get
            {
                return (string)this["ENABLEFLAG"];
            }
            set
            {
                this["ENABLEFLAG"] = value;
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
    public class C_WEIGHT
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string TYPE { get; set; }
        public string SKUNO { get; set; }
        public string PARTNO { get; set; }
        public string MPN { get; set; }
        public string MAXWEIGHT { get; set; }
        public string AVGWEIGHT { get; set; }
        public string MINWEIGHT { get; set; }
        public string STATION { get; set; }
        public string ENABLEFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}