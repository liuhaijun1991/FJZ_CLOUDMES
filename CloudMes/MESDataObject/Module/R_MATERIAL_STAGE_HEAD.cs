using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MATERIAL_STAGE_HEAD : DataObjectTable
    {
        public T_R_MATERIAL_STAGE_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MATERIAL_STAGE_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MATERIAL_STAGE_HEAD);
            TableName = "R_MATERIAL_STAGE_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MATERIAL_STAGE_HEAD : DataObjectBase
    {
        public Row_R_MATERIAL_STAGE_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_MATERIAL_STAGE_HEAD GetDataObject()
        {
            R_MATERIAL_STAGE_HEAD DataObject = new R_MATERIAL_STAGE_HEAD();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.GROUPNAME = this.GROUPNAME;
            DataObject.EVENTNAME = this.EVENTNAME;
            DataObject.ISMATERIAL = this.ISMATERIAL;
            DataObject.SEQNO = this.SEQNO;
            DataObject.EDITBY = this.EDITBY;
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
        public string GROUPNAME
        {
            get
            {
                return (string)this["GROUPNAME"];
            }
            set
            {
                this["GROUPNAME"] = value;
            }
        }
        public string EVENTNAME
        {
            get
            {
                return (string)this["EVENTNAME"];
            }
            set
            {
                this["EVENTNAME"] = value;
            }
        }
        public string ISMATERIAL
        {
            get
            {
                return (string)this["ISMATERIAL"];
            }
            set
            {
                this["ISMATERIAL"] = value;
            }
        }
        public string SEQNO
        {
            get
            {
                return (string)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
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
    }
    public class R_MATERIAL_STAGE_HEAD
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string GROUPNAME { get; set; }
        public string EVENTNAME { get; set; }
        public string ISMATERIAL { get; set; }
        public string SEQNO { get; set; }
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}