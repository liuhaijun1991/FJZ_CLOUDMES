using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TO_HEAD : DataObjectTable
    {
        public T_R_TO_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TO_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TO_HEAD);
            TableName = "R_TO_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TO_HEAD : DataObjectBase
    {
        public Row_R_TO_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_TO_HEAD GetDataObject()
        {
            R_TO_HEAD DataObject = new R_TO_HEAD();
            DataObject.ID = this.ID;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.DROP_FLAG = this.DROP_FLAG;
            DataObject.EXTERNAL_NO = this.EXTERNAL_NO;
            DataObject.VEHICLE_NO = this.VEHICLE_NO;
            DataObject.CONTAINER_NO = this.CONTAINER_NO;
            DataObject.TO_CREATETIME = this.TO_CREATETIME;
            DataObject.PLAN_ENDTIME = this.PLAN_ENDTIME;
            DataObject.PLAN_STARTIME = this.PLAN_STARTIME;
            DataObject.TO_NO = this.TO_NO;
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
        public string DROP_FLAG
        {
            get
            {
                return (string)this["DROP_FLAG"];
            }
            set
            {
                this["DROP_FLAG"] = value;
            }
        }
        public string EXTERNAL_NO
        {
            get
            {
                return (string)this["EXTERNAL_NO"];
            }
            set
            {
                this["EXTERNAL_NO"] = value;
            }
        }
        public string VEHICLE_NO
        {
            get
            {
                return (string)this["VEHICLE_NO"];
            }
            set
            {
                this["VEHICLE_NO"] = value;
            }
        }
        public string CONTAINER_NO
        {
            get
            {
                return (string)this["CONTAINER_NO"];
            }
            set
            {
                this["CONTAINER_NO"] = value;
            }
        }
        public string TO_CREATETIME
        {
            get
            {
                return (string)this["TO_CREATETIME"];
            }
            set
            {
                this["TO_CREATETIME"] = value;
            }
        }
        public DateTime? PLAN_ENDTIME
        {
            get
            {
                return (DateTime?)this["PLAN_ENDTIME"];
            }
            set
            {
                this["PLAN_ENDTIME"] = value;
            }
        }
        public DateTime? PLAN_STARTIME
        {
            get
            {
                return (DateTime?)this["PLAN_STARTIME"];
            }
            set
            {
                this["PLAN_STARTIME"] = value;
            }
        }
        public string TO_NO
        {
            get
            {
                return (string)this["TO_NO"];
            }
            set
            {
                this["TO_NO"] = value;
            }
        }
    }
    public class R_TO_HEAD
    {
        public string ID{ get; set; }
        public DateTime? CREATETIME{ get; set; }
        public string DROP_FLAG{ get; set; }
        public string EXTERNAL_NO{ get; set; }
        public string VEHICLE_NO{ get; set; }
        public string CONTAINER_NO{ get; set; }
        public string TO_CREATETIME{ get; set; }
        public DateTime? PLAN_ENDTIME{ get; set; }
        public DateTime? PLAN_STARTIME{ get; set; }
        public string TO_NO{ get; set; }
    }
}