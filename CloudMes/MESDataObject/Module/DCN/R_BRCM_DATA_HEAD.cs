using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BRCM_DATA_HEAD : DataObjectTable
    {
        public T_R_BRCM_DATA_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BRCM_DATA_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BRCM_DATA_HEAD);
            TableName = "R_BRCM_DATA_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_BRCM_DATA_HEAD : DataObjectBase
    {
        public Row_R_BRCM_DATA_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_BRCM_DATA_HEAD GetDataObject()
        {
            R_BRCM_DATA_HEAD DataObject = new R_BRCM_DATA_HEAD();
            DataObject.ID = this.ID;
            DataObject.DNNO = this.DNNO;
            DataObject.SONO = this.SONO;
            DataObject.SOLINENO = this.SOLINENO;
            DataObject.FILENAME = this.FILENAME;
            DataObject.FILETYPE = this.FILETYPE;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.FLAG = this.FLAG;
            DataObject.CREATEFILETIME = this.CREATEFILETIME;
            DataObject.SENDTIME = this.SENDTIME;
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
        public string DNNO
        {
            get
            {
                return (string)this["DNNO"];
            }
            set
            {
                this["DNNO"] = value;
            }
        }
        public string SONO
        {
            get
            {
                return (string)this["SONO"];
            }
            set
            {
                this["SONO"] = value;
            }
        }
        public string SOLINENO
        {
            get
            {
                return (string)this["SOLINENO"];
            }
            set
            {
                this["SOLINENO"] = value;
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
        public string FILETYPE
        {
            get
            {
                return (string)this["FILETYPE"];
            }
            set
            {
                this["FILETYPE"] = value;
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
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
        public DateTime? CREATEFILETIME
        {
            get
            {
                return (DateTime?)this["CREATEFILETIME"];
            }
            set
            {
                this["CREATEFILETIME"] = value;
            }
        }
        public DateTime? SENDTIME
        {
            get
            {
                return (DateTime?)this["SENDTIME"];
            }
            set
            {
                this["SENDTIME"] = value;
            }
        }
    }
    public class R_BRCM_DATA_HEAD
    {
        public string ID { get; set; }
        public string DNNO { get; set; }
        public string SONO { get; set; }
        public string SOLINENO { get; set; }
        public string FILENAME { get; set; }
        public string FILETYPE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string FLAG { get; set; }
        public DateTime? CREATEFILETIME { get; set; }
        public DateTime? SENDTIME { get; set; }
    }
}
