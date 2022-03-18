using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_I285 : DataObjectTable
    {
        public T_R_I285(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        { 
        }
        public T_R_I285(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I285);
            TableName = "R_I285".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int InSertRow(R_I285 Row, OleExec DB)
        {
            return DB.ORM.Insertable<R_I285>(Row).ExecuteCommand();
        }
    }
    public class Row_R_I285 : DataObjectBase
    {
        public Row_R_I285(DataObjectInfo info) : base(info)
        {

        }
        public R_I285 GetDataObject()
        {
            R_I285 DataObject = new R_I285();
            DataObject.ID = this.ID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATE = this.CREATIONDATE;
            DataObject.SENDERID = this.SENDERID;
            DataObject.PN = this.PN;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.STARTDATE = this.STARTDATE;
            DataObject.ENDDATE = this.ENDDATE;
            DataObject.TRANID = this.TRANID;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string F_PLANT
        {
            get
            {
                return (string)this["F_PLANT"];
            }
            set
            {
                this["F_PLANT"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public DateTime? CREATIONDATE
        {
            get
            {
                return (DateTime?)this["CREATIONDATE"];
            }
            set
            {
                this["CREATIONDATE"] = value;
            }
        }
        public string SENDERID
        {
            get
            {
                return (string)this["SENDERID"];
            }
            set
            {
                this["SENDERID"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string QUANTITY
        {
            get
            {
                return (string)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
            }
        }
        public DateTime? STARTDATE
        {
            get
            {
                return (DateTime?)this["STARTDATE"];
            }
            set
            {
                this["STARTDATE"] = value;
            }
        }
        public DateTime? ENDDATE
        {
            get
            {
                return (DateTime?)this["ENDDATE"];
            }
            set
            {
                this["ENDDATE"] = value;
            }
        }
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
            }
        }
        public DateTime? F_LASTEDITDT
        {
            get
            {
                return (DateTime?)this["F_LASTEDITDT"];
            }
            set
            {
                this["F_LASTEDITDT"] = value;
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
    }
    public class R_I285
    {
        public string ID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATE { get; set; }
        public string SENDERID { get; set; }
        public string PN { get; set; }
        public string QUANTITY { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        public string TRANID { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}