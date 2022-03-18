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
    public class T_R_ARUBADATA_HEAD : DataObjectTable
    {
        public T_R_ARUBADATA_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ARUBADATA_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ARUBADATA_HEAD);
            TableName = "R_ARUBADATA_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ARUBADATA_HEAD : DataObjectBase
    {
        public Row_R_ARUBADATA_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_ARUBADATA_HEAD GetDataObject()
        {
            R_ARUBADATA_HEAD DataObject = new R_ARUBADATA_HEAD();
            DataObject.ID = this.ID;
            DataObject.DATEKEY = this.DATEKEY;
            DataObject.DATETYPE = this.DATETYPE;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.FILENAME = this.FILENAME;
            DataObject.HEADERRECORD = this.HEADERRECORD;
            DataObject.TRAILERRECORD = this.TRAILERRECORD;
            DataObject.SEQ = this.SEQ;
            DataObject.DATATYPE = this.DATATYPE;
            DataObject.GET = this.GET;
            DataObject.CONVERT = this.CONVERT;
            DataObject.SEND = this.SEND;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITTIME = this.EDITTIME;
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
        public string DATEKEY
        {
            get
            {
                return (string)this["DATEKEY"];
            }
            set
            {
                this["DATEKEY"] = value;
            }
        }
        public string DATETYPE
        {
            get
            {
                return (string)this["DATETYPE"];
            }
            set
            {
                this["DATETYPE"] = value;
            }
        }
        public DateTime? STARTTIME
        {
            get
            {
                return (DateTime?)this["STARTTIME"];
            }
            set
            {
                this["STARTTIME"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
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
        public string HEADERRECORD
        {
            get
            {
                return (string)this["HEADERRECORD"];
            }
            set
            {
                this["HEADERRECORD"] = value;
            }
        }
        public string TRAILERRECORD
        {
            get
            {
                return (string)this["TRAILERRECORD"];
            }
            set
            {
                this["TRAILERRECORD"] = value;
            }
        }
        public string SEQ
        {
            get
            {
                return (string)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public string DATATYPE
        {
            get
            {
                return (string)this["DATATYPE"];
            }
            set
            {
                this["DATATYPE"] = value;
            }
        }
        public string GET
        {
            get
            {
                return (string)this["GET"];
            }
            set
            {
                this["GET"] = value;
            }
        }
        public string CONVERT
        {
            get
            {
                return (string)this["CONVERT"];
            }
            set
            {
                this["CONVERT"] = value;
            }
        }
        public string SEND
        {
            get
            {
                return (string)this["SEND"];
            }
            set
            {
                this["SEND"] = value;
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
    }
    public class R_ARUBADATA_HEAD
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string DATEKEY { get; set; }
        public string DATETYPE { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string FILENAME { get; set; }
        public string HEADERRECORD { get; set; }
        public string TRAILERRECORD { get; set; }
        public string SEQ { get; set; }
        public string DATATYPE { get; set; }
        public string GET { get; set; }
        public string CONVERT { get; set; }
        public string SEND { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}
