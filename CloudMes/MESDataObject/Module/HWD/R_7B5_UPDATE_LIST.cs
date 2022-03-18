using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_UPDATE_LIST : DataObjectTable
    {
        public T_R_7B5_UPDATE_LIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_UPDATE_LIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_UPDATE_LIST);
            TableName = "R_7B5_UPDATE_LIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int SaveData(OleExec sfcdb, R_7B5_UPDATE_LIST obj)
        {
            return sfcdb.ORM.Insertable<R_7B5_UPDATE_LIST>(obj).ExecuteCommand();
        }
    }
    public class Row_R_7B5_UPDATE_LIST : DataObjectBase
    {
        public Row_R_7B5_UPDATE_LIST(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_UPDATE_LIST GetDataObject()
        {
            R_7B5_UPDATE_LIST DataObject = new R_7B5_UPDATE_LIST();
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.DATA5 = this.DATA5;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA1 = this.DATA1;
            DataObject.REMARK = this.REMARK;
            DataObject.CANCEL_FLAG = this.CANCEL_FLAG;
            DataObject.PO_FLAG = this.PO_FLAG;
            DataObject.QTY = this.QTY;
            DataObject.PLAN_FLAG = this.PLAN_FLAG;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.TYPE = this.TYPE;
            return DataObject;
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
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
        public string CANCEL_FLAG
        {
            get
            {
                return (string)this["CANCEL_FLAG"];
            }
            set
            {
                this["CANCEL_FLAG"] = value;
            }
        }
        public string PO_FLAG
        {
            get
            {
                return (string)this["PO_FLAG"];
            }
            set
            {
                this["PO_FLAG"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string PLAN_FLAG
        {
            get
            {
                return (string)this["PLAN_FLAG"];
            }
            set
            {
                this["PLAN_FLAG"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
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
    }
    public class R_7B5_UPDATE_LIST
    {
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public string DATA5 { get; set; }
        public string DATA4 { get; set; }
        public string DATA3 { get; set; }
        public string DATA2 { get; set; }
        public string DATA1 { get; set; }
        public string REMARK { get; set; }
        public string CANCEL_FLAG { get; set; }
        public string PO_FLAG { get; set; }
        public double? QTY { get; set; }
        public string PLAN_FLAG { get; set; }
        public string TASK_NO { get; set; }
        public string TYPE { get; set; }
    }
}